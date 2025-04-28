using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using NooSphere;
using Debug = NooSphere.Debug;  

public class KeyPadManager : UIBase
{
    private string _curQuizID;
    private QuizStructure _curQuiz;
    [SerializeField] private TextMeshProUGUI _inputText;
    
    private static KeyPadManager _instance;
    public static KeyPadManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<KeyPadManager>();
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject(nameof(KeyPadManager));
                    _instance = singletonObject.AddComponent<KeyPadManager>();
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
        _inputText.text ="";
    }
    public override void OnClose()
    {
        base.OnClose();
        transform.GetChild(0).gameObject.SetActive(false);
        QuizManager.Instance.OnQuizEnd?.Invoke();
    }
    public void SetDialText(string number)
    {
        if (number == "-1")
        {
            if (_inputText.text.Length > 0)
            {
                _inputText.text = "";
            }
        }
        else if (number == "*")
        {
            CheckAnswer();
        }
        else
        {
            if (_inputText.text.Length < 4)
            {
                _inputText.text += number;
            } 
        }
    }

    private void CheckAnswer()
    {
        if(_inputText.text ==_curQuiz.correctAnswer)
        {
            _curQuiz.isSolved = true;
            SoundManager.Instance.PlaySFX("Soundresource_084");
            UIManager.Instance.CloseTopUI();
        }
        else
        {
            SoundManager.Instance.PlaySFX("Soundresource_055");
            _inputText.text = "";
        }
    }

}
