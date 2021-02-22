using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public enum UserState
{
    Online,
    Offline,
    MyParty
}

public class UserInfoPrefabHandler : MonoBehaviour, IPointerClickHandler {

    public Transform parent;
    public Transform partyTransform;
    public Transform onlineTransform;
    public Transform offlineTransform;
    public Text userTxt;
    private string _username = "";
    private UserState _userState;
    public Image UserAvatar;
    public GameObject myOptionsList;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            RightClick();
        }
    }

    public void RightClick()
    {
        StartCoroutine(ResetAnimation(true));
    }

    IEnumerator ResetAnimation(bool isOpen)
    {
        myOptionsList.GetComponent<Animator>().enabled = false;
        yield return new WaitForEndOfFrame();
        myOptionsList.GetComponent<Animator>().enabled = true;
        if (isOpen)
        {
            myOptionsList.SetActive(true);
        }
        else
        {
            myOptionsList.GetComponent<Animator>().Play("Exit");
            StartCoroutine("SetEnable");
        }
    }

    public void ExitMouse()
    {
        StartCoroutine(ResetAnimation(false));
    }

    IEnumerator SetEnable()
    {
        yield return new WaitForSecondsRealtime(1f);
        myOptionsList.SetActive(false);
    }
    public void Construct(string Username, UserState state, Sprite avatar)
    {
        this._username = Username;
        _userState = state;
        UserAvatar.sprite = avatar;
    }
}