using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class SecondEndingBranch : MonoBehaviour
{
    /*
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FinalPlayer"))
        {
            // 다프네에게 연구소 도면을 받았다면
            if (DataManager.Instance._events["Event_B065"].isExecuted && InventoryManager.Instance.IsEvidenceInInventory("Evidence_016"))
            {
                Debug.LogWarning("[일반 엔딩] 탈출 성공");
                EventManagerYKM.Instance.ExecuteEvent("Event_D103").Forget();
            }
            else
            {
                Debug.LogWarning("[일반 엔딩] 탈출 실패");
                EventManagerYKM.Instance.ExecuteEvent("Event_D104").Forget();
            }
        }
    }
    */
    public void CheckSecondEndingBranch()
    {
        // Debug.LogWarning("[엔딩 2차 분기점]");
        // // 다프네에게 연구소 도면을 받았다면
        // if (DataManager.Instance._events["Event_B065"].isExecuted && InventoryManager.Instance.IsEvidenceInInventory("Evidence_016"))
        // {
        //     Debug.LogWarning("[일반 엔딩] 탈출 성공");
        //     EffectManager.Instance.DoEffect2("Artresource_0077");
        //     // EventManagerYKM.Instance.ExecuteEvent("Event_D103").Forget();
        // }
        // else
        // {
        //     Debug.LogWarning("[일반 엔딩] 탈출 실패");
        //     // EffectManager.Instance.SetEffect("Effect_055");
        //     EffectManager.Instance.DoEffect2("Artresource_0075");
        //     // EventManagerYKM.Instance.ExecuteEvent("Event_D104").Forget();
        // }
        /*
        // 레이의 도움을 받아 탈출했다면
        if (DataManager.Instance._events["Event_D100"].isExecuted)
        {
            Debug.LogWarning("[진엔딩]주인공 방(현실) 탈출 시");
            EventManagerYKM.Instance.ExecuteEvent("Event_D101").Forget();
        }
        else
        {
            
        }
        */
    }

}