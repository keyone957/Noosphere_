using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComputerController : UIBase
{
    [SerializeField] private ButtonEventChannel _eventChannel;
    [SerializeField] private Button _exitBtn;

    private void OnEnable()
    {
        UIManager.Instance.OpenUI(this);   
    }

    public override void OnOpen()
    {
        base.OnOpen();
        transform.GetChild(0).gameObject.SetActive(true);
        EscapeUI.Instance.Active();
        _eventChannel.RaiseEvent("0");//컴퓨터 열었을때 맨처음 버튼 눌리는 이벤트 전달
        SoundManager.Instance.PlaySFX("Soundresource_091");
        _exitBtn.onClick.AddListener(()=>
        {
            SoundManager.Instance.PlaySFX("Soundresource_092");
            UIManager.Instance.CloseTopUI();
        });
    }
    
    public override void OnClose()
    {
        base.OnClose();
        _exitBtn.onClick.RemoveAllListeners();
        SoundManager.Instance.PlaySFX("Soundresource_092");
        transform.GetChild(0).gameObject.SetActive(false);
        gameObject.SetActive(false);
        EffectManager.Instance.OnEffectEnd.Invoke();
    }
}
