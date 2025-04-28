using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;

public class DialogueManagerLegacy : Singleton<DialogueManagerLegacy>
{
    /*
    [SerializeField] private GameObject _toggleIcon;
    private string _currentDialogueId = "";
    private int _currentLineIndex = 0;
    private string _initialDialogueId = "";

    private float _letterDelay = 0.05f;
    private float _currentTextElapsedTime = 0f;
    private int _currentLetterIndex = 0;
    public bool isTyping = false;
    public bool isDialogeEnd = false; // 완전히 대화 끝났을때 true반환

    private void Start()
    {
        InputManager.Instance.exitBtnAction += OnEscapePressed;
    }

    private void OnDestroy()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.exitBtnAction -= OnEscapePressed;
        }
    }

    public void SetDialogue(string id)
    {
        _currentDialogueId = id;
        isDialogeEnd = false;
        if (DataManager.Instance._dialogue.TryGetValue(_currentDialogueId, out DialogueStructure dialogue))
        {
            if (dialogue.triggerType == "auto") //대화창 바로 뜨기
            {
                PlayerController.Instance.isDialogueOn = true;
                UIManager.Instance.dialogueUI.gameObject.SetActive(true);
                ShowNextLine().Forget();
            }

            if (dialogue.triggerType == "interact")
            {
                UIManager.Instance.dialogueUI.gameObject.SetActive(false);
                PlayerController.Instance.isDialogueOn = false;
                SetInteractDialogue(dialogue.interactionType);
            }
        }
        else
        {
            Debug.LogWarning($" {_currentDialogueId} 못찾음");
        }
    }

    private void SetInteractDialogue(string interactionType)
    {
        if (interactionType == "npc"||interactionType=="object")
        {
            GameObject npcObject = GameObject.Find(DataManager.Instance._dialogue[_currentDialogueId].characterID);
            if (npcObject != null)
            {
                NpcDialogue npcDialogue = npcObject.GetComponent<NpcDialogue>();
                npcDialogue.dialogueId = _currentDialogueId;
                // npcDialogue.dialogueText = _dialogue[_currentDialogueId].Dialogue_Text_List;
            }
            else
            {
                Debug.LogWarning($"{DataManager.Instance._dialogue[_currentDialogueId].characterID} npc가 없습니다");
            }
        }
        // else if (interactionType == "object")
        // {
        //     //상호작용 대상이 물건일때 .
        // }
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.E)&&!string.IsNullOrEmpty(_currentDialogueId))
        {
            if (DataManager.Instance._dialogue[_currentDialogueId].triggerType == "auto")
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
            else if (DataManager.Instance._dialogue[_currentDialogueId].triggerType == "interact")
            {
                 if (PlayerController.Instance._currentNPC != null && PlayerController.Instance.isPlayerNearNPC)
                {
                    NpcDialogue npcDialogue = PlayerController.Instance._currentNPC.GetComponent<NpcDialogue>();

                    if (npcDialogue != null && !string.IsNullOrEmpty(npcDialogue.dialogueId))
                    {
                        if (!UIManager.Instance.dialogueUI.gameObject.activeSelf)
                        {
                            PlayerController.Instance.isDialogueOn = true;
                            PlayerController.Instance.NpcCameraOn();
                            UIManager.Instance.dialogueUI.gameObject.SetActive(true);
                            StartDialogue(npcDialogue.dialogueId);
                        }
                        else if (isTyping)
                        {
                            isTyping = false;
                        }
                        else
                        {
                            ShowNextLine().Forget();
                        }
                    }
                }
            }
        }


        // if (Input.GetKeyDown(KeyCode.E))
        // {
        //     if (!string.IsNullOrEmpty(_currentDialogueId) &&
        //         DataManager.Instance._dialogue[_currentDialogueId].trigger_type == "auto") // auto 타입 체크
        //     {
        //         if (isTyping)
        //         {
        //             isTyping = false;
        //         }
        //         else
        //         {
        //             ShowNextLine().Forget();
        //         }
        //     }
        // }=====================
    }
    public void StartDialogue(string dialogueId)
    {
        // UIManager.Instance.popUI.gameObject.SetActive(false);
        if (DataManager.Instance._dialogue.ContainsKey(dialogueId))
        {
            _currentDialogueId = dialogueId;
            _currentLineIndex = 0;
            ShowNextLine().Forget();
        }
        else
        {
            Debug.LogWarning($"Dialogue ID {dialogueId} not found.");
        }
    }

    public async UniTaskVoid ShowNextLine()
    {
        _toggleIcon.SetActive(false);
        if (string.IsNullOrEmpty(_currentDialogueId))
        {
            Debug.LogWarning("현재 대화 ID가 설정되지 않았습니다.");
            return;
        }

        if (!DataManager.Instance._dialogue.TryGetValue(_currentDialogueId, out DialogueStructure dialogue))
        {
            Debug.LogWarning($"Dialogue ID {_currentDialogueId} not found.");
            return;
        }

        if (_currentLineIndex < dialogue.Dialogue_Text_List.Count)
        {
            // 타이핑 시작
            await TypeText(dialogue.Dialogue_Text_List[_currentLineIndex]);
            // 타이핑이 완료되었거나 스킵되었을 때만 다음 라인으로 진행
            _currentLineIndex++;
        }
        else
        {
            if (!string.IsNullOrEmpty(dialogue.nextDialougeId))
            {
                if (DataManager.Instance._dialogue.ContainsKey(dialogue.nextDialougeId))
                {
                    _currentDialogueId = dialogue.nextDialougeId;
                    _currentLineIndex = 0;
                    ShowNextLine().Forget();
                }
                else  
                {
                    Debug.LogWarning($"Next Dialogue ID {dialogue.nextDialougeId} not found.");
                }
            }
            else
            {
               
                Debug.LogWarning("대화가 종료되었습니다.");
                
                PlayerController.Instance.isDialogueOn = false;
                isDialogeEnd = true;
                _currentDialogueId = "";
                _currentLineIndex = 0;
                UIManager.Instance.dialogueUI.gameObject.SetActive(false);
                PlayerController.Instance.ResetCamera();
                if (PlayerController.Instance._currentNPC != null)
                {
                    NpcDialogue npcDialogue = PlayerController.Instance._currentNPC.GetComponent<NpcDialogue>();
                    if (npcDialogue != null)
                    {
                        npcDialogue.dialogueId = string.Empty;
                    }
                    PlayerController.Instance.isPlayerNearNPC = false;
                    PlayerController.Instance._currentNPC = null;
                }

            }
        }
    }


    private async UniTask TypeText(string text)
    {
        isTyping = true;
        UIManager.Instance.dialogueUI.text = "";

        if (!isTyping)
        {
            UIManager.Instance.dialogueUI.text = text;
            return;
        }

        for (int i = 0; i < text.Length && isTyping; i++)
        {
            UIManager.Instance.dialogueUI.text += text[i];
            await UniTask.Delay((int)(_letterDelay * 1000));
        }

        UIManager.Instance.dialogueUI.text = text;
        isTyping = false;
        
        _toggleIcon.SetActive(true);
    }

    private void OnEscapePressed()
    {
        if (!string.IsNullOrEmpty(_currentDialogueId) &&
            DataManager.Instance._dialogue[_currentDialogueId].triggerType == "interact" &&
            PlayerController.Instance.isDialogueOn)
        {
            PlayerController.Instance.isDialogueOn = false;
            isDialogeEnd = true;
            _currentDialogueId = "";
            _currentLineIndex = 0;
            UIManager.Instance.dialogueUI.gameObject.SetActive(false);
            //UIManager.Instance.PopUp(true, "E를 눌러 대화시작");
        }
    }
    */
}