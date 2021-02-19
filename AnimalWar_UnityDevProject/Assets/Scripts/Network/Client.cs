using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Network;
using UnityEngine;

public class Client : MonoBehaviour
{
    private const int DataBufferSize = 4096;
    public static Client Instance;
    private static Dictionary<int, PacketHandler> _packetHandlers;
    public string user = "";
    public string Pass = "";
    public int port = 27017;
    public int myId;


    private bool _isConnected;

    private string ip = "";
    public TCP Tcp;
    public UDP Udp;

    private void Awake()
    {
        ip = string.IsNullOrEmpty(Constants.IpAdress) ? "127.0.0.1" : Constants.IpAdress;
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(this);
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Tcp = new TCP();
        Udp = new UDP();
    }

    private void OnApplicationQuit()
    {
        Disconnect();
    }

    public void ConnectToServer()
    {
        InitializeClientData();
        try
        {
            Tcp.Connect();
            _isConnected = true;
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    private void InitializeClientData()
    {
        _packetHandlers = new Dictionary<int, PacketHandler>
        {
            {(int) ServerPackets.welcome, ClientHandle.Welcome},
            {(int) ServerPackets.spawnPlayer, ClientHandle.SpawnPlayer},
            {(int) ServerPackets.playerPosition, ClientHandle.PlayerPosition},
            {(int) ServerPackets.playerRotation, ClientHandle.PlayerRotation},
            {(int) ServerPackets.playerDisconnected, ClientHandle.PlayerDisconnected},
            {(int) ServerPackets.handleLoginInfo, ClientHandle.HandleLogin},
            {(int) ServerPackets.sendInviteServer, ClientHandle.InvitationReceived},
            {(int) ServerPackets.sendInviteAnswer, ClientHandle.InvitationResponse},
            {(int) ServerPackets.mmOk, ClientHandle.MMState},
            {(int) ServerPackets.removeLFButtons, ClientHandle.InteractableButtons},
            {(int) ServerPackets.matchFound, ClientHandle.MatchFound},
            {(int) ServerPackets.beginMatch, ClientHandle.GoToScene},
            {(int) ServerPackets.lobbyconnect, ClientHandle.HandleLobbyPacket},
            {(int) ServerPackets.sendPickUpdate, ClientHandle.HandlePickLobby},
            {(int) ServerPackets.endLobby, ClientHandle.HandleLobbyEnd},
            {(int) ServerPackets.removeCanvas, ClientHandle.RemoveCanvas},
            {(int) ServerPackets.sendPlayerConflictZone, ClientHandle.HandleZoneUpdate},
            {(int) ServerPackets.sendPlayerHealth, ClientHandle.UpdateHealth},
            {(int) ServerPackets.playerDied, ClientHandle.HandleDeath},
            {(int) ServerPackets.updateZoneState, ClientHandle.UpdateZoneState},
            {(int) ServerPackets.setMatchEndResult, ClientHandle.SetMatchEndResult},
            {(int) ServerPackets.forceSceneLoad, ClientHandle.LoadSceneForce},
            {(int) ServerPackets.updateTutorialStage, ClientHandle.UpdateTutorialState},
            {(int) ServerPackets.beginTutorial, ClientHandle.BeginTutorial}
        };
        Debug.Log("Initialized packets.");
    }

    private void Disconnect()
    {
        if (!_isConnected) return;
        _isConnected = false;
        Tcp?.socket.Close();
        Udp?.socket.Close();
        Debug.Log("Disconnected");
    }

    private delegate void PacketHandler(Packet _packet);


    public class TCP
    {
        private bool online;
        private byte[] receiveBuffer;
        private Packet receivedData;
        public TcpClient socket;
        private NetworkStream stream;

        public void Connect()
        {
            socket = new TcpClient
            {
                ReceiveBufferSize = DataBufferSize,
                SendBufferSize = DataBufferSize
            };

            receiveBuffer = new byte[DataBufferSize];
            try
            {
                socket.BeginConnect(Instance.ip, Instance.port, ConnectCallback, socket);
            }
            catch (Exception e)
            {
                Debug.Log($"An error occurred during TCP Connection Launch : {e}");
            }
        }


        private void ConnectCallback(IAsyncResult _result)
        {
            socket.EndConnect(_result);

            if (!socket.Connected) return;

            stream = socket.GetStream();

            receivedData = new Packet();

            stream.BeginRead(receiveBuffer, 0, DataBufferSize, ReceiveCallback, null);
        }

        public void SendData(Packet _packet)
        {
            try
            {
                if (socket != null) stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null);
            }
            catch (Exception _ex)
            {
                Debug.Log($"Error sending data to server via TCP: {_ex}");
            }
        }

        private void Disconnect()
        {
            Instance.Disconnect();

            stream = null;
            receivedData = null;
            receiveBuffer = null;
            socket = null;
        }

        private void ReceiveCallback(IAsyncResult _result)
        {
            try
            {
                var _byteLength = stream.EndRead(_result);
                if (_byteLength <= 0)
                {
                    Instance.Disconnect();
                    return;
                }

                var _data = new byte[_byteLength];
                Array.Copy(receiveBuffer, _data, _byteLength);

                receivedData.Reset(HandleData(_data));
                stream.BeginRead(receiveBuffer, 0, DataBufferSize, ReceiveCallback, null);
            }
            catch
            {
                Disconnect();
            }
        }

        private bool HandleData(byte[] _data)
        {
            var _packetLength = 0;

            receivedData.SetBytes(_data);

            if (receivedData.UnreadLength() >= 4)
            {
                _packetLength = receivedData.ReadInt();
                if (_packetLength <= 0) return true;
            }

            while (_packetLength > 0 && _packetLength <= receivedData.UnreadLength())
            {
                var _packetBytes = receivedData.ReadBytes(_packetLength);
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (var _packet = new Packet(_packetBytes))
                    {
                        var _packetId = _packet.ReadInt();
                        Debug.Log(
                            $"Packet Id Received was {_packetId} wich corresponds with {((ServerPackets) _packetId).ToString()}");
                        _packetHandlers[_packetId](_packet);
                    }
                });

                _packetLength = 0;
                if (receivedData.UnreadLength() < 4) continue;
                _packetLength = receivedData.ReadInt();
                if (_packetLength <= 0) return true;
            }

            if (_packetLength <= 1) return true;

            return false;
        }
    }

    public class UDP
    {
        public IPEndPoint endPoint;
        public UdpClient socket;

        public UDP()
        {
            endPoint = new IPEndPoint(IPAddress.Parse(Instance.ip), Instance.port);
        }

        public void Connect(int _localPort)
        {
            socket = new UdpClient(_localPort);

            socket.Connect(endPoint);
            socket.BeginReceive(ReceiveCallback, null);

            using (var _packet = new Packet())
            {
                SendData(_packet);
            }
        }

        private void Disconnect()
        {
            Instance.Disconnect();
            endPoint = null;
            socket = null;
        }

        public void SendData(Packet _packet)
        {
            try
            {
                _packet.InsertInt(Instance.myId);
                if (socket != null) socket.BeginSend(_packet.ToArray(), _packet.Length(), null, null);
            }
            catch (Exception _ex)
            {
                Debug.Log($"Error sending data to server via UDP: {_ex}");
            }
        }

        private void ReceiveCallback(IAsyncResult _result)
        {
            try
            {
                var _data = socket.EndReceive(_result, ref endPoint);
                socket.BeginReceive(ReceiveCallback, null);

                if (_data.Length < 4)
                {
                    Instance.Disconnect();
                    return;
                }

                HandleData(_data);
            }
            catch
            {
                Disconnect();
            }
        }

        private void HandleData(byte[] _data)
        {
            using (var _packet = new Packet(_data))
            {
                var _packetLength = _packet.ReadInt();
                _data = _packet.ReadBytes(_packetLength);
            }

            ThreadManager.ExecuteOnMainThread(() =>
            {
                using (var _packet = new Packet(_data))
                {
                    var _packetId = _packet.ReadInt();
                    _packetHandlers[_packetId](_packet);
                }
            });
        }
    }
}