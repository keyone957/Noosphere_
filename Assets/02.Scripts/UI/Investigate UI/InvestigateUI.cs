using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InvestigateUI : UIBase
{
    [Header("증거물 조사 UI")] [SerializeField] private EvidenceStructure _curEvidence;
    [SerializeField] private GameObject _curSelectedBtn;

    [Space(5)] [SerializeField] private TextMeshProUGUI _evidenceName;
    [SerializeField] private TextMeshProUGUI _evidenceDescription;
    [SerializeField] private Image _evidenceImage;
    [Space(5)]
    [SerializeField] private GameObject _yesBtn;
    [SerializeField] private GameObject _noBtn;

    public override void OnOpen(EvidenceStructure evidence)
    {
        base.OnOpen(evidence);
        
        //조사하는 증거물에 대한 정보 반영
        SetInvestigateUI(evidence);
        _curEvidence = evidence;
        UIManager.Instance.isYesClicked = false;
        //기본적으로 Yes 버튼 호버되게
        HoverYesBtn();
        //조사 UI 열기
        transform.GetChild(0).gameObject.SetActive(true);
    }

    public override void OnClose()
    {
        base.OnClose();
        UIManager.Instance.OnSelectEnd?.Invoke();
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public override void HandleKeyboardInput()
    {
        base.HandleKeyboardInput();
        //키보드 A - YES 버튼
        if (Input.GetKeyDown(KeyCode.A))
        {
            SoundManager.Instance.PlaySFX("Soundresource_035");
            HoverYesBtn();
        }

        //키보드 D - NO 버튼
        if (Input.GetKeyDown(KeyCode.D))
        { 
            SoundManager.Instance.PlaySFX("Soundresource_035");
            HoverNoBtn();
        }

        //스페이스 - 버튼 선택
        if (_curSelectedBtn != null && Input.GetKeyDown(KeyCode.Space))
        {
            SoundManager.Instance.PlaySFX("Soundresource_037");
            if (_curSelectedBtn == _yesBtn)
            {
                ClickYesBtn();
            }
            else if (_curSelectedBtn == _noBtn)
            {
                ClickNoBtn();
            }
        }
            
        //ESC - NO 선택
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SoundManager.Instance.PlaySFX("Soundresource_036");
            //NO 버튼 선택
            ClickNoBtn();
        }
    }
    
    public void HoverYesBtn()
    {
        SetButtonSelected(_yesBtn, UnityExtension.HexColor(GreenColor));
        SetButtonSelected(_noBtn, UnityExtension.HexColor(WhiteColor));
        _curSelectedBtn = _yesBtn;
    }

    public void HoverNoBtn()
    {
        SetButtonSelected(_noBtn,UnityExtension.HexColor(GreenColor));
        SetButtonSelected(_yesBtn,UnityExtension.HexColor(WhiteColor));
        _curSelectedBtn = _noBtn;
    }

    public void ClickYesBtn()
    {
        UIManager.Instance.isYesClicked = true;
        HoverYesBtn();
        UIManager.Instance.CloseTopUI();
        
        _curEvidence.AcquireEvidence();
        UIManager.Instance.OpenUI(UIManager.Instance.evidenceDetailUI,_curEvidence);
        
        UIManager.Instance.OnSelectEnd?.Invoke();
        
        _curEvidence = null;
    }

    public void ClickNoBtn()
    {
        HoverNoBtn();
        UIManager.Instance.CloseTopUI();
        
        _curEvidence.AcquireEvidence();
        UIManager.Instance.OnSelectEnd?.Invoke();
        
        _curEvidence = null;
    }
    //증거물 조사 UI의 정보 세팅하기
    public void SetInvestigateUI(EvidenceStructure evidence)
    {
        _evidenceName.text = evidence.evidenceName;
        _evidenceDescription.text = evidence.evidenceTextDisplay;
        
        //아트 리소스 불러오기
        if (DataManager.Instance._artResources.ContainsKey(evidence.artresourceId))
        {
            //아트 리소스 내 증거물 인벤토리 이미지 가져오기
            ArtResourceStructure artResource = DataManager.Instance._artResources[evidence.artresourceId];
            _evidenceImage.sprite = artResource.GetSpriteFromFilePath(artResource.filePathInventoryThumbnail);
        }
        else
        {
            Debug.Log(evidence.artresourceId + "가 리소스 내에 존재하지 않습니다.");
        }
    }
}
