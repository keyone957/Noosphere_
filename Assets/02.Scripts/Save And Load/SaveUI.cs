using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Debug = NooSphere.Debug;

public class SaveUI : DefaultUIBase
{
    [Header("Slot 관련")]
    [Space(5)]
    [SerializeField] private List<Image> slotContents = new List<Image>();
    [SerializeField] private List<Image> slotBackgrounds = new List<Image>();
    [SerializeField] private List<TextMeshProUGUI> slotEmptyTexts = new List<TextMeshProUGUI>();

    [System.Serializable]
    public struct SlotInfoTexts
    {
        public TextMeshProUGUI locationText;
        public TextMeshProUGUI playTimeText;
        public TextMeshProUGUI dateTimeText;
    }
    [SerializeField] private List<SlotInfoTexts> slotInfoTexts = new List<SlotInfoTexts>();
    [SerializeField] private List<Sprite> slotActiveSprites = new List<Sprite>();
    [SerializeField] private List<Sprite> slotInactiveSprites = new List<Sprite>();
    [SerializeField] private List<Sprite> slotEmptySprites; // 저장된 데이터가 없을 때의 스프라이트 0: 비활성화, 1: 활성화

    [Header("Button 관련")]
    [Space(5)]
    [SerializeField] private List<Image> buttonBackgrounds = new List<Image>();
    [SerializeField] private List<Image> buttonContents = new List<Image>();
    [SerializeField] private List<Sprite> buttonContentSprites = new List<Sprite>(); // 0 비활성화, 1 활성화
    
    public override void OnOpen()
    {
        base.OnOpen();
        transform.GetChild(0).gameObject.SetActive(true);

        InitSlotState();
        InitBtnState();
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

    public void ClickSaveSlot(int slotIndex)
    {
        for (int i = 1; i <= 3; i++)
        {
            if (slotIndex == i)
            {
                NooSphere.SaveManager.Instance.SetSlotIndex(slotIndex);
                SetBackgroundOpacity(slotBackgrounds, slotIndex, 1);
                slotContents[slotIndex - 1].sprite = SelectSlotSprite(slotIndex, true);
            }
            else
            {
                if(slotIndex == 1 || slotIndex == 2 || slotIndex == 3) SetBackgroundOpacity(slotBackgrounds, i, 0);
                slotContents[i - 1].sprite = SelectSlotSprite(i, false);
            }
        }

        UpdateAllBtnState();
    }

    public void DoubleClickSaveSlot(int slotIndex)
    {
        NooSphere.SaveManager.Instance.SetSlotIndex(slotIndex);

        bool hasData = NooSphere.SaveManager.Instance.HasSaveData(slotIndex);
        if (hasData)
        {
            DefaultUIController.Instance.OpenUI(DefaultUIController.Instance.slotOverwriteUI);
        }
        else
        {
            DefaultUIController.Instance.OpenUI(DefaultUIController.Instance.slotSaveUI);
        }
    }

    // 슬롯에 호버 진입했을 때
    public void HoverEnterSaveSlot(int slotIndex)
    {
        for (int i = 1; i <= 3; i++)
        {
            if (slotIndex == i)
            {
                //if (NooSphere.SaveManager.Instance.selectSlotIndex == i) continue;
                //slotContents[slotIndex - 1].sprite = SelectSlotSprite(slotIndex, false);
                SetBackgroundOpacity(slotBackgrounds, slotIndex, 1);
            }
            else
            {
                //if (NooSphere.SaveManager.Instance.selectSlotIndex == i) continue;
                //slotContents[i - 1].sprite = SelectSlotSprite(slotIndex, false);
                SetBackgroundOpacity(slotBackgrounds, i, 0);
            }
        }
    }

    private Sprite SelectSlotSprite(int slotIndex, bool isActive)
    {
        var hasData = NooSphere.SaveManager.Instance.HasSaveData(slotIndex);

        if (!hasData)
        {
            if (isActive)
            {
                return slotEmptySprites[1];
            }
            else
            {
                return slotEmptySprites[0];
            }
        }

        var location = NooSphere.SaveManager.Instance.GetLocationData(slotIndex);
        if (isActive)
        {
            Debug.Log("활성화된 슬롯의 위치: " + location);
            // 활성화 상태
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
        else
        {
            // 비활성화 상태
            switch (location)
            {
                case "Lounge":
                    return slotInactiveSprites[0];
                case "R-101":
                    return slotInactiveSprites[1];
                case "R-102":
                    return slotInactiveSprites[2];
                case "R-103":
                    return slotInactiveSprites[4];
                case "R-104":
                    return slotInactiveSprites[3];
            }
        }
        return slotEmptySprites[0];
    }

    void SetSlotText(int slotIndex)
    {
        int listIndex = slotIndex - 1;
        bool hasData = NooSphere.SaveManager.Instance.HasSaveData(slotIndex);

        if (hasData)
        {
            string location = LocalizationSettings.StringDatabase.GetLocalizedString(LocalizationConstants.UITextTable, "UI_Local_010", LocalizationSettings.SelectedLocale);
            string playTime = LocalizationSettings.StringDatabase.GetLocalizedString(LocalizationConstants.UITextTable, "UI_Local_011", LocalizationSettings.SelectedLocale);
            string dateTime = LocalizationSettings.StringDatabase.GetLocalizedString(LocalizationConstants.UITextTable, "UI_Local_012", LocalizationSettings.SelectedLocale);
            slotInfoTexts[listIndex].locationText.text = location + " : " + NooSphere.SaveManager.Instance.GetLocationData(slotIndex);
            slotInfoTexts[listIndex].playTimeText.text = playTime + " : " + NooSphere.SaveManager.Instance.GetPlayTimeData(slotIndex);
            slotInfoTexts[listIndex].dateTimeText.text = dateTime + " : " + NooSphere.SaveManager.Instance.GetDateTimeData(slotIndex);
        }

        slotInfoTexts[listIndex].locationText.gameObject.SetActive(hasData);
        slotInfoTexts[listIndex].playTimeText.gameObject.SetActive(hasData);
        slotInfoTexts[listIndex].dateTimeText.gameObject.SetActive(hasData);
        slotEmptyTexts[listIndex].gameObject.SetActive(!hasData);

    }

    // 배경 이미지 투명도 설정 함수
    void SetBackgroundOpacity(List<Image> list, int slotIndex, float opacity)
    {
        Color _color = list[slotIndex - 1].color;

        _color.a = opacity;
        list[slotIndex - 1].color = _color;
    }

    public void UpdateSaveUI()
    {
        // 슬롯 상태 초기화
        InitSlotState();
        // 버튼 상태 초기화
        InitBtnState();
    }

    // 슬롯 상태 초기화 함수
    // auto 슬롯이 선택되어 있는 기본 상태
    void InitSlotState()
    {
        NooSphere.SaveManager.Instance.SetSlotIndex(2);

        UpdateAllSlotState();
    }

    // 이어하기 UI 업데이트 할 때 사용하는 함수
    public void UpdateAllSlotState()
    {
        int curSelectedSlotIndex = NooSphere.SaveManager.Instance.selectSlotIndex;

        // 슬롯 문구 업데이트
        SetSlotText(1);
        SetSlotText(2);
        SetSlotText(3);

        // 슬롯 배경 업데이트
        ClickSaveSlot(curSelectedSlotIndex);
    }

    // 버튼 호버 진입할 때
    public void HoverEnterBtn(int btnIndex)
    {
        if (btnIndex != 3 && !CanInteractWithBtn()) return;

        UpdateAllSlotState();

        for (int i = 1; i <= 3; i++)
        {
            if (btnIndex == i)
            {
                SetBackgroundOpacity(buttonBackgrounds, btnIndex, 1);
            }
            else
            {
                SetBackgroundOpacity(buttonBackgrounds, i, 0);
            }
        }
    }

    // 버튼 초기화 상태
    public void InitBtnState()
    {
        // 버튼 상태 업데이트
        UpdateAllBtnState();
    }

    // 슬롯 저장 버튼을 클릭할 경우
    public void ClickSlotSaveBtn()
    {
        DefaultUIController.Instance.OpenUI(DefaultUIController.Instance.slotSaveUI);
    }

    // 슬롯 삭제 버튼을 클릭할 경우
    public void ClickSlotDeleteBtn()
    {
        if (!CanInteractWithBtn()) return;

        int curSelectedSlotIndex = NooSphere.SaveManager.Instance.selectSlotIndex;
        bool hasData = NooSphere.SaveManager.Instance.HasSaveData(curSelectedSlotIndex);
        if (!hasData) return;

        Debug.LogWarning("슬롯 삭제 클릭");
        DefaultUIController.Instance.OpenUI(DefaultUIController.Instance.slotDeleteUI);
    }

    // 돌아가기 버튼 클릭할 경우 혹은 ESC 클릭할 경우
    public void ClickPrevBtn()
    {
        Debug.LogWarning("돌아가기 클릭");
        DefaultUIController.Instance.CloseTopUI();
    }

    // 슬롯에 대해 모든 버튼 상태를 업데이트하는 함수
    void UpdateAllBtnState()
    {
        int curSelectedSlotIndex = NooSphere.SaveManager.Instance.selectSlotIndex;

        for (int i = 1; i <= 3; i++)
        {
            SetBackgroundOpacity(buttonBackgrounds, i, 0);
        }

        if (CanInteractWithBtn())
        {
            int index = NooSphere.SaveManager.Instance.selectSlotIndex;
            if (NooSphere.SaveManager.Instance.HasSaveData(index))
            {
                SetBtnState(1, true);
            }
            else
            {
                SetBtnState(1, false);
            }
            SetBtnState(2, true);

        }
        else
        {
            SetBtnState(1, false);
            SetBtnState(2, false);
        }
    }

    // 버튼의 상태를 설정하는 함수
    void SetBtnState(int btnIndex, bool state)
    {
        buttonContents[btnIndex - 1].GetComponentInParent<SaveBtn>().isActive = state;
        if (state)
        {
            // 활성화 상태로 스프라이트 교체
            buttonContents[btnIndex - 1].sprite = buttonContentSprites[1];
        }
        else
        {
            buttonContents[btnIndex - 1].sprite = buttonContentSprites[0];
        }
    }

    // 현재 선택된 슬롯이 있는지 -> 즉, 버튼이 활성화 될 수 있는지를 알려 줌
    public bool CanInteractWithBtn()
    {
        int curSelectedSlotIndex = NooSphere.SaveManager.Instance.selectSlotIndex;

        if (curSelectedSlotIndex == 1 || curSelectedSlotIndex == 2 || curSelectedSlotIndex == 3)
        {
            return true;
        }
        else return false;
    }


    //void OnEnable()
    //{
    //    NooSphere.SaveManager.Instance.OnSaveStart += HandleSaveStart;
    //    NooSphere.SaveManager.Instance.OnSaveFinish += HandleSaveFinish;
    //}

    //void OnDisable()
    //{
    //    NooSphere.SaveManager.Instance.OnSaveStart -= HandleSaveStart;
    //    NooSphere.SaveManager.Instance.OnSaveFinish -= HandleSaveFinish;
    //}

    //void HandleSaveStart()
    //{
    //    _loadingUI.SetActive(true);
    //}

    //void HandleSaveFinish(int slotIndex)
    //{
    //    _loadingUI.SetActive(false);
    //    UpdateSlotUI(slotIndex);
    //}
}
