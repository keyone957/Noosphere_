using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnEnabledMirror : MonoBehaviour
{
    [SerializeField] private GameObject _mirror;

    void Start()
    {
        if (DataManager.Instance._events["Event_B099"].isExecuted&&
            DataManager.Instance._events["Event_B067"].isExecuted)
        {
            _mirror.SetActive(false);
        }
        else if (DataManager.Instance._events["Event_B099"].isExecuted )
        {
            _mirror.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}