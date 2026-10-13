using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AssignPlayer : MonoBehaviour
{
    private CinemachineVirtualCamera _loungeCamera;
    [SerializeField] private CinemachineVirtualCamera _dialogueCamera;
    public bool isDialogueCamera = false;

    private void Awake()
    {
        _loungeCamera = GetComponent<CinemachineVirtualCamera>();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        DoAssign();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        DoAssign();
    }

    private void DoAssign()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            _loungeCamera.LookAt = player.transform;

            if (isDialogueCamera)
            {
                PlayerController.Instance.SetDialogueCam(_dialogueCamera);
            }
        }
        else
        {
            Debug.LogWarning("플레이어가 없습니다");
        }
    }
}
