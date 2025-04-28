using System;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class dialoguetest : MonoBehaviour
{
    private Dictionary<string, DialogueStructure> dialogueDictionary = new Dictionary<string, DialogueStructure>();
    [SerializeField] private string GetId;

    private async void Start()
    {
        await InitializeDialogueData();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            TestDialogue(GetId);
        }
    }

    private void TestDialogue(string id)
    {
        if (dialogueDictionary != null && dialogueDictionary.Count > 0)
        {
            if (dialogueDictionary.TryGetValue(id, out DialogueStructure dialogue))
            {
                Debug.Log($"=== Dialogue Data for ID: {id} ===");
                Debug.Log($"Dialogue ID: {dialogue.dialogueId}");
                Debug.Log($"Character ID: {dialogue.characterId}");
                Debug.Log($"Trigger Type: {dialogue.triggerType}");
                Debug.Log($"Interaction Type: {dialogue.interactionType}");
                Debug.Log($"Next Dialogue ID: {dialogue.nextDialougeId}");
            
                if (dialogue.Dialogue_Text_List != null && dialogue.Dialogue_Text_List.Count > 0)
                {
                    Debug.Log("=== Dialogue Texts ===");
                    for (int i = 0; i < dialogue.Dialogue_Text_List.Count; i++)
                    {
                        Debug.Log($"Text {i + 1}: {dialogue.Dialogue_Text_List[i]}");
                    }
                }
            }
            else
            {
                Debug.LogError($"ID '{id}'를 찾을 수 없습니다.");
            }
        }
        else
        {
            Debug.LogError("대화 데이터가 로드되지 않았습니다.");
        }
    }

    
    private async UniTask InitializeDialogueData()
    {
        try
        {
            dialogueDictionary = await LoadDialogueData();
            Debug.Log("대화 데이터 초기화 완료");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"대화 데이터 초기화 실패: {e.Message}");
        }
    }

    private async UniTask<Dictionary<string, DialogueStructure>> LoadDialogueData()
    {
        DialogueCSVParser parser = new DialogueCSVParser();
        return await parser.Parse("DialogueTest");
    }

    
}