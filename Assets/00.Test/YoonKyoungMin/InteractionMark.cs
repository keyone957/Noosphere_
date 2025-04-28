using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionMark : MonoBehaviour
{
    [SerializeField] private Camera _curCamera;

    private void OnEnable()
    {
        _curCamera = PlayerController.Instance._mainCamera;
    }

    void LateUpdate()
    {
        if (_curCamera != null)
        {
            transform.LookAt(transform.position + _curCamera.transform.forward);
        }
    }
}
