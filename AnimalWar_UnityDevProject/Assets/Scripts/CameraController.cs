﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraController : MonoBehaviour
{
    public float mouseSensitivity;
    public bool invertY;
    public bool invertX;
    private float _yaw;
    private float _pitch;
    public Transform target;
    [Range(2f, 8f)]
    public float dstToTarget;
    public float minY = -1;
    public float maxY = 15;
    public float zoomSensitivity = .5f;
    public float rotationSmoothTime = .1f;
    private Vector3 _rotationSmoothVelocity;
    private Vector3 _currentRotation;
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;     
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false; 
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        } 
        // ReSharper disable once Unity.PerformanceCriticalCodeNullComparison
        if (target == null)
        {
            SearchTarget();
        }
        else
        {
            var zoom = Input.GetAxis("Mouse ScrollWheel");
      
            if (zoom < 0)
            {
                Debug.Log("OUT");
                dstToTarget += zoomSensitivity;
            }
            else if(zoom > 0){
                Debug.Log("INS");
                dstToTarget -= zoomSensitivity;
            }

            dstToTarget = Mathf.Clamp(dstToTarget, 2f, 8f);
        }

    }

    private void SearchTarget()
    {
       target = GameObject.FindWithTag("CAMARA_TARGET").transform;
    }

    private void LateUpdate()
    {
        if(target == null) return;
        GetYawAndPitch();
        _currentRotation = Vector3.SmoothDamp(_currentRotation, new Vector3(_pitch, _yaw), ref _rotationSmoothVelocity, rotationSmoothTime);
        //targetRotation = new Vector3(_pitch, _yaw);
        transform.eulerAngles = _currentRotation;
        transform.position = target.position - transform.forward * dstToTarget;
    }
    
    private void GetYawAndPitch()
    {
        if (!invertX)
        {
            _yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        }
        else
        {
            _yaw -= Input.GetAxis("Mouse X") * mouseSensitivity;

        }

        if (!invertY)
        {
            _pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
 
        }
        else
        {
            _pitch += Input.GetAxis("Mouse Y")* mouseSensitivity;
        }
        //Debug.Log($"UNCLAMPED PITCH = {_pitch}, UNCLAMPED _YAW = {_yaw}");
        _pitch = Mathf.Clamp(_pitch, minY, maxY);
        //_yaw = Mathf.Clamp(_yaw, minX, maxX);
        //Debug.Log($"CLAMPED PITCH = {_pitch}, CLAMPED _YAW = {_yaw}");

    }
}
