using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Debug = NooSphere.Debug;

public class CardKeyButton : MonoBehaviour
{
    [SerializeField] private ButtonEventChannel _eventChannel;
    [SerializeField] private string _cardKeyEvidenceID;
    [SerializeField] private Button _button;
    [SerializeField] private Image _cardImg;
    [SerializeField] private Sprite _normalSpr;
    [SerializeField] private Sprite _selectedSprite;
    [SerializeField] private string _takeEventID;
    [SerializeField] private string _returnEventID;
    [SerializeField] private GameObject _cardKeyStateText;
    [SerializeField] private GameObject _returnText;
    private bool _isTakenCard;
    

    private void Awake()
    {
        _button.onClick.AddListener(OnClickCard);
    }
    private void OnEnable()
    
    {
        _eventChannel.OnButtonClicked += OnOtherButtonClicked;
        _eventChannel.OnResetCardState += OnCardStateReturn; 
    }

    private void OnDisable()
    {
        _eventChannel.OnButtonClicked -= OnOtherButtonClicked;
        _eventChannel.OnResetCardState -= OnCardStateReturn; 
    }

    private async void OnClickCard()
    {
        if (!_isTakenCard)//카드 가져갔을때
        {
            SoundManager.Instance.PlaySFX("Soundresource_070");
            _eventChannel.RaiseEvent(_cardKeyEvidenceID);
            //인벤토리에 추가
            InventoryManager.Instance.AddEvidence(DataManager.Instance._evidences[_cardKeyEvidenceID]);
            _cardImg.sprite = _selectedSprite;
            _button.transition = Selectable.Transition.None;
            Selected();
            DialogueManager.Instance.SetDialogue(_takeEventID);
        }
        else//카드 반납할때
        { 
            SoundManager.Instance.PlaySFX("Soundresource_070");
            _eventChannel.RaiseReturnEvent(_cardKeyEvidenceID);
            //인벤토리에 있는 카드키 제거
            InventoryManager.Instance.RemoveEvidence(DataManager.Instance._evidences[_cardKeyEvidenceID]);
            _cardImg.sprite = _normalSpr;
            InitTextState();
            DialogueManager.Instance.SetDialogue(_returnEventID);
            _button.transition = Selectable.Transition.SpriteSwap;
        }
        EventSystem.current.SetSelectedGameObject(null);
    }

    private void OnOtherButtonClicked(string clickedButtonId)//클릭한 버튼이외에 다른 버튼들 이벤트 전달
    {
        if(clickedButtonId != _cardKeyEvidenceID)
        {
            Debug.Log($"Button {_cardKeyEvidenceID} received event from Button {clickedButtonId}");
            _button.interactable = false;
            _isTakenCard = false;
            UnSelected();
        }
        else
        {
            _isTakenCard = true;
            Selected();
        }
    }
    private void OnCardStateReturn(string returnedButtonId)
    {
        if(returnedButtonId != _cardKeyEvidenceID)
        {
            _button.interactable = true;
            InitTextState();
        }
        else
        {
            _isTakenCard = false;
            InitTextState();
        }
    }

    public void InitCardKey(bool isAcquired, bool noOwned)
    {
        if (isAcquired)
        {
            _isTakenCard = true;
            _cardImg.sprite = _selectedSprite;
            _button.transition = Selectable.Transition.None;
            Selected();
            _eventChannel.OnButtonClicked?.Invoke(_cardKeyEvidenceID);
        }
        else
        {
            _isTakenCard = false;
            _cardImg.sprite = _normalSpr;
            _button.transition = Selectable.Transition.SpriteSwap;
            
            //하나도 획득된 게 없으면 사용 가능, 있으면 잠금
            _button.interactable = noOwned;
            if (noOwned)
                InitTextState();
            else
                UnSelected();
        }
    }
    private void Selected()
    {
        _returnText.SetActive(true);
        _cardKeyStateText.SetActive(false);
    }
    private void UnSelected()
    {
        _returnText.SetActive(false);
        _cardKeyStateText.SetActive(true);
    }
    private void InitTextState()
    {
        _returnText.SetActive(false);
        _cardKeyStateText.SetActive(false);
    }
   
}
