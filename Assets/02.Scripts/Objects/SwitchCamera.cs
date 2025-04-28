using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class SwitchCamera : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera camera1;
    [SerializeField] private bool _onCam1;
    [SerializeField] private CinemachineVirtualCamera camera2;
    [SerializeField] private bool _onCam2;
    
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_onCam1)
            {
                camera1.enabled = true;
                camera2.enabled = false;
                
                return;
            }

            if (_onCam2)
            {
                camera2.enabled = true;
                camera1.enabled = false;
                
                return;
            }
        }
    }
}
