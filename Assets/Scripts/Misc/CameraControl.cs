using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    Transform  _myTransform, _playerTr;
    float _posZ = -10f;
    public float followSpeed = 10f;

    void Awake()
    {
        _myTransform = transform;
        _playerTr = GameManager.Instance.pc.myTransform;
    }

    void LateUpdate()
    {
        Vector3 targetPos  = new Vector3(_playerTr.position.x, _playerTr.position.y, _posZ);
        _myTransform.position = targetPos;
      //  _myTransform.position = Vector3.MoveTowards(_myTransform.position, targetPos, Time.deltaTime * followSpeed);
    }
}
