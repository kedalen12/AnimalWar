﻿using System;
using UnityEngine;

public class DummyLoop : MonoBehaviour
{
    public GameObject player;
    public float Radius = 10f;
    public Collider[] Colliders;
    public GameObject Shield;
    public enum TypeOfDummy
    {
        Attack,
        Defense
    }

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("LPlayer");
        if (dummyType == TypeOfDummy.Attack)
        {
            foreach (var collider in Colliders)
            {
                collider.gameObject.AddComponent<ColliderDummyCallBack>();
            }
            InvokeRepeating("AttackLoop", 1, 1.5f);
        }
        else
        {
            InvokeRepeating("DefenseLoop", 1, 4f);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, Radius);
    
    }

    public TypeOfDummy dummyType;


    private void Update()
    {
        if (!(Vector3.Distance(player.transform.position, transform.position) <= Radius)) return;
     
    }

    private void DefenseLoop()
    {
        Shield.SetActive(!Shield.activeSelf);
    }

    private void AttackLoop()
    {
        Debug.Log("About to attack");
    }
}

public class ColliderDummyCallBack : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("LPlayer"))
        {
            if (other.GetComponent<Panda>().shield.activeSelf)
            {
                TutorialManager.Instance.UpdateStage(TutorialStage.Ultimate);
            }
            else
            {
                TutorialManager.Instance.ShowAlertText("Press LMB to use shield!");
                other.GetComponent<HandlePlayerStats>().UpdateHealth(100);
            }
            
        }
    }
}