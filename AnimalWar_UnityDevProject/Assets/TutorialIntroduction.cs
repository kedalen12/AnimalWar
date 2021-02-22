using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialIntroduction : MonoBehaviour
{
    public GameObject cam1;
    public GameObject panda;
    public void StopSelf()
    {
        cam1.SetActive(true);
        gameObject.SetActive(false);
    }
}
