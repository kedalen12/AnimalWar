﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
public class LogoManager : MonoBehaviour
{
    public VideoPlayer player;
    private int _currentFrame = 0;

    private void Start()
    {
        Constants.IpAdress = ReadArguments("-ip");
        Constants.User = ReadArguments("-user");
        Constants.Pass = ReadArguments("-pass");
        if (string.IsNullOrEmpty(Constants.IpAdress))
        {
            Constants.UsualLaunch = true;
        }
        using (var sw = File.AppendText(Constants.TestPath))
        {
            sw.WriteLine(Constants.IpAdress);
            sw.WriteLine(Constants.User);
            sw.WriteLine(Constants.Pass);
        }	
    }
    private string ReadArguments(string argName)
    {
        var args = System.Environment.GetCommandLineArgs();
        for (var i = 0; i < args.Length; i++)
        {
            if (args[i] == argName && args.Length > i + 1)
            {
                return args[i + 1];
            }
        }
        return null;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(1);
        }
        _currentFrame++;
        if (_currentFrame < 10) return;
        if(!player.isPlaying){
            SceneManager.LoadScene(1);
        }
    }
}
