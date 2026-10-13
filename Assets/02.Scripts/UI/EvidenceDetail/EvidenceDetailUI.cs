using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Settings;
using Debug = NooSphere.Debug;

public class EvidenceDetailUI : UIBase
{
    [Header("현재 보고 있는 증거물")] 
    [SerializeField] private string _curEvidenceID;
    private EvidenceStructure _curEvidence;

    [Header("증거물 상세 UI GameObject")] [SerializeField]
    private GameObject _keyUI;
    [SerializeField] private GameObject _objectUI;
    [SerializeField] private GameObject _onePageUI;
    [SerializeField] private GameObject _twoPageUI;
    
    [Header("증거물 상세내용 공통 UI")]
    [SerializeField] private Image _bgImg;
    [SerializeField] private Sprite _inventoryBgImgReal;
    [SerializeField] private Sprite _inventoryBgImgMental;

    [Header("Page 증거물")] [SerializeField] private int _curPage;
    [SerializeField] private int _totalPage;
    [SerializeField] private List<Sprite> _pages;
    [SerializeField] private Image _firstPage;
    [SerializeField] private Image _secondPage;
    [SerializeField] private GameObject _prevPageBtn; //이전 페이지 버튼
    [SerializeField] private GameObject _nextPageBtn; //다음 페이지 버튼

    [Header("서브 증거물")] [SerializeField] private bool isSubEvidence = false;
    
    public override void OnOpen(EvidenceStructure evidence)
    {
        base.OnOpen(evidence);
        Debug.Log($"#{evidence}에 대한 상세 설명 오픈");
        
        if (evidence == null)
        {
            return;
        }
        
        UIManager.Instance.cctvFrame.SetActive(false);
        
        //인벤토리에서 증거물 상세사항을 오픈할 경우에는 UI 순서를 위해 아래의 설정이 필요함.
        if (!UIManager.Instance.isInMap)
        {
            PlayerController.Instance._uiCanvas.renderMode = RenderMode.ScreenSpaceCamera;
            PlayerController.Instance._uiCanvas.worldCamera = PlayerController.Instance._mainCamera;
        }
        
        SetDetailEvidence(evidence);
        _curEvidenceID = evidence.evidenceId;
        
        if (transform.childCount > 0)
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }

        // 나가기 버튼 활성화
        EscapeUI.Instance.Active();
    }

    public override void OnClose()
    {
        base.OnClose();
        
        if (PlayerController.Instance._uiCanvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            PlayerController.Instance._uiCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }
        
        UIManager.Instance.cctvFrame.SetActive(true);
        
        _objectUI.SetActive(false);
        _onePageUI.SetActive(false);
        _twoPageUI.SetActive(false);
        _keyUI.SetActive(false);
        
        transform.GetChild(0).gameObject.SetActive(false);
        
        _curEvidenceID = "";
        _curEvidence = null;
        isSubEvidence = false;
    }

    public override void HandleKeyboardInput()
    {
        base.HandleKeyboardInput();
        
        // Object UI이면 페이지 넘기는 소리 나오지 않도록
        if(_objectUI.activeSelf) return;
        
        //키보드 A - 이전 페이지 버튼 
        if (_prevPageBtn != null && _nextPageBtn != null)
        {
            if (_prevPageBtn.activeSelf && InputRouter.Instance.ConsumeA())
            {
                RemoveAllListeners();
                AddOnClickListener(ClickPrevPageEvent);
                OnClickEvent?.Invoke();
            }

            //키보드 D - 다음 페이지 버튼
            if (_nextPageBtn.activeSelf && InputRouter.Instance.ConsumeD())
            { 
                RemoveAllListeners();
                AddOnClickListener(ClickNextPageEvent);
                OnClickEvent?.Invoke();
            }
        }
    }
    
    void SetDetailEvidence(EvidenceStructure evidence)
    {
        ArtResourceStructure artResource = DataManager.Instance._artResources[evidence.artresourceId];
        Debug.Log(artResource.artresourceId + " 아트 리소스 아이디!");
        
        if (artResource != null)
        {
            if (!UIManager.Instance.isInMap)
            {
                if (evidence.evidenceType == 'R')
                {
                    _bgImg.sprite = _inventoryBgImgReal;
                }
                else if (evidence.evidenceType == 'M')
                {
                    _bgImg.sprite = _inventoryBgImgMental;
                }
            }
            else
            {
                _bgImg.sprite = artResource.GetSpriteFromFilePath(artResource.filePathMapBackground);
            }
            
            //세부 증거물이 있는지 체크
            if (!string.IsNullOrEmpty(evidence.subEvidenceId) &&
                DataManager.Instance._evidences.ContainsKey(evidence.subEvidenceId))
            {
                isSubEvidence = true;
                _curEvidence = DataManager.Instance._evidences[evidence.evidenceId];
            }
            
            //evidence 성질에 따라 프리팹인지 UI인지 결정
            if (evidence.shapeType == "Object")
            {
                SetObjectDetail(artResource);
                _objectUI.SetActive(true);
                _keyUI.SetActive(true);
            }
            else if (evidence.shapeType == "OnePage")
            {
                InitPageVariables(artResource);
                SetOnePageDetail(artResource);
                _onePageUI.SetActive(true);
            }
            else if (evidence.shapeType == "TwoPage")
            {
                InitPageVariables(artResource);
                SetTwoPageDetail(artResource);
                _twoPageUI.SetActive(true);
            }
        }
        else
        {
            Debug.Log($"#{evidence.artresourceId} 아트 리소스가 존재하지 않습니다.");
        }
    }
    
    // shapeType이 object인 증거물인 경우
    void SetObjectDetail(ArtResourceStructure artResource)
    {
        //prefab parent 아래에 자식 오브젝트가 있다면 제거 후, 올바른 오브젝트 생성
        if (_objectUI.transform.childCount > 0)
        {
            foreach (Transform child in _objectUI.transform)
            {
                Destroy(child.gameObject);
            }
        }

        GameObject prefab;
        if (artResource.artresourceId == "Artresource_0016")
        {
            prefab = artResource.GetPrefabFromFilePath(true);
        }
        else
        {
            prefab = artResource.GetPrefabFromFilePath(false);
        }
        
        if (prefab != null)
        {
            Instantiate(prefab, _objectUI.transform);
        }
        else
        {
            Debug.LogError($"🔥 {artResource.filePath}에 해당하는 프리팹이 존재하지 않습니다.");
        }
    }

    void InitPageVariables(ArtResourceStructure artResource)
    {
        //페이지 리스트 초기화
        _pages.Clear();
        //상세 이미지 불러와서 페이지 리스트에 저장
        _totalPage = artResource.pageCnt;
        //시작 위치에서 totalPage 수 만큼, 변수 증가해서 읽어들이기
        string imgPath = artResource.filePathStartPage;
        
        for (int page = 0; page < _totalPage; page++)
        {
            string localInfo = LocalizationSettings.SelectedLocale.Identifier.Code;
            imgPath = imgPath.Replace("LOCAL",localInfo);
            int lastUnderscoreIndex = imgPath.LastIndexOf('_'); 
            string prefix = imgPath.Substring(0, lastUnderscoreIndex + 1);
            string modifiedString = $"{prefix}{page:D2}";
            
            Sprite pageImg = artResource.GetSpriteFromFilePath(modifiedString);
            _pages.Add(pageImg);
        }
        //이미지 null로 초기화
        _firstPage = null;
        _secondPage = null;
    }
    
    // shapeType이 onePage인 증거물인 경우
    void SetOnePageDetail(ArtResourceStructure artResource)
    {
        //페이지 초기화
        _curPage = 1;
        //페이지가 1개인 경우 vs 1개 이상인 경우
        //1개인 경우 : 좌우이동 버튼 비활성화
        //1개 이상인 경우 : 좌우이동 버튼 활성화
        
        //버튼 참조하기
        _prevPageBtn = _onePageUI.transform.Find("PageBtns").GetChild(0).gameObject;
        _nextPageBtn = _onePageUI.transform.Find("PageBtns").GetChild(1).gameObject;
        
        if (_totalPage == 1)
        {
            _prevPageBtn.SetActive(false);
            _nextPageBtn.SetActive(false);
        }
        else if (_totalPage > 1)
        {
            _prevPageBtn.SetActive(true);
            _nextPageBtn.SetActive(true);
        }
        else
        {
            Debug.Log($"{artResource.artresourceId}의 pageCnt가 올바르지 않습니다.");
        }
        
        //이미지 설정하기
        _firstPage = _onePageUI.transform.Find("Pages").GetChild(0).gameObject.GetComponent<Image>();
        UpdateOnePage();
    }
    
    // shapeType이 twoPage인 증거물인 경우
    void SetTwoPageDetail(ArtResourceStructure artResource)
    {
        //페이지 초기화
        _curPage = 2;
        
        //버튼 참조하기
        _prevPageBtn = _twoPageUI.transform.Find("PageBtns").GetChild(0).gameObject;
        _nextPageBtn = _twoPageUI.transform.Find("PageBtns").GetChild(1).gameObject;
        
        //페이지가 2개인 경우, 2개 이상인 경우
        if (_totalPage == 2)
        {
            _prevPageBtn.SetActive(true);
            _nextPageBtn.SetActive(true);
        }
        else if (_totalPage > 2)
        {
            _prevPageBtn.SetActive(true);
            _nextPageBtn.SetActive(true);
        }
        else
        {
            Debug.Log($"{artResource.artresourceId}의 pageCnt가 올바르지 않습니다.");
        }
        
        //이미지 설정하기
        _twoPageUI.GetComponent<Image>().sprite = artResource.GetSpriteFromFilePath(artResource.filePathContentBackground);
        _firstPage = _twoPageUI.transform.Find("Pages").GetChild(0).gameObject.GetComponent<Image>();
        _secondPage = _twoPageUI.transform.Find("Pages").GetChild(1).gameObject.GetComponent<Image>();
        UpdateTwoPage();
    }

    void UpdateOnePage()
    {
        //이미지 업데이트
        _firstPage.sprite = _pages[_curPage - 1];
        
        //서브 증거물 체크
        if (isSubEvidence)
        {
            CheckSubEvidence();
        }
        
        //버튼 업데이트
        if (_curPage == 1)
        {
            SetBtnAble(_nextPageBtn);
            SetBtnDisable(_prevPageBtn);
            return;
        }

        if (_curPage == _totalPage)
        {
            SetBtnAble(_prevPageBtn);
            SetBtnDisable(_nextPageBtn);
            return;
        }
        
        SetBtnAble(_prevPageBtn);
        SetBtnAble(_nextPageBtn);
    }
    
    void UpdateTwoPage()
    {
        //이미지 업데이트
        _firstPage.sprite = _pages[_curPage - 2];
        _secondPage.sprite = _pages[_curPage - 1];
        
        //서브 증거물 체크
        if (isSubEvidence)
        {
            CheckSubEvidence();
        }

        //버튼 업데이트
        if (_totalPage > 2)
        {
            if (_curPage == 2)
            {
                SetBtnAble(_nextPageBtn);
                SetBtnDisable(_prevPageBtn);
                return;
            }

            if (_curPage == _totalPage)
            {
                SetBtnAble(_prevPageBtn);
                SetBtnDisable(_nextPageBtn);
                return;
            }
            SetBtnAble(_prevPageBtn);
            SetBtnAble(_nextPageBtn);
        }
        else
        {
            SetBtnDisable(_prevPageBtn);
            SetBtnDisable(_nextPageBtn);
        }
    }

    public void ClickNextPageEvent()
    {
        if (_secondPage == null)
        {
            if(_curPage >= _totalPage) return;
            
            _curPage++;
            UpdateOnePage();
            SoundManager.Instance.PlaySFX("Soundresource_070");
            
            int random = Random.Range(0, 4);
            string id = "";
            switch (random)
            {
                case 0 :
                    id = "Soundresource_038";
                    break;
                case 1:
                    id = "Soundresource_039";
                    break;
                case 2:
                    id = "Soundresource_040";
                    break;
                case 3:
                    id = "Soundresource_041";
                    break;
            }
            SoundManager.Instance.PlaySFX(id);
        }
        else
        {
            if(_curPage >= _totalPage) return;
            
            _curPage += 2;
            UpdateTwoPage();
            SoundManager.Instance.PlaySFX("Soundresource_070");
            int random = Random.Range(0, 4);
            string id = "";
            switch (random)
            {
                case 0 :
                    id = "Soundresource_038";
                    break;
                case 1:
                    id = "Soundresource_039";
                    break;
                case 2:
                    id = "Soundresource_040";
                    break;
                case 3:
                    id = "Soundresource_041";
                    break;
            }
            SoundManager.Instance.PlaySFX(id);
        }
    }

    public void ClickPrevPageEvent()
    {
        if (_secondPage == null)
        {
            if(_curPage <= 1) return;
            
            _curPage--;
            UpdateOnePage();
            SoundManager.Instance.PlaySFX("Soundresource_070");
            int random = Random.Range(0, 4);
            string id = "";
            switch (random)
            {
                case 0 :
                    id = "Soundresource_038";
                    break;
                case 1:
                    id = "Soundresource_039";
                    break;
                case 2:
                    id = "Soundresource_040";
                    break;
                case 3:
                    id = "Soundresource_041";
                    break;
            }
            SoundManager.Instance.PlaySFX(id);
        }
        else
        {
            if(_curPage <= 2) return;
            
            _curPage -= 2;
            UpdateTwoPage();
            SoundManager.Instance.PlaySFX("Soundresource_070");
            int random = Random.Range(0, 4);
            string id = "";
            switch (random)
            {
                case 0 :
                    id = "Soundresource_038";
                    break;
                case 1:
                    id = "Soundresource_039";
                    break;
                case 2:
                    id = "Soundresource_040";
                    break;
                case 3:
                    id = "Soundresource_041";
                    break;
            }
            SoundManager.Instance.PlaySFX(id);
        }
    }

    void SetBtnDisable(GameObject btn)
    {
        btn.GetComponent<Image>().color = UnityExtension.HexColor(GrayColor);
    }

    void SetBtnAble(GameObject btn)
    {
        btn.GetComponent<Image>().color = UnityExtension.HexColor(WhiteColor);
    }

    async void CheckSubEvidence()
    {
        //우선은 OnePage, TwoPage에 대해서만 작성
        //OnePage에서는 _curPage와 바로 비교
        //TwoPage에서는 _curPage - 1 ~ _curPage와 비교
        Debug.Log($"CheckSubEvidence 실행 중인지 확인. 현재 증거물 {_curEvidence.evidenceId} , 서브 증거물 {_curEvidence.subEvidenceId}");
        if (_curEvidence.subEvidenceAcquisitionType == "Page" && _curEvidence.acquisitionPageNum > 0)
        {
            Debug.Log($"현재 페이지 {_curPage} , 목표 페이지 {_curEvidence.acquisitionPageNum}");
            if (_curEvidence.shapeType == "OnePage")
            {
                if (_curPage == _curEvidence.acquisitionPageNum)
                {
                    Debug.Log($"#{_curEvidence.evidenceId}의 서브 증거물 {_curEvidence.subEvidenceId}을 {_curPage}({_curEvidence.acquisitionPageNum})에서 발견");
                    string resultID = _curEvidence.acquisitionPageResultId;
                    //예외 처리 코드
                }
            }
            else if (_curEvidence.shapeType == "TwoPage")
            {
                if (_curEvidence.acquisitionPageNum >= _curPage-1 && _curEvidence.acquisitionPageNum <= _curPage)
                {
                    EvidenceStructure _subEvidence = DataManager.Instance._evidences[_curEvidence.subEvidenceId];
                    Debug.Log($"#{_curEvidence.evidenceId}의 서브 증거물 {_curEvidence.subEvidenceId}을 {_curPage}({_curEvidence.acquisitionPageNum})에서 발견");
                    Debug.Log($"#{_curEvidence.subEvidenceId} 증거물에 대한 접근 횟수는 {_subEvidence.accessCnt}입니다.");

                    string resultID = _curEvidence.acquisitionPageResultId;
                    
                    //예외 처리 코드
                    if (_subEvidence.evidenceId == "Evidence_008" && EventManager.Instance.curRoomInfo !=
                        EventManager.RoomInfo.Room_101)
                    {
                        //프롤로그에서만 볼 수 없는 이벤트이기 때문에, 프롤로그가 아니면 강제 종료 이벤트 실행되지 않도록 이른 리턴
                        return;
                    }
                    
                    if (_subEvidence.evidenceId == "Evidence_008" && _subEvidence.accessCnt >= 1)
                    {
                        resultID = "Event_A024";
                    }
                    
                    await EventManager.Instance.ExecuteEvent(resultID);
                }
            }   
        }
    }

    public void CloseUI()
    {
        Debug.Log("UI 닫기 버튼 마우스 클릭함");
        UIManager.Instance.CloseTopUI();
    }
}
