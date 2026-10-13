using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SaveBtn : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool isActive;
    public int btnIndex;
    private SaveUI saveUI;
    void Start()
    {
        saveUI = GetComponentInParent<SaveUI>();

        if (saveUI == null)
        {
            Debug.LogError("SaveUI가 부모 오브젝트에 부착되어 있지 않습니다.");
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isActive && btnIndex != 3) return;
        saveUI.HoverEnterBtn(btnIndex);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isActive && btnIndex != 3) return;
        saveUI.InitBtnState();
    }
}
