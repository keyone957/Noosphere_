using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;


public class SetLoungeCamera : MonoBehaviour
{
    private CinemachineFreeLook _loungeCamera;

    private void Awake()
    {
        _loungeCamera = GetComponent<CinemachineFreeLook>();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        AssignPlayer();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AssignPlayer();
    }

    private void AssignPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            _loungeCamera.Follow = player.transform;
            _loungeCamera.LookAt = player.transform;
        }
        else
        {
            Debug.LogWarning("플레이어가 없습니다");
        }
    }

    public void AssignLoungeCamera()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            _loungeCamera.Follow = player.transform;
            _loungeCamera.LookAt = player.transform;
        }
        else
        {
            Debug.LogWarning("플레이어가 없습니다");
        }
    }
}
