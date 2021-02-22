using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public enum UserState
{
    Online,
    Offline,
    MyParty
}
public class UserInfoPrefabHandler : MonoBehaviour
{

    public Transform parent;
    public Transform partyTransform;
    public Transform onlineTransform;
    public Transform offlineTransform;
    public Text userTxt;
    private string _username = "";
    private UserState _userState;
    public SpriteRenderer UserAvatar;
    public GameObject myOptionsList;

    public void LeftClick()
    {
        myOptionsList.SetActive(true);
    }
    
    
    public void Construct(string Username, UserState state, Sprite avatar)
    {
        this._username = Username;
        _userState = state;
        UserAvatar.sprite = avatar;
    }
    
}
