using Unity.VisualScripting;
using UnityEngine;

public class EventStructure
{
    //csv 필드
    public string eventId;
    public string description;
    public bool repeatType; //반복 여부 true,false
    public string conditionType; //조건 타입 or,and
    public bool isAuto; //자동 실행 여부 true, false
    public string[] conditions;
    public string[] results;
    public string[] conditionFalseResults;
    public string repeatFalseResult;
    public string evidenceId;
    public string lockConditionId; //이벤트 실행 시 락되는 조건
    public string locationId;
    public string nextEventId;
    
    //이벤트 실행 여부
    public bool isExecuted = false;
    
    
    //조건 충족하는지
    public bool IsConditionMet(string conditionID)
    {
        //조건이 비어있으면 충족함으로 리턴
        if (string.IsNullOrEmpty(conditionID)) return true;
        
        bool isMet = false;
        
        //조건 id 앞에 !(not)이 붙어있는지 체크
        char boolType = conditionID[0];
        string conditionType;
        string id;
        
        if (boolType == '!')
        {
            conditionType = conditionID.Substring(1, conditionID.IndexOf('_')-1);
            id = conditionID.Substring(1, conditionID.Length-1);
        }
        else
        {
            conditionType = conditionID.Substring(0, conditionID.IndexOf('_'));
            id = conditionID;
        }
        
        if (conditionType == "Evidence")
        {
            //증거 인벤토리에 있는지 확인, 혹은 사용했는지 구분
            EvidenceStructure evidence = DataManager.Instance._evidences[id];
            char canUse = evidence.canUse;
            char acquisitionType = evidence.acquisitionType;

            if (canUse == 'Y')
            {
                Debug.Log($"증거물 사용 여부 : {PlayerInteract.Instance.isUsingEvidence}");
                //아이템을 사용했는지 체크
                if (PlayerInteract.Instance.isUsingEvidence)
                {
                    isMet = true;
                    PlayerInteract.Instance.InitUsingEvidence();
                }
            }
            else if (canUse == 'N')
            {
                //사용할 수 없는 아이템이나, 인벤토리에 획득 가능한 아이템이면 인벤토리에 있는지 체크
                if (acquisitionType == 'Y')
                {
                    if (DataManager.Instance._evidences[id].isAcquired)
                    {
                        isMet = true;
                    }
                    else
                    {
                        Debug.Log(id+"가 인벤토리 내에 존재하지 않습니다.");
                    }
                }
                else if(acquisitionType == 'N')
                {
                    //접근 횟수로 체크(나중에 필요하면 구현)
                }
            }
        }
        else if (conditionType == "Event")
        {
            if (DataManager.Instance._events.ContainsKey(id) && DataManager.Instance._events[id].isExecuted)
            {
                isMet = true;
            }
        }
        else if (conditionType == "Quiz")
        {
            if (DataManager.Instance._quiz.ContainsKey(id) && DataManager.Instance._quiz[id].isSolved)
            {
                isMet = true;
            }
        }
        
        //혹시 not 조건이 있다면 반대로 값을 출력
        if (boolType == '!') return !isMet;
        
        return isMet;
    }
}