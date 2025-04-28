using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = NooSphere.Debug;
using Random = UnityEngine.Random;

public class PlayerInteract : Singleton<PlayerInteract>
{
    [Header("상호작용")] 
    public bool canInteract = true; //상호작용을 할 수 있는지(lock 조건에 이용)
    [SerializeField] private GameObject _interactionMark;
    private bool _isRepeatFalseCondition = false;
    public Action OnInteract;
    public bool isInsideTrigger = false;
    public EventTrigger curTrigger;

    [Space(5)] [Header("정신세계 진입")] public bool isInMental = false;
    public Action OnMentalInteract;

    [Space(5)] [Header("증거물 사용")] public bool isUsingEvidence = false;
    void Update()
    {
        if (!UIManager.Instance.IsAnyUIOpen())
        {
            //E키를 이용한 상호작용
            if (canInteract && Input.GetKeyDown(KeyCode.E))
            {
                if (OnInteract != null)
                {
                    Debug.LogWarning("OnInteract 에 등록되어있는 메소드 실행");
                    if (OnInteract != null)
                    {
                        int random = Random.Range(0, 5);
                        string id = "";
                        switch (random)
                        {
                            case 0 :
                                id = "Soundresource_030";
                                break;
                            case 1:
                                id = "Soundresource_031";
                                break;
                            case 2:
                                id = "Soundresource_032";
                                break;
                            case 3:
                                id = "Soundresource_033";
                                break;
                            case 4:
                                id = "Soundresource_034";
                                break;
                        }
                        SoundManager.Instance.PlaySFX(id);
                        OnInteract.Invoke();
                    }
                    OnInteract = null;   
                }
            }
        }
    }

    public void ShowInteractionMark()
    {
        _interactionMark.SetActive(true);
    }

    public void HideInteractionMark()
    {
        _interactionMark.SetActive(false);
    }
    
    public void InitUsingEvidence()
    {
        isUsingEvidence = false;
        InventoryManager.Instance.GetComponent<InventoryNavigator>().InitUsingEvidence();
    }
}
