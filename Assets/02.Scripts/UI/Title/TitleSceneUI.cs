using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.XR;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleSceneUI : DefaultUIBase
{
    [SerializeField] private Button _loadGameBtn;
    [SerializeField] private Button _newGameBtn;
    [SerializeField] private Button _creditBtn;
    [SerializeField] private Button _settingBtn;
    [SerializeField] private Button _exitBtn;
    
    public override void OnOpen()
    {
        base.OnOpen();
        transform.GetChild(0).gameObject.SetActive(true);
    }

    public override void OnClose()
    {
        base.OnClose();
        //transform.GetChild(0).gameObject.SetActive(false);
    }

    private void Start()
    {
        _loadGameBtn.onClick.AddListener(OnClickLoadBtn);
        _newGameBtn.onClick.AddListener(OnClickNewGameBtn);
        _creditBtn.onClick.AddListener(OnClickCreditBtn);
        _settingBtn.onClick.AddListener(OnClickSettingBtn);
        _exitBtn.onClick.AddListener(OnClickExitBtn);
    }

    private void OnClickLoadBtn()
    {
        DefaultUIController.Instance.OpenUI(DefaultUIController.Instance.resumeUI);
    }

    // TODO : 세이브데이터가 있는 경우와 없는 경우를 구분해서 기획에 따라 다르게 구현해야 함.
    private void OnClickNewGameBtn()
    {
        SoundManager.Instance.PlaySFX("Soundresource_037");

        if (NooSphere.SaveManager.Instance.IsAnySaveDataExists())
        {
            DefaultUIController.Instance.OpenUI(DefaultUIController.Instance.resetUI);
            return;
        }

        DisableBtnEvent();
        NooSphere.SaveManager.Instance.SetLoadType(NooSphere.GameLoadType.NewGame);
        SceneChanger.Instance.ChangeScene("LoadingScene").Forget();
    }

    private void DisableBtnEvent()
    {
        _loadGameBtn.onClick.RemoveAllListeners();
        _newGameBtn.onClick.RemoveAllListeners();
        _creditBtn.onClick.RemoveAllListeners();
        _settingBtn.onClick.RemoveAllListeners();
        _exitBtn.onClick.RemoveAllListeners();
    }

    private void OnClickCreditBtn()
    {
        DefaultUIController.Instance.OpenUI(DefaultUIController.Instance.creditUI);
    }

    private void OnClickSettingBtn()
    {
        DefaultUIController.Instance.OpenUI(DefaultUIController.Instance.settingUI);
    }

    private void OnClickExitBtn()
    {
        DefaultUIController.Instance.OpenUI(DefaultUIController.Instance.exitGameUI);
    }

    public void OnClickSound()
    {
        SoundManager.Instance.PlaySFX("Soundresource_037");
    }

    public void OnHoverSound()
    {
        SoundManager.Instance.PlaySFX("Soundresource_035");
    }
}
