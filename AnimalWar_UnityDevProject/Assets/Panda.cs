using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Network;
using Player;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Panda : MonoBehaviour
{
    public GameObject shield;
    public Animator pandaAnimator;
    public Transform attackSpawner;
    public float attackRadius;
    public LayerMask attackLayer;
    public SkinnedMeshRenderer bodyRenderPanda;
    public int punchDamage = 100;
    private bool _jumping = false;
    public PlayerMovement myMovement;
    public HandlePlayerStats myStats;
    private bool _shieldAvailable = true;
    public bool _ultimateAvailable = true;
    public MoveTarget myTarget;
    private Task endUltimate;
    CancellationTokenSource source = new CancellationTokenSource();
    private CancellationToken removeEndUltimateCall;
    //public Transform centerOfGfx;
    private bool UsingUltimate = false;
    private Vector3 ultimateTarget;
    public Quaternion rotatie;
    public GameObject gfx;
    public GameObject fakeBamboo;
    public GameObject selfBamboo;
    public bool bambooIsActive = true;
    private Vector3 lastPosToSnap;
    private Vector3 currentUltTarget;
    private bool isInUlti = false;

    public void DeleteBamboo()
    {
        fakeBamboo.SetActive(false);
        selfBamboo.SetActive(true);
    }

    public void ActivateBamboo()
    {
        fakeBamboo.SetActive(true);
        selfBamboo.SetActive(false);

    }
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

    IEnumerator SnapToParent()
    {
        yield return new WaitForEndOfFrame();
        //gfx.transform.position = new Vector3(0, 0, 0);
        pandaAnimator.SetInteger("UltiState", -1);
    }

    public void SnapIf()
    {
        //pandaAnimator.enabled = !(Vector3.Distance(gfx.transform.localPosition, new Vector3(0,0,0)) >= 0.5f);
    }

    public void CallOnIdle()
    {
       Debug.Log("My Code");
    }

    private void OnAnimatorMove()
    {

    }

    public IEnumerator SnapBack()
    {
        _ultimateAvailable = false;
        bambooIsActive = true;
        gfx.transform.eulerAngles = Vector3.zero;
        gfx.transform.rotation = new Quaternion(0,0,0,0);
        gfx.transform.localRotation = new Quaternion(0,0,0,0);
        gfx.transform.localEulerAngles = Vector3.zero;
        myMovement.gameObject.transform.SetParent(transform);
        isInUlti = false;
        pandaAnimator.SetInteger("UltiState", -1);
        transform.position = gfx.transform.position;
        gfx.transform.rotation = new Quaternion(0, 0, 0, 0);
        gfx.transform.eulerAngles = new Vector3(0, 0, 0);
        yield return new WaitForSecondsRealtime(.3f);
        gfx.transform.SetParent(transform);
        gfx.transform.eulerAngles = Vector3.zero;
        gfx.transform.rotation = new Quaternion(0,0,0,0);
        gfx.transform.localRotation = new Quaternion(0,0,0,0);
        gfx.transform.localEulerAngles = Vector3.zero;  
        myMovement.canMove = true;

        //pandaAnimator.enabled = true;

        //bodyRenderPanda.gameObject.SetActive(true);

        //bodyRenderPanda.enabled = true;
        /*Debug.Log("SON NOW !");
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        gfx.transform.localPosition = Vector3.zero;*/

        //StartCoroutine(SnapToParent());
        /* pandaAnimator.enabled = false;
         gfx.transform.SetParent(transform);
         myTarget.gameObject.transform.SetParent(transform);
         myTarget.Active = false;*/
        // StartCoroutine(SnapToParent());
    }
    
    /*ORDER PRIORITY
     SHIELD - PUNCH - JUMPING - MOVING - EMOTE */
    private KeyCode holdingCode;
    private void CheckInputs()
    {
        Debug.Log(_isPunching);
        /*gfx.transform.position = transform.position;
        gfx.transform.rotation = transform.rotation;
        gfx.transform.eulerAngles = transform.eulerAngles;*/
        pandaAnimator.SetBool("Jump", false);
        //pandaAnimator.SetBool("Punch", false);
        //pandaAnimator.SetBool("PunchInv", false);
        pandaAnimator.SetBool("Shield", false);

        if (!isInUlti)
        {
            if (Input.GetKeyDown(Bindings.PlayerBinds.ability))
            {
                //myTarget.Active = true;
                UseUltimate();
            }

            if (!pandaAnimator.GetBool("Run") && Input.GetKeyDown(KeyCode.K))
            {
                pandaAnimator.SetBool("DoEmote", true);
            }

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

            if (Input.GetKeyDown(Bindings.PlayerBinds.jump) && pandaAnimator.GetBool("Run"))
            {
                Jump();
            }

            if (!_jumping)
            {
                if (Input.GetKey(Bindings.PlayerBinds.forward) || Input.GetKey(Bindings.PlayerBinds.backwards) ||
                    Input.GetKey(Bindings.PlayerBinds.left) || Input.GetKey(Bindings.PlayerBinds.right))
                {
                    pandaAnimator.SetBool("DoEmote", false);
                    Run(true);
                } else if (Input.GetKeyUp(holdingCode))
                {
                    Run(false);

                }
            }
        }


        if (Input.GetMouseButtonDown(0))
        {
            if (isInUlti && myTarget.Active)
            {
                ActivateBamboo();

                ultimateTarget = myTarget.currentPos;
                myTarget.Active = false;
                gfx.transform.SetParent(null);
                myTarget.gameObject.transform.SetParent(null);
                /*UWU :)*/
                Vector3 D = ultimateTarget - gfx.transform.position;
                // calculate the Quaternion for the rotation
                Quaternion rot = Quaternion.Slerp(gfx.transform.rotation, Quaternion.LookRotation(D),
                    360 * Time.deltaTime);
                //Apply the rotation 
                gfx.transform.rotation = rot;
                // put 0 on the axys you do not want for the rotation object to rotate
                gfx.transform.eulerAngles = new Vector3(0, gfx.transform.eulerAngles.y, 0);
                
                pandaAnimator.SetInteger("UltiState", 1);
                ClientSend.SendAnimation(7, true, rot);


            }
            else
            {
                Punch();
            }
        }
    }

    private void OnGUI()
    {
        holdingCode = Event.current.keyCode;

    }
    private void UseUltimate()
    {
        isInUlti = true;
        myMovement.canMove = false; 
        pandaAnimator.SetInteger("UltiState", 0);
        SendAnimation(5);

    }

    //public bool goToParent = false;
    private void Update()
    {
        /*if (gfx.transform.parent != transform && !bambooIsActive)
        {
            transform.position = gfx.transform.position;
        }*/

        if (isInUlti && !myTarget.Active && !bambooIsActive)
        {
            transform.position = ultimateTarget;
        }

        CheckInputs();
    }


    private void Run(bool value)
    {
        pandaAnimator.SetBool("Run", value);
        SendAnimation(1);
    }

    public void SendAnimation(int number)
    {
        /*
 * 0 -> IDLE
 * 1 -> RUN
 * 2 -> JUMP
 * 3 -> PUNCH
 * 4 -> SHIELD
 * 5 -> STARTULTI
 * 6 -> ULTIIDLE
 * 7 -> JUMPFROMULTI
 * 8 -> STUNNED
 */
        ClientSend.SendAnimation(number, false);
        
    }
    private void Jump()
    {
        pandaAnimator.SetBool("Jump", true);
        SendAnimation(2);

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackSpawner.position, attackRadius);
    }

    private bool _isPunching;
    private void Punch()
    {
        if(_isPunching) return;
        var what = Random.Range(0, 101);
        pandaAnimator.SetBool("Punch" , true);
        SendAnimation(3);

        _isPunching = true;
    }

    public void CheckDamage()
    {
        pandaAnimator.SetBool("Punch", false);
        pandaAnimator.SetBool("PunchInv", false);
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
                        TutorialManager.Instance.UpdateStage(TutorialStage.Attack);
                }
            }
            else if (target.gameObject.GetComponent<HandlePlayerStats>() && target.CompareTag("EPlayer"))
            {
                players.Add(target.gameObject.GetComponent<HandlePlayerStats>().PlayerId);
            }
        }

        if (players.Count != 0)
        {
            //ClientSend.DamageDealt(players, punchDamage); 
        }
        _isPunching = false;

        Debug.Log("Checking...");
    }

    private void UseShield()
    {
        pandaAnimator.SetBool("Shield", true);
        SendAnimation(4);

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

    public void UpdatePhase(int value)
    {
        myTarget.Active = true;
    }
}