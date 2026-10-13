using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Debug = NooSphere.Debug;

public class InteractionMarkManager : MonoBehaviour
{
    public static InteractionMarkManager Instance { get; private set; }
    [Header("키 UI들")]
    public GameObject _interactionKeyPrefab; // 상호작용 키 UI
    public GameObject _enterMentalKeyPrefab; // 세계 진입 키 UI
    public GameObject _useEvidenceKeyPrefab; // 증거물 사용하기 키 UI
    public GameObject _openDoorKeyPrefab; // 문 열기 UI
    public GameObject _talkNpcPrefab; // NPC 대화 UI

    [Header("방 이름")]
    public GameObject _R101Prefab;
    public GameObject _R102Prefab;
    public GameObject _R103Prefab;
    public GameObject _R104Prefab;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 상호작용 키 활성화 함수
    public void EnableInteractionMarkUI(Transform trigger, string eventID)
    {
        // EventTrigger 하위에 키 UI를 추가할 부모 오브젝트 찾기
        Transform parent = trigger.GetComponentInChildren<InteractionMark>(true).transform;

        // 부모 오브젝트 하위에, 키 관련 UI가 있다면 초기화
        foreach (Transform chiild in parent)
        {
            Destroy(chiild.gameObject);
        }

        // 상호작용 트리거인 경우
        if (trigger.tag == "EventInteractionTrigger")
        {
            EventTrigger eventTrigger = trigger.GetComponent<EventTrigger>();
            GameObject interactionKey;
            if (eventTrigger.isDoor)
            {
                if (!eventTrigger.isDoorOpen)
                {
                    if(eventID == "Event_B028")
                    {
                        GameObject roomName = Instantiate(_R101Prefab);
                        roomName.transform.SetParent(parent, false);
                    }

                    interactionKey = Instantiate(_openDoorKeyPrefab);
                    interactionKey.transform.SetParent(parent, false);
                }
            }
            else if (eventTrigger.isNpc)
            {
                interactionKey = Instantiate(_talkNpcPrefab);
                interactionKey.transform.SetParent(parent, false);
            }
            else
            {
                interactionKey = Instantiate(_interactionKeyPrefab);
                interactionKey.transform.SetParent(parent, false);
            }
            
            EventStructure _event = DataManager.Instance._events[eventID];

            PlayerInteract.Instance.canUse = false;
            foreach (var condition in _event.conditions)
            {
                if (condition.StartsWith("Evidence"))
                {
                    EvidenceStructure evidence = DataManager.Instance._evidences[condition];
                    if(evidence.canUse == 'Y' && InventoryManager.Instance.IsEvidenceInInventory(evidence.evidenceId))
                    {
                        GameObject evidenceKey = Instantiate(_useEvidenceKeyPrefab);
                        evidenceKey.transform.SetParent(parent, false);
                        PlayerInteract.Instance.canUse = true;
                        
                        Debug.LogWarning("OnEvidenceUse 액션에 메소드 등록");
                        PlayerInteract.Instance.OnEvidenceUse = null;
                        PlayerInteract.Instance.OnEvidenceUse += () =>
                        {
                            InventoryManager.Instance.isUsingEvidence = true;
                            InventoryManager.Instance.GetComponent<InventoryNavigator>()
                                .SetEvidenceUseEventID(_event);
                            UIManager.Instance.OpenUI(UIManager.Instance.inventoryUI);
                        };
                        break;
                    }
                }
            }
        }
        else if (trigger.tag == "EventMentalEnterTrigger") // 정신세계 진입 트리거인 경우
        {
            GameObject mentalKey = Instantiate(_enterMentalKeyPrefab);
            mentalKey.transform.SetParent(parent, false);
        }
        else
        {
            Debug.LogError("정신세계 트리거의 mark parent가 올바르지 않음");
        }
        
        parent.gameObject.SetActive(true);
    }

    // 키 UI 비활성화
    public void DisableInteractionMarkUI(Transform trigger)
    {
        if (trigger != null && (trigger.CompareTag("EventInteractionTrigger") ||
                                trigger.CompareTag("EventMentalEnterTrigger")))
        {
            Transform parent = trigger.GetComponentInChildren<InteractionMark>(true).transform;
            if (parent != null)
            {
                parent.GetComponent<Animator>().SetTrigger("Hide");
            }
        }
    }
}
