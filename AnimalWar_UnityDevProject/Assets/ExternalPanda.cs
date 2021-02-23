using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum AnimationNames 
{
    UltiState,
    Run,
    Punch,
    Jump,
    Shield,
    PunchInv
}
public class ExternalPanda : MonoBehaviour
{
    public Animator pandaBodyAnimator;
    public Animator fakeBamboo;
    public GameObject FakebambooObject;
    public GameObject TrueBamboo;
    public GameObject Shield;
    public float setIdle = 0f;

    IEnumerator RecoverBamboo()
    {
        yield return new WaitForSecondsRealtime(5f);
        TrueBamboo.SetActive(true);
    }
    public void PlayAnimation(int animationToPlay){
    pandaBodyAnimator.Play($"{animationToPlay}");
    if (animationToPlay == 7)
    {
        TrueBamboo.SetActive(false);
    }
    if (animationToPlay != 4) return;
    if (!Shield.activeSelf)
    {
        Shield.SetActive(true);
    }
    }
}
