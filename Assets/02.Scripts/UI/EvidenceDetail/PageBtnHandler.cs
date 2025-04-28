using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PageBtnHandler : MonoBehaviour, IPointerClickHandler
{
    private EvidenceDetailUI _navigator;
    void Start()
    {
        _navigator = GetComponentInParent<EvidenceDetailUI>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //젤 위에 있는 UI 아니면 작동 X
        if (!UIManager.Instance.IsUIOpen(UIManager.Instance.evidenceDetailUI))
        {
            return;
        }

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (gameObject.name == "LeftBtn")
            {
                _navigator.ClickPrevPageEvent();
            }
            else if (gameObject.name == "RightBtn")
            {
                _navigator.ClickNextPageEvent();
            }
        }
    }
}
