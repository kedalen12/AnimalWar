using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CallPanda : MonoBehaviour
{
    public Panda myPanda;


    public void DeleteSelf()
    {
        this.gameObject.SetActive(false);
    }
    public void IdleCall()
    {
        myPanda.DeleteBamboo();
    }

    public void InactiveSelf()
    {
        myPanda.DeleteBamboo();
    }

    public void AllowMovement()
    {
        myPanda.AllowJump();
    }

    public void CallCheckDmg()
    {
        myPanda.CheckDamage();
    }

    public void EndShield()
    {
        myPanda.EndShield();
    }

    public void HaltAnimator()
    {
        myPanda.ActivateBamboo();
    }

    public void BeginUltimateFlyPhase()
    {
        myPanda.UpdatePhase(1);
    }

    public void EndUltimate()
    {
        StartCoroutine(myPanda.SnapBack());
    }
}