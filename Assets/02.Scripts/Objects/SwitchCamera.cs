using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class SwitchCamera : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera camera1;
    [SerializeField] private bool _onCam1;
    [SerializeField] private CinemachineFreeLook camera2;
    [SerializeField] private bool _onCam2;
    [SerializeField] private CinemachineVirtualCamera cutSceneCamera;
    [SerializeField] private bool _onCam3;
    [SerializeField] private bool isMainCamTrigger; // true면 메인카메라(camera1), false면 나머지 카메라 조건 활성화
    public bool isTest;
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (isMainCamTrigger)
        {
            ActivateCamera(camera1);
        }
        else
        {
            bool isEventExecuted = DataManager.Instance._events["Event_C082"].isExecuted;
            // bool isEventExecuted =isTest;
            if (isEventExecuted)
                ActivateCamera(cutSceneCamera);
            else
                ActivateCamera(camera2);
        }
    }
    
    private void ActivateCamera(CinemachineVirtualCamera targetCamera)
    {
        camera1.enabled = (targetCamera == camera1);
        cutSceneCamera.enabled = (targetCamera == cutSceneCamera);
        camera2.enabled = false; // FreeLook 비활성화
    }
    
    private void ActivateCamera(CinemachineFreeLook targetCamera)
    {
        camera1.enabled = false;
        cutSceneCamera.enabled = false;
        camera2.enabled = (targetCamera == camera2);
    }
}
