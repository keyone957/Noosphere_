using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SaveSlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private int slotIndex;
    private SaveUI saveUI;
    void Start()
    {
        saveUI = GetComponentInParent<SaveUI>();

        if (saveUI == null)
        {
            Debug.LogError("SaveUI가 부모 오브젝트에 부착되어 있지 않습니다.");
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount == 1)
        {
            saveUI.ClickSaveSlot(slotIndex);
        }
        else if (eventData.clickCount == 2)
        {
            saveUI.DoubleClickSaveSlot(slotIndex);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        saveUI.HoverEnterSaveSlot(slotIndex);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        saveUI.UpdateAllSlotState();
    }
}
