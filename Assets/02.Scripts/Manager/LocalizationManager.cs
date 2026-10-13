using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using Debug = NooSphere.Debug;

public class LocalizationManager : MonoBehaviour
{
    [SerializeField] private int _localIndex;
    [SerializeField] private List<string> _localList = new List<string>();

    [Space(5)]
    [Header("UI 관련 변수")]
    [SerializeField] private TextMeshProUGUI _localText;
    [SerializeField] private SettingUI _settingUI;

    public void ClickPrevBtn()
    {
        _localIndex--;
        if (_localIndex < 0) _localIndex = _localList.Count - 1;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_localIndex];

        UpdateText();
        _settingUI.OnLocalChanged();
    }

    public void ClickNextBtn()
    {
        _localIndex++;
        if (_localIndex > _localList.Count - 1) _localIndex = 0;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_localIndex];

        UpdateText();
        _settingUI.OnLocalChanged();
    }

    // 언어 관련 변경사항 적용
    public void ApplyLocalization()
    {
        // 언어 정보 저장
        ES3.Save("Language", _localIndex, "Setting.es3");
    }

    // 언어 관련 변경사항 철회
    public void WithdrawLocalization()
    {
        InitLocalization();
    }

    private void UpdateText()
    {
        _localText.text = _localList[_localIndex];
    }

    public void InitLocalization()
    {
        if (ES3.KeyExists("Language", "Setting.es3"))
        {
            _localIndex = ES3.Load<int>("Language", "Setting.es3");
        }
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_localIndex];
        UpdateText();
    }
}
