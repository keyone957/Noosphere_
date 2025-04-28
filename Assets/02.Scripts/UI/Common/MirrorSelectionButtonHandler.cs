using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MirrorSelectionButtonHandler : MonoBehaviour,IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private GameObject _lastEnteredObject;
    [SerializeField] private MirrorDialogueManager _navigator;
    
    public void OnPointerClick(PointerEventData eventData)
    {
        //클릭한 오브젝트가 슬롯인지 파악
        GameObject clickedObject = eventData.pointerClick;
        Debug.Log($"{clickedObject.gameObject.name} 클릭");
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (clickedObject.gameObject.name == "YesBtn")
            {
                SoundManager.Instance.PlaySFX("Soundresource_037");
                _navigator.ClickYesBtn();
            }
            else if(clickedObject.gameObject.name == "NoBtn")
            {
                SoundManager.Instance.PlaySFX("Soundresource_037");
                _navigator.ClickNoBtn();
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GameObject enteredObject = eventData.pointerEnter;
        Debug.Log($"{enteredObject.gameObject.name} 호버");
        Debug.Log($"슬롯 호버 진입");
        _lastEnteredObject = enteredObject;
        if (enteredObject.gameObject.name == "YesBtn")
        {
            SoundManager.Instance.PlaySFX("Soundresource_035");
            _navigator.HoverYesBtn();
        }
        else if(enteredObject.gameObject.name == "NoBtn")
        {
            SoundManager.Instance.PlaySFX("Soundresource_035");
            _navigator.HoverNoBtn();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_lastEnteredObject != null)
        {
            Debug.Log("슬롯 호버 끝");
            
            _lastEnteredObject = null;
        }
    }
}
