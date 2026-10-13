using Cysharp.Threading.Tasks;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using NooSphere;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = NooSphere.Debug;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }
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

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

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

    //자동저장 딜레이 시간
    [SerializeField] private float _autoSaveDelayTime = 2.5f;

    //플레이어 프리팹
    [SerializeField] private GameObject _player;

    //이벤트 세팅 완료 flag
    private bool _isEventSetupComplete = false;

    void InitState()
    {
        Debug.LogWarning("이벤트 매니저 초기화");
        //게임 시작 시, 스테이지 정보 초기화
        curChapterInfo = (ChapterInfo)InventoryManager.Instance.currentViewChapter;
        if (FindObjectOfType<RoomInfoManager>() is RoomInfoManager roomInfoManager)
        {
            curRoomInfo = roomInfoManager.roomInfo;
        }

        // 경로 설정
        string folderPath = Path.Combine(Application.persistentDataPath, "Saves");
        int selectSlotIndex = NooSphere.SaveManager.Instance.selectSlotIndex;
        string filePath = Path.Combine(folderPath, $"slot{selectSlotIndex}.es3");
        
        // 이벤트 상태 초기화
        currentEventID = ES3.Load<string>("CurrentEventID", filePath);
        nextEventID = ES3.Load<string>("NextEventID", filePath);

        // 플레이어 위치, 회전 초기화
        SetPlayerPosFromSaveData();

        // 카메라 초기화
        if(FindAnyObjectByType<AssignPlayer>() is AssignPlayer assignPlayer)
        {
            //assignPlayer.DoAssign();
        }

        // 정신세계 진입 상태 초기화
        MentalEnterProcess mentalInfo = PlayerController.Instance.GetComponent<MentalEnterProcess>();
        mentalInfo.SetCombackEventId(ES3.Load<string>("CombackEventID", filePath));
        PlayerInteract.Instance.isInMental = ES3.Load<bool>("IsInMental", filePath);
        mentalInfo.mentalInfo = ES3.Load<MentalStructure>("MentalInfo", filePath);

        // 이어하기 모든 단계 완료, 로드 상태 NewGame으로 초기화
        //NooSphere.SaveManager.Instance.SetLoadType(NooSphere.GameLoadType.NewGame);
        NooSphere.SaveManager.Instance.SetSlotIndex(-1);

        // 인벤토리 설정
        InventoryManager.Instance.LoadInventoryData();

        PlayerController.Instance.canMove = true;

        StartCoroutine(CompleteLoad());
    }

    //현재 실행될 수 있는 이벤트인지 검사 -> 실행 가능하다면 플레이어에게 ? 띄우기
    public bool CheckExecutable(string eventID)
    {
        if (string.IsNullOrEmpty(eventID)) return false;
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
                        _isEventSuccess = true;
                    }
                }
            }
            
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
                //예외처리 - 친구에게 편지 주면 더이상 상호작용하지 못하도록
                if (currentEventID == "Event_C066")
                {
                    DataManager.Instance._events["Event_C082"].repeatType = false;
                }
            }
            else
            {
                if (currentEventID == "Event_D099" || currentEventID == "Event_D100")
                {
                    CloseEventSuccess(_event);
                }
                else
                {
                    CloseEventFailure(_event);
                }
            }
            
            //Debug.LogWarning($"{eventID} 이벤트의 repeatType {_event.repeatType} , isExecuted : {_event.isExecuted}");

            if (_isEventSuccess)
            {
                if (!string.IsNullOrEmpty(nextEventID) && DataManager.Instance._events.ContainsKey(nextEventID))
                {
                    if (DataManager.Instance._events[nextEventID].isAuto)
                    {
                        await ExecuteEvent(nextEventID);
                    }
                }
            }
            
            //현재 위치한 곳에 트리거가 있다면 해당 트리거 실행 가능한지 다시 체크
            if (PlayerInteract.Instance.isInsideTrigger && PlayerInteract.Instance.curTrigger != null)
            {
                await Task.Delay(100);
                if (PlayerInteract.Instance.isInsideTrigger && PlayerInteract.Instance.curTrigger != null)
                {
                    bool canCheck = false;
                    foreach (var EVENTID in PlayerInteract.Instance.curTrigger.GetComponent<EventTrigger>().eventIdList)
                    {
                        if (!DataManager.Instance._events[EVENTID].repeatType && DataManager.Instance._events[EVENTID].isExecuted)
                        {
                            canCheck = false;
                            continue;
                        }

                        canCheck = true;
                        break;
                    }
                    if(!canCheck) return;
                    if(currentEventID == "Event_D090") return;
                    PlayerInteract.Instance.curTrigger.OnTriggerEnter(PlayerInteract.Instance.GetComponent<CapsuleCollider>());
                }
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
                    if (resultID == "Effect_048") _isEventSuccess = true;
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
                        if (quizStructure.quizId == "Quiz_009")
                        {
                            _isQuizSolved = true;
                            return;
                        }
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
            if (evidenceID == "Evidence_023")
            {
                return;
            }

            if (evidenceID == "Evidence_020" || evidenceID == "Evidence_021" ||
                evidenceID == "Evidence_022" || evidenceID == "Evidence_019")
            {
                DialogueManager.Instance.SetDialogue("Dialogue_0065");
                _evidence.AcquireEvidence();
                MirrorPuzzleManager.Instance.GetMirrorPiece(evidenceID);
                await UniTask.Yield();
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
        if (effectID == "Effect_048")
        {
            DataManager.Instance._events["Event_D099"].isExecuted = true;
        }
        else if (effectID == "Effect_054")
        {
            DataManager.Instance._events["Event_D102"].isExecuted = true;
        }
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
        if (_event.eventId == "Event_D102")
        {
            Debug.LogWarning("[엔딩 2차 분기점]");
            // 다프네에게 연구소 도면을 받았다면
            if (DataManager.Instance._events["Event_B065"].isExecuted && InventoryManager.Instance.IsEvidenceInInventory("Evidence_016"))
            {
                Debug.LogWarning("[일반 엔딩] 탈출 성공");
                DataManager.Instance._events["Event_D102"].nextEventId = "Event_D103";
            }
            else
            {
                Debug.LogWarning("[일반 엔딩] 탈출 실패");
                DataManager.Instance._events["Event_D102"].nextEventId = "Event_D104";
            }     
        }
        nextEventID = _event.nextEventId;

        // autoSave가 true라면, 자동 저장 진행
        if (_event.autoSave && !_event.autoSaveComplete)
        {
            Debug.Log(_event.autoSaveDelay + "자동 저장 딜레이 옵션");
            if (_event.autoSaveDelay == 2)
            {
                _event.autoSaveComplete = true;
                StartCoroutine(DoAutoSaveDelay(_event));
            }
            else if(_event.autoSaveDelay != 1)
            {
                Debug.Log(_event.eventId + "자동 저장 실행");
                NooSphere.SaveManager.Instance.SetSlotIndex(1);
                _event.autoSaveComplete = true;
                StartCoroutine(NooSphere.SaveManager.Instance.DoAutoSave());
            }
        }
    }

    public bool IsConditionMet()
    {
        return _isConditionMet;
    }

    IEnumerator DoAutoSaveDelay(EventStructure _event)
    {
        yield return new WaitForSecondsRealtime(_autoSaveDelayTime);
        Debug.Log(_event.eventId + "자동 저장 딜레이 실행");
        NooSphere.SaveManager.Instance.SetSlotIndex(1);
        StartCoroutine(NooSphere.SaveManager.Instance.DoAutoSave());
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        var isEventScene = FindAnyObjectByType<IsEventScene>();
        if (isEventScene == null || _isEventSetupComplete) return;
        Debug.LogWarning("씬 로드 완료 - 이벤트 씬임!!" + scene.name);
        if (NooSphere.SaveManager.Instance.CurrentLoadType == NooSphere.GameLoadType.NewGame)
        {
            Debug.LogWarning("이벤트 매니저 초기화 여기 실행 돼??" + NooSphere.SaveManager.Instance.CurrentLoadType);
            curChapterInfo = ChapterInfo.Prologue;
            curRoomInfo = RoomInfo.Room_101;
            nextEventID = startEventID;

            // 인벤토리 설정
            InventoryManager.Instance.InitInventory();

            ExecuteEvent(startEventID).Forget();
            
            // LoadComplete로 상태 변경해서 SetPlayerPos와 SetPlayerLoungePos에서
            // 플레이어 위치 설정해주는 경우를 처음 이어하기 했을 때와 그 이후의 경우로 구분할 수 있도록 함.
            NooSphere.SaveManager.Instance.SetLoadType(GameLoadType.LoadComplete);
        }
        else
        {
            InitState();
        }
        _isEventSetupComplete = true;
    }

    public void SetEventSetUpFlag(bool value)
    {
        _isEventSetupComplete = value;
    }

    
    // SetPlayerPos와 SetPlayerLoungePos에서 이어하기 시, 플레이어 위치를 세이브 데이터의 위치로 설정하는 함수
    void SetPlayerPosFromSaveData()
    {
        NooSphere.Debug.LogWarning("이어하기 시 실행되는 플레이어 위치 설정");
        int selectSlotIndex = NooSphere.SaveManager.Instance.selectSlotIndex;
        Instantiate(_player, NooSphere.SaveManager.Instance.GetPlayerTransform(selectSlotIndex).position, NooSphere.SaveManager.Instance.GetPlayerTransform(selectSlotIndex).rotation);
        PlayerInteract.Instance.OnInteract = null;
        PlayerInteract.Instance.OnEvidenceUse = null;
        PlayerInteract.Instance.OnMentalInteract = null;
        PlayerInteract.Instance.isInsideTrigger = false;
    }

    private IEnumerator CompleteLoad()
    {
        yield return new WaitForSeconds(2f);
        NooSphere.SaveManager.Instance.SetLoadType(GameLoadType.LoadComplete);
        NooSphere.Debug.LogWarning("로드 타입 로드 완료로 변경");
    }
}