using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChapterClickHandler : MonoBehaviour, IPointerClickHandler/*, IPointerEnterHandler, IPointerExitHandler*/
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
        
        //클릭한 오브젝트가 챕터인지 파악
        GameObject clickedObject = eventData.pointerClick;
        
        Debug.Log($"챕터 클릭 {clickedObject.name}");
        int chapterIndex;

        switch (clickedObject.name)
        {
            case "Chapter0":
                chapterIndex = 0;
                break;
            case "Chapter1":
                chapterIndex = 1;
                break;
            case "Chapter2":
                chapterIndex = 2;
                break;
            case "Chapter3":
                chapterIndex = 3;
                break;
            default:
                chapterIndex = 0;
                break;
        }
        
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            _navigator.PlayClickSound();
            _navigator.SetChapterSelected(chapterIndex);
        }
    }

    /*
    public void OnPointerEnter(PointerEventData eventData)
    {
        //젤 위에 있는 UI 아니면 작동 X
        if (!UIManager.Instance.IsUIOpen(UIManager.Instance.inventoryUI))
        {
            return;
        }
        
        //클릭한 오브젝트가 챕터인지 파악
        GameObject enteredObject = eventData.pointerEnter;
        if (InventoryManager.Instance.deselectedChapterUIList.Contains(enteredObject))
        {
            _lastEnteredObject = enteredObject;
            
            int chapterIndex = InventoryManager.Instance.deselectedChapterUIList.IndexOf(enteredObject);
            _navigator.HoverEnterOnChapter(chapterIndex);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //젤 위에 있는 UI 아니면 작동 X
        if (!UIManager.Instance.IsUIOpen(UIManager.Instance.inventoryUI))
        {
            return;
        }
        
        //클릭한 오브젝트가 챕터인지 파악
        int curChapterIndex = InventoryManager.Instance.currentViewChapter;
        
        GameObject selectedChapter = InventoryManager.Instance.selectedChapterUIList[curChapterIndex];
        GameObject deselectedChapter = InventoryManager.Instance.deselectedChapterUIList[curChapterIndex];
        selectedChapter.SetActive(true);
        deselectedChapter.SetActive(false);

        foreach (var _chapter in InventoryManager.Instance.selectedChapterUIList)
        {
            if(_chapter != selectedChapter) _chapter.SetActive(false);
        }
        foreach (var _chapter in InventoryManager.Instance.deselectedChapterUIList)
        {
            if(_chapter != deselectedChapter) _chapter.SetActive(true);
        }

        _lastEnteredObject = null;
    }
    */
}
