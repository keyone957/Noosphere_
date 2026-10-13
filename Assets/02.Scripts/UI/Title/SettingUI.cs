using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SettingUI : DefaultUIBase
{
    [Header("세팅 버튼")]
    [SerializeField] private Button _exitBtn;
    [SerializeField] private Button _okBtn;
    [SerializeField] private Slider _soundVolumeSlider;
    [SerializeField] private Slider _bgmVolumeSlider;
    [SerializeField] private Toggle _fullScreenToggle;
    [SerializeField] private Toggle _windowToggle;
    [Header("텍스트")] 
    [SerializeField] private TextMeshProUGUI _soundText;
    [SerializeField] private TextMeshProUGUI _bgmText;

    [Header("언어")]
    [SerializeField] private LocalizationManager _localizationManager;

    private void Awake()
    {
        Init();
    }

    public override void OnOpen()
    {
        base.OnOpen();
        InitSetting();
        transform.GetChild(0).gameObject.SetActive(true);
    }

    public override void HandleKeyboardInput()
    {
        base.HandleKeyboardInput();

        if (InputRouter.Instance.ConsumeEscape())
        {
            OnClickExitBtn();
        }
    }

    public override void OnClose()
    {
        base.OnClose();
        transform.GetChild(0).gameObject.SetActive(false);
    }

    private void Init()
    {  
        _okBtn.interactable = false; 
        _soundVolumeSlider.onValueChanged.AddListener(SoundVolumeChanged);
        _bgmVolumeSlider.onValueChanged.AddListener(BgmVolumeChanged);
        _okBtn.onClick.AddListener(OnClickOkBtn);
        _exitBtn.onClick.AddListener(OnClickExitBtn);
    }
    private void InitSetting()
    {
        bool screenMode = true; // 기본 전체화면
        if (ES3.KeyExists("ScreenMode", "Setting.es3"))
            screenMode = ES3.Load<bool>("ScreenMode", "Setting.es3");
        _fullScreenToggle.isOn = screenMode;
        _windowToggle.isOn = !screenMode;
        // 잠깐 리스너 빼고 값 세팅
        _bgmVolumeSlider.onValueChanged.RemoveListener(BgmVolumeChanged);
        _soundVolumeSlider.onValueChanged.RemoveListener(SoundVolumeChanged);

        _bgmVolumeSlider.value = SoundManager.Instance.bgmVolume;
        _soundVolumeSlider.value = SoundManager.Instance.sfxVolume;
        
        int bgmText = Mathf.RoundToInt(_bgmVolumeSlider.value * 100);
        int soundText= Mathf.RoundToInt(_soundVolumeSlider.value * 100);
        _bgmText.text = $"{bgmText}%";
        _soundText.text = $"{soundText}%";

        // 다시 리스너 추가
        _bgmVolumeSlider.onValueChanged.AddListener(BgmVolumeChanged);
        _soundVolumeSlider.onValueChanged.AddListener(SoundVolumeChanged);
        _okBtn.interactable = false;

        // 언어 관련 설정
        _localizationManager.InitLocalization();
    }

    private void OnClickOkBtn()
    {
        float bgmVolume = _bgmVolumeSlider.value;
        float soundVolume = _soundVolumeSlider.value;
        ES3.Save("BgmVolume", bgmVolume, "Setting.es3");
        ES3.Save("SoundVolume", soundVolume, "Setting.es3");
        ES3.Save("ScreenMode", _fullScreenToggle.isOn, "Setting.es3");

        // 언어 관련 설정 적용
        _localizationManager.ApplyLocalization();

        DefaultUIController.Instance.CloseTopUI();
    }

    public void OnClickFullScreenBtn()
    {
        Screen.SetResolution(1920, 1080, true);
        _okBtn.interactable = true;
    }

    public void OnClickWindowBtn()
    {
        Screen.SetResolution(1280, 720, false);
        _okBtn.interactable = true;
    }

    private void OnClickExitBtn()
    {
        float bgmVolume = 0.5f;
        float soundVolume = 0.5f;
        bool screenMode = true; //true ==> 풀스크린 모드
        if (ES3.KeyExists("BgmVolume", "Setting.es3"))
            bgmVolume = ES3.Load<float>("BgmVolume", "Setting.es3");
        if (ES3.KeyExists("SoundVolume", "Setting.es3"))
            soundVolume = ES3.Load<float>("SoundVolume", "Setting.es3");
        if (ES3.KeyExists("ScreenMode", "Setting.es3"))
            screenMode = ES3.Load<bool>("ScreenMode", "Setting.es3");

        SoundManager.Instance.bgmVolume = bgmVolume;
        SoundManager.Instance._bgmSource.volume = bgmVolume;
        SoundManager.Instance.sfxVolume = soundVolume;

        _fullScreenToggle.isOn = screenMode;
        _windowToggle.isOn = !screenMode;
        
        if (screenMode)
        {
            Screen.SetResolution(1920, 1080, true);
        }
        else
        {
            Screen.SetResolution(1280, 720, false);
        }

        // 언어 관련 설정 적용
        _localizationManager.WithdrawLocalization();

        EventSystem.current.sendNavigationEvents = true;
        DefaultUIController.Instance.CloseTopUI();
    }

    private void SoundVolumeChanged(float value)
    {
        SoundManager.Instance.sfxVolume = value;
        int intValue = Mathf.RoundToInt(value * 100);
        _soundText.text = $"{intValue}%";
        _okBtn.interactable = true;
    }

    private void BgmVolumeChanged(float value)
    {
        SoundManager.Instance.bgmVolume = value;
        SoundManager.Instance._bgmSource.volume = value;
        int intValue = Mathf.RoundToInt(value * 100);
        _bgmText.text = $"{intValue}%";
        _okBtn.interactable = true;
    }

    public void OnLocalChanged()
    {
        _okBtn.interactable = true;
    }

    //슬라이더에 있는 eventtrigger에 들어갈 이벤트 함수
    public void OnSoundVolumePointerUp(BaseEventData data)
    {
        SoundManager.Instance.PlaySFXNoEffect("Soundresource_037");
    }
}