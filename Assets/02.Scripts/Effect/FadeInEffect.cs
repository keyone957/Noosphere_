using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class FadeInEffect : MonoBehaviour
{
    [SerializeField] private Image _fadeImage;
    [SerializeField]private float _fadeDuration;

    private void OnEnable()
    {
        SoundManager.Instance.StopAllSFX();
        PlayerController.Instance.canMove = false;
        _fadeImage.color = new Color(0, 0, 0, 1);
        _fadeImage.DOFade(0f, _fadeDuration)
            .SetEase(Ease.InOutQuad).OnComplete(() => _fadeImage.gameObject.SetActive(false));
    }
}