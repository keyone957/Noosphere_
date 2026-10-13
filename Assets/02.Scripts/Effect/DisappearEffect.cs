using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisappearEffect : MonoBehaviour
{
    [SerializeField]private bool _playerTrigger ;
    [SerializeField] private string _evidenceID;

    private void Start()
    {
        if (DataManager.Instance._evidences[_evidenceID].isAcquired)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerTrigger = false;
        }
    }

    private void Update()
    {
        if ( _playerTrigger&&Input.GetKeyDown(KeyCode.E))
        {
           gameObject.SetActive(false);
        }
    }
}
