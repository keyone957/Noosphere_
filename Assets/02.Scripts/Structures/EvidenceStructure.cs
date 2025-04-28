using System;
using UnityEngine;
using Debug = NooSphere.Debug;
public class EvidenceStructure
{
    //기본 csv 파일 내 필드 값들
    public string evidenceId;
    public string evidenceName;
    public char evidenceType;
    public string shapeType; //증거물 타입 종류. OnePage : 한페이지, TwoPage : 두페이지, Object : 물체
    public char acquisitionType;
    public int inventoryIndex;
    public char canInvestigate;
    public char canUse;
    public string[] unlockConditions;
    public string evidenceTextDisplay;
    public string artresourceId;
    public string subEvidenceId;
    public string subEvidenceAcquisitionType;
    public int acquisitionPageNum;
    public string acquisitionPageResultId;
    
    //추가적으로 필요한 필드 값
    public int accessCnt = 0; //증거물 접근 횟수
    public bool isAcquired = false; //증거물 습득했는지

    
    //증거물 습득하기
    public void AcquireEvidence()
    {
        if (acquisitionType == 'Y')
        {
            isAcquired = true;
            InventoryManager.Instance.AddEvidence(this);
        }
        else if (acquisitionType == 'N')
        {
            isAcquired = true;
            accessCnt++;
        }
        
    }

    public bool CanAcquireEvidence()
    {
        //이미 습득했는지 확인
        if (isAcquired)
        {
            if (acquisitionType == 'Y')
            {
                return false;
            }
        }
   
        if (!IsUnLockConditionMet())
        {
            return false;
        }

        return true;
    }

    bool IsUnLockConditionMet()
    {
        foreach (var unlockCondition in unlockConditions)
        {
            if (!String.IsNullOrEmpty(unlockCondition))
            {
                string unlockType = unlockCondition.Substring(0, unlockCondition.IndexOf('_'));

                if (unlockType == "Event")
                {
                    if (DataManager.Instance._events.ContainsKey(unlockCondition) && DataManager.Instance._events[unlockCondition].isExecuted)
                    {
                        continue;
                    }
                }
                else if (unlockType == "Evidence")
                {
                    if (DataManager.Instance._evidences.ContainsKey(unlockCondition) &&
                        DataManager.Instance._evidences[unlockCondition].isAcquired)
                    {
                        continue;
                    }
                }
                
                Debug.Log("#" + unlockCondition + " 언락 조건을 만족하지 못하여, 획득 불가능");
                return false;
            }
        }
        
        Debug.Log($"#{evidenceId}의 언락 조건을 모두 만족 함.");
        return true;
    }
    
    //현재 상세보기하고 있는 증거물에 획득 가능한 서브 증거물이 있는지
    public bool CheckSubEvidence()
    {
        if (!String.IsNullOrEmpty(subEvidenceId)) return true;
        return false;
    }
}
