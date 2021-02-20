using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Network;
using Player;
using UnityEngine;

public class Panda : MonoBehaviour
{
    public GameObject shield;
    public Animator pandaAnimator;
    public Transform attackSpawner;
    public float attackRadius;
    public LayerMask attackLayer;
    public int punchDamage = 100;
    private bool _jumping = false;
    public PlayerMovement myMovement;
    public HandlePlayerStats myStats;
    private bool _shieldAvailable = true;
    private bool _ultimateAvailable = true;
    public MoveTarget myTarget;
    private Task endUltimate;
    CancellationTokenSource source = new CancellationTokenSource();
    private CancellationToken removeEndUltimateCall;
    private bool UsingUltimate = false;
    private Vector3 ultimateTarget;
    public GameObject gfx;
    public GameObject bamboo;
    private Vector3 lastPosToSnap;
    

    private void Awake()
    {
        removeEndUltimateCall = source.Token;
        if (myStats == null)
        {
            myStats = GetComponent<HandlePlayerStats>();
        }
    }

    private void Start()
    {
        _jumping = false;
    }

    private void SetAvailable(string toSet)
    {
        switch (toSet)
        {
            case "shield":
                _shieldAvailable = true;
                break;
            case "ultimate":
                _ultimateAvailable = true;
                break;
        }
    }

    private async Task SetCoolDownShield(int delay)
    {
        await Task.Delay(delay * 1000);
        SetAvailable("shield");
    }

    /*ORDER PRIORITY
     SHIELD - PUNCH - JUMPING - MOVING - EMOTE */
    private void CheckInputs(){
        if (_shieldAvailable)
        {
            if (Input.GetMouseButtonDown(1) && _shieldAvailable)
            {
                //SHIELD
                Task.Run(() => SetCoolDownShield(12));
                _shieldAvailable = false;
                myStats.hasShield = true;
                UseShield();
            }
        }
        if (Input.GetKeyDown(Bindings.PlayerBinds.jump))
        {
            Jump();
        }
        if (!_jumping)
        {
            if (Input.GetKey(Bindings.PlayerBinds.forward) || Input.GetKey(Bindings.PlayerBinds.backwards) ||
                Input.GetKey(Bindings.PlayerBinds.left) || Input.GetKey(Bindings.PlayerBinds.right))
            {
                Run();
            }
            else
            {
                pandaAnimator.enabled = false;
                pandaAnimator.enabled = true;
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            Punch();
        }
    }
    private void Update()
    {
        CheckInputs();
    }



    private void Run()
    {
        pandaAnimator.Play("Run");
    }
    private void Jump()
    {
        _jumping = true;
        pandaAnimator.Play("Jump");
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackSpawner.position, attackRadius);
    }

    private bool isPunching = false;
    private void Punch()
    {
        isPunching = true;
        pandaAnimator.Play("Punch");
    }
    public void CheckDamage()
    {
        var players = new List<int>();
        var targets = Physics.OverlapSphere(attackSpawner.position, attackRadius, attackLayer);
        foreach (var target in targets)
        {
            if (target.gameObject.GetComponent<DummyLoop>())
            {
                var dml = target.gameObject.GetComponent<DummyLoop>();
                if (dml.dummyType == DummyLoop.TypeOfDummy.Defense)
                {
                    if (dml.Shield.activeSelf)
                        TutorialManager.Instance.ShowAlertText(
                            "Do not attack the enemy while its protective shield is active!");
                    else
                        TutorialManager.Instance.NewStage();
                }
            }
            else if(target.gameObject.GetComponent<HandlePlayerStats>() && target.CompareTag("EPlayer"))
            {
                players.Add( target.gameObject.GetComponent<HandlePlayerStats>().PlayerId);
            }
        }

        if (players.Count != 0)
        {
            ClientSend.DamageDealt(players, punchDamage); 
        }
        Debug.Log("Checking...");

    }
    public void UseShield()
    {

        shield.SetActive(true);
    }
    public void AllowJump()
    {
        _jumping = false;
    }
    public void EndShield()
    {
        myStats.hasShield = false;
        myMovement.Slow(0);
        shield.SetActive(false);
    }
}