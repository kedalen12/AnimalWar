using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;
using UnityEngine.UI;

public enum TutorialStage
{
    Introduction = 1,
    MoveWASD,
    Defense,
    Attack,
    Ultimate,
    Exit
}

public class TutorialManager : MonoBehaviour
{
    public Collider startingCollider;
    public Collider attackCollider;
    public Collider ultimateCollider;

    public AudioSource voiceSource;
    public AudioClip zero;
    public AudioClip one;
    public AudioClip two;
    public AudioClip three;
    public static TutorialManager Instance;
    private int lastStage = 1;
    public bool disableNext;
    private int currentText = 0;
    public GameObject WASDImage;
    public GameObject player;
    public GameObject aok;
    public Text alertText;
    public Text alertTextBG;
    public System.Action playerIsInRangeOfDummies;
    public GameObject sok;
    public GameObject portalExit;
    public GameObject dok;
    public GameObject wok;
    public GameObject spaceok;
    private List<int> values = new List<int>();
    private List<KeyCode> pressedKeys = new List<KeyCode>();
    private TutorialStage currentStage;

    private string[] tutorialStrings = new[]
    {
        "Greetings Traveler this is the peak of serenity a place where brave souls like yourself come to train",
        "Oh Forget my manners my name is master FU, and I am your trainer",
        "Move your character! press",
        "Attack the blue dummy by pressing RMB",
        "Defend from red dummy by pressing LMB",
        "Press F to use your inner power!",
        "Exit the tutorial by crossing the portal!"
    };

    public Text tutorialText;
    public Text tutorialTextBg;

    private void Awake()
    {
        tutorialStrings[2] =
            $"Move your character by pressing {Bindings.PlayerBinds.forward.ToString()} {Bindings.PlayerBinds.backwards.ToString()} {Bindings.PlayerBinds.left.ToString()} {Bindings.PlayerBinds.right.ToString()} {Bindings.PlayerBinds.jump.ToString()}";
        tutorialStrings[5] =
            $"Press {Bindings.PlayerBinds.ability.ToString()} to use your inner power!";
        Instance = this;
        WASDImage.SetActive(false);
        tutorialText.text = tutorialStrings[0];
        tutorialTextBg.text = tutorialStrings[0];
        playerIsInRangeOfDummies += UpdateText;
        Invoke(nameof(UpdateText), 6.5f);
        Invoke(nameof(UpdateText), 16);
        values.Add((int) Bindings.PlayerBinds.forward);
        values.Add((int) Bindings.PlayerBinds.left);
        values.Add((int) Bindings.PlayerBinds.backwards);
        values.Add((int) Bindings.PlayerBinds.right);
        values.Add((int) Bindings.PlayerBinds.jump);
    }
    private void Update()
    {
        Debug.Log(currentStage.ToString());
        if (currentStage != TutorialStage.MoveWASD)
        {
            WASDImage.SetActive(false);
        }
        switch (currentStage)
        {
            case TutorialStage.MoveWASD:
                GatherKeys();
                break;
            case TutorialStage.Ultimate:
                if (voiceSource.clip == null && !voiceSource.isPlaying)
                {
                    voiceSource.clip = two;
                    voiceSource.Play();
                }
                if (!player.GetComponent<Panda>()._ultimateAvailable)
                {
                    UpdateStage(TutorialStage.Exit);
                }
                break;
            case TutorialStage.Exit:
                if (!portalExit.activeSelf)
                {
                    portalExit.SetActive(true);
                }
                break;
        }
     
    }

    void GatherKeys()
    {
        if (pressedKeys.Count >= 5)
        {
            UpdateStage(TutorialStage.Defense);
        }

        if (!Input.GetKeyDown(Bindings.PlayerBinds.forward) && !Input.GetKeyDown(Bindings.PlayerBinds.left) && !Input.GetKeyDown(Bindings.PlayerBinds.backwards) &&
            !Input.GetKeyDown(Bindings.PlayerBinds.right) && !Input.GetKeyDown(Bindings.PlayerBinds.jump)) return;
        KeyCode key;
        foreach (var t in values)
        {
            if (Input.GetKey((KeyCode) t))
            {
                if ((KeyCode) t == Bindings.PlayerBinds.left)
                {
                    aok.SetActive(true);
                }

                if ((KeyCode) t == Bindings.PlayerBinds.forward)
                {
                    wok.SetActive(true);
                }

                if ((KeyCode) t == Bindings.PlayerBinds.backwards)
                {
                    sok.SetActive(true);
                }

                if ((KeyCode) t == Bindings.PlayerBinds.right)
                {
                    dok.SetActive(true);
                }

                if ((KeyCode) t == Bindings.PlayerBinds.jump)
                {
                    spaceok.SetActive(true);
                }

                GatherWASDCondition((KeyCode) t);
            }
        }
    }
    
    public void UpdateStage(TutorialStage nextStage)
    {
        var expectedNextStage = TutorialStage.Introduction;
        if (currentStage == TutorialStage.MoveWASD && pressedKeys.Count >= 5)
        {
            expectedNextStage = TutorialStage.Defense;
        }
        else if (currentStage != TutorialStage.MoveWASD)
        {
            if (currentStage != TutorialStage.Ultimate)
            {
                expectedNextStage = currentStage + 1;
            }
            else
            {
                expectedNextStage = TutorialStage.Exit;
            }
        }

        if(nextStage != expectedNextStage) return;
        if(nextStage == currentStage) return;
        switch (nextStage)
        {
            case TutorialStage.Introduction:
                break;
            case TutorialStage.MoveWASD:
                voiceSource.Stop();
                voiceSource.clip = null;
                voiceSource.clip = zero;
                voiceSource.Play();
                break;
            case TutorialStage.Defense:
                voiceSource.Stop();
                voiceSource.clip = null;
                tutorialText.text = tutorialStrings[3];
                tutorialTextBg.text = tutorialStrings[3];
                voiceSource.clip = one;
                voiceSource.Play();
                break;
            case TutorialStage.Attack:
                tutorialText.text = tutorialStrings[4];
                tutorialTextBg.text = tutorialStrings[4];
                break;
            case TutorialStage.Ultimate:
                voiceSource.Stop();
                voiceSource.clip = null;
                tutorialText.text = tutorialStrings[5];
                tutorialTextBg.text = tutorialStrings[5];
                voiceSource.clip = two;
                voiceSource.Play();
                break;
            case TutorialStage.Exit:
                tutorialText.text = tutorialStrings[6];
                tutorialTextBg.text = tutorialStrings[6];
                voiceSource.clip = three;
                voiceSource.Play();
                break;
            default:
                break;
        }

        currentStage = nextStage;

    }

    private void UpdateText()
    {
        currentText++;
        if (currentText == 1 || currentText == 0)
        {
            UpdateStage(TutorialStage.Introduction);
        }
        if (currentText == 2)
        {
            UpdateStage(TutorialStage.MoveWASD);
            WASDImage.SetActive(true);
        }
        else
        {
            if (WASDImage.activeSelf)
            {
                WASDImage.SetActive(false);
            }
        }

        tutorialText.text = tutorialStrings[currentText];
        tutorialTextBg.text = tutorialStrings[currentText];
    }


    private void GatherWASDCondition(KeyCode keyCode)
    {
        if (!pressedKeys.Contains(keyCode))
        {
            pressedKeys.Add(keyCode);
        }
    }

    public void UpdateCurrentStage()
    {
        if (currentText < 4)
            UpdateText();
    }

    public void NewStage()
    {
        if (currentText < 5)
            UpdateText();
    }

    public void ShowAlertText(string beCarefulYouAreTakingDamageFallBack)
    {
        alertText.gameObject.SetActive(true);
        alertTextBG.gameObject.SetActive(true);
        alertText.text = beCarefulYouAreTakingDamageFallBack;
        alertTextBG.text = beCarefulYouAreTakingDamageFallBack;
    }
}