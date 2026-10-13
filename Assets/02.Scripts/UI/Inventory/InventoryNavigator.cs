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
using UnityEngine.Localization.Settings;
using Debug = NooSphere.Debug;

public class LocalConstants
{
    public static readonly string EvidenceNameTable = "EvidenceName";
    public static readonly string EvidenceContentTable = "EvidenceDescription";
    public static readonly string DialogueTable = "Dialogue";
    public static readonly string CharacterTable = "Character";
    public static readonly string UITextTable = "UIText";
    public static readonly string ImageAssestTable = "ImageTables";
}

public class InventoryNavigator : UIBase
{
    [SerializeField] private GameObject _inventoryWindow;
    [Header("인벤토리 네비게이션 정보")]
    public GameObject _curSelectedSlot;
    public int currentIndex = 0;
    
    [Space(5)][Header("인벤토리 네비게이션 UI")]
    public List<GameObject> inventorySlots = new List<GameObject>();
    private GameObject _slotOn;

    [Space(5)] [Header("인벤토리 디테일 UI")] [SerializeField]
    private TextMeshProUGUI _evidenceName;
    [SerializeField] private Image _evidenceBackground;
    [SerializeField] private List<Sprite> _backgroundImgs;
    [SerializeField] private Image _evidenceImg;
    [SerializeField] private TextMeshProUGUI _evidenceContent;
    [SerializeField] private Sprite _evidenceDefaultImg;
    
    [Space(5)][Header("증거물 사용 정보")]
    [SerializeField] private EventStructure _evidenceUseEvent;
    [SerializeField] private GameObject _useBtn;
    [SerializeField] private GameObject _openBtn;

    [Space(5)] [Header("카드 키 사용 정보")] public bool isUsingCardKey = false;
    public List<EventTrigger.KeyInfo> keyInfos = new List<EventTrigger.KeyInfo>();
    [SerializeField] int previousChapter = -1;

    public override void OnOpen()
    {
        base.OnOpen();
        UIManager.Instance.isInMap = false;
        
        // 인벤토리인지 증거물 사용인지에 따라 버튼 활성화
        if (InventoryManager.Instance.isUsingEvidence)
        {
            _openBtn.SetActive(false);
            _useBtn.SetActive(true);
        }
        else
        {
            _openBtn.SetActive(true);
            _useBtn.SetActive(false);
        }

        // 카드 키 사용일 경우, R101 챕터 열기
        if (isUsingCardKey)
        {
            previousChapter = InventoryManager.Instance.currentViewChapter;
            InventoryManager.Instance.currentViewChapter = 0; // R101 챕터
        }
        
        //InventoryManager.Instance.currentViewChapter = (int)EventManagerYKM.Instance.curRoomInfo;
        //인벤토리 열었을 때, 현재 상태를 바탕으로 인벤토리 업데이트 진행
        SetChapterSelected(InventoryManager.Instance.currentViewChapter);

        _inventoryWindow.SetActive(true);
        SoundManager.Instance.PlaySFX("Soundresource_042");
        EscapeUI.Instance.Active();
    }

    public override void OnClose()
    {
        base.OnClose();
        UIManager.Instance.isInMap = true;
        InventoryManager.Instance.isUsingEvidence = false;
        InventoryManager.Instance.currentViewChapter = (int)EventManager.Instance.curRoomInfo;
        _inventoryWindow.SetActive(false);
    }

    public override void HandleKeyboardInput()
    {
        if (_curSelectedSlot != null)
        {
            //슬롯 상하좌우 이동 - 키보드 WASD
            if (InputRouter.Instance.ConsumeW())
            {
                PlaySlotMoveSound();
                MoveUp();
            }
            if (InputRouter.Instance.ConsumeA())
            {
                PlaySlotMoveSound();
                MoveLeft();
            }
            if (InputRouter.Instance.ConsumeS())
            {
                PlaySlotMoveSound();
                MoveDown();
            }
            if (InputRouter.Instance.ConsumeD())
            {
                PlaySlotMoveSound();
                MoveRight();
            }
                
            //증거물 상세 정보 열기 - 키보드 E
            if (!InventoryManager.Instance.isUsingEvidence && InputRouter.Instance.ConsumeE())
            {
                PlayClickSound();
                OpenEvidenceDetailUI();
            }
            
            
            //Space 버튼을 누르면 증거물 사용하기
            if (InventoryManager.Instance.isUsingEvidence && InputRouter.Instance.ConsumeSpace())
            {
                PlayClickSound();
                UseEvidence();
            }
        }
        // Tab 키로 인벤토리 챕터 변경
        if (UIManager.Instance.IsUIOpen(UIManager.Instance.inventoryUI) && InputRouter.Instance.ConsumeTab())
        {
            PlayClickSound();
            int chapterIndex = InventoryManager.Instance.currentViewChapter + 1;
            if (chapterIndex > 3)
            {
                chapterIndex = 0;
            }
            SetChapterSelected(chapterIndex);
        }
    }
    public void InitNavigator(GameObject inventoryBase)
    {
        //초기화
        inventorySlots.Clear();
        
        //현재 인벤토리 기준으로 재설정
        inventorySlots = GetChildSlots(inventoryBase.transform);
        
        if (inventorySlots.Count == 0)
        {
            //Debug.Log("증거물이 아무것도 존재하지 않아");
            _curSelectedSlot = null;
            // 증거물 디테일 UI 초기화
            InitEvidenceDetailUI();
            return;
        }
        
        //슬롯 선택 초기화
        currentIndex = 0;
        UpdateSelection();
        InitChapter(InventoryManager.Instance.currentViewChapter);
    }
    
    // 증거물 디테일 UI 초기화
    void InitEvidenceDetailUI()
    {
        _evidenceName.text = "";
        _evidenceImg.sprite = _evidenceDefaultImg;
        _evidenceContent.text = "";
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
        currentIndex -= 4;
        if(currentIndex < 0) currentIndex = Mathf.FloorToInt((inventorySlots.Count - 1) / 4) * 4;
        UpdateSelection();
    }
    
    //현재 슬롯에서 아래로 이동 - 정신(현실) 세계 증거물만 있는 경우
    public void MoveDown()
    {
        currentIndex += 4;
        if (currentIndex >= inventorySlots.Count)
        {
            int currentRow = currentIndex / 4;
            int endRow = Mathf.FloorToInt((inventorySlots.Count - 1) / 4);
            if (currentRow == endRow) currentIndex = endRow * 4;
            else if (currentRow > endRow) currentIndex = 0;
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

    // 슬롯 선택 시, 슬롯 선택에 따른 업데이트
    public void UpdateSelection()
    {
        _curSelectedSlot = inventorySlots[currentIndex];
        SetSlotSelected(_curSelectedSlot);
        SetSlotDetailInfo(_curSelectedSlot);
        //SetSlotUseBtn(_curSelectedSlot);
    }

    // 슬롯 디테일 정보 업데이트
    public void SetSlotDetailInfo(GameObject slot)
    {
        if (DataManager.Instance._evidences.ContainsKey(slot.GetComponent<InventorySlotInfo>().evidenceId))
        {
            EvidenceStructure evidence = DataManager.Instance._evidences[slot.GetComponent<InventorySlotInfo>().evidenceId];
            
            // 증거물 이름과 설명 업데이트
            _evidenceName.text = LocalizationSettings.StringDatabase.GetLocalizedString(LocalConstants.EvidenceNameTable, evidence.evidenceName, LocalizationSettings.SelectedLocale);
            _evidenceContent.text = LocalizationSettings.StringDatabase.GetLocalizedString(LocalConstants.EvidenceContentTable, evidence.evidenceTextDisplay, LocalizationSettings.SelectedLocale);
            
            // 증거물 디테일 배경 업데이트
            if (evidence.evidenceType == 'R')
            {
                _evidenceBackground.sprite = _backgroundImgs[0];
            }
            else if (evidence.evidenceType == 'M')
            {
                _evidenceBackground.sprite = _backgroundImgs[1];
            }
            
            // 증거물 디테일 이미지 업데이트
            if (DataManager.Instance._artResources.ContainsKey(evidence.artresourceId))
            {
                ArtResourceStructure artResource = DataManager.Instance._artResources[evidence.artresourceId];
                _evidenceImg.sprite = artResource.GetSpriteFromFilePath(artResource.filePathInventoryDetail);
            }
        }
    }

    //슬롯 선택 배경 업데이트
    public void SetSlotSelected(GameObject slot)
    {
        _slotOn = slot.transform.GetChild(0).gameObject;
        _slotOn.SetActive(true);
        
        foreach (var _slot in inventorySlots)
        {
            if (_slot != slot) SetSlotDeselected(_slot);
        }
    }

    public void SetSlotDeselected(GameObject slot)
    {
        slot.transform.GetChild(0).gameObject.SetActive(false);
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
        UIManager.Instance.CloseAllUI();
        
        // 증거물 사용 변수 초기화
        InventoryManager.Instance.isUsingEvidence = false;

        // 카드 키 제외 증거물 사용 코드
        if (!isUsingCardKey)
        {
            Debug.LogWarning($"{_evidenceUseEvent.eventId} 현재 트리거의 이벤트 아이디");
            // 현재 선택된 슬롯의 증거물 아이디가 올바른 증거물 아이디인지 체크
            foreach (var condition in _evidenceUseEvent.conditions)
            {
                if (condition.StartsWith("Evidence"))
                {
                    if (_curSelectedSlot.GetComponent<InventorySlotInfo>().evidenceId == condition)
                    {
                        PlayerInteract.Instance.isUsingEvidence = true;
                        EventManager.Instance.ExecuteEvent(_evidenceUseEvent.eventId).Forget();
                        return;
                    }
                    else
                    {
                        PlayerInteract.Instance.isUsingEvidence = false;
                        DialogueManager.Instance.SetDialogue("Dialogue_0064");
                        return;
                    }
                }
            }
        }
        else // 카드 키 관련 증거물 사용 코드
        {
            foreach (var key in keyInfos)
            {
                if (_curSelectedSlot.GetComponent<InventorySlotInfo>().evidenceId == key.evidenceID)
                {
                    PlayerInteract.Instance.isUsingEvidence = true;
                    EventManager.Instance.ExecuteEvent(key.resultID).Forget();
                    
                    isUsingCardKey = false;
                    keyInfos = null;
                    return;
                }
            }
            PlayerInteract.Instance.isUsingEvidence = false;
            DialogueManager.Instance.SetDialogue("Dialogue_0064");
                
            isUsingCardKey = false;
            keyInfos = null;

            // 카드 키 사용 후, 이전 챕터로 돌아가기
            if (previousChapter != -1)
            {
                InventoryManager.Instance.currentViewChapter = previousChapter;
                previousChapter = -1;
            }
        }
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
    
    // 증거물 사용 이벤트가 발생한 이벤트 id 가 무엇인지 체크
    public void SetEvidenceUseEventID(EventStructure _event)
    {
        _evidenceUseEvent = _event;
    }
}
