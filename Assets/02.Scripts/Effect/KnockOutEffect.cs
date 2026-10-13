using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class KnockOutEffect : MonoBehaviour
{
    [SerializeField] private Image _knockOutImg;
    [Header("효과 시간 설정")]
    [SerializeField] private float _flashDuration = 0.1f;
    [SerializeField] private float _fadeDuration = 1f;

    private void OnEnable()
    {
        Play();
    }

    public void Play()
    {
        _knockOutImg.color = Color.black;
        _knockOutImg.gameObject.SetActive(true);

        Sequence seq = DOTween.Sequence();
        seq.Append(_knockOutImg.DOColor(Color.white, _flashDuration).SetEase(Ease.Linear));
        seq.Append(_knockOutImg.DOFade(0f, _fadeDuration).SetEase(Ease.Linear));
        seq.OnComplete(() => _knockOutImg.gameObject.SetActive(false));
    }
    
}
