using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadGameEnableObejct : MonoBehaviour
{
    [SerializeField] private string _eventID;
    [SerializeField] private string _eventID2;
    [SerializeField] private GameObject _effectObj;

    private void Start()
    {
        if (DataManager.Instance._events[_eventID].isExecuted||DataManager.Instance._events[_eventID2].isExecuted)
        {
            _effectObj.SetActive(true);
        }
    }
}
