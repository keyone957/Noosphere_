using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Debug = NooSphere.Debug;


public class DialogueManager : UIBase
{
    // 텍스트 요소를 나타내는 클래스
    private class TextElement
    {
        public string content;
        public bool isTag; // true면 태그, false면 일반 텍스트
    }

    //싱글톤
    private static DialogueManager _instance;

    public static DialogueManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<DialogueManager>();
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject(nameof(DialogueManager));
                    _instance = singletonObject.AddComponent<DialogueManager>();
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
        DontDestroyOnLoad(gameObject);
        contentSizeFitter = dialogueText.gameObject.AddComponent<ContentSizeFitter>();
        contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        textRectTransform = dialogueText.GetComponent<RectTransform>();
        textRectTransform.pivot = new Vector2(0, 0.5f); //
    }

    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private GameObject _dialogueGroup;
    [SerializeField] private TextMeshProUGUI _speakerText;

    private DialogueStructure _curDialogue;

    private string _currentDialogueId = "";
    private int _currentLineIndex = 0;
    private string _initialDialogueId = "";

    [SerializeField] float _letterDelay = 0.05f;
    private float _currentTextElapsedTime = 0f;
    private int _currentLetterIndex = 0;
    public bool isTyping = false;
    public Action OnDialogueEnd;
    private ContentSizeFitter contentSizeFitter;
    private RectTransform textRectTransform;


    public override void OnOpen()
    {
        base.OnOpen();
        _dialogueGroup.SetActive(true);
    }

    public override void OnClose()
    {
        base.OnClose();
        _dialogueGroup.SetActive(false);

        if (_curDialogue.triggerType == "interact" && _curDialogue.interactionType == "npc")
        {
            PlayerController.Instance.ResetCamera();
        }
    }

    public void SetDialogue(string id)
    {
        InitDialogue();
        if (!string.IsNullOrEmpty(id) && DataManager.Instance._dialogue.ContainsKey(id))
        {
            _curDialogue = DataManager.Instance._dialogue[id];
            _currentDialogueId = id;
            EscapeUI.Instance.DisActive();
            UIManager.Instance.OpenUI(UIManager.Instance.dialogueUI);

            if (_curDialogue.triggerType == "interact" && _curDialogue.interactionType == "npc")
            {
                PlayerController.Instance.NpcCameraOn();
            }

            ShowNextLine().Forget();
        }
        else
        {
            Debug.LogWarning($" {_currentDialogueId} 못찾음");
        }
    }

    public override void HandleKeyboardInput()
    {
        if ((InputRouter.Instance.ConsumeSpace() || Input.GetMouseButtonDown(0)) &&
            !string.IsNullOrEmpty(_currentDialogueId))
        {
            if (isTyping)
            {
                isTyping = false;
            }
            else
            {
                ShowNextLine().Forget();
            }
        }
    }

    public async UniTask ShowNextLine()
    {
        // _speakerText.text = _curDialogue.characterId;
        if (_currentLineIndex < _curDialogue.Dialogue_Text_List.Count)
        {
            //튜토리얼 있으면 실행
            TutorialManager.Instance.ShowTutorial(_curDialogue.Dialogue_Text_List[_currentLineIndex].tutorialID);
            // 타이핑 시작
            string speakerName = LocalizationSettings.StringDatabase.GetLocalizedString(LocalConstants.CharacterTable, _curDialogue.Dialogue_Text_List[_currentLineIndex].characterId, LocalizationSettings.SelectedLocale);
            _speakerText.text = speakerName;
            await TypeText(_curDialogue.Dialogue_Text_List[_currentLineIndex].text);
            // 타이핑이 완료되었거나 스킵되었을 때만 다음 라인으로 진행
            _currentLineIndex++;
        }
        else
        {
            if (!string.IsNullOrEmpty(_curDialogue.nextDialougeId))
            {
                if (DataManager.Instance._dialogue.ContainsKey(_curDialogue.nextDialougeId))
                {
                    _currentDialogueId = _curDialogue.nextDialougeId;
                    _curDialogue = DataManager.Instance._dialogue[_currentDialogueId];
                    _currentLineIndex = 0;
                    ShowNextLine().Forget();
                }
                else
                {
                    Debug.LogWarning($"Next Dialogue ID {_curDialogue.nextDialougeId} not found.");
                }
            }
            else
            {
                Debug.LogWarning("대화가 종료되었습니다.");
                UIManager.Instance.CloseTopUI();

                //예외 이벤트 처리 코드
                if (EventManager.Instance.currentEventID == "Event_A004")
                {
                    GameObject.Find("Artresource_0002").transform.GetChild(0).gameObject.SetActive(false);
                }

                OnDialogueEnd?.Invoke();

                //현재 위치한 곳에 트리거가 있다면 해당 트리거 실행 가능한지 다시 체크
                if (PlayerInteract.Instance.isInsideTrigger && PlayerInteract.Instance.curTrigger != null)
                {
                    await Task.Delay(100);
                    if (PlayerInteract.Instance.isInsideTrigger && PlayerInteract.Instance.curTrigger != null)
                    {
                        if (_curDialogue.dialogueId == "Dialogue_0065") return;
                        PlayerInteract.Instance.curTrigger.OnTriggerEnter(PlayerInteract.Instance
                            .GetComponent<CapsuleCollider>());
                    }
                }
            }
        }
    }
    private async UniTask TypeText(string key)
    {
        string text = LocalizationSettings.StringDatabase.GetLocalizedString(LocalConstants.DialogueTable, key, LocalizationSettings.SelectedLocale);
        isTyping = true;

        dialogueText.alpha = 0;
        text = text.Replace("\\n", "\n");
        dialogueText.text = text;
        await UniTask.NextFrame();

        float totalWidth = dialogueText.preferredWidth;
        Canvas canvas = dialogueText.transform.GetComponentInParent<Canvas>();
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        float canvasCenter = canvasRect.rect.width / 2f;
        float startX = canvasCenter - (totalWidth / 2f);
        textRectTransform.anchoredPosition = new Vector2(startX, textRectTransform.anchoredPosition.y);

        dialogueText.text = "";
        dialogueText.alpha = 1;
        StartDialogueSound(_curDialogue.Dialogue_Text_List[_currentLineIndex].characterId);

        if (!isTyping)
        {
            dialogueText.text = text;
            return;
        }

        // 텍스트를 태그와 글자로 파싱
        List<TextElement> elements = ParseTextWithTags(text);
        string displayText = "";

        foreach (var element in elements)
        {
            if (!isTyping) break;

            if (element.isTag)
            {
                // 태그는 한번에 추가 (딜레이 없음)
                displayText += element.content;
            }
            else
            {
                // 글자는 하나씩 나타내기
                foreach (char c in element.content)
                {
                    if (!isTyping) break;

                    displayText += c;
                    dialogueText.text = displayText;
                    await UniTask.Delay((int)(_letterDelay * 1000));
                }
            }
        }

        dialogueText.text = text;
        isTyping = false;
        SoundManager.Instance.StopAllSFX();
    }

    // 텍스트를 태그와 일반 텍스트로 파싱
    private List<TextElement> ParseTextWithTags(string text)
    {
        List<TextElement> elements = new List<TextElement>();
        int i = 0;

        while (i < text.Length)
        {
            if (text[i] == '<')
            {
                // 태그 찾기
                int closeIndex = text.IndexOf('>', i);
                if (closeIndex != -1)
                {
                    string tag = text.Substring(i, closeIndex - i + 1);
                    elements.Add(new TextElement
                    {
                        content = tag,
                        isTag = true
                    });
                    i = closeIndex + 1;
                    continue;
                }
            }

            // 다음 태그까지 또는 끝까지 일반 텍스트 수집
            int nextTagIndex = text.IndexOf('<', i);
            if (nextTagIndex == -1)
                nextTagIndex = text.Length;

            string textContent = text.Substring(i, nextTagIndex - i);
            if (textContent.Length > 0)
            {
                elements.Add(new TextElement
                {
                    content = textContent,
                    isTag = false
                });
            }

            i = nextTagIndex;
        }

        return elements;
    }

    void InitDialogue()
    {
        _currentDialogueId = "";
        _currentLineIndex = 0;
        _curDialogue = null;
        PlayerController.Instance.ResetCamera();
    }

    private void StartDialogueSound(string speaker)
    {
        switch (speaker)
        {
            case "Character_Local_001":
                SoundManager.Instance.PlayLoopingSound("Soundresource_026");
                break;
            case "Character_Local_002":
                SoundManager.Instance.PlayLoopingSound("Soundresource_027");
                break;
            case "Character_Local_003":
                SoundManager.Instance.PlayLoopingSound("Soundresource_077");
                break;
            case "Character_Local_004":
                SoundManager.Instance.PlayLoopingSound("Soundresource_076");
                break;
            case "Character_Local_006":
                SoundManager.Instance.PlayLoopingSound("Soundresource_075");
                break;
            case "Character_Local_005":
                SoundManager.Instance.PlayLoopingSound("Soundresource_076");
                break;
        }
    }

    public string GetCurDialogueId()
    {
        return _currentDialogueId;
    }
}