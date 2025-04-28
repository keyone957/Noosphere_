using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;


public class DialogueCSVParser 
{
     public async UniTask<Dictionary<string, DialogueStructure>> Parse(string sheetName)
        {
            Dictionary<string, DialogueStructure> dictionary = new Dictionary<string, DialogueStructure>();
            string csvUrl = $"https://docs.google.com/spreadsheets/d/1rxLYxA5PoZcaMP9xGD0NrBs78GOLY9sKJv7_Ft9oPww/gviz/tq?tqx=out:csv&sheet={sheetName}";
            
            string csvData = await LoadCSVFromURL(csvUrl);
            if (string.IsNullOrWhiteSpace(csvData)) return dictionary;
    
            string[] lines = csvData.Split("\n");
            string[] headers = GetHeaders(lines[5]);
            
            DialogueStructure currentDialogue = null;
            string currentDialogueId = "";
    
            for (int i = 6; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                if (string.IsNullOrWhiteSpace(line)) continue;
    
                string[] values = SplitCsvLine(line);
                if (values.Length == 0) continue;
    
                string dialogueId = values[0].Trim().Replace("\"", "");
    
                if (!string.IsNullOrEmpty(dialogueId))
                {
                    currentDialogueId = dialogueId;
                    currentDialogue = new DialogueStructure();
                    currentDialogue.dialogueId = dialogueId;
                    currentDialogue.Dialogue_Text_List = new List<DialogueStructure.DialougeText>();
    
                    for (int j = 1; j < headers.Length && j < values.Length; j++)
                    {
                        string header = headers[j].Trim().Replace("\"", "");
                        if(string.IsNullOrEmpty(header)) continue;
    
                        string value = values[j].Trim().Replace("\"", "");
                        SetField(currentDialogue, header, value);
                    }

                    DialogueStructure.DialougeText dialougeText = new DialogueStructure.DialougeText();
                    dialougeText.text = currentDialogue.dialogueText;
                    dialougeText.tutorialID = currentDialogue.tutorialId;
                    
                    currentDialogue.Dialogue_Text_List.Add(dialougeText);
                    dictionary[currentDialogueId] = currentDialogue;
                }
                else if (currentDialogue != null)
                {
                    int textIndex = Array.FindIndex(headers, h => h.Trim().Replace("\"", "") == "dialogueText");
                    int tutorialIndex = Array.FindIndex(headers, h => h.Trim().Replace("\"", "") == "tutorialId");
                    if (textIndex >= 0 && textIndex < values.Length && tutorialIndex >= 0 && tutorialIndex < values.Length)
                    {
                        string additionalText = values[textIndex].Trim().Replace("\"", "");
                        string tutorialID = values[tutorialIndex].Trim().Replace("\"", "");
                        
                        DialogueStructure.DialougeText dialougeText = new DialogueStructure.DialougeText();
                        dialougeText.text = additionalText;
                        dialougeText.tutorialID = tutorialID;
                        
                        currentDialogue.Dialogue_Text_List.Add(dialougeText);
                    }
                }
            }
    
            return dictionary;
        }
    
        private async UniTask<string> LoadCSVFromURL(string url)
        {
            using (UnityWebRequest www = UnityWebRequest.Get(url))
            {
                try
                {
                    await www.SendWebRequest();
                    if (www.result != UnityWebRequest.Result.Success)
                    {
                        Debug.LogError($"CSV 다운로드 실패: {www.error}");
                        return string.Empty;
                    }
                    return www.downloadHandler.text;
                }
                catch (Exception e)
                {
                    Debug.LogError($"CSV 다운로드 중 오류: {e.Message}");
                    return string.Empty;
                }
            }
        }
    
        private string[] GetHeaders(string headerLine)
        {
            return SplitCsvLine(headerLine);
        }
    
        private string[] SplitCsvLine(string line)
        {
            List<string> values = new List<string>();
            bool inQuotes = false;
            string current = "";
    
            foreach (char c in line)
            {
                if (c == '\"')
                {
                    inQuotes = !inQuotes;
                    continue;
                }
    
                if (c == ',' && !inQuotes)
                {
                    values.Add(current);
                    current = "";
                }
                else
                {
                    current += c;
                }
            }
            values.Add(current);
            
            return values.ToArray();
        }
    
        private void SetField(DialogueStructure dialogue, string fieldName, string value)
        {
            FieldInfo field = typeof(DialogueStructure).GetField(fieldName);
            if (field != null)
            {
                try
                {
                    object convertedValue = Convert.ChangeType(value, field.FieldType);
                    field.SetValue(dialogue, convertedValue);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"필드 변환 오류 - 필드: {fieldName}, 값: {value}, 오류: {ex.Message}");
                }
            }
        }
}