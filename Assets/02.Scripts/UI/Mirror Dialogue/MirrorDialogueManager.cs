using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MirrorDialogueManager : UIBase
{
    [SerializeField] private string _curQuizID;
    private QuizStructure _curQuiz;
    [SerializeField] private TextMeshProUGUI _dialogueText;
    public bool isTyping = false;
    
    private DialogueStructure _curDialogue;
    private float _letterDelay = 0.05f;
    private float _currentTextElapsedTime = 0f;
    private int _currentLetterIndex = 0;

    [SerializeField] private GameObject _btns;
    [SerializeField] private GameObject _curSelectedBtn;
    [SerializeField] private GameObject _yesBtn;
    [SerializeField] private GameObject _noBtn;
    
    public override async void OnOpen(string quizID)
    {
        base.OnOpen(quizID);
        _curQuizID = "";
        _curQuiz = null;
        _curQuizID = quizID;
        _curQuiz = DataManager.Instance._quiz[_curQuizID];
        _curDialogue = DataManager.Instance._dialogue["Dialogue_0030"];
        SoundManager.Instance.PlaySFX("Soundresource_076");
        transform.GetChild(0).gameObject.SetActive(true);
        await TypeText(_curDialogue.Dialogue_Text_List[0].text);
        SoundManager.Instance.StopSFX("Soundresource_076");
        HoverYesBtn();
        _btns.SetActive(true);
    }

    public override void OnClose()
    {
        base.OnClose();
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public override void HandleKeyboardInput()
    {
        base.HandleKeyboardInput();
        
        //키보드 A - YES 버튼
        if (Input.GetKeyDown(KeyCode.W))
        {
            SoundManager.Instance.PlaySFX("Soundresource_035");
            HoverYesBtn();
        }

        //키보드 D - NO 버튼
        if (Input.GetKeyDown(KeyCode.S))
        { 
            SoundManager.Instance.PlaySFX("Soundresource_035");
            HoverNoBtn();
        }

        //스페이스 - 버튼 선택
        if (_curSelectedBtn != null && Input.GetKeyDown(KeyCode.Space))
        {
            SoundManager.Instance.PlaySFX("Soundresource_037");
            if (_curSelectedBtn == _yesBtn)
            {
                ClickYesBtn();
            }
            else if (_curSelectedBtn == _noBtn)
            {
                ClickNoBtn();
            }
        }
    }

    private async UniTask TypeText(string text)
    {
        isTyping = true;
        _dialogueText.text = "";

        if (!isTyping)
        {
            _dialogueText.text = text;
            return;
        }

        for (int i = 0; i < text.Length && isTyping; i++)
        {
            _dialogueText.text += text[i];
            await UniTask.Delay((int)(_letterDelay * 1000));
        }

        _dialogueText.text = text;
        isTyping = false;
    }
    
    public void HoverYesBtn()
    {
        SetButtonSelected(_yesBtn, UnityExtension.HexColor(RedColor));
        SetButtonSelected(_noBtn, UnityExtension.HexColor(WhiteColor));
        _curSelectedBtn = _yesBtn;
    }

    public void HoverNoBtn()
    {
        SetButtonSelected(_noBtn,UnityExtension.HexColor(RedColor));
        SetButtonSelected(_yesBtn,UnityExtension.HexColor(WhiteColor));
        _curSelectedBtn = _noBtn;
    }

    public void ClickYesBtn()
    {
        UIManager.Instance.isYesClicked = true;
        HoverYesBtn();
        UIManager.Instance.CloseTopUI();
        DataManager.Instance._quiz["Quiz_009"].isSolved = true;
        QuizManager.Instance.OnQuizEnd?.Invoke();
    }

    public void ClickNoBtn()
    {
        HoverNoBtn();
        UIManager.Instance.CloseTopUI();
        DataManager.Instance._quiz["Quiz_009"].isSolved = false;
        QuizManager.Instance.OnQuizEnd?.Invoke();
    }
}
