using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingDoorTrigger : MonoBehaviour
{
    [SerializeField] private bool isOpened = false;
    [SerializeField] private Animator _doorAnimator;
   

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (isOpened)
            {
                //문이 열려있으면 닫음
                _doorAnimator.SetTrigger("Close");
                isOpened = false;
            }
            else
            {
                //문이 닫혀있으면 열음
                _doorAnimator.SetTrigger("Open");
                isOpened = true;
            }
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (isOpened)
            {
                //문이 열려있으면 닫음
                _doorAnimator.SetTrigger("Close");
                isOpened = false;
            }
            else
            {
                //문이 닫혀있으면 열음
                _doorAnimator.SetTrigger("Open");
                isOpened = true;
            }
        }
    }
}
