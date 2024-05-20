using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Vector3 _offset;
    [SerializeField] private Transform _target;
    [SerializeField] private float _smoothTime;
    private Vector3 _velocity = Vector3.zero;

    private void Awake()
    {
        _offset = transform.position - _target.position;
    }
    
    private void LateUpdate()
    {
        if (_target == null) return;
        // Check if the target is not on screen
        var pos = Camera.main.WorldToScreenPoint(_target.position);
        bool onScreen = Screen.safeArea.Contains(pos);
        
        if (onScreen == false) {
            transform.position = _target.position + _offset;
            return;
        }
        
        Vector3 targetPosition = _target.position + _offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _velocity, _smoothTime);
    }
}
