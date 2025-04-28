using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EscapeUI : Singleton<EscapeUI>, IPointerClickHandler
{
    [SerializeField] private GameObject _escapeBtn;
    public void OnPointerClick(PointerEventData eventData)
    {
        //클릭한 오브젝트가 챕터인지 파악
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            UIManager.Instance.CloseTopUI();
            _escapeBtn.SetActive(false);
        }
    }

    public void Active()
    {
        _escapeBtn.SetActive(true);
    }
    
    public void DisActive()
    {
        _escapeBtn.SetActive(false);
    }
}
