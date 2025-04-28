using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class FadeOutEffect : MonoBehaviour
{
    [SerializeField] private Image _fadeImage;

    private void OnEnable()
    {
        SoundManager.Instance.StopAllSFX();
        SoundManager.Instance.PlayLoopingSound("Soundresource_090");
        _fadeImage.color = new Color(0, 0, 0, 0);
        _fadeImage.DOFade(1f, 5f)
            .SetEase(Ease.InOutQuad).OnComplete(()=>SceneManager.LoadScene("StartScene"));
        // todo 최종스테이지로
    }
}