using System.Collections.Generic;
using UnityEngine;

public class DataManager_ : Singleton<DataManager_>
{
    private Dictionary<string, CustomEventData> eventDictionary 
        = new Dictionary<string, CustomEventData>();
    private Dictionary<string, DialogueData> dialogueDictionary
        = new Dictionary<string, DialogueData>();
    
    public void InitializeEvents(List<CustomEventData> eventDataList)
    {
        eventDictionary.Clear(); // 기존 데이터 초기화

        foreach (var eventData in eventDataList)
        {
            if (!eventDictionary.ContainsKey(eventData.EventId))
            {
                eventDictionary.Add(eventData.EventId, eventData);
            }
            else
            {
                Debug.LogWarning($"Duplicate EventId found: {eventData.EventId}");
            }
        }

        Debug.Log($"Initialized {eventDictionary.Count} events in DataManager.");
    }
    
    public void InitializeDialogues(List<DialogueData> dialogueDataList)
    {
        dialogueDictionary.Clear(); // 기존 데이터 초기화

        foreach (var dialogueData in dialogueDataList)
        {
            if (!dialogueDictionary.ContainsKey(dialogueData.DialogueId))
            {
                dialogueDictionary.Add(dialogueData.DialogueId, dialogueData);
            }
            else
            {
                Debug.LogWarning($"Duplicate DialogueId found: {dialogueData.DialogueId}");
            }
        }

        Debug.Log($"Initialized {dialogueDictionary.Count} dialogues in DataManager.");
        
    }

    /// <summary>
    /// 이벤트 ID로 이벤트 데이터를 가져옵니다.
    /// </summary>
    /// <param name="eventId">이벤트 ID</param>
    /// <returns>해당 이벤트 데이터 또는 null</returns>
    public CustomEventData GetEventById(string eventId)
    {
        if (eventDictionary.TryGetValue(eventId, out var eventData))
        {
            return eventData;
        }

        Debug.LogWarning($"Event with ID {eventId} not found.");
        return null;
    }

    /// <summary>
    /// 대화 ID로 대화 데이터를 가져옵니다.
    /// </summary>
    /// <param name="dialogueId">대화 ID</param>
    /// <returns>해당 대화 데이터 또는 null</returns>
    public DialogueData GetDialogueById(string dialogueId)
    {
        if (dialogueDictionary.TryGetValue(dialogueId, out var dialogueData))
        {
            return dialogueData;
        }

        Debug.LogWarning($"Dialogue with ID {dialogueId} not found.");
        return null;
    }
    
    public void PrintEventDetails(string eventId)
    {
        var eventData = GetEventById(eventId);
        if (eventData != null)
        {
            Debug.Log("=== " + eventId + " Details ===");
            Debug.Log($"Event ID: {eventData.EventId}");
            Debug.Log($"Description: {eventData.Description}");
            Debug.Log($"Repeat Type: {eventData.RepeatType}");
            Debug.Log($"Condition Type: {eventData.ConditionType}");
            Debug.Log($"Conditions: {string.Join(", ", eventData.Conditions)}");
            Debug.Log($"Results: {string.Join(", ", eventData.Results)}");
            Debug.Log($"Evidence ID: {eventData.EvidenceId}");
            Debug.Log($"Lock Condition ID: {eventData.LockConditionId}");
            Debug.Log($"Location ID: {eventData.LocationId}");
            Debug.Log($"Next Event ID: {eventData.NextEventId}");
            Debug.Log("==============================");
        }
        else
        {
            Debug.LogWarning($"{eventId}을(를) 찾을 수 없습니다.");
        }
    }
    
    public void PrintDialogueDetails(string dialogueId)
    {
        var dialogueData = GetDialogueById(dialogueId);
        if (dialogueData != null)
        {
            Debug.Log("=== " + dialogueId + " Details ===");
            Debug.Log($"Dialogue ID: {dialogueData.DialogueId}");
            Debug.Log($"Character ID: {dialogueData.CharacterId}");
            // foreach (var line in dialogueData.DialogueTextLines)
            // {
            //     Debug.Log($"Dialogue Line: {line}");
            // }
            Debug.Log($"Next Dialogue ID: {dialogueData.NextDialogueId}");
            Debug.Log("==============================");
        }
        else
        {
            Debug.LogWarning($"{dialogueId}을(를) 찾을 수 없습니다.");
        }
    }
}

