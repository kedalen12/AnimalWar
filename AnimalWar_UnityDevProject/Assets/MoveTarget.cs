using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MoveTarget : MonoBehaviour
{
    public bool Active;
    private Terrain _myTerrain;
    private Camera _mainCam;
    public LayerMask collideMask;
    private CameraController _controller;
    public Transform player;
    private SpriteRenderer _spriteRenderer;
    private Bounds _bounds;
    public bool positionIsAdecuate;
    public Vector3 currentPos;

    private void Awake()
    {
        _myTerrain = Terrain.activeTerrain;
        _mainCam = Camera.main;
        if (!(_mainCam is null)) _controller = _mainCam.GetComponent<CameraController>();
        //player = GameObject.FindGameObjectWithTag("LPlayer").transform;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.enabled = false;
    }
    private void Update()
    {
        _spriteRenderer.enabled = Active;
        if (!Active)  return;
        var ray = _mainCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, 25f + _controller.dstToTarget, collideMask))
        {
            _spriteRenderer.enabled = true;
            var newPos = new Vector3();
            newPos.x = hitInfo.point.x;
            newPos.z = hitInfo.point.z;
            newPos.y = _myTerrain.SampleHeight(hitInfo.point) + .1f;
            transform.position = newPos;
            currentPos = new Vector3(newPos.x, _myTerrain.SampleHeight(hitInfo.point), newPos.z);
            positionIsAdecuate = true;
        }
        else
        {
            positionIsAdecuate = false;

            _spriteRenderer.enabled = false;
        }
    }
}
