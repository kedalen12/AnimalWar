﻿using System;
using System.Collections;
using System.Collections.Generic;
using Network;
using Player;
using UnityEngine;
using UnityEngine.Serialization;

/*
 * THIS SCRIPT WILL HANDLE PLAYER INPUTS AND SERVER ANSWERS TO THOSE INPUTS
 */
//[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
 public CharacterController playerController;
 public float maxYVelocity = 10f, maxSpeed = 5f;

 [Range(0f, 1f)] public float jumpControl = .3f;
 public float jumpHeight = 1f;
 private float _currentSpeed;
 private float _currentYVelocity;

 public float rotationSmoothDuration = .1f;
 public float speedSmoothDuration = .1f;

 public float gravitationalForce = -12f;
 private float _rotationSmoothVelocity, _speedSmoothVelocity;

 public bool isGravityEnabled = true;
 public Animator characterAnimator;
 private Vector2 _inputDir;
 private Transform _camaraTransform;
 public bool isDead = false;

 private void Start()
 {
  if (!(Camera.main is null)) _camaraTransform = Camera.main.transform;
 }


 public void SetGravityState()
 {
  isGravityEnabled = !isGravityEnabled;
 }

 public bool canMove = true;

 #region LOCALINPUTS

// ReSharper disable Unity.PerformanceAnalysis
 private void Update()
 {
  if(!canMove) return;
  if (isDead)
  {
    Move(Vector2.zero);
  }
  else
  {
   _inputDir = GetInputs().normalized;
   Move(_inputDir);
  }
 }

 private void Move(Vector2 inputDir)
 {
  if (inputDir != Vector2.zero)
  {
   SetCharacterRotation();
  }

  _currentSpeed = CalculateSpeed() * inputDir.magnitude;
  ApplyGravity();
  ApplyCharacterMovement();
  //SetAnimationValue("VelocityY", currentYVelocity);
  if (playerController.isGrounded)
  {
   _currentYVelocity = 0f;
  }

  SendMovement();
 }

 private void Jump()
 {
  if (!playerController.isGrounded) return;
  Debug.Log("Jumping");
  var jumpVel = Mathf.Sqrt(-2 * gravitationalForce * jumpHeight);
  _currentYVelocity = jumpVel;
 }

 private float CalculateSpeed()
 {
  return Mathf.SmoothDamp(_currentSpeed, maxSpeed, ref _speedSmoothVelocity,
   GetModifiedSmoothTime(speedSmoothDuration));
 }

 private Vector2 GetInputs()
 {
  return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
 }

 private void SetCharacterRotation()
 {
  if (_camaraTransform == null)
  {
   if (!(Camera.main is null)) _camaraTransform = Camera.main.transform;
  }
  float targetRotation = (Mathf.Atan2(_inputDir.x, _inputDir.y) * Mathf.Rad2Deg + _camaraTransform.eulerAngles.y);
  transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation,
   ref _rotationSmoothVelocity, GetModifiedSmoothTime(rotationSmoothDuration));
 }

 private void ApplyGravity()
 {
 if(!isGravityEnabled) return;
  if (_currentYVelocity <= maxYVelocity)
  {
   _currentYVelocity += Time.deltaTime * gravitationalForce;
  }
 }

 private void ApplyCharacterMovement()
 {
  var velocity = transform.forward * _currentSpeed + Vector3.up * _currentYVelocity;
  playerController.Move(velocity * Time.deltaTime);
 }
 
 private float GetModifiedSmoothTime(float smoothTime)
 {
  if (playerController.isGrounded)
  {
   return smoothTime;
  }

  if (jumpControl == 0f)
  {
   return float.MaxValue;
  }

  return smoothTime / jumpControl;
 }

 #endregion

 #region UPDATESERVERINPUTS

 private void SendMovement()
 {
  var transform1 = transform;
  ClientSend.PlayerMovement(transform1.position, transform1.rotation);
 }

 #endregion

 public void SetDeathState()
 {
  isDead = !isDead;
 }

 private void ResetSpeed()
 {
  
 }
 public void Slow(int percentage)
 {
  if (percentage != 0)
  {
   _currentSpeed *= percentage / 0b1100100;
  }
  else
  {
   _currentSpeed = 5;
  }
 }
}
