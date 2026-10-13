using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlotDeleteUI : DefaultUIBase
{
    public override void OnOpen()
    {
        base.OnOpen();
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
    }
    
    public void ClickYesBtn()
    {
        int index = NooSphere.SaveManager.Instance.selectSlotIndex;
        // 슬롯 데이터 삭제
        NooSphere.SaveManager.Instance.DeleteSlotData(index);
        // UI 닫기
        ClickNoBtn();
        // 슬롯 UI 업데이트하기
        if(DefaultUIController.Instance.IsUIOpen(DefaultUIController.Instance.saveUI))
            FindAnyObjectByType<SaveUI>().UpdateSaveUI();
        else if(DefaultUIController.Instance.IsUIOpen(DefaultUIController.Instance.resumeUI))
            FindAnyObjectByType<ResumeUI>().UpdateResumeUI();
    }

    public void ClickNoBtn()
    {
        DefaultUIController.Instance.CloseTopUI();
    }
}
