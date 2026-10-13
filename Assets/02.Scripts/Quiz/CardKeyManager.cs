using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using NooSphere;
using Debug = NooSphere.Debug;  

public class CardKeyManager : UIBase
{
    string[] _cardKeys = {"Evidence_011","Evidence_012","Evidence_013","Evidence_014"};
    [SerializeField] private List<CardKeyButton> _cardKeyButtons = new List<CardKeyButton>();
    
    void OnEnable()
    {
        UIManager.Instance.OpenUI(this);
    }

    public override void OnOpen()
    {
        base.OnOpen();
        UIManager.Instance.cctvFrame.SetActive(false);
        InitCardKey();
        transform.GetChild(0).gameObject.SetActive(true);
        EscapeUI.Instance.Active();
    }

    public override void OnClose()
    {
        base.OnClose();
        UIManager.Instance.cctvFrame.SetActive(true);
        PlayerInteract.Instance.CheckTriggerOnceAgain();
        transform.GetChild(0).gameObject.SetActive(false);
        this.gameObject.SetActive(false);
    }

    void Update()
    {
        if(IsTopUI()) EscapeUI.Instance.Active();
    }

    void InitCardKey()
    {
        bool hasAnyAcquired = false;
        foreach (string key in _cardKeys)
        {
            if (DataManager.Instance._evidences[key].isAcquired)
            {
                hasAnyAcquired = true;
                break;
            }
        }
        for (int i = 0; i < _cardKeys.Length; i++)
        {
            string key = _cardKeys[i];
            bool isAcquired = DataManager.Instance._evidences[key].isAcquired;
            _cardKeyButtons[i].InitCardKey(isAcquired, !hasAnyAcquired);
        }
    }
}