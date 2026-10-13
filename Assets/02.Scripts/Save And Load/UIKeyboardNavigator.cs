using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIKeyboardNavigator : MonoBehaviour
{
    public bool isActive;
    public int slotIndex;
    public int btnIndex;

    public bool IsActive() => isActive;

    private SaveUI saveUI;
    void Start()
    {
        if (saveUI == null)
        {
            Debug.LogError("SaveUI가 부모 오브젝트에 부착되어 있지 않습니다.");
        }
    }

    public void OnHoverEnter()
    {
        if (!isActive) return;

        saveUI = GetComponentInParent<SaveUI>();

        if (slotIndex > 0)
        {
            saveUI.HoverEnterSaveSlot(slotIndex);
            saveUI.InitBtnState();
        }
        else
        {
            saveUI.HoverEnterBtn(btnIndex);
        }
    }

    public void OnClick()
    {
        if (!isActive) return;

        saveUI = GetComponentInParent<SaveUI>();

        if (slotIndex > 0)
        {
            saveUI.ClickSaveSlot(slotIndex);
        }
        else
        {
            var trigger = GetComponent<UnityEngine.EventSystems.EventTrigger>();
            if (trigger != null)
            {
                foreach (var entry in trigger.triggers)
                {
                    if (entry.eventID == EventTriggerType.PointerClick)
                        entry.callback.Invoke(new PointerEventData(EventSystem.current));
                }
            }
        }
    }

    public void OnDoubleClick()
    {
        if (!isActive) return;

        saveUI = GetComponentInParent<SaveUI>();

        if (slotIndex > 0)
        {
            saveUI.DoubleClickSaveSlot(slotIndex);
        }
    }
}
