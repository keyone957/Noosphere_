using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class DialogueTextON : Singleton<DialogueTextON>
{
    [SerializeField] private TextMeshProUGUI dialogueUI;
    [SerializeField] private GameObject dialoguePanel;

    private void Update()
    {
        if (dialoguePanel.activeSelf && Input.GetKeyDown(KeyCode.E))
        {
            HideDialogue();
        }
    }

    public void ShowSimpleText(string text)
    {
        dialoguePanel.SetActive(true);
        dialogueUI.text = text;
    }

    public void HideDialogue()
    {
        dialoguePanel.SetActive(false);
    }
}
