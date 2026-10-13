using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadEffect : MonoBehaviour
{
    [SerializeField] private string _eventID;
    [SerializeField] private GameObject _effectObj;

    private void Start()
    {
        if (DataManager.Instance._events[_eventID].isExecuted)
        {
            _effectObj.SetActive(true);
        }
    }
}
