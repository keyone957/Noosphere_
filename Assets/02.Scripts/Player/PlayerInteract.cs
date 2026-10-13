using System;
using System.Collections;
using UnityEngine;
using Debug = NooSphere.Debug;
using Random = UnityEngine.Random;

public class PlayerInteract : Singleton<PlayerInteract>
{
    [Header("상호작용")] 
    public bool canInteract = true; //상호작용을 할 수 있는지(lock 조건에 이용)
    public bool canUse = false; // 증거물 사용 상호작용 할 수 있는지(InteractionMarkManager.cs에서 값을 관리함)
    private bool _isRepeatFalseCondition = false;
    public Action OnInteract;
    public Action OnEvidenceUse;
    public bool isInsideTrigger = false;
    public EventTrigger curTrigger;

    [Space(5)] [Header("정신세계 진입")] public bool isInMental = false;
    public Action OnMentalInteract;

    [Space(5)] [Header("증거물 사용")] public bool isUsingEvidence = false;
    void Update()
    {
        if (!UIManager.Instance.IsAnyUIOpen() && !DefaultUIController.Instance.IsAnyUIOpen())
        {
            //E키를 이용한 상호작용
            if (canInteract && InputRouter.Instance.ConsumeE())
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
                        InteractionMarkManager.Instance.DisableInteractionMarkUI(curTrigger.transform);
                    }
                    OnInteract = null;   
                }
            }

            // Z키를 이용한 증거물 사용하기
            if(canInteract && canUse && InputRouter.Instance.ConsumeQ())
            {
                if (OnEvidenceUse != null)
                {
                    Debug.LogWarning("OnEvidenceUse 에 등록되어있는 메소드 실행");
                    if (OnEvidenceUse != null)
                    {
                        int random = Random.Range(0, 5);
                        string id = "";
                        switch (random)
                        {
                            case 0:
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
                        OnEvidenceUse.Invoke();
                        InteractionMarkManager.Instance.DisableInteractionMarkUI(curTrigger.transform);
                    }
                    OnEvidenceUse = null;
                }
            }
        }
    }
    
    public void CheckTriggerOnceAgain()
    {
        //현재 위치한 곳에 트리거가 있다면 해당 트리거 실행 가능한지 다시 체크
        if (isInsideTrigger && curTrigger != null)
        {
            StartCoroutine(CheckTrigger(0.3f));
        }
    }

    IEnumerator CheckTrigger(float second)
    {
        yield return new WaitForSeconds(second);
        
        if (isInsideTrigger && curTrigger != null)
        {
            Debug.LogWarning("체크");
            curTrigger.OnTriggerEnter(GetComponent<CapsuleCollider>());
        }
    }
}
