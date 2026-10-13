using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class CreditUI : DefaultUIBase
{
    [SerializeField] private RawImage _creditImg;
    [SerializeField] private Button _exitBtn;
    public override void OnOpen()
    {
        base.OnOpen();

        _creditImg.rectTransform.anchoredPosition = new Vector2(0, -1450);
        _creditImg.rectTransform.DOAnchorPosY(2048f, 30f)
            .SetDelay(1.0f)
            .SetEase(Ease.Linear);

        transform.GetChild(0).gameObject.SetActive(true);
    }

    public override void HandleKeyboardInput()
    {
        base.HandleKeyboardInput();
        if (InputRouter.Instance.ConsumeEscape())
        {
            DefaultUIController.Instance.CloseTopUI();
        }
    }

    public override void OnClose()
    {
        base.OnClose();

        DOTween.Kill(_creditImg.rectTransform);

        transform.GetChild(0).gameObject.SetActive(false);
    }

    private void Start()
    {
        _exitBtn.onClick.AddListener(() => { DefaultUIController.Instance.CloseTopUI();});
    }
}