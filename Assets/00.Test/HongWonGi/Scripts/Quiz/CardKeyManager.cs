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
        InitCardKey();
        transform.GetChild(0).gameObject.SetActive(true);
        EscapeUI.Instance.Active();
    }

    public override void OnClose()
    {
        base.OnClose();
        transform.GetChild(0).gameObject.SetActive(false);
        this.gameObject.SetActive(false);
    }

    void InitCardKey()
    {
        int index = 1;
        foreach (var key in _cardKeys)
        {
            if (DataManager.Instance._evidences[key].isAcquired)
            {
                _cardKeyButtons[index-1].InitCardKey();
                return;
            }
            index++;
        }
    }
}