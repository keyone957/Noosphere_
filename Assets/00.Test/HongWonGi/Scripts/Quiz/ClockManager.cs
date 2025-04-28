using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class ClockManager : UIBase
{
    private string _curQuizID;
    private QuizStructure _curQuiz;
    
    [Header("현재 시간")]
    public int _hour;
    public int _minute;
    [Header("답")]
    [SerializeField]private int _targetHour = 1;
    [SerializeField]private int _targetMinute = 5;
    private static ClockManager _instance;
    public static ClockManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ClockManager>();
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject(nameof(ClockManager));
                    _instance = singletonObject.AddComponent<ClockManager>();
                }
            }

            return _instance;
        }
    }
    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
    }
    
    public override void OnOpen(string quizID) 
    {
         base.OnOpen(quizID);
         _curQuizID = "";
         _curQuiz = null;
         _curQuizID = quizID;
         _curQuiz = DataManager.Instance._quiz[_curQuizID];
         Debug.Log($"# quiz id : {quizID}, _curQuiz : {_curQuiz}");
         transform.GetChild(0).gameObject.SetActive(true);
        _hour=0;
        _minute=0;
    }
    
    public override void OnClose()
    {
        base.OnClose(); 
        transform.GetChild(0).gameObject.SetActive(false);
        QuizManager.Instance.OnQuizEnd?.Invoke();
    }
    public async UniTaskVoid CheckAnswerWithDelay()
    {
        await UniTask.Delay(1000);

        // 정답 체크
        //추후에 시트에서 답 가져오기
        if (_hour == _targetHour && _minute == _targetMinute)
        {
            _curQuiz.isSolved = true;
            Debug.Log($"정답입니다! {_hour}시 {_minute}분");
            UIManager.Instance.CloseTopUI();
        }
    }
    
}
