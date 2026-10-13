 using System;
using System.Collections;
using System.Collections.Generic;
 using System.IO;
 using NooSphere;
 using TMPro;
 using Unity.VisualScripting;
 using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Debug = NooSphere.Debug;
public class InventorySlot
{
    public string evidenceId;
    public string evidenceName;
    public string evidenceContent;
    public char evidenceType;
    public char canUse;
    public string artresourceId;

    public InventorySlot(EvidenceStructure evidence)
    {
        evidenceId = evidence.evidenceId;
        evidenceName = evidence.evidenceName;
        evidenceContent = evidence.evidenceTextDisplay;
        evidenceType = evidence.evidenceType;
        canUse = evidence.canUse;
        artresourceId = evidence.artresourceId;
    }
}
public class ChapterInventory
{
    public List<InventorySlot> evidences { get; set; } = new List<InventorySlot>();
}
public class InventoryManager : Singleton<InventoryManager>
{
    //key : 챕터 숫자
    public Dictionary<int, ChapterInventory> chapterInventories = new Dictionary<int, ChapterInventory>();
    
    //인벤토리 UI
    [Header("인벤토리 UI 오브젝트")]
    [SerializeField] private GameObject _inventorySlotParent;
    [SerializeField] private GameObject _inventorySlotPrefab;
    [SerializeField] private List<Sprite> _slotSprite;
    
    [Space(5)][Header("인벤토리 정보")]
    //챕터 관련
    public List<GameObject> deselectedChapterUIList;
    public List<GameObject> selectedChapterUIList;
    public int currentViewChapter = 0;

    [Space(5)]
    [Header("증거물 사용 관련 변수")]
    public bool isUsingEvidence = false;

    private InventoryNavigator _navigator;

    public bool canOpenInventory = false;//컷씬 진행도중 인벤토리 열리는거 막기위함

    void Update()
    {
        //인벤토리 열기
        if (!UIManager.Instance.IsAnyUIOpen() && !DefaultUIController.Instance.IsAnyUIOpen())
        {
            if (canOpenInventory)
                return;

            if (PlayerInteract.Instance.GetComponent<MentalEnterProcess>().IsEnterNow())
            {
                return;
            }
            
            //키보드 입력 - Tab 버튼
            if (FindObjectOfType<HintImage>() == null && InputRouter.Instance.ConsumeTab())
            {
                UIManager.Instance.OpenUI(UIManager.Instance.inventoryUI);
            }
        
            //마우스 입력 - 인벤토리 아이콘 클릭
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            foreach (var result in results)
            {
                if (result.gameObject == UIManager.Instance.inventoryIcon && Input.GetMouseButtonDown(0))
                {
                    UIManager.Instance.OpenUI(UIManager.Instance.inventoryUI);
                }
            }
        }
    }
    
    //인벤토리 초기화(맨 처음 실행 후, 다시 실행되지 않음)
    public void InitInventory()
    {
        //챕터 정보 저장하기
        int chapterCount = Enum.GetValues(typeof(EventManager.RoomInfo)).Length;
        currentViewChapter = 0;
        chapterInventories.Clear();
        
        for (int i = 0; i < chapterCount; i++)
        {
            chapterInventories.Add(i,new ChapterInventory());
        }
    }

    // 증거물 가져오기
    public void AddEvidence(EvidenceStructure evidence)
    {
        // 인벤토리에 해당 증거물이 있으면 추가하지 않음
        if(IsEvidenceInInventory(evidence.evidenceId)) return;
        
        // 현재 어디 챕터인지 정보 가져오기
        int chapterNum = evidence.inventoryIndex;
        ChapterInventory _chapterInventory = chapterInventories[chapterNum];
        
        // 인벤토리 슬롯 생성
        InventorySlot newSlot = new InventorySlot(evidence);
        
        // 인벤토리에 추가
        _chapterInventory.evidences.Add(newSlot);
        evidence.isAcquired = true;
    }
    
    //인벤토리에서 증거물 제거하는 함수
    public void RemoveEvidence(EvidenceStructure evidence)
    {
        foreach (var chapter in chapterInventories)
        {
            foreach (var slot in chapter.Value.evidences)
            {
                if (slot.evidenceId == evidence.evidenceId)
                {
                    evidence.isAcquired = false;
                    chapter.Value.evidences.Remove(slot);
                    return;
                }
            }
        }
    }

    //현재 보이는 인벤토리 UI 업데이트
    public void UpdateInventoryUI()
    {
        //현실 세계, 정신 세계 인벤토리 초기화
        StartCoroutine(ClearInventoryUI());
    }
    
    //인벤토리 슬롯 초기화
    IEnumerator ClearInventoryUI()
    {
        int childCount = _inventorySlotParent.transform.childCount;
        //Debug.Log($"{_inventorySlotParent.name}의 자식은 {childCount}개 입니다.");

        if (childCount > 0)
        {
            for (int i = childCount - 1; i >= 0; i--)
            {
                Transform child = _inventorySlotParent.transform.GetChild(i);
                if (child != null)
                {
                    //Debug.Log($"{_inventorySlotParent.name}의 자식 {child.name}을 제거");
                    Destroy(child.gameObject);
                }
            }
        }
        
        yield return null;
        
        //현재 챕터에 따른 인벤토리 가져오기
        ChapterInventory currentInventory = chapterInventories[currentViewChapter];

        // 현실 세계 증거물 추가
        if (currentInventory.evidences.Count != 0)
        {
            foreach (var evidence in currentInventory.evidences)
            {
                GameObject slot = Instantiate(_inventorySlotPrefab, _inventorySlotParent.transform);
                slot.GetComponent<InventorySlotInfo>().evidenceId = evidence.evidenceId;
                UpdateSlotUI(slot, evidence);
            }
        }

        yield return null;
        
        //인벤토리 네비게이션 업데이트
        GetComponent<InventoryNavigator>().InitNavigator(_inventorySlotParent);
    }
    
    //slot UI 업데이트 함수
    void UpdateSlotUI(GameObject slot, InventorySlot evidence)
    {
        // 증거물 슬롯 배경(현실/정신) 업데이트
        if (evidence.evidenceType == 'R')
        {
            slot.GetComponent<Image>().sprite = _slotSprite[0];
        }
        else if(evidence.evidenceType == 'M')
        {
            slot.GetComponent<Image>().sprite = _slotSprite[1];
        }
        
        // 증거물 이미지 업데이트
        Image[] images = slot.GetComponentsInChildren<Image>(true);
        foreach (Image img in images)
        {
            if (img.gameObject.name == "EvidenceImg")
            {
                //아트 리소스 불러오기
                if (DataManager.Instance._artResources.ContainsKey(evidence.artresourceId))
                {
                    ArtResourceStructure artResource = DataManager.Instance._artResources[evidence.artresourceId];
                    img.sprite = artResource.GetSpriteFromFilePath(artResource.filePathInventoryThumbnail);
                }
                else
                {
                    Debug.Log(evidence.artresourceId +"가 리소스 내에 존재하지 않습니다.");
                }
                break;
            }
        }
    }

    public bool IsEvidenceInInventory(string evidenceID)
    {
        foreach (var chapter in chapterInventories)
        {
            foreach (var evidence in chapter.Value.evidences)
            {
                if (evidence.evidenceId == evidenceID)
                {
                    Debug.LogWarning($"{evidenceID}는 이미 획득한 증거물입니다.");
                    return true;
                }
            }
        }

        return false;
    }

    public void LoadInventoryData()
    {
        InitInventory();
        
        Debug.Log("인벤토리 데이터 로드 시작");
        foreach (var evidence in DataManager.Instance._evidences)
        {
            if (evidence.Value.isAcquired)
            {
                // 일기장 속 닥터와 딸 사진 증거물로 획득되지 않도록 예외처리
                if (evidence.Value.evidenceId == "Evidence_008")
                {
                    continue;
                }
                AddEvidence(evidence.Value);
            }
        }
    }
}
