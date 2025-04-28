using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class InventoryNavigator : UIBase
{
    [SerializeField] private GameObject _inventoryWindow;
    [Header("인벤토리 네비게이션 정보")]
    public GameObject _curSelectedSlot;
    public int currentIndex = 0;
    public bool isBothInventory = false; //정신세계 증거물과 현실세계 증거물이 모두 있을 때
    
    [Space(5)][Header("인벤토리 네비게이션 UI")]
    public List<GameObject> inventorySlots = new List<GameObject>();
    private List<GameObject> realWorldSlots = new List<GameObject>();
    private List<GameObject> mentalWorldSlots = new List<GameObject>();
    [SerializeField] private Sprite _selectedSprite;
    [SerializeField] private Sprite _deselectedSprite;
    [SerializeField] private Image _slotUseBtn;
    
    [Space(5)][Header("증거물 사용 정보")]
    public bool canEvidenceUse = false;
    [SerializeField] private string _evidenceUseEventId;
    [SerializeField] private List<Sprite> _useBtnImgs = new List<Sprite>();
    
    public override void OnOpen()
    {
        base.OnOpen();
        UIManager.Instance.isInMap = false;
        _inventoryWindow.SetActive(true);
        SoundManager.Instance.PlaySFX("Soundresource_042");
        EscapeUI.Instance.Active();
        InventoryManager.Instance.currentViewChapter = (int)EventManager.Instance.curRoomInfo;
        //인벤토리 열었을 때, 현재 상태를 바탕으로 인벤토리 업데이트 진행
        SetChapterSelected(InventoryManager.Instance.currentViewChapter);
    }

    public override void OnClose()
    {
        base.OnClose();
        UIManager.Instance.isInMap = true;
        InventoryManager.Instance.currentViewChapter = (int)EventManager.Instance.curRoomInfo;
        _inventoryWindow.SetActive(false);
    }

    public override void HandleKeyboardInput()
    {
        if (_curSelectedSlot != null)
        {
            //슬롯 상하좌우 이동 - 키보드 WASD
            if (Input.GetKeyDown(KeyCode.W))
            {
                PlaySlotMoveSound();
                MoveUp();
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                PlaySlotMoveSound();
                MoveLeft();
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                PlaySlotMoveSound();
                MoveDown();
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                PlaySlotMoveSound();
                MoveRight();
            }
                
            //증거물 상세 정보 열기 - 키보드 E
            if (Input.GetKeyDown(KeyCode.E))
            {
                PlayClickSound();
                OpenEvidenceDetailUI();
            }
            
            //Space 버튼을 누르면 증거물 사용하기
            if (canEvidenceUse && Input.GetKeyDown(KeyCode.Space))
            {
                PlayClickSound();
                UseEvidence();
            }
        }
    }
    public void InitNavigator(GameObject realWorld, GameObject mentalWorld)
    {
        //초기화
        realWorldSlots.Clear();
        mentalWorldSlots.Clear();
        inventorySlots.Clear();
        
        //현재 인벤토리 기준으로 재설정
        realWorldSlots = GetChildSlots(realWorld.transform);
        mentalWorldSlots = GetChildSlots(mentalWorld.transform);
        
        
        //실제 이동에 사용할 슬롯 리스트
        if (realWorldSlots.Count > 0 && mentalWorldSlots.Count > 0)
        {
            //현실세계 증거물과 정신세계 증거물을 교차로 넣기
            //Debug.Log("현실 증거물 정신 증거물 둘 다 존재?");
            AddSlotsPerRow();
            isBothInventory = true;
        }
        else if(realWorldSlots.Count > 0)
        {
            //Debug.Log($"현실 증거물 {realWorldSlots.Count}개 만 존재");
            inventorySlots = realWorldSlots;
        }
        else if (mentalWorldSlots.Count > 0)
        {
            //Debug.Log($"정신 증거물 {mentalWorldSlots.Count}개 만 존재");
            inventorySlots = mentalWorldSlots;
        }

        if (inventorySlots.Count == 0)
        {
            //Debug.Log("증거물이 아무것도 존재하지 않아");
            _curSelectedSlot = null;
            return;
        }

        //슬롯 선택 초기화
        currentIndex = 0;
        UpdateSelection();
        InitChapter(InventoryManager.Instance.currentViewChapter);
    }

    //증거물 행 별로 슬롯에 추가 함수
    void AddSlotsPerRow()
    {
        int maxCount = (int)Mathf.Max(realWorldSlots.Count,mentalWorldSlots.Count);
        int maxRow = Mathf.CeilToInt((float)maxCount / 3);

        for (int row = 0; row < maxRow; row++)
        {
            int startIndex = row * 3;
            int endIndexR = Mathf.Min(startIndex + 3, realWorldSlots.Count);
            int endIndexM = Mathf.Min(startIndex + 3, mentalWorldSlots.Count);
            for (int index = startIndex; index < endIndexR; index++)
            {
                inventorySlots.Add(realWorldSlots[index]);
            }
            for (int index = startIndex; index < endIndexM; index++)
            {
                inventorySlots.Add(mentalWorldSlots[index]);
            }
        }
    }

    //슬롯들 가져오기
    List<GameObject> GetChildSlots(Transform parent)
    {
        List<GameObject> childSlots = new List<GameObject>();

        if (parent.childCount > 0)
        {
            Debug.Log($"{parent.name}의 자식 개수는 {parent.childCount}개 입니다.");
            foreach (Transform child in parent)
            {
                Debug.Log($"{parent.name}의 자식 {child.name} 을 ChildSlot에 추가");
                childSlots.Add(child.gameObject);
            }
        }
        return childSlots;
    }
    
    //현재 슬롯에서 위로 이동 - 정신(현실) 세계 증거물만 있는 경우
    public void MoveUp()
    {
        if (isBothInventory)
        {
            //현실 세계에 있다면
            if (IsInRealWorldSlot())
            {
                MoveUpInBothInventory(realWorldSlots);
            }
            else
            {
                MoveUpInBothInventory(mentalWorldSlots);
            }
            
        }
        else
        {
            currentIndex -= 3;
            if(currentIndex < 0) currentIndex = Mathf.FloorToInt((inventorySlots.Count - 1) / 3) * 3;
        }
        UpdateSelection();
    }

    //현재 슬롯에서 위로 이동 - 정신과 현실 세계 증거물이 모두 있는 경우
    private void MoveUpInBothInventory(List<GameObject> slots)
    {
        int index = slots.IndexOf(_curSelectedSlot);
        int targetIndex = index - 3;

        if (targetIndex < 0)
        {
            targetIndex = Mathf.FloorToInt((slots.Count - 1) / 3) * 3;
        }
        
        GameObject target = slots[targetIndex];
        currentIndex = inventorySlots.IndexOf(target);
    }

    //현재 슬롯에서 아래로 이동 - 정신과 현실 세계 증거물이 모두 있는 경우
    private void MoveDownInBothInventory(List<GameObject> slots)
    {
        int index = slots.IndexOf(_curSelectedSlot);
        int targetIndex = index + 3;
        
        if (targetIndex >= slots.Count)
        {
            int currentRow = targetIndex / 3;
            int endRow = Mathf.FloorToInt((slots.Count - 1) / 3);
            if (currentRow == endRow) targetIndex = endRow * 3;
            else if (currentRow > endRow) targetIndex = 0;
        }
        GameObject target = slots[targetIndex];
        currentIndex = inventorySlots.IndexOf(target);
    }
    
    //현재 슬롯에서 아래로 이동 - 정신(현실) 세계 증거물만 있는 경우
    public void MoveDown()
    {
        if (isBothInventory)
        {
            if (IsInRealWorldSlot())
            {
                MoveDownInBothInventory(realWorldSlots);
            }
            else
            {
                MoveDownInBothInventory(mentalWorldSlots);
            }
        }
        else
        {
            currentIndex += 3;
            if (currentIndex >= inventorySlots.Count)
            {
                int currentRow = currentIndex / 3;
                int endRow = Mathf.FloorToInt((inventorySlots.Count - 1) / 3);
                if (currentRow == endRow) currentIndex = endRow * 3;
                else if (currentRow > endRow) currentIndex = 0;
            }
        }
        UpdateSelection();
    }

    //현재 슬롯에서 왼쪽으로 이동
    public void MoveLeft()
    {
        currentIndex -= 1;
        if (currentIndex < 0) currentIndex = inventorySlots.Count - 1;
        UpdateSelection();
    }

    //현재 슬롯에서 오른쪽으로 이동
    public void MoveRight()
    {
        currentIndex += 1;
        if (currentIndex >= inventorySlots.Count) currentIndex = 0;
        UpdateSelection();
    }

    //슬롯 선택 시, 슬롯 선택에 따른 업데이트
    public void UpdateSelection()
    {
        _curSelectedSlot = inventorySlots[currentIndex];
        SetSlotSelected(_curSelectedSlot);
        SetSlotUseBtn(_curSelectedSlot);
    }

    //슬롯 배경 업데이트
    public void SetSlotSelected(GameObject slot)
    {
        Image slotImg = slot.GetComponent<Image>();
        slotImg.sprite = _selectedSprite;
        foreach (var _slot in inventorySlots)
        {
            if (_slot != slot) _slot.GetComponent<Image>().sprite = _deselectedSprite;
        }
    }

    public void SetSlotDeselected(GameObject slot)
    {
        Image slotImg = slot.GetComponent<Image>();
        slotImg.sprite = _deselectedSprite;
    }

    //챕터 선택
    public void SetChapterSelected(int index)
    {
        InventoryManager.Instance.currentViewChapter = index;
        
        InitChapter(index);
        
        //인벤토리 업데이트 하기
        InventoryManager.Instance.UpdateInventoryUI();
    }
    
    public void InitChapter(int index)
    {
        InventoryManager.Instance.currentViewChapter = index;
        
        GameObject selectedChapterUI = InventoryManager.Instance.selectedChapterUIList[index];
        GameObject deselectedChapterUI = InventoryManager.Instance.deselectedChapterUIList[index];
        
        if (!selectedChapterUI.activeSelf)
        {
            //선택 버전이 활성화되고
            selectedChapterUI.SetActive(true);
            //비선택 버전이 비활성화 되기
            deselectedChapterUI.SetActive(false);

            //선택 버전의 나머지 애들 비활성화
            foreach (var chapter in InventoryManager.Instance.selectedChapterUIList)
            {
                if (chapter != selectedChapterUI)
                {
                    chapter.SetActive(false);
                }
            }
            
            //비선택 버전의 나머지 애들 활성화
            foreach (var chapter in InventoryManager.Instance.deselectedChapterUIList)
            {
                if (chapter != deselectedChapterUI)
                {
                    chapter.SetActive(true);
                }
            }
        }
    }
    
    //챕터 호버 enter
    public void HoverEnterOnChapter(int index)
    {
        //호버를 한 챕터의 선택 버전과 비선택 버전을 할당하기
        GameObject selectedChapterUI = InventoryManager.Instance.selectedChapterUIList[index];
        GameObject deselectedChapterUI = InventoryManager.Instance.deselectedChapterUIList[index];
        
        //호버한 챕터가 비활성화 되어 있다면
        if (!selectedChapterUI.activeSelf)
        {
            //선택 버전이 활성화되고
            selectedChapterUI.SetActive(true);
            //비선택 버전이 비활성화 되기
            deselectedChapterUI.SetActive(false);

            //선택 버전의 나머지 애들 비활성화
            foreach (var chapter in InventoryManager.Instance.selectedChapterUIList)
            {
                if (chapter != selectedChapterUI)
                {
                    chapter.SetActive(false);
                }
            }
            
            //비선택 버전의 나머지 애들 활성화
            foreach (var chapter in InventoryManager.Instance.deselectedChapterUIList)
            {
                if (chapter != deselectedChapterUI)
                {
                    chapter.SetActive(true);
                }
            }
        }
    }
    //현실 세계 항목에 있는 슷롯인지 확인
    private bool IsInRealWorldSlot()
    {
        if (realWorldSlots.Contains(_curSelectedSlot))
        {
            return true;
        }

        return false;
    }

    //슬롯 버튼 업데이트
    void SetSlotUseBtn(GameObject slot)
    {
        string evidenceId = slot.GetComponent<InventorySlotInfo>().evidenceId;
        char canUse = DataManager.Instance._evidences[evidenceId].canUse;
        
        if (canUse == 'Y')
        {
            //플레이어가 현재 이벤트 트리거 내에 있다면 이벤트 가져오기
            if (PlayerInteract.Instance.isInsideTrigger && PlayerInteract.Instance.curTrigger != null)
            {
                //상호작용 할 수 있는 이벤트가 있는지 확인
                foreach (var eventID in PlayerInteract.Instance.curTrigger.eventIdList)
                {
                    if (!string.IsNullOrEmpty(eventID) && DataManager.Instance._events.ContainsKey(eventID))
                    {
                        //상호작용 가능하다면
                        if (PlayerInteract.Instance.OnInteract != null)
                        {
                            EventStructure _event = DataManager.Instance._events[eventID];
                            //해당 이벤트의 조건에 증거물이 있는지 체크
                            foreach (var condition in _event.conditions)
                            {
                                if (evidenceId == condition)
                                {
                                    _slotUseBtn.sprite = _useBtnImgs[1];
                                    _evidenceUseEventId = eventID;
                                    canEvidenceUse = true;
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }
        _slotUseBtn.sprite = _useBtnImgs[0];
        canEvidenceUse = false;
    }
    
    //증거물 상세 내용 UI 열기
    public void OpenEvidenceDetailUI()
    {
        Debug.Log("인벤토리에서 상세 내용 오픈");
        string id = _curSelectedSlot.GetComponent<InventorySlotInfo>().evidenceId;
        EvidenceStructure evidence = DataManager.Instance._evidences[id];
        if(evidence != null) UIManager.Instance.OpenUI(UIManager.Instance.evidenceDetailUI,evidence);
    }

    //증거물 사용하기
    public void UseEvidence()
    {
        PlayerInteract.Instance.isUsingEvidence = true;
        UIManager.Instance.CloseAllUI();
        EventManager.Instance.ExecuteEvent(_evidenceUseEventId).Forget();
    }

    public void InitUsingEvidence()
    {
        canEvidenceUse = false;
        _evidenceUseEventId = "";
    }

    public void PlaySlotMoveSound()
    {
        int random = Random.Range(0, 5);
        string id = "";
        switch (random)
        {
            case 0 :
                id = "Soundresource_043";
                break;
            case 1:
                id = "Soundresource_044";
                break;
            case 2:
                id = "Soundresource_045";
                break;
            case 3:
                id = "Soundresource_046";
                break;
            case 4:
                id = "Soundresource_047";
                break;
        }
        SoundManager.Instance.PlaySFX(id);
    }

    public void PlayClickSound()
    {
        SoundManager.Instance.PlaySFX("Soundresource_070");
    }
}
