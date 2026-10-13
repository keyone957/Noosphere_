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
    [SerializeField] private float _frontAngle = 40f;
    [Space(5)][Header("NPC 관련")]
    public GameObject npcCameraPoint;
    public bool isNpc;
    [Space(5)] [Header("사용자 안내 키 UI 관련")] public bool isDoor;
    public bool isDoorOpen;
    [Space(5)][Header("사용자 카드키 관련")]
    public bool isCardKeyDoor;
    [System.Serializable]
    public class KeyInfo
    {
        public string evidenceID;
        public string resultID;
    }
    
    [Header("사용 증거물 관련")] 
    public List<KeyInfo> keyInfos = new List<KeyInfo>();
    private void OnEnable()
    {
        isDoorOpen = false;
    }

    //플레이어가 트리거 내에 진입한다면
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && PlayerInteract.Instance.canInteract)
        {
            PlayerInteract.Instance.isInsideTrigger = true;
            PlayerInteract.Instance.curTrigger = this;
            if (!IsPlayerFront(other.transform))
            {
                //TriggerDebugScript.Instance.WhenNotFront();
                return;
            }
            else
            {
                //TriggerDebugScript.Instance.WhenFront();
            }


            if (isCardKeyDoor)
            {
                NooSphere.Debug.LogWarning("문과 상호작용 가능");
                // E 열기는 디폴트로 띄우기
                // 하위에 키 UI를 추가할 부모 오브젝트 찾기
                Transform parent = gameObject.GetComponentInChildren<InteractionMark>(true).transform;
                // 부모 오브젝트 하위에, 키 관련 UI가 있다면 초기화
                foreach (Transform child in parent)
                {
                    Destroy(child.gameObject);
                }

                if (isDoor)
                {
                    if (!isDoorOpen)
                    {
                        // 방 이름 추가하기
                        GameObject roomName = Instantiate(GetRoomName());
                        roomName.transform.SetParent(parent, false);
                        GameObject interactionKey = Instantiate(InteractionMarkManager.Instance._openDoorKeyPrefab);
                        interactionKey.transform.SetParent(parent, false);

                        if (gameObject.name == "EventInteractionTrigger-R102" && DataManager.Instance._quiz["Quiz_003"].isSolved)
                        {
                            // 문 열기 액션 추가
                            PlayerInteract.Instance.OnInteract = null;
                            PlayerInteract.Instance.OnInteract += () =>
                            {
                                EventManager.Instance.ExecuteEvent("Event_B036").Forget();
                            };
                            parent.gameObject.SetActive(true);
                            return;
                        }
                        // 문 열기 액션 추가
                        PlayerInteract.Instance.OnInteract = null;
                        PlayerInteract.Instance.OnInteract += () =>
                        {
                            EventManager.Instance.ExecuteEvent(eventIdList[0]).Forget();
                        };

                        // 플레이어의 인벤토리에 관련 증거물 있는지 체크
                        foreach (var key in keyInfos)
                        {
                            EvidenceStructure evidence = DataManager.Instance._evidences[key.evidenceID];
                            if (evidence.canUse == 'Y' && InventoryManager.Instance.IsEvidenceInInventory(evidence.evidenceId))
                            {
                                GameObject evidenceKey = Instantiate(InteractionMarkManager.Instance._useEvidenceKeyPrefab);
                                evidenceKey.transform.SetParent(parent, false);
                                PlayerInteract.Instance.canUse = true;

                                Debug.LogWarning("OnEvidenceUse 액션에 메소드 등록");
                                PlayerInteract.Instance.OnEvidenceUse = null;
                                PlayerInteract.Instance.OnEvidenceUse += () =>
                                {
                                    InventoryManager.Instance.isUsingEvidence = true;
                                    InventoryManager.Instance.GetComponent<InventoryNavigator>().isUsingCardKey = true;
                                    InventoryManager.Instance.GetComponent<InventoryNavigator>().keyInfos = this.keyInfos;
                                    UIManager.Instance.OpenUI(UIManager.Instance.inventoryUI);
                                };
                                break;
                            }
                        }
                        parent.gameObject.SetActive(true);
                    }
                }
            }
            else
            {
                CheckTriggerAvail();
            }
            
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
        InteractionMarkManager.Instance.DisableInteractionMarkUI(transform);
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
        PlayerInteract.Instance.OnEvidenceUse = null;
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
                
                if (canExecute && PlayerInteract.Instance.canInteract && PlayerController.Instance.canMove && PlayerController.Instance.GetComponent<MentalEnterProcess>().CanEnterProcess())
                {
                    string[] results = EventManager.Instance.CheckConditions(_curEvent);

                    //결과 실행
                    if (results != null && results.Length > 0)
                    {
                        if (!EventManager.Instance.IsConditionMet() &&
                            string.IsNullOrEmpty(_curEvent.conditionFalseResults[0]))
                        {
                            Debug.LogWarning($"{_curEvent.eventId}는 조건을 만족하지 못했으나 conditionFalseResult도 존재하지 않아 할당하지 않음");
                            continue;
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
                                PlayerInteract.Instance.OnInteract = null;
                                Debug.Log("OnInteract 할당");
                                //플레이어의 OnInteract 액션에 실행 메소드 할당
                                PlayerInteract.Instance.OnInteract += () =>
                                {
                                    EventManager.Instance.ExecuteEvent(_curEvent.eventId).Forget();
                                    InteractionMarkManager.Instance.DisableInteractionMarkUI(transform);
                                };
                                InteractionMarkManager.Instance.EnableInteractionMarkUI(this.transform, _curEvent.eventId);
                                break;
                            case "EventMentalEnterTrigger":
                                //플레이어에게 ? 띄우기
                                PlayerInteract.Instance.OnMentalInteract = null;
                                Debug.Log("OnMentalInteract에 할당");
                                //정신세계 진입 프로세스의 OnInteract 액션에 실행 메소드 할당
                                PlayerInteract.Instance.OnMentalInteract += () =>
                                {
                                    EventManager.Instance.ExecuteEvent(_curEvent.eventId).Forget();
                                    InteractionMarkManager.Instance.DisableInteractionMarkUI(transform);
                                };
                                InteractionMarkManager.Instance.EnableInteractionMarkUI(this.transform, _curEvent.eventId);
                                break;
                        }
                        return;
                    }
                }
            }
        }
    }

    // 플레이어가 트리거를 정면 방향으로 진입했는지 체크하는 함수
    protected bool IsPlayerFront(Transform player)
    {
        Vector3 triggerDirection = (transform.GetChild(0).position - player.position).normalized;
        float angle = Vector3.Angle(player.forward, triggerDirection);
        
        if (angle <= _frontAngle)
        {
            //Debug.Log("플레이어가 정면을 바라보고 들어 옴.");
            return true;
        }
        //Debug.Log("플레이어가 뒤돌거나 옆을 보고 들어오지 않음");
        //현재 이벤트 초기화하기
        _curEvent = null;
        InteractionMarkManager.Instance.DisableInteractionMarkUI(transform);

        //npc 변수 초기화
        PlayerController.Instance.npcCam = null;
        PlayerController.Instance.npcState = null;
        
        //플레이어에게 할당된 액션 다 초기화
        PlayerInteract.Instance.OnInteract = null;
        PlayerInteract.Instance.OnMentalInteract = null;
        return false;
    }

    private GameObject GetRoomName()
    {
        int splitIndex = gameObject.name.LastIndexOf('-');
        string roomName = gameObject.name.Substring(splitIndex+1, gameObject.name.Length - (splitIndex+1));

        switch (roomName)
        {
            case "R101":
                return InteractionMarkManager.Instance._R101Prefab;
            case "R102":
                return InteractionMarkManager.Instance._R102Prefab;
            case "R103":
                return InteractionMarkManager.Instance._R103Prefab;
            case "R104":
                return InteractionMarkManager.Instance._R104Prefab;
            default:
                return null;
        }
    }
}

