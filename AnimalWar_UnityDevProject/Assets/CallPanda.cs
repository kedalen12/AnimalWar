using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CallPanda : MonoBehaviour
{
    public Panda myPanda;
    
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
       
    }

    public void BeginUltimateFlyPhase()
    {
        
    }

    public void EndUltimate()
    {
    }
}