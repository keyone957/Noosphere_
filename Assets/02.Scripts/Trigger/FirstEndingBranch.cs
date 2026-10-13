using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using Debug = NooSphere.Debug;

public class FirstEndingBranch : MonoBehaviour
{
    /*
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FinalPlayer"))
        {
            // 레이한테 앨리즈의 편지를 주었다면
            if (DataManager.Instance._events["Event_C066"].isExecuted)
            {
                Debug.LogWarning("[진엔딩] 레이 등장");
                EventManagerYKM.Instance.ExecuteEvent("Event_D100").Forget();
            }
            else
            {
                Debug.LogWarning("[일반 엔딩] 닥터와의 접촉");
                EventManagerYKM.Instance.ExecuteEvent("Event_D102").Forget();
            }
        }
    }
    */
    public void CheckFirstEndingBranch()
    {
        // 레이한테 앨리즈의 편지를 주었다면
        Debug.LogWarning($"[엔딩 1차 분기점] Event_C066 : {DataManager.Instance._events["Event_C066"].isExecuted}");
        if (DataManager.Instance._events["Event_C066"].isExecuted)
        {
            Debug.LogWarning("[진엔딩] 레이 등장");
            DataManager.Instance._events["Event_D099"].nextEventId = "Event_D100";
        }
        else
        {
            Debug.LogWarning("[일반 엔딩] 닥터와의 접촉");
            DataManager.Instance._events["Event_D099"].nextEventId = "Event_D102";
            // EventManagerYKM.Instance.ExecuteEvent("Event_D102").Forget();
        }
    }
}

