using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoungeKeyController : MonoBehaviour
{
    [SerializeField] private GameObject keyTriggerObject;
    [SerializeField] private GameObject keyLight;
    [SerializeField] private List<GameObject> roomTriggerObjects = new List<GameObject>();

    private void Update()
    {
        if (DataManager.Instance == null) return;
        if (!DataManager.Instance._events.ContainsKey("Event_C064")) return;
        if (DataManager.Instance._events["Event_C064"].isExecuted)
        {
            foreach (var room in roomTriggerObjects)
            {
                room.SetActive(false);
            }
        }
    }

    private void OnEnable()
    {
        if (DataManager.Instance._events["Event_C063"].isExecuted)
        {
            keyTriggerObject.SetActive(true);
            keyLight.SetActive(true);
        }
        else
        {
            keyTriggerObject.SetActive(false);
            keyLight.SetActive(false);
        }
    }
}
