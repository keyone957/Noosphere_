using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class FinalStageRealStart : MonoBehaviour
{
    private void Start()
    {
        DataManager.Instance._lockConditions["Lock_condition_001"].Lock();
        EventManager.Instance.ExecuteEvent("Event_D098").Forget();
        UIManager.Instance.inventoryIcon.SetActive(false);
        InventoryManager.Instance.canOpenInventory = true;
        
    }
}
