using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMirrorAnimation : MonoBehaviour
{
    [SerializeField] private Animator mirror;
    [SerializeField] private GameObject black;
    [SerializeField] private GameObject bg;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            bg.SetActive(false);
            mirror.SetBool("IsBroke",true);
            black.SetActive(true);
        }
    }
}
