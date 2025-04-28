using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MentalEnterProcess : MonoBehaviour
{
    //정신세계 진입 관련 변수
    [Header("정신세계 진입")]
    [SerializeField] private float _enterTime = 5.0f;
    [SerializeField] bool _startEnter = false;
    [SerializeField] float _timer = 0f;
    public bool isComplete = false;
    public MentalStructure mentalInfo;
    [SerializeField] private string _comebackEventId;
    [SerializeField] private float _coolTime = 3f;
    [SerializeField] private bool _canEnter = true;
    [SerializeField] private bool _isForceQuit = false;
    
    [Header("정신세계 진입 UI")]
    [SerializeField] private GameObject _progressBarUI;
    [SerializeField] private EnterProgressBar _progressBarFill;
    private bool _soundPlayed=false;
    
    void Update()
    {
        if (!UIManager.Instance.IsAnyUIOpen())
        {
            if (PlayerInteract.Instance.canInteract && _canEnter && !_startEnter && Input.GetKeyDown(KeyCode.R))
            {
                if (PlayerInteract.Instance.isInMental)
                {
                    //정신세계 -> 현실세계 진입
                    if (mentalInfo != null)
                    {
                        Debug.LogWarning("현실세계로 돌아가기 위헤 Space 클릭");
                        SoundManager.Instance.PlaySFX("Soundresource_037");
                        //이벤트 실행 가능하다면 실행
                        EventManager.Instance.ExecuteEvent(_comebackEventId).Forget();
                    }
                }
                else
                {
                    //현실세계 -> 정신세계 진입
                    if (PlayerInteract.Instance.OnMentalInteract != null)
                    {
                        Debug.LogWarning("OnMentalInteract 에 등록되어있는 메소드 실행");
                        SoundManager.Instance.PlaySFX("Soundresource_037");
                        PlayerInteract.Instance.OnMentalInteract?.Invoke();
                        PlayerInteract.Instance.OnMentalInteract = null;
                    }
                }
            }
            
            //진입시작했고, 완료되지 않았고, 스페이스를 계속 누르고 있다면
            if (_startEnter && !isComplete)
            {
                PlayerInteract.Instance.HideInteractionMark();
                if (Input.GetKey(KeyCode.R))
                {
                    float value = _progressBarFill.FillAmount();
                    EffectManager.Instance.StartMentalEffect(value);
                    if(!_soundPlayed)
                    {
                        SoundManager.Instance.PlaySFX("Soundresource_029");
                        _soundPlayed = true;
                    }

                    if (value >= 1f)
                    {
                        isComplete = true;
                        // 완료되면 소리 재생
                        SoundManager.Instance.StopSFX("Soundresource_029");
                        SoundManager.Instance.PlaySFX("Soundresource_028");
                    }
                    if (value >= 1f)
                    {
                        isComplete = true;
                    }
                }
                else
                {
                    //스페이스에서 손 때면, 현 상태에서 게이지 감소하는 코드
                    float value = _progressBarFill.DrainAmount();
                    EffectManager.Instance.StartMentalEffect(value);
                    _soundPlayed = false; 
                    SoundManager.Instance.StopSFX("Soundresource_029");
                    if (value <= 0)
                    {
                        FailEnter();
                    }
                }
            }
        }
        
    }
    void FixedUpdate()
    {
        //정신세게 진입이 시작되었다면
        if (_startEnter)
        {
            _timer += Time.fixedDeltaTime;

            if (isComplete)
            {
                Debug.Log($"#{mentalInfo.mentalId} 시간 내에 진입 완료.");
                CompleteEnter();
            }

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) ||
                Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
            {
                Debug.Log($"#{mentalInfo.mentalId} 진입 중에 움직여서 초기화 됨.");
                FailEnter();
            }

            if (_timer >= _enterTime)
            {
                if (!isComplete)
                {
                    Debug.Log($"#{mentalInfo.mentalId} 시간 내에 진입하지 못함.");
                    FailEnter();
                }
            }

            if (_isForceQuit)
            {
                Debug.Log($"#{mentalInfo.mentalId} 진입 강제 종료");
                FailEnter();
            }
        }
    }
    
    public void InitProgressBar()
    {
        _progressBarUI.SetActive(false);
        EffectManager.Instance.ResetMetalEffect();
        
        _timer = 0f;
        _progressBarFill.InitFillAmount();
        isComplete = false;
        _startEnter = false;
        _isForceQuit = false;
        _soundPlayed = false;
    }

    public void StartEnter(string mentalId)
    {
        if (DataManager.Instance._mental.ContainsKey(mentalId))
        {
            mentalInfo = DataManager.Instance._mental[mentalId];
            if (!PlayerInteract.Instance.isInMental) _comebackEventId = mentalInfo.combackEventId;
            _startEnter = true;
            _progressBarUI.SetActive(true);
        }
        else
        {
            Debug.Log($"${mentalId} 키의 MentalStructure이 존재하지 않습니다.");
        }
       
    }

    void CompleteEnter()
    {
        //바 초기화
        // InitProgressBar();
        _progressBarUI.SetActive(false);
        _timer = 0f;
        _progressBarFill.InitFillAmount();
        isComplete = false;
        _startEnter = false;
        _isForceQuit = false;
        _soundPlayed = false;
        //씬 이동
        // StartCoroutine(LoadSceneAsync(mentalInfo.destination));
        LoadSceneAsync(mentalInfo.destination).Forget();
        PlayerInteract.Instance.isInMental = !PlayerInteract.Instance.isInMental;
        
        //이동 성공 시 결과가 있다면 실행
        // if (!string.IsNullOrEmpty(mentalInfo.mentalTrueResult))
        // {
        //     EventManagerYKM.Instance.ExecuteEvent(mentalInfo.mentalTrueResult).Forget();
        // }

        if (!PlayerInteract.Instance.isInMental)
        {
            mentalInfo = null;
            _comebackEventId = "";
            
            PlayerInteract.Instance.isInsideTrigger = false;
            PlayerInteract.Instance.curTrigger = null;
        }
        else
        {
            PlayerInteract.Instance.isInsideTrigger = false;
            PlayerInteract.Instance.curTrigger = null;
        }
        
        //성공적으로 도착한 경우, 쿨타임 시작
        StartCoroutine(StartCoolTime());
    }

    async UniTaskVoid FailEnter()
    {
        //바 초기화
        InitProgressBar();
        
        //이동 실패 시 결과가 있다면 실행
        await EventManager.Instance.DoResult(mentalInfo.mentalFalseResults);
        
        if (EventManager.Instance.currentEventID == "Event_A026")
        {
            EventManager.Instance.nextEventID = "Event_A026";
            DataManager.Instance._events["Event_A026"].isExecuted = false;
        }

        if (EventManager.Instance.currentEventID == "Event_A009")
        {
            EventManager.Instance.nextEventID = "Event_A009";
            DataManager.Instance._events["Event_A009"].isExecuted = false;
        }
        
        //트리거 내에 있는지 재검사
        if (PlayerInteract.Instance.isInsideTrigger && PlayerInteract.Instance.curTrigger != null)
        {
            PlayerInteract.Instance.curTrigger.CheckTriggerAvail();
        }
    }
    
    // private IEnumerator LoadSceneAsync(string sceneName)
    // {
    //     AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
    //     
    //     while (!operation.isDone)
    //     {
    //         yield return null;
    //     }
    //     PlayerInteract.Instance.HideInteractionMark();
    // }
    private async UniTask LoadSceneAsync(string sceneName)
    {
        string eventId;//씬전환이후에 mentalinfo가 null이 되는 문제 해결위함
        if (mentalInfo != null)
        {
            eventId = mentalInfo.mentalTrueResult;
        }
        else
        {
            eventId = null;
        }

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
    
        while (!operation.isDone)
        {
            await UniTask.Yield();
        }

        // await UniTask.Yield(); 
    
        // 씬 이동 후 이펙트 실행
        if (EffectManager.Instance != null)
        {
            await EffectManager.Instance.StartMentalEffectReverse(eventId);
        }
      
        PlayerInteract.Instance.HideInteractionMark();
        
    }

    
    IEnumerator StartCoolTime()
    {
        PlayerInteract.Instance.HideInteractionMark();
        _canEnter = false;
        yield return new WaitForSeconds(_coolTime);
        _canEnter = true;
    }

    public void LockEnterProcess()
    {
        _canEnter = false;
    }

    public void UnLockEnterProcess()
    {
        _canEnter = true;
    }

    public void ForceQuitMentalProcess()
    {
        StartCoroutine(BreakMirror());
    }

    IEnumerator BreakMirror()
    {
        yield return new WaitForSeconds(1.0f);
        _isForceQuit = true;
        SoundManager.Instance.StopAllSFX();
    }

    public void SetCombackEventId(string id)
    {
        _comebackEventId = id;
    }
}
