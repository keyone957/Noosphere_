using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Debug = NooSphere.Debug;
public class SlotClickHandler : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private GameObject _lastEnteredObject;
    private InventoryNavigator _navigator;

    void Start()
    {
        _navigator = InventoryManager.Instance.transform.GetComponent<InventoryNavigator>();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        //젤 위에 있는 UI 아니면 작동 X
        if (!UIManager.Instance.IsUIOpen(UIManager.Instance.inventoryUI))
        {
            return;
        }
        
        //클릭한 오브젝트가 슬롯인지 파악
        GameObject clickedObject = eventData.pointerClick;
        Debug.Log($"{clickedObject.gameObject.name} 클릭");
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            //인덱스 값 가져오기
            _navigator.currentIndex = _navigator.inventorySlots.IndexOf(clickedObject);
            if (_navigator._curSelectedSlot != clickedObject)
            {
                //현재 선택된 오브젝트와 클릭한 오브젝트가 다를 경우 -> 신규 선택
                Debug.Log($"슬롯{_navigator.currentIndex} 클릭");
                _navigator.PlaySlotMoveSound();
                _navigator.UpdateSelection();
            }
            else
            {
                //현재 선택된 오브젝트와 클릭한 오브젝트가 같을 경우 -> 상세 보기 기능
                Debug.Log($"슬롯{_navigator.currentIndex} 선택");
                _navigator.OpenEvidenceDetailUI();
                _navigator.PlayClickSound();
            }
        }

        // 우클릭 감지
        if (_navigator.canEvidenceUse && eventData.button == PointerEventData.InputButton.Right)
        {
            //현재 선택된 오브젝트가 우클릭한 오브젝트와 같아야 함. -> 사용하기 기능
            if (_navigator._curSelectedSlot == clickedObject)
            {
                Debug.Log("아이템 사용");
                _navigator.UseEvidence();
                _navigator.PlayClickSound();
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //젤 위에 있는 UI 아니면 작동 X
        if (!UIManager.Instance.IsUIOpen(UIManager.Instance.inventoryUI))
        {
            return;
        }
        
        GameObject enteredObject = eventData.pointerEnter;
        Debug.Log($"{enteredObject.gameObject.name} 호버");
        Debug.Log($"슬롯 호버 진입");
        _lastEnteredObject = enteredObject;
        _navigator.SetSlotSelected(enteredObject);
        _navigator.PlaySlotMoveSound();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //젤 위에 있는 UI 아니면 작동 X
        if (!UIManager.Instance.IsUIOpen(UIManager.Instance.inventoryUI))
        {
            return;
        }
        
        if (_lastEnteredObject != null)
        {
            Debug.Log("슬롯 호버 끝");
            _navigator.UpdateSelection();
            _lastEnteredObject = null;
        }
    }
}
