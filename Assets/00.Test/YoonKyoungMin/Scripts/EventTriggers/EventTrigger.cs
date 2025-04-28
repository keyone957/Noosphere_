using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Debug = NooSphere.Debug;

public class EventTrigger : MonoBehaviour
{
    [Header("이벤트 목록")]
    public List<string> eventIdList = new List<string>();
    private EventStructure _curEvent;
    [Space(5)] [Header("트리거 설정")] 
    [SerializeField] private bool _canDestroy = false;
    [Space(5)][Header("NPC 관련")]
    public GameObject npcCameraPoint;
    public bool isNpc;
    
    
    //플레이어가 트리거 내에 진입한다면
    protected void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && PlayerInteract.Instance.canInteract)
        {
            CheckTriggerAvail();
            PlayerInteract.Instance.isInsideTrigger = true;
            PlayerInteract.Instance.curTrigger = this;
            
            if (isNpc)
            {
                PlayerController.Instance.npcCam = npcCameraPoint;
                PlayerController.Instance.npcState = transform.GetChild(0).GetComponent<NpcState>();
            }
        }
    }
    
    //플레이어가 트리거 내에서 나간다면
    protected void OnTriggerExit(Collider other)
    {
        //플레이어에게 ? 없애기
        PlayerInteract.Instance.HideInteractionMark();
        PlayerInteract.Instance.isInsideTrigger = false;
        PlayerInteract.Instance.curTrigger = null;
        
        //현재 이벤트 초기화하기
        _curEvent = null;
        
        //npc 변수 초기화
        PlayerController.Instance.npcCam = null;
        PlayerController.Instance.npcState = null;
        
        //플레이어에게 할당된 액션 다 초기화
        PlayerInteract.Instance.OnInteract = null;
        PlayerInteract.Instance.OnMentalInteract = null;
    }

    public void CheckTriggerAvail()
    {
        bool canExecute = false;
        foreach (var eventID in eventIdList)
        {
            if (EventManager.Instance.CheckExecutable(eventID))
            {
                _curEvent = DataManager.Instance._events[eventID];
                canExecute = true;
                break;
            }
        }

        if (canExecute)
        {
            string[] results = EventManager.Instance.CheckConditions(_curEvent);

            //결과 실행
            if (results != null && results.Length > 0)
            {
                if (!EventManager.Instance.IsConditionMet() &&
                    string.IsNullOrEmpty(_curEvent.conditionFalseResults[0]))
                {
                    Debug.LogWarning($"{_curEvent.eventId}는 조건을 만족하지 못했으나 conditionFalseResult도 존재하지 않아 할당하지 않음");
                    return;
                }
                //트리거 종류에 따라 할당하기
                switch (tag)
                {
                    case "EventTrigger":
                        //바로 실행 메소드 호출
                        EventManager.Instance.ExecuteEvent(_curEvent.eventId).Forget();
                        Debug.Log("바로 실행");
                        break;
                    case "EventInteractionTrigger":
                        //플레이어에게 ? 띄우기
                        PlayerInteract.Instance.ShowInteractionMark();
                        PlayerInteract.Instance.OnInteract = null;
                        Debug.Log("OnInteract 할당");
                        //플레이어의 OnInteract 액션에 실행 메소드 할당
                        PlayerInteract.Instance.OnInteract += () =>
                        {
                            EventManager.Instance.ExecuteEvent(_curEvent.eventId).Forget();
                        };
                        break;
                    case "EventMentalEnterTrigger":
                        //플레이어에게 ? 띄우기
                        PlayerInteract.Instance.ShowInteractionMark();
                        PlayerInteract.Instance.OnMentalInteract = null;
                        Debug.Log("OnMentalInteract에 할당");
                        //정신세계 진입 프로세스의 OnInteract 액션에 실행 메소드 할당
                        PlayerInteract.Instance.OnMentalInteract += () =>
                        {
                            EventManager.Instance.ExecuteEvent(_curEvent.eventId).Forget();
                        };
                        break;
                }
            }
        }
    }
}
