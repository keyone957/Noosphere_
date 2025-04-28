using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardKeyButton : MonoBehaviour
{
    [SerializeField] private ButtonEventChannel _eventChannel;
    [SerializeField] private string _cardKeyEvidenceID;
    [SerializeField] private Button _button;
    [SerializeField] private TextMeshProUGUI _buttonText;
    [SerializeField] private GameObject _cardObj;
    [SerializeField] private string _takeEventID;
    [SerializeField] private string _returnEventID;
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

            DialogueManager.Instance.SetDialogue(_takeEventID);
        }
        else//카드 반납할때
        { 
            SoundManager.Instance.PlaySFX("Soundresource_070");
            _eventChannel.RaiseReturnEvent(_cardKeyEvidenceID);
            //인벤토리에 있는 카드키 제거
            InventoryManager.Instance.RemoveEvidence(DataManager.Instance._evidences[_cardKeyEvidenceID]);
            
            DialogueManager.Instance.SetDialogue(_returnEventID);
        }
    }

    private void OnOtherButtonClicked(string clickedButtonId)//클릭한 버튼이외에 다른 버튼들 이벤트 전달
    {
        if(clickedButtonId != _cardKeyEvidenceID)
        {
            Debug.Log($"Button {_cardKeyEvidenceID} received event from Button {clickedButtonId}");
            _buttonText.text = "사용 불가";
            _button.interactable = false;
            _isTakenCard = false;
        }
        else
        {
            _cardObj.SetActive(false);
            _buttonText.text = "반납 하기";
            _isTakenCard = true;
        }
    }
    private void OnCardStateReturn(string returnedButtonId)
    {
        if(returnedButtonId != _cardKeyEvidenceID)
        {
            _buttonText.text = "사용 가능";
            _button.interactable = true;
        }
        else
        {
            _cardObj.SetActive(true);
            _buttonText.text = "사용 가능";
            _isTakenCard = false;
        }
    }

    public void InitCardKey()
    {
        _eventChannel.OnButtonClicked?.Invoke(_cardKeyEvidenceID);
    }
}
