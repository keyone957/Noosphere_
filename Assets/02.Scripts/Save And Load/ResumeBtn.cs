using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ResumeBtn : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool isActive;
    public int btnIndex;
    private ResumeUI resumeUI;
    void Start()
    {
        resumeUI = GetComponentInParent<ResumeUI>();

        if (resumeUI == null)
        {
            Debug.LogError("ResumeUI가 부모 오브젝트에 부착되어 있지 않습니다.");
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        resumeUI.HoverEnterBtn(btnIndex);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        resumeUI.InitBtnState();
    }
}
