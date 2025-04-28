using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisappearEffect : MonoBehaviour
{
    private bool _playerTrigger = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerTrigger = true;
        }
    }
    private void Update()
    {
        if ( _playerTrigger && Input.GetKeyDown(KeyCode.E))
        {
           Destroy(gameObject);
        }
    }
}
