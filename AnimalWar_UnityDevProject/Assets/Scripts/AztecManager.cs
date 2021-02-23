using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public partial class LoadingScene
{
    public class AztecManager
    {
        #region PICK_OBJ

        public GameObject LocalTiger;
        public GameObject LocalWolf;
        public GameObject LocalPanda;
        public GameObject TeamTiger;
        public GameObject TeamWolf;
        public GameObject TeamPanda;
        public GameObject EnemyTiger;
        public GameObject EnemyWolf;
        public GameObject EnemyPanda;

        public Text LocalHpText;
        public Slider LocalHpSlider;
        #endregion
        private int maxHpAssasin = 1000;
        private int maxHpTank = 1500;
        private int maxHpRanged = 800;
        private List<int> overrideRotFor = new List<int>();
        private Dictionary<int, GameObject> _teamObjs = new Dictionary<int, GameObject>();
        private Dictionary<int, GameObject> _enemyObjs = new Dictionary<int, GameObject>();
        private Dictionary<int, HandlePlayerStats> _playerStats = new Dictionary<int, HandlePlayerStats>();
        public AztecManager(GameObject LocalTiger, GameObject LocalWolf, GameObject LocalPanda, GameObject TeamTiger, GameObject TeamWolf, GameObject TeamPanda, GameObject EnemyTiger, GameObject EnemyWolf, GameObject EnemyPanda, Text localText, Slider localSlider)
        {
            this.LocalTiger = LocalTiger;
            this.LocalWolf = LocalWolf;
            this.LocalPanda = LocalPanda;
            
            this.TeamTiger = TeamTiger;
            this.TeamWolf = TeamWolf;
            this.TeamPanda = TeamPanda;
            
            this.EnemyTiger = EnemyTiger;
            this.EnemyWolf = EnemyWolf;
            this.EnemyPanda = EnemyPanda;
            LocalHpSlider = localSlider;
            LocalHpText = localText;
            _teamObjs.Add(Constants.ServerID, null);
            foreach (var xPlayer in MatchVariables.TeamMates.Keys)
            {
                _teamObjs.Add(xPlayer, null);
            }
            
            foreach (var xPlayer in MatchVariables.Enemies.Keys)
            {
                _enemyObjs.Add(xPlayer, null);
            }

        }

        public void InstantiateLocalPlayer(int whatPick, Vector3 pos)
        {
            switch (whatPick)
            {
                case 0:
                    _teamObjs[Constants.ServerID] = Instantiate(LocalTiger, pos, Quaternion.identity);
                    break;
                case 2:
                    _teamObjs[Constants.ServerID] = Instantiate(LocalWolf, pos, Quaternion.identity);
                    break;
                case 1:
                    _teamObjs[Constants.ServerID] = Instantiate(LocalPanda, pos, Quaternion.identity);
                    break;
            }

           _playerStats.Add(Constants.ServerID, _teamObjs[Constants.ServerID].GetComponent<HandlePlayerStats>());
           _playerStats[Constants.ServerID].IsLocal = true;
           _playerStats[Constants.ServerID].HpText = LocalHpText;
           _playerStats[Constants.ServerID].HpSlider = LocalHpSlider;
           _playerStats[Constants.ServerID].MAXHp =
               whatPick == 0 ? maxHpAssasin : whatPick == 1 ? maxHpTank : maxHpRanged;
           _playerStats[Constants.ServerID].SetSlider();
           _playerStats[Constants.ServerID].PlayerId = Constants.ServerID;


        }

        public void InstantiateTeamMate(int playerId, int whatPick, Vector3 pos)
        {
            switch (whatPick)
            {
                case 0:
                    _teamObjs[playerId] = Instantiate(TeamTiger, pos, Quaternion.identity);
                    break;
                case 2:
                    _teamObjs[playerId] = Instantiate(TeamWolf, pos, Quaternion.identity);
                    break;
                case 1:
                    _teamObjs[playerId] = Instantiate(TeamPanda, pos, Quaternion.identity);
                    break;
            }
            _playerStats.Add(playerId, _teamObjs[playerId].GetComponent<HandlePlayerStats>());
            _playerStats[playerId].IsLocal = false;
            _playerStats[playerId].MAXHp =
                whatPick == 0 ? maxHpAssasin : whatPick == 1 ? maxHpTank : maxHpRanged;
            _playerStats[playerId].SetSlider();
            _playerStats[playerId].PlayerId = playerId;


        }

        public void SetDeathLocal()
        {
            _teamObjs[Constants.ServerID].GetComponent<PlayerMovement>().SetDeathState();
        }

        public void UpdateAnimation(int who, int what)
        {
            if (overrideRotFor.Contains(who))
            {
                overrideRotFor.Remove(who);
            }

            if (_teamObjs.ContainsKey(who))
            {
                if (_teamObjs[who].TryGetComponent<ExternalPanda>(out var externalPanda))
                {
                    externalPanda.PlayAnimation(what);
                }
            } else if (_enemyObjs.ContainsKey(who))
            {
                if (_enemyObjs[who].TryGetComponent<ExternalPanda>(out var externalPanda))
                {
                    externalPanda.PlayAnimation(what);
                }
            }
        }
        public void Respawn()
        {
            
        }
        public void InstantiateEnemy(int playerId,int whatPick, Vector3 pos)
        {
            switch (whatPick)
            {
                case 0:
                    _enemyObjs[playerId] = Instantiate(EnemyTiger, pos, Quaternion.identity);
                    break;
                case 2:
                    _enemyObjs[playerId] = Instantiate(EnemyWolf, pos, Quaternion.identity);
                    break;
                case 1:
                    _enemyObjs[playerId] = Instantiate(EnemyPanda, pos, Quaternion.identity);
                    break;
            }
            _playerStats.Add(playerId, _enemyObjs[playerId].GetComponent<HandlePlayerStats>());
            _playerStats[playerId].IsLocal = false;
            _playerStats[playerId].MAXHp =
                whatPick == 0 ? maxHpAssasin : whatPick == 1 ? maxHpTank : maxHpRanged;
            _playerStats[playerId].SetSlider();
            _playerStats[playerId].PlayerId = playerId;

        }

        public void UpdateLocalPosition(Vector3 position)
        {
            _teamObjs[Constants.ServerID].transform.position = position;
        }
        public void UpdatePosition(int whoToUpdate, Vector3 position, Quaternion rotation)
        {
            //Debug.Log($"Updating Position on {whoToUpdate}");
            if (MatchVariables.TeamMates.ContainsKey(whoToUpdate))
            {
                // Debug.Log("TEAMMATE !!!");
                _teamObjs[whoToUpdate].transform.position = position;
                if (!overrideRotFor.Contains(whoToUpdate))
                {
                    _teamObjs[whoToUpdate].transform.rotation = rotation;
                }
            }
            else
            {
                // Debug.Log("ENEMY !!!");

                _enemyObjs[whoToUpdate].transform.position = position;
                if (!overrideRotFor.Contains(whoToUpdate))
                {
                    _enemyObjs[whoToUpdate].transform.rotation = rotation;
                }
            }
            //Debug.Log("DONE");
        }

        public HandlePlayerStats GetPlayerStats(int playerId)
        {
            return _playerStats[playerId];
        }

        public void ReActivateGameObject(int whoToDie)
        {
            if (_teamObjs.ContainsKey(whoToDie))
            {
                _teamObjs[whoToDie].SetActive(true);
            } else if (_enemyObjs.ContainsKey(whoToDie))
            {
                _enemyObjs[whoToDie].SetActive(true);
            }
        }
        public void DeactivateGameObject(int whoToDie)
        {
            if (_teamObjs.ContainsKey(whoToDie))
            {
                _teamObjs[whoToDie].SetActive(false);
            } else if (_enemyObjs.ContainsKey(whoToDie))
            {
                _enemyObjs[whoToDie].SetActive(false);
            }
        }

        public void UpdateAnimationOvrrideMovement(int who, int what, Quaternion rot)
        {
            overrideRotFor.Add(who);
            if (_teamObjs.ContainsKey(who))
            {
                if (_teamObjs[who].TryGetComponent<ExternalPanda>(out var externalPanda))
                {
                    externalPanda.PlayAnimation(what);
                    _teamObjs[who].transform.rotation = rot;
                }
            } else if (_enemyObjs.ContainsKey(who))
            {
                if (_enemyObjs[who].TryGetComponent<ExternalPanda>(out var externalPanda))
                {
                    externalPanda.PlayAnimation(what);
                    _enemyObjs[who].transform.rotation = rot;

                }
            }
        }
    }
}