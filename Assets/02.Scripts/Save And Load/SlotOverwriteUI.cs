using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class SlotOverwriteUI : DefaultUIBase
{
    [Header("슬롯 관련")]
    [Space(5)]
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _slotTitle;
    [SerializeField] private TextMeshProUGUI _slotLocationInfo;
    [SerializeField] private TextMeshProUGUI _slotPlaytimeInfo;
    [SerializeField] private TextMeshProUGUI _slotDateInfo;
    [SerializeField] private Image _slotImage;
    [SerializeField] private List<Sprite> slotActiveSprites = new List<Sprite>();
    public override void OnOpen()
    {
        base.OnOpen();
        InitSlotInfo();
        transform.GetChild(0).gameObject.SetActive(true);
    }

    public override void OnClose()
    {
        base.OnClose();
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public override void HandleKeyboardInput()
    {
        base.HandleKeyboardInput();

        if (InputRouter.Instance.ConsumeEscape())
        {
            DefaultUIController.Instance.CloseTopUI();
        }
    }

    void InitSlotInfo()
    {
        // 슬롯 번호 가져와서 반영하기
        int slotIndex = NooSphere.SaveManager.Instance.selectSlotIndex - 1;

        string titleString = LocalizationSettings.StringDatabase.GetLocalizedString(LocalizationConstants.UITextTable, "UI_Local_039", LocalizationSettings.SelectedLocale);
        titleString = titleString.Replace("NUMBER", slotIndex.ToString());
        _titleText.text = titleString;

        string slotString = LocalizationSettings.StringDatabase.GetLocalizedString(LocalizationConstants.UITextTable, "UI_Local_063", LocalizationSettings.SelectedLocale);
        _slotTitle.text = slotString + " " + slotIndex.ToString();

        // 슬롯 정보 가져오기
        string location = LocalizationSettings.StringDatabase.GetLocalizedString(LocalizationConstants.UITextTable, "UI_Local_010", LocalizationSettings.SelectedLocale);
        string playTime = LocalizationSettings.StringDatabase.GetLocalizedString(LocalizationConstants.UITextTable, "UI_Local_011", LocalizationSettings.SelectedLocale);
        string dateTime = LocalizationSettings.StringDatabase.GetLocalizedString(LocalizationConstants.UITextTable, "UI_Local_012", LocalizationSettings.SelectedLocale);
        _slotLocationInfo.text = location + " : " + NooSphere.SaveManager.Instance.GetCurrentLocation();
        _slotPlaytimeInfo.text = playTime + " : " + NooSphere.SaveManager.Instance.GetCurrentPlayTime();
        _slotDateInfo.text = dateTime + " : " + NooSphere.SaveManager.Instance.GetCurrentDateTime();

        // 슬롯 이미지 설정
        SetSlotImage();
    }

    public void ClickYesBtn()
    {
        // UI 닫기
        ClickNoBtn();
        DefaultUIController.Instance.OpenUI(DefaultUIController.Instance.slotCompleteUI);
    }

    public void ClickNoBtn()
    {
        DefaultUIController.Instance.CloseTopUI();
    }

    private void SetSlotImage()
    {
        _slotImage.sprite = GetSlotSprite();
    }

    private Sprite GetSlotSprite()
    {
        if (FindObjectOfType<RoomInfoManager>() is RoomInfoManager roomInfoManager)
        {
            var location = roomInfoManager.roomName;

            switch (location)
            {
                case "Lounge":
                    return slotActiveSprites[0];
                case "R-101":
                    return slotActiveSprites[1];
                case "R-102":
                    return slotActiveSprites[2];
                case "R-103":
                    return slotActiveSprites[4];
                case "R-104":
                    return slotActiveSprites[3];
            }
        }

        return slotActiveSprites[0];
    }
}
