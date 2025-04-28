using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using NooSphere;
using Debug=NooSphere.Debug;
public class CSVParser
{
    public async UniTask<Dictionary<string, T>> Parse<T>(string sheetName) where T : new()
    {
        // 딕셔너리 생성
        Dictionary<string, T> dictionary = new Dictionary<string, T>();
        string csvUrl = $"https://docs.google.com/spreadsheets/d/1rxLYxA5PoZcaMP9xGD0NrBs78GOLY9sKJv7_Ft9oPww/gviz/tq?tqx=out:csv&sheet={sheetName}";
        
        // CSV 데이터 가져오기
        string csvData = await LoadCSVFromURL(csvUrl);
        if (string.IsNullOrWhiteSpace(csvData))
        {
            Debug.Log("CSV 데이터를 로드할 수 없습니다.");
            return dictionary;
        }
        
        string[] lines = csvData.Split("\n");
        int headerLineIndex = 5;
        string[] headers = SplitCsvLine(lines[5]);
    
        // 데이터 줄 순회
        for (int i = headerLineIndex + 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrWhiteSpace(line)) continue;
    
            string[] values = SplitCsvLine(line);
            if (values.Length == 0)
            {
                Debug.LogWarning($"빈 데이터 줄: {line}");
                continue;
            }
    
            string key = values[0].Trim().Replace("\"", "");
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogWarning($"키가 없는 데이터 줄: {line}");
                continue;
            }
    
            T entry = new T();
            
            List<string> conditionsList = new List<string>(); 
            List<string> resultsList = new List<string>();
            List<string> conditionFalseResultsList = new List<string>();
            List<string> mentalFalseResults = new List<string>();
            List<string> unlockConditions = new List<string>();
            List<string> quizCorrects = new List<string>();
    
            for (int j = 0; j < headers.Length && j < values.Length; j++)
            {
                string header = headers[j].Trim().Replace("\"", "");
                if(string.IsNullOrEmpty(header))
                {
                    // 빈 헤더는 건너뜀
                    continue;
                }
                string value = values[j].Trim().Replace("\"", "");
                //Debug.Log(header + " : " + value);
                FieldInfo field = typeof(T).GetField(header, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                
                if (field != null)
                {
                    try
                    {
                        // 해당 필드 타입으로 값을 변환해서 저장하기
                        object convertedValue = Convert.ChangeType(value, field.FieldType);
                        field.SetValue(entry, convertedValue);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"필드 변환 오류 - 필드: {header}, 값: {value}, 오류: {ex.Message}");
                    }
                }
                else
                {
                    if (header.StartsWith("conditionNum")) 
                    {
                        conditionsList.Add(value);
                    }
                    else if (header.StartsWith("result")) 
                    {
                        resultsList.Add(value);
                    }
                    else if (header.StartsWith("conditionFalseResult"))
                    {
                        conditionFalseResultsList.Add(value);
                    }
                    else if (header.StartsWith("mentalFalseResult"))
                    {
                        mentalFalseResults.Add(value);
                    }
                    else if (header.StartsWith("unlockCondition"))
                    {
                        unlockConditions.Add(value);
                    }
                    else if (header.StartsWith("quizCorrect"))
                    {
                        quizCorrects.Add(value);
                    }
                }
            }
            FieldInfo conditionsField = typeof(T).GetField("conditions", BindingFlags.Public | BindingFlags.Instance);
            FieldInfo resultsField = typeof(T).GetField("results", BindingFlags.Public | BindingFlags.Instance);
            FieldInfo conditionFalseResultField = typeof(T).GetField("conditionFalseResults", BindingFlags.Public | BindingFlags.Instance);
            FieldInfo mentalFalseResultField = typeof(T).GetField("mentalFalseResults", BindingFlags.Public | BindingFlags.Instance);
            FieldInfo unlockConditionField = typeof(T).GetField("unlockConditions", BindingFlags.Public | BindingFlags.Instance);
            FieldInfo quizCorrectField = typeof(T).GetField("quizCorrects", BindingFlags.Public | BindingFlags.Instance);
            
            if (conditionsField != null && conditionsField.FieldType == typeof(string[]))
            {
                conditionsField.SetValue(entry, conditionsList.ToArray());
            }
            
            if (resultsField != null && resultsField.FieldType == typeof(string[]))
            {
                resultsField.SetValue(entry, resultsList.ToArray());
            }
            
            if (conditionFalseResultField != null && conditionFalseResultField.FieldType == typeof(string[]))
            {
                conditionFalseResultField.SetValue(entry, conditionFalseResultsList.ToArray());
            }
            
            if (mentalFalseResultField != null && mentalFalseResultField.FieldType == typeof(string[]))
            {
                mentalFalseResultField.SetValue(entry, mentalFalseResults.ToArray());
            }
            
            if (unlockConditionField != null && unlockConditionField.FieldType == typeof(string[]))
            {
                unlockConditionField.SetValue(entry, unlockConditions.ToArray());
            }
            
            if (quizCorrectField != null && quizCorrectField.FieldType == typeof(string[]))
            {
                quizCorrectField.SetValue(entry, quizCorrects.ToArray());
            }
            
            dictionary[key] = entry;
        }
    
        return dictionary;
    }
   
    private static Dictionary<string, string> _csvCache = new Dictionary<string, string>();
    private async UniTask<string> LoadCSVFromURL(string url)
    {
        // 캐시에 있는지 확인
        if (_csvCache.TryGetValue(url, out string cachedData))
        {
            return cachedData;
        }

        // 캐시에 없으면 다운로드
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

                string csvText = www.downloadHandler.text;
                // 캐시에 저장
                _csvCache[url] = csvText;
                return csvText;
            }
            catch (Exception e)
            {
                Debug.LogError($"CSV 다운로드 중 오류: {e.Message}");
                return string.Empty;
            }
        }
    }

    // CSV 라인을 쉼표로 정확히 분리하는 메서드 (따옴표 처리 포함)
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
        
        int lastNonEmpty = values.Count - 1;
        while (lastNonEmpty >= 0 && string.IsNullOrEmpty(values[lastNonEmpty]))
        {
            lastNonEmpty--;
        }
        return values.GetRange(0, lastNonEmpty + 1).ToArray();
    }
}
