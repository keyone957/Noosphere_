using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SettingBtn : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,IPointerUpHandler
{
    [SerializeField] private Image _hoverImg;
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(gameObject.GetComponent<Button>().interactable)
            _hoverImg.color = new Color(_hoverImg.color.r, _hoverImg.color.g, _hoverImg.color.b, 1f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _hoverImg.color = new Color(_hoverImg.color.r, _hoverImg.color.g, _hoverImg.color.b, 0f);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _hoverImg.color = new Color(_hoverImg.color.r, _hoverImg.color.g, _hoverImg.color.b, 0f);
    }
}
