using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MirrorPuzzleManager : UIBase
{ 
    [SerializeField] private string _curQuizID;
    private QuizStructure _curQuiz;
    //싱글톤
    private static MirrorPuzzleManager _instance;
    
    public static MirrorPuzzleManager Instance 
    { 
        get 
        { 
            if (_instance == null) 
            {
                _instance = FindObjectOfType<MirrorPuzzleManager>();
                if (_instance == null) 
                {
                    GameObject singletonObject = new GameObject(nameof(MirrorPuzzleManager));
                    _instance = singletonObject.AddComponent<MirrorPuzzleManager>();
                }
            }
            return _instance;
        } 
    }

    void Awake(){
        
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        //DontDestroyOnLoad(gameObject); 
    }

    void Start()
    {
        _mirrorPiecesDictionary.Add("Evidence_019",_mirrorPieces[0]);
        _mirrorPiecesDictionary.Add("Evidence_020",_mirrorPieces[1]);
        _mirrorPiecesDictionary.Add("Evidence_021",_mirrorPieces[2]);
        _mirrorPiecesDictionary.Add("Evidence_022",_mirrorPieces[3]);
        _mirrorPiecesDictionary.Add("Evidence_023",_mirrorPieces[4]);
    }
    
    [SerializeField] private GameObject _mirrorPanel;
    [SerializeField] private List<GameObject> _mirrorPieces;
    private Dictionary<string, GameObject> _mirrorPiecesDictionary = new Dictionary<string, GameObject>();
    [Header("World Mirror Object")]
    [SerializeField] private GameObject _mirror;
    [SerializeField] private GameObject _brokeMirror;
   
    public bool isPuzzleClear = false;
    public Action OnResetPuzzle;
    public bool isMirrorBroke = false;


    public override void OnOpen(string quizID)
    {
        base.OnOpen(quizID);
        _curQuizID = quizID;
        _curQuiz = DataManager.Instance._quiz[_curQuizID];
        Debug.Log($"# quiz id : {quizID}, _curQuiz : {_curQuiz}");
        
        ResetAllPieces();
        transform.GetChild(0).gameObject.SetActive(true);
    }

    public override void OnClose()
    {
        base.OnClose();
        ResetAllPieces();
        transform.GetChild(0).gameObject.SetActive(false);
        
        //결과에 따라 실행
        if (_curQuiz.isSolved)
        {
            DataManager.Instance._events[EventManager.Instance.currentEventID].repeatType = false;
            
            //거울 원상복구
            _mirror.SetActive(true);
            _brokeMirror.SetActive(false);
            //현실 세계로 돌아갈 수 있도록 변경
            DataManager.Instance._lockConditions["Lock_condition_005"].UnLock();
            PlayerInteract.Instance.GetComponent<MentalEnterProcess>().SetCombackEventId("Event_B059");
        }
        
        _curQuizID = "";
        _curQuiz = null;
    }
    
    public void GetMirrorPiece(string evidenceID)
    {
        if (!isMirrorBroke)
        {
            Debug.Log("거울 안깨져서 조각 획득 x");
            return;
        }

        Debug.Log("거울이 깨져있으니 습득하기");
        //거울조각 획득
        _mirrorPiecesDictionary[evidenceID].SetActive(true);
    }

    public void CheckAnswer()
    {
        bool allCorrect = true;

        foreach (var piece in _mirrorPieces)
        {
            if (!piece.GetComponent<PuzzlePiece>().isRight)
            {
                allCorrect = false;
                break;
            }
        }

        if (allCorrect)
        {
            Debug.Log("퍼즐 클리어!");
            isPuzzleClear = true;
            SoundManager.Instance.PlaySFX("Soundresource_083");
            //퀴즈 해결되었다고 표시
            _curQuiz.isSolved = true;
            UIManager.Instance.CloseTopUI();
            QuizManager.Instance.OnQuizEnd?.Invoke();
        }
    }

    public void ResetAllPieces() //거울 퍼즐 초기화
    {
        OnResetPuzzle?.Invoke();
    }
}