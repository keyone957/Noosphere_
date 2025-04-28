# NoosPhere

진행 기간: 2024. 09 ~ 진행 중
사용한 기술 스택: C#, Unity
개발 인원(역할): 개발자 2명 + 기획자 2명 + 디자이너 1명 + 사운드 1명
한 줄 설명: 3D 추리 게임
비고: 팀 프로젝트/최우수상 수상작 / 진행중인 프로젝트

## 게임 플레이 풀 영상

---

[https://www.youtube.com/watch?v=5mgx3IzGAn8](https://www.youtube.com/watch?v=5mgx3IzGAn8)

- 깃허브

[https://github.com/keyone957/Noosphere_](https://github.com/keyone957/Noosphere_)

## 프로젝트 소개

---

![image.png](image.png)

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

### [구글 스프레드 시트를 이용한 게임 이벤트 관리]

![image.png](image%201.png)

=⇒ 이벤트 동작 방식 플로우 차트 

![image.png](image%202.png)

![image.png](image%203.png)

⇒ 구글 스프레드시트를 이용하여 다른 부서와 개발자 사이의 협업을 원활하게 진행

- 구글시트 CSV 파일을 파싱하여 실시간으로 데이터를 가져오도록 하였습니다.
- 추리 게임의 특성상 다양한 루트가 존재하므로 각 행의 이벤트를 객체화하여 여러 루트로 이어지게 하여 다양한 엔딩을 구현하였습니다.
- 기획자가 실시간으로 스프레드시트를 수정하면 바로 게임에 적용되게 구현하였습니다
- 모든 게임에 필요한 데이터(이벤트, 기믹, 증거물 상세 정보 및 속성, 대화, 소리, 사진 등…)를 모두 스프레드시트로 관리하여 다른 부서 간 협업이 원활하게 진행되도록 하였습니다.
- Eventmanager.cs
    
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
    
- EventTrigger.cs
    
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
    

## 개발 내용 및 플레이 영상

---

### [기본 시스템]

![증거물획득.gif](%25EC%25A6%259D%25EA%25B1%25B0%25EB%25AC%25BC%25ED%259A%258D%25EB%2593%259D.gif)

특정한 위치 or 조건을 충족하여 상호작용하면 다양한 이벤트가 실행이 된다.  

![능력사용.gif](%25EB%258A%25A5%25EB%25A0%25A5%25EC%2582%25AC%25EC%259A%25A9.gif)

능력을 사용하여 정신세계로 진입.

![인벤토리.mp4_20250309_153131.gif](%EC%9D%B8%EB%B2%A4%ED%86%A0%EB%A6%AC.mp4_20250309_153131.gif)

인벤토리를 열어 게임 플레이 도중 얻은 증거물을 상세하게 살펴보거나 특정 위치에서 사용할 수 있다.

### [기믹]

![라디오기믹.gif](%25EB%259D%25BC%25EB%2594%2594%25EC%2598%25A4%25EA%25B8%25B0%25EB%25AF%25B9.gif)

올바른 주파수를 입력하여 정답을 맞춘다.

![벽시계.gif](%25EB%25B2%25BD%25EC%258B%259C%25EA%25B3%2584.gif)

올바른 시간을 맞춰서 정답을 맞춘다

![책장기믹.gif](%25EC%25B1%2585%25EC%259E%25A5%25EA%25B8%25B0%25EB%25AF%25B9.gif)

올바른 순서로 책을 배치하여 정답을 맞춘다

![거울기믹.gif](%25EA%25B1%25B0%25EC%259A%25B8%25EA%25B8%25B0%25EB%25AF%25B9.gif)

각종 기믹들을 풀고 맵 내에 숨겨진 거울 조각을 획득하고 올바른 위치에 조각을 배치하여 정답을 맞춘다

### [연출]

![컷씬연출.gif](%25EC%25BB%25B7%25EC%2594%25AC%25EC%2597%25B0%25EC%25B6%259C.gif)

게임 플레이 도중 컷씬을 실행 시키는 연출

![감금연출.gif](%25EA%25B0%2590%25EA%25B8%2588%25EC%2597%25B0%25EC%25B6%259C.gif)

카메라를 흔드는 연출

![new.mp4_20250309_154140.gif](new.mp4_20250309_154140.gif)

![카메라 연출.mp4_20250309_154301.gif](%EC%B9%B4%EB%A9%94%EB%9D%BC_%EC%97%B0%EC%B6%9C.mp4_20250309_154301.gif)

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
