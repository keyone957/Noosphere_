using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class cabinetTrigger : MonoBehaviour
{
    private bool isTriggered = false;
    private bool isDialogueActive = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isTriggered = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isTriggered = false;
        }
    }

    private void Update()
    {
        if (isTriggered && Input.GetKeyDown(KeyCode.E))
        {
            ToggleDialogue();
        }
    }

    private void ToggleDialogue()
    {
        /*
        if (!isDialogueActive && !EventManagerYKM.Instance.isGetDiary)
        {
            // 'E' 키 입력 처리 - 대화 창 표시
            DialogueTextON.Instance.ShowSimpleText("굳게 닫혀있다");
            isDialogueActive = true;
        }
        else
        {
            // 'E' 키 입력 처리 - 대화 창 숨기기
            DialogueTextON.Instance.HideDialogue();
            isDialogueActive = false;
        }
        */
    }
}