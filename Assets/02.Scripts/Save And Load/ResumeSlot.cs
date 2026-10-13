using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ResumeSlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public bool isActive;
    public int slotIndex;
    private ResumeUI resumeUI;
    void Start()
    {
        resumeUI = GetComponentInParent<ResumeUI>();

        if (resumeUI == null)
        {
            Debug.LogError("ResumeUI가 부모 오브젝트에 부착되어 있지 않습니다.");
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        bool hasData = NooSphere.SaveManager.Instance.HasSaveData(slotIndex);
        if (!hasData) return;

        if (eventData.clickCount == 1)
        {
            resumeUI.ClickLoadSlot(slotIndex);
        }
        else if(eventData.clickCount == 2)
        {
            resumeUI.DoubleClickLoadSlot();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        bool hasData = NooSphere.SaveManager.Instance.HasSaveData(slotIndex);
        if (!hasData) return;
        resumeUI.HoverEnterLoadSlot(slotIndex);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        bool hasData = NooSphere.SaveManager.Instance.HasSaveData(slotIndex);
        if (!hasData) return;
        resumeUI.UpdateAllSlotState();
    }
}
