﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ResetLevel : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
    
        if(other.gameObject.CompareTag("Player"))
        { SceneManager.LoadScene(2);

        }
    }
}
