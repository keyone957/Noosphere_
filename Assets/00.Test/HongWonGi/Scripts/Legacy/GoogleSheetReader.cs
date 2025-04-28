using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks; 
using System.Linq;
public class GoogleSheetLoader : MonoBehaviour
{
    [SerializeField] private string sheetId = "1_FGkVesLGBKbpmC8z1mUbG4eAKJRUxRIqBI6XJeGda8"; // 스프레드시트 ID
    [SerializeField] private List<string> sheetNames = new List<string> { "Event", "Dialogue" };

    private List<CustomEventData> eventList = new List<CustomEventData>();
    private List<DialogueData> dialogueList = new List<DialogueData>();
    async void Start()
    {
        LoadAllSheetsAsync().Forget();
    }

    private async UniTaskVoid LoadAllSheetsAsync()
    {
        foreach (var sheetName in sheetNames)
        {
            await LoadSheetAsync(sheetName);
        }

        // 모든 시트 로드가 끝난 후 DataManager_ 초기화
        if (eventList.Count > 0)
        {
            DataManager_.Instance.InitializeEvents(eventList);
        }

        if (dialogueList.Count > 0)
        {
            DataManager_.Instance.InitializeDialogues(dialogueList);
        }
    }

    private async UniTask LoadSheetAsync(string sheetName)
    {
        string csvUrl = $"https://docs.google.com/spreadsheets/d/{sheetId}/gviz/tq?tqx=out:csv&sheet={sheetName}";

        using (UnityWebRequest www = UnityWebRequest.Get(csvUrl))
        {
            var operation = www.SendWebRequest();

            while (!operation.isDone)
            {
                await UniTask.Yield();
            }

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"[{sheetName}] 데이터 로드 실패: {www.error}");
            }
            else
            {
                string csvData = www.downloadHandler.text;
                Debug.Log($"[{sheetName}] CSV 데이터 로드 성공");
                ProcessSheetData(sheetName, csvData);
            }
        }
    }

    private void ProcessSheetData(string sheetName, string csvData)
    {
        string[] rows = csvData.Split('\n');

        if (rows.Length <= 6)
        {
            Debug.LogWarning($"[{sheetName}] 데이터가 충분하지 않습니다.");
            return;
        }

        // 첫 5줄 무시, 6번째 줄은 헤더, 7번째 줄부터 데이터
        string[] headers = ParseCSVLine(rows[5]);

        for (int i = 1; i < rows.Length; i++) // 6번째 인덱스부터 (0 기반)
        {
            if (string.IsNullOrWhiteSpace(rows[i])) continue;

            string[] values = ParseCSVLine(rows[i]);

            // 데이터의 열 수가 예상과 다를 경우 처리
            if (values.Length < headers.Length)
            {
                Debug.LogWarning($"[{sheetName}] 데이터 행 {i + 1}의 열 수가 부족합니다.");
                continue;
            }

            if (sheetName == "Event")
            {
                CustomEventData eventData = new CustomEventData
                {
                    EventId = values[0].Trim(),
                    Description = values[1].Trim(),
                    RepeatType = values[2].Trim().ToUpper() == "TRUE",
                    ConditionType = values[3].Trim(),
                    Conditions = new List<string>
                    {
                        string.IsNullOrEmpty(values[4].Trim()) ? null : values[4].Trim(),
                        string.IsNullOrEmpty(values[5].Trim()) ? null : values[5].Trim(),
                        string.IsNullOrEmpty(values[6].Trim()) ? null : values[6].Trim()
                    },
                    Results = new List<string>
                    {
                        string.IsNullOrEmpty(values[7].Trim()) ? null : values[7].Trim(),
                        string.IsNullOrEmpty(values[8].Trim()) ? null : values[8].Trim(),
                        string.IsNullOrEmpty(values[9].Trim()) ? null : values[9].Trim()
                    },
                    EvidenceId = string.IsNullOrEmpty(values[10].Trim()) ? null : values[10].Trim(),
                    LockConditionId = string.IsNullOrEmpty(values[11].Trim()) ? null : values[11].Trim(),
                    LocationId = string.IsNullOrEmpty(values[12].Trim()) ? null : values[12].Trim(),
                    NextEventId = values.Length > 13 ? (string.IsNullOrEmpty(values[13].Trim()) ? null : values[13].Trim()) : null
                };

                eventList.Add(eventData);
            }
            else if (sheetName == "Dialogue")
            {
            //     List<string> dialogueTextLines = new List<string>(values[2].Split('\n'));
            //     DialogueData dialogueData = new DialogueData
            //     {
            //         DialogueId = values[0].Trim(),
            //         CharacterId = values[1].Trim(),
            //         DialogueTextLines = dialogueTextLines,
            //         NextDialogueId = values.Length > 3 ? (string.IsNullOrEmpty(values[3].Trim()) ? null : values[3].Trim()) : null
            //     };
            //     
            //     dialogueList.Add(dialogueData);
            //     
            //     // 대화 텍스트를 줄바꿈 기준으로 분리하고 빈 줄 제거
            //     //
            //     // DialogueId와 CharacterId는 첫 번째 줄에 존재하지만, DialogueText는 여러 줄로 구성되어 있음
             // if (values.Length < 4)  // dialogue_id, character_id, dialogue_text, next_dialogue_id 총 4개가 필요
             //    {
             //        Debug.LogWarning($"[{sheetName}] Row has insufficient columns. Expected 4, got {values.Length}");
             //        continue;
             //    }
             //
             //    DialogueData dialogueData = new DialogueData
             //    {
             //        DialogueId = values[0].Trim(),
             //        CharacterId = values[1].Trim(),
             //        NextDialogueId = values[3].Trim()
             //    };
             //
             //    // dialogue_text를 줄바꿈을 기준으로 나누어 DialogueTextLines에 저장
             //    string dialogueText = string.Empty;
             //    if (values.Length > 2 && !string.IsNullOrEmpty(values[2].Trim()))
             //    {
             //        dialogueText = values[2]
             //            .Replace("\r\n", "\n") // Windows 스타일 줄바꿈을 Unix 스타일로 통일
             //            .Replace("\r", "\n");   // Mac 스타일 줄바꿈 처리
             //
             //        dialogueData.DialogueTextLines = dialogueText
             //            .Split('\n')
             //            .Select(line => line.Trim())
             //            .Where(line => !string.IsNullOrEmpty(line))
             //            .ToList();
             //    }
             //    else
             //    {
             //        Debug.LogWarning($"[{sheetName}] 데이터 행 {i + 1}의 dialogue_Text가 비어 있습니다.");
             //    }
             //
             //    dialogueList.Add(dialogueData);
            
            }
        }

        // 각 시트별 데이터 확인용 로그 (선택 사항)
        if (sheetName == "Event")
        {
            foreach (var eventData in eventList)
            {
                Debug.Log(FormatEventData(eventData));
            }
        }
        else if (sheetName == "Dialogue")
        {
            foreach (var dialogueData in dialogueList)
            {
                Debug.Log(FormatDialogueData(dialogueData));
            }
        }
    }

    private string FormatEventData(CustomEventData eventData)
    {
        return $"Event ID: {eventData.EventId}\n" +
               $"Description: {eventData.Description}\n" +
               $"Repeat Type: {eventData.RepeatType}\n" +
               $"Condition Type: {eventData.ConditionType}\n" +
               $"Conditions: {string.Join(", ", eventData.Conditions)}\n" +
               $"Results: {string.Join(", ", eventData.Results)}\n" +
               $"Evidence ID: {eventData.EvidenceId}\n" +
               $"Lock Condition ID: {eventData.LockConditionId}\n" +
               $"Location ID: {eventData.LocationId}\n" +
               $"Next Event ID: {eventData.NextEventId}\n" +
               $"--------------------------";
    }

    private string FormatDialogueData(DialogueData dialogueData)
    {
        string dialogueTextFormatted = string.Join("\n", dialogueData.DialogueTextLines);
        return $"Dialogue ID: {dialogueData.DialogueId}\n" +
               $"Character ID: {dialogueData.CharacterId}\n" +
               $"Dialogue Text: {dialogueTextFormatted}\n" +
               $"Next Dialogue ID: {dialogueData.NextDialogueId}\n" +
               $"--------------------------";
    }

    /// <summary>
    /// CSV 한 줄을 파싱하여 문자열 배열로 반환합니다.
    /// </summary>
    /// <param name="line">CSV 한 줄</param>
    /// <returns>파싱된 문자열 배열</returns>
    private string[] ParseCSVLine(string line)
    {
        List<string> result = new List<string>();
        bool inQuotes = false;
        string currentValue = "";

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '"')
            {
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    // 이스케이프된 따옴표 처리
                    currentValue += '"';
                    i++; // 다음 따옴표는 무시
                }
                else
                {
                    inQuotes = !inQuotes;
                }
            }
            else if (c == ',' && !inQuotes)
            {
                result.Add(currentValue);
                currentValue = "";
            }
            else
            {
                currentValue += c;
            }
        }

        result.Add(currentValue); // 마지막 값 추가

        return result.ToArray();
    }
}
