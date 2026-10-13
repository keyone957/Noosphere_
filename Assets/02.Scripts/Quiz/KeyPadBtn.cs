using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NooSphere;
using Debug = NooSphere.Debug;
using Random = UnityEngine.Random;

public class KeyPadBtn : MonoBehaviour
{
    [SerializeField] private string _keyPadNum;
    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClickKeyPadNum);
    }

    private void OnClickKeyPadNum()
    {
        KeyPadManager.Instance.SetDialText(_keyPadNum);
    }
    
}
