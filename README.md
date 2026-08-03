# NoosPhere

진행 기간: 2024. 09 ~ 2025. 12

사용한 기술 스택: C#, Unity

개발 인원(역할): 개발자 2명 + 기획자 2명 + 디자이너 1명 + 사운드 1명

한 줄 설명: 3D 추리 게임

비고: 팀 프로젝트/최우수상 수상작

## 게임 플레이 풀 영상

---
[스팀 링크](https://store.steampowered.com/app/3618090/Noosphere/)

[게임 플레이 풀영상](https://www.youtube.com/watch?v=BJhUm7ZLg7I)

[게임 트레일러 영상](https://youtu.be/XdEZNaAy4jw?si=auye50-PDBeIfqMf)


## 프로젝트 소개

---

![Image](https://github.com/user-attachments/assets/e2220397-9438-43ab-9d71-c634e22450b6)

## 나의 역할

---

1. 게임 데이터 관리
    - 구글 스프레드시트를 이용한 각종 이벤트를 관리하는 이벤트 매니저 및 게임 플로우 관리
    - 시트 내 데이터들을 리플렉션을 사용하여 객체화한 DataManager 구현
    - 대화 시스템 구성
2. UGUI 기반 게임 기믹(미니게임) 구현
    - 게임 내 라디오 게임, 책장 순서 맞추기 미니게임 등 모든 기믹들 구현
3. 연출 및 기능 구현
    - 포스트 프로세싱을 이용한 플레이어 정신세계 이동 연출 및 기능 구현
    - Unity Timeline 기능을 이용한 연출
    - 각종 게임 내 카메라 연출 구현
4. 플레이어 이동 및 물체 상호작용 구현
5. 게임 내 UI 구현
    - 대화 시스템, 타이틀 메뉴 등 다양한 UI 구현
    - Scriptable Object 기반 이벤트 채널 패턴을 활용한 UI 구현


<details>
<summary><h2>기획 변경에 빠르게 대응하기 위한 스프레드시트 기반 이벤트 실행 시스템 구축</h2></summary>

## 개요

> 4개월 안에 추리게임의 3개 엔딩을 구현해야 했기 때문에, 스토리 분기, 대사, 증거물 등 반복적으로 수정되는 이벤트 데이터를 개발자가 코드로 일일이 반영하는 방식으로는 기획 변경에 빠르게 대응하기 어렵다고 판단했습니다. 이에 기획자가 구글 스프레드 시트에서 데이터를 작성, 수정하고 게임에서는 해당 데이터를 불러와 실행할 수 있는 이벤트 시스템을 구축했습니다.

### `구글 스프레드 시트를 이용한 게임 이벤트 관리`

구글 스프레드 시트를 이용해 각 게임에 필요한 이벤트를 규칙을 정하여 작성하도록 하여 게임에 반영되도록 하는 시스템을 구현.

![스프레드시트 예시](https://github.com/user-attachments/assets/cd88a576-5cff-440c-8d4e-f42be3c9e403)

⇒ 이벤트 동작 방식 플로우 차트

![이벤트 동작 방식 플로우 차트](https://github.com/user-attachments/assets/690efdab-3a76-416d-8f09-22624046cb4a)

![협업 구조 예시](https://github.com/user-attachments/assets/ab92bbe9-3487-49a5-b609-143161d5dc4f)

⇒ 여러 직군이 동일한 이벤트 데이터를 기준으로 작업할 수 있는 협업 구조

## `이벤트 관리 및 실행`

- 구글 시트 데이터를 CSV 형식으로 불러와 파싱.
- Reflection을 이용해 시트 헤더와 구조체 필드를 매핑하여 데이터를 객체화 하고, 실제 분기는 `EventManager`가 조건·결과·다음 이벤트 ID를 기준으로 처리합니다.
- 기획자가 스프레드시트를 수정하면 코드 수정 없이 바로 게임에 적용되게 구현.
- 이벤트 실행에 필요한 데이터(이벤트, 기믹, 증거물 상세 정보 및 속성, 대화, 사운드, 스프라이트 이미지 등…)를 모두 스프레드시트로 관리하여 다른 부서 간 협업이 원활하게 진행되도록 하였음.

## `이벤트 실행 과정`

**`EventManager`** 는 이벤트 실행을 총괄하며 각 이벤트의 실행 가능 여부, 조건, 결과 실행, 다음 이벤트로 연결을 관리함

이벤트는 Dialogue, Effect, Quiz, Evidence등 게임에 필요한 객체들로 구성이 되어있으며 각 이벤트가 끝날 때까지 **`UniTask`** 를 이용해 비동기로 처리하도록 설계함

또한 **`EventTrigger`** 를 통해 플레이어가 특정 위치에 진입하면 이벤트 실행 가능 여부를 검사 후에 조건을 확인하고 이벤트가 실행되도록 구현

<details>
<summary><code>EventManager.cs</code></summary>

```csharp
using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class EventManager : Singleton<EventManager>
{
    //스테이지 번호
    public enum ChapterInfo
    {
        Prologue,
        Stage1,
        Stage2,
        Final
    }
    public enum RoomInfo
    {
        Room_101,
        Room_102,
        Room_103,
        Room_104
    }
    public RoomInfo curRoomInfo;
    public ChapterInfo curChapterInfo;
    
    //다음 이벤트 정보
    public string startEventID;
    public string currentEventID;
    public string nextEventID = "";
    
    //이벤트 성공 여부 
    private bool _isEventSuccess = false;
    private bool _isQuizSolved = true;
    [SerializeField] private bool _isRepeatFalse = false;
    [SerializeField] private bool _isConditionMet = false;
    
    void Awake()
    {
        //게임 시작 시, 스테이지 정보 초기화
        curChapterInfo = ChapterInfo.Prologue;
        curRoomInfo = RoomInfo.Room_101;
        nextEventID = startEventID;
    }

    void Start()
    {
        ExecuteEvent(startEventID).Forget();
    }
 
    //현재 실행될 수 있는 이벤트인지 검사 -> 실행 가능하다면 플레이어에게 ? 띄우기
    public bool CheckExecutable(string eventID)
    {
        //변수 초기화
        _isRepeatFalse = false;
        
        //실행하는 이벤트가 이벤트 목록 내에 존재하는지 체크
        if (!DataManager.Instance._events.ContainsKey(eventID))
        {
            Debug.LogWarning($"{eventID} 이벤트가 존재하지 않습니다.");
            return false;
        }
        
        //nextEventID가 비어있지 않은데, 실행하고자 하는 이벤트ID와 같지 않다면
        if (!string.IsNullOrEmpty(nextEventID) && nextEventID != eventID)
        {
            Debug.LogWarning($"현재 실행되어야 하는 이벤트는 {eventID}가 아니라 {nextEventID} 입니다.");
            return false;
        }

        //repeatType이 false인데 이미 실행되었다면
        EventStructure eventData = DataManager.Instance._events[eventID];
        if (!eventData.repeatType && eventData.isExecuted)
        {
            Debug.LogWarning($"{eventID} 이벤트의 repeatType {eventData.repeatType} , isExecuted : {eventData.isExecuted}");
            //repeat false result가 존재한다면
            if (!string.IsNullOrEmpty(eventData.repeatFalseResult))
            {
                Debug.LogWarning($"{eventID}는 repeat false result {eventData.repeatFalseResult}가 존재합니다.");
                _isRepeatFalse = true;
                return true;
            }
            
            //repeat false result가 존재하지 않다면
            return false;
        }
        return true;
    }

    public string[] CheckConditions(EventStructure _curEvent)
    {
        //변수 초기화
        _isConditionMet = false;
        
        //_isRepeatFalse 가 true이면 해당 이벤트를 실행할 준비
        if (_isRepeatFalse)
        {
            string[] results = new string[1];
            results[0] = _curEvent.repeatFalseResult;
            return results;
        }
            
        //_isRepeatFalse 가 false이면 해당 이벤트의 조건을 만족하는지 검사
        if (_curEvent.conditionType == "and")
        {
            int conditionNum = 1;
            //conditionType이 and이면 추가적인 조건 검증을 해야 햠.
            foreach (var conditionID in _curEvent.conditions)
            {
                if (_curEvent.IsConditionMet(conditionID))
                {
                    Debug.Log($"{_curEvent.eventId}의 조건{conditionNum} 만족");
                    _isConditionMet = true;
                }
                else
                {
                    Debug.LogWarning($"{_curEvent.eventId}의 조건{conditionNum} 불만족");
                    _isConditionMet = false;
                    break;
                }
                conditionNum++;
            }

            //조건을 모두 만족했다면 결과 실행 준비
            if (_isConditionMet)
            {
                return _curEvent.results;
            }
            //조건을 모두 만족하지 않았다면 condition false result를 실행할 준비
            return _curEvent.conditionFalseResults;
        }
        if (_curEvent.conditionType == "or")
        {
            //conditionType이 or 인 경우는 결과 바로 실행
            if (_curEvent.conditions == null || _curEvent.conditions.Length == 0)
            {
                _isConditionMet = true;
            }
            else
            {
                int conditionNum = 1;
                foreach (var conditionID in _curEvent.conditions)
                {
                    if (_curEvent.IsConditionMet(conditionID))
                    {
                        Debug.Log($"{_curEvent.eventId}의 조건{conditionNum} 만족");
                        _isConditionMet = true;
                    }
                    else
                    {
                        Debug.LogWarning($"{_curEvent.eventId}의 조건{conditionNum} 불만족");
                    }
                    conditionNum++;
                }
            }

            if (!_isConditionMet)
            {
                Debug.LogWarning($"{_curEvent.eventId}의 conditionType은 or이지만 조건을 모두 만족하지 않았아 isConditionMet이 false가 됨.");
            }
            
            return _curEvent.results;
        }
        Debug.LogError($"{_curEvent.eventId}의 conditionType이 올바른 값이 아닙니다.");
        _isConditionMet = false;
        return null;
    }
    

    public async UniTask ExecuteEvent(string eventID)
    {
        if (CheckExecutable(eventID))
        {
            Debug.LogWarning($"{eventID} 이벤트 실행");
            
            _isEventSuccess = false;
            _isQuizSolved = true;
            EventStructure _event = DataManager.Instance._events[eventID];
            currentEventID = eventID;
            string[] results = CheckConditions(_event);
            Debug.LogWarning($"{eventID} 이벤트의 repeatType {_event.repeatType} , isExecuted : {_event.isExecuted}");

            //락 걸기
            if (!string.IsNullOrEmpty(_event.lockConditionId) &&
                DataManager.Instance._lockConditions.ContainsKey(_event.lockConditionId))
            {
                await DataManager.Instance._lockConditions[_event.lockConditionId].Lock();
            }
            
            //실행할 수 있는 결과가 있다면
            if (results != null && results.Length > 0)
            {
                //UniTask DoResult가 모두 실행될 때까지 대기
                await DoResult(results);
            }
            
            Debug.LogWarning($"{eventID} 이벤트의 repeatType {_event.repeatType} , isExecuted : {_event.isExecuted}");
            
            //이벤트가 성공적으로 실행이 되었다고
            if (!_isRepeatFalse && _isConditionMet)
            {
                if (!_isQuizSolved)
                {
                    _isEventSuccess = false;
                }
                else
                {
                    _isEventSuccess = true;
                    //증거물이 있다면
                    if (!string.IsNullOrEmpty(_event.evidenceId) &&
                        DataManager.Instance._evidences.ContainsKey(_event.evidenceId))
                    {
                        await AcquireEvidence(_event, _event.evidenceId);
                        Debug.LogWarning($"{_event.evidenceId} 증거물 획득 다 실행 됨.");
                    }
                }
            }

            Debug.LogWarning($"{eventID} 이벤트의 repeatType {_event.repeatType} , isExecuted : {_event.isExecuted}");
            
            if (_isEventSuccess)
            {
                CloseEventSuccess(_event);
                
                //예외 이벤트 처리 코드 - 정신세계 첫 진입 시, 인벤토리 관련 다이얼로그
                if (currentEventID == "Event_A007")
                {
                    ExecuteEvent(nextEventID).Forget();
                }
                //예외처리
                if(currentEventID == "Event_B044") return;
            }
            else
            {
                CloseEventFailure(_event);
            }
            
            Debug.LogWarning($"{eventID} 이벤트의 repeatType {_event.repeatType} , isExecuted : {_event.isExecuted}");

            if (_isEventSuccess)
            {
                if (!string.IsNullOrEmpty(nextEventID) && DataManager.Instance._events.ContainsKey(nextEventID))
                {
                    if (DataManager.Instance._events[nextEventID].isAuto)
                    {
                        ExecuteEvent(nextEventID).Forget();
                    }
                }
            }
            
            //현재 위치한 곳에 트리거가 있다면 해당 트리거 실행 가능한지 다시 체크
            if (PlayerInteract.Instance.isInsideTrigger && PlayerInteract.Instance.curTrigger != null)
            {
                PlayerInteract.Instance.curTrigger.CheckTriggerAvail();
            }
        }
    }

    public async UniTask DoResult(string[] results)
    {
        foreach (var resultID in results)
        {
            if (!string.IsNullOrEmpty(resultID))
            {
                _isEventSuccess = true;
                string resultType = resultID.Substring(0, resultID.IndexOf('_'));
                
                if (resultType == "Dialogue")
                {
                    //라디오 다이얼로그 예외처리
                    if (resultID == "Dialogue_0027" || resultID == "Dialogue_0028" || resultID == "Dialogue_0024")
                    {
                        return;
                    }

                    StartDialogue(resultID);

                    // 대화가 끝날 때까지 대기
                    await WaitForDialogueEndAsync();
                    Debug.Log("#4-2 : " + resultID + " 대화 끝");
                }
                else if (resultType == "Effect")
                {
                    StartEffect(resultID);

                    // 효과가 끝날 때까지 대기
                    await WaitForEffectEndAsync();
                    Debug.Log("#4-2 : " + resultID + " 효과 끝");
                }
                else if (resultType == "Quiz")
                {
                    StartQuiz(resultID);
                    // input이 끝날 때까지 기다리기
                    await WaitForQuizEndAsync();
                    Debug.Log("#4-2 : " + resultID + " quiz 끝");
                    QuizStructure quizStructure = DataManager.Instance._quiz[resultID];

                    if (quizStructure.isSolved)
                    {
                        await QuizManager.Instance.DoCorrectResult(quizStructure);
                        _isQuizSolved = true;
                    }
                    else
                    {
                        await QuizManager.Instance.DoWrongResult(quizStructure);
                        _isQuizSolved = false;
                    }
                }
                else if (resultType == "Event")
                {
                    await ExecuteEvent(resultID);
                }
                else if (resultType == "Mental")
                {
                    PlayerInteract.Instance.GetComponent<MentalEnterProcess>().StartEnter(resultID);
                }
                else if (resultType == "Evidence")
                {
                    await AcquireEvidence(DataManager.Instance._events[currentEventID], resultID);
                }
            }
        }
    }
    
    private async UniTask WaitForDialogueEndAsync()
    {
        DialogueManager.Instance.OnDialogueEnd = null;
        var tcs = new UniTaskCompletionSource();
        DialogueManager.Instance.OnDialogueEnd += () => tcs.TrySetResult();
        await tcs.Task;
    }
    
    private async UniTask WaitForEffectEndAsync()
    {
        EffectManager.Instance.OnEffectEnd = null;
        var tcs = new UniTaskCompletionSource();
        EffectManager.Instance.OnEffectEnd += () => tcs.TrySetResult();
        await tcs.Task;
    }
    
    private async UniTask WaitForQuizEndAsync()
    {
        QuizManager.Instance.OnQuizEnd = null;
        var tcs = new UniTaskCompletionSource();
        QuizManager.Instance.OnQuizEnd += () => tcs.TrySetResult();
        await tcs.Task;
    }
    
    private async UniTask WaitForInvestigateEndAsync()
    {
        UIManager.Instance.OnSelectEnd = null;
        var tcs = new UniTaskCompletionSource();
        UIManager.Instance.OnSelectEnd += () => tcs.TrySetResult();
        await tcs.Task;
    }

    public async UniTask AcquireEvidence(EventStructure _event, string evidenceID)
    {
        EvidenceStructure _evidence = DataManager.Instance._evidences[evidenceID];
        
        //증거물을 습득할 수 있는지 체크
        if (_evidence.CanAcquireEvidence())
        {
            //거울 조각이면 유리 UI에 반영하기
            if (evidenceID == "Evidence_019" || evidenceID == "Evidence_023")
            {
                return;
            }
            
            //Investigate UI를 열수 있으면 열기
            if (_evidence.canInvestigate == 'Y')
            {
                if (UIManager.Instance.IsAnyUIOpen())
                {
                    await UniTask.Delay(500);
                    
                    Debug.LogWarning($"Investigate UI 열기 전, 창이 열려있어서 닫기");
                    UIManager.Instance.CloseAllUI();
                    await UniTask.Yield();
                    if (UIManager.Instance.evidenceDetailUI.transform.GetChild(0).gameObject.activeSelf)
                    {
                        UIManager.Instance.evidenceDetailUI.transform.GetChild(0).gameObject.SetActive(false);
                    }

                    await UniTask.Delay(200);
                }
                
                UIManager.Instance.OpenUI(UIManager.Instance.investigateUI,_evidence);
                //yes, no 선택 기다리기
                await WaitForInvestigateEndAsync();
                Debug.LogWarning($"Investigate UI 버튼 선택 다 기다림.");
                
                //yes라면
                if (UIManager.Instance.isYesClicked)
                {
                    Debug.LogWarning("Investigate UI에서 YES를 선택함.");
                    
                    //증거물 상세 ui가 닫힐 때까지 기다리기
                    await UniTask.WaitUntil(() => !UIManager.Instance.IsAnyUIOpen());
                    Debug.LogWarning("창 닫힐 때까지 다 기다림.");
                }
                else
                {
                    //no라면
                    Debug.LogWarning("Investigate UI에서 NO를 선택함.");
                }
                

                if (evidenceID == "Evidence_020" || evidenceID == "Evidence_021" ||
                    evidenceID == "Evidence_022")
                {
                    MirrorPuzzleManager.Instance.GetMirrorPiece(evidenceID);
                }
                
                //습득할 수 있는 증거물의 이벤트 type이 true이고 repeat fasle result가 없으면 repeatType을 false로 변경
                if (_event.repeatType && _evidence.acquisitionType == 'Y' && string.IsNullOrEmpty(_event.repeatFalseResult))
                {
                    _event.repeatType = false;
                    Debug.LogWarning($"{_event.eventId}의 repeatType true에서 false로 변경");
                }
                    
                //예외처리
                if (currentEventID == "Event_A018" || currentEventID == "Event_A023")
                {
                    //일기 습득 성공하면 더이상 캐비넷에 접근할 수 없도록
                    Debug.LogWarning($"캐비넷 더이상 접근 불가능하도록 설정");
                    DataManager.Instance._events["Event_A025"].repeatType = false;
                    DataManager.Instance._events["Event_A025"].isExecuted = true;
                    DataManager.Instance._events["Event_A017"].isExecuted = true;
                    DataManager.Instance._events["Event_A017"].repeatType = false;
                }
            }
            else if (_evidence.canInvestigate == 'N')
            {
                _evidence.AcquireEvidence();
            }

            _isEventSuccess = true;
        }
        else
        {
            _isEventSuccess = false;
        }
    }
     //dialogue 시작
    void StartDialogue(string dialogueID)
    {
        Debug.Log($"{dialogueID} 대화 시작");
        DialogueManager.Instance.SetDialogue(dialogueID);
    }

    //effect 시작
    void StartEffect(string effectID)
    {
        Debug.Log($"{effectID} 효과 시작");
        EffectManager.Instance.SetEffect(effectID);
    }
    
    //Quiz 시작
    void StartQuiz(string quizID)
    {
        Debug.Log($"{quizID} 퀴즈 시작");
        QuizManager.Instance.SetQuiz(quizID);
    }
    
    public void CloseEventFailure(EventStructure _event)
    {
        Debug.LogWarning($"{_event.eventId} 가 성공적으로 실행되지 않음.");
        if (!string.IsNullOrEmpty(_event.lockConditionId) &&
            DataManager.Instance._lockConditions.ContainsKey(_event.lockConditionId))
        {
            DataManager.Instance._lockConditions[_event.lockConditionId].UnLock();
        }
        
        _event.isExecuted = false;
    }
    
    public void CloseEventSuccess(EventStructure _event)
    {
        Debug.Log($"{_event.eventId} 가 성공적으로 실행.");
        if (!string.IsNullOrEmpty(_event.lockConditionId) &&
            DataManager.Instance._lockConditions.ContainsKey(_event.lockConditionId))
        {
            DataManager.Instance._lockConditions[_event.lockConditionId].UnLock();   
        }
        _event.isExecuted = true;
        nextEventID = _event.nextEventId;
    }

    public bool IsConditionMet()
    {
        return _isConditionMet;
    }
}
```

</details>

<details>
<summary><code>EventTrigger.cs</code></summary>

```csharp
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
```

</details>

<details>
<summary>시트 내 데이터 구성 코드</summary>

```csharp
public class DialogueStructure
{
    public class DialougeText
    {
        public string text;
        public string tutorialID;
        public string characterId;
    }
    
    public string dialogueId;
    public string characterId;
    public string triggerType;
    public string interactionType;
    public string dialogueText;
    public string nextDialougeId;
    public string tutorialId;
    public List<DialougeText> Dialogue_Text_List;
}
```

```csharp
public class EventStructure
{
    //csv 필드
    public string eventId;
    public string description;
    public bool repeatType; //반복 여부 true,false
    public string conditionType; //조건 타입 or,and
    public bool isAuto; //자동 실행 여부 true, false
    public string[] conditions;
    public string[] results;
    public string[] conditionFalseResults;
    public string repeatFalseResult;
    public string evidenceId;
    public string lockConditionId; //이벤트 실행 시 락되는 조건
    public string locationId;
    public string nextEventId;
    
    //이벤트 실행 여부
    public bool isExecuted = false;
    
    
    //조건 충족하는지
    public bool IsConditionMet(string conditionID)
    {
        //조건이 비어있으면 충족함으로 리턴
        if (string.IsNullOrEmpty(conditionID)) return true;
        
        bool isMet = false;
        
        //조건 id 앞에 !(not)이 붙어있는지 체크
        char boolType = conditionID[0];
        string conditionType;
        string id;
        
        if (boolType == '!')
        {
            conditionType = conditionID.Substring(1, conditionID.IndexOf('_')-1);
            id = conditionID.Substring(1, conditionID.Length-1);
        }
        else
        {
            conditionType = conditionID.Substring(0, conditionID.IndexOf('_'));
            id = conditionID;
        }
        
        if (conditionType == "Evidence")
        {
            //증거 인벤토리에 있는지 확인, 혹은 사용했는지 구분
            EvidenceStructure evidence = DataManager.Instance._evidences[id];
            char canUse = evidence.canUse;
            char acquisitionType = evidence.acquisitionType;

            if (canUse == 'Y')
            {
                Debug.Log($"증거물 사용 여부 : {PlayerInteract.Instance.isUsingEvidence}");
                //아이템을 사용했는지 체크
                if (PlayerInteract.Instance.isUsingEvidence)
                {
                    isMet = true;
                    PlayerInteract.Instance.isUsingEvidence = false;
                }
            }
            else if (canUse == 'N')
            {
                //사용할 수 없는 아이템이나, 인벤토리에 획득 가능한 아이템이면 인벤토리에 있는지 체크
                if (acquisitionType == 'Y')
                {
                    if (DataManager.Instance._evidences[id].isAcquired)
                    {
                        isMet = true;
                    }
                    else
                    {
                        Debug.Log(id+"가 인벤토리 내에 존재하지 않습니다.");
                    }
                }
                else if(acquisitionType == 'N')
                {
                    //접근 횟수로 체크(나중에 필요하면 구현)
                }
            }
        }
        else if (conditionType == "Event")
        {
            if (DataManager.Instance._events.ContainsKey(id) && DataManager.Instance._events[id].isExecuted)
            {
                isMet = true;
            }
        }
        else if (conditionType == "Quiz")
        {
            if (DataManager.Instance._quiz.ContainsKey(id) && DataManager.Instance._quiz[id].isSolved)
            {
                isMet = true;
            }
        }
        
        //혹시 not 조건이 있다면 반대로 값을 출력
        if (boolType == '!') return !isMet;
        
        return isMet;
    }
}
```

</details>

---

</details>


  
## 개발 내용 및 플레이 영상

---

### [기본 시스템]

<img src="https://github.com/user-attachments/assets/c6e63205-8d3a-4c1b-b738-f2b764e78b7c"  width="574" height="358"/>


특정한 위치 or 조건을 충족하여 상호작용하면 다양한 이벤트가 실행이 된다.  

<img src="https://github.com/user-attachments/assets/e2045f56-3ba2-4ff8-b95e-742331f66675"  width="574" height="358"/>

능력을 사용하여 정신세계로 진입.

<img src="https://github.com/user-attachments/assets/b0f03721-2dca-4028-95dd-974ab752c367"  width="574" height="358"/>


인벤토리를 열어 게임 플레이 도중 얻은 증거물을 상세하게 살펴보거나 특정 위치에서 사용할 수 있다.

### [기믹]

<img src="https://github.com/user-attachments/assets/cbf6ce28-d7b3-441a-9713-436c2a536e58"  width="574" height="358"/>


올바른 주파수를 입력하여 정답을 맞춘다.

<img src="https://github.com/user-attachments/assets/efd1e750-d6a9-4478-a926-8cd3dffc50a8"  width="574" height="358"/>

올바른 시간을 맞춰서 정답을 맞춘다

<img src="https://github.com/user-attachments/assets/c2adafe9-4559-4346-a11f-e663dd01e431"  width="574" height="358"/>

올바른 순서로 책을 배치하여 정답을 맞춘다

<img src="https://github.com/user-attachments/assets/04b981eb-41b5-4d5c-b8cf-e1397ea7cc11"  width="574" height="358"/>

각종 기믹들을 풀고 맵 내에 숨겨진 거울 조각을 획득하고 올바른 위치에 조각을 배치하여 정답을 맞춘다

### [연출]

<img src="https://github.com/user-attachments/assets/f7cfd1eb-25e3-4c7b-97fb-82a3972fe788"  width="574" height="358"/>

게임 플레이 도중 컷씬을 실행 시키는 연출

<img src="https://github.com/user-attachments/assets/2e57a9c8-aa9a-4b05-95af-ecf7bd97301a"  width="574" height="358"/>

카메라를 흔드는 연출

<p align="left">
  <img src="https://github.com/user-attachments/assets/f30dcc2b-2e37-4d30-a2e6-30c13c449e8f" width="400" height="300"/>
  <img src="https://github.com/user-attachments/assets/16a6e5f5-98aa-40af-b5a6-909dcbe8d9c9" width="400" height="300"/>
</p>

NPC에 따른 고유한 카메라 위치 이동 연출

## 프로젝트 사용기술

---

### ⚒️ 클라이언트

- Unity
- C#

### ⚒️ 버전 관리 및 협업

- Git
- Notion
- Figma
- Google Sheet

### ⚒️ 개발 환경

- Visual Studio
