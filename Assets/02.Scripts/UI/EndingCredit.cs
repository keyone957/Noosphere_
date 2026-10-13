using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;


public class EndingCredit : MonoBehaviour
{
    [SerializeField] private RawImage _creditImg;
    private void Start()
    {
        _creditImg.rectTransform.anchoredPosition = new Vector2(0, -1024);
        _creditImg.rectTransform.DOAnchorPosY(2048f, 30f)
            .SetDelay(1.0f)
            .SetEase(Ease.Linear);

    }
}
