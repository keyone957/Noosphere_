using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
public class FadeForEndingBranch : MonoBehaviour
{
    [SerializeField] private Image _fadeImage;
    private void OnEnable()
    {
        // SoundManager.Instance.StopAllSFX();
        // PlayerController.Instance.canMove = false;
        // _fadeImage.color = new Color(0, 0, 0, 0);
        // _fadeImage.DOFade(1f, 5f)
        //     .SetEase(Ease.InOutQuad).OnComplete(()=>
        //     {
        //         EffectManager.Instance.OnEffectEnd?.Invoke();
        //         if (DataManager.Instance._evidences["Evidence_016"].isAcquired)
        //         {
        //             SceneManager.LoadSceneAsync("EndingScene");
        //         }
        //         else
        //         {
        //             SceneManager.LoadSceneAsync("DemoEndScene");
        //         }
        //     });
        // // todo 최종스테이지로
    }

    public void StartFadeIn()
    {
        gameObject.SetActive(true);
        // SoundManager.Instance.StopAllSFX();
        SoundManager.Instance.StopBGM(2.0f);
        PlayerController.Instance.canMove = false;
        _fadeImage.color = new Color(0, 0, 0, 0);
        _fadeImage.DOFade(1f, 3f);
    }

    public void LoadEndingScene()
    {
        if (DataManager.Instance._evidences["Evidence_016"].isAcquired)
        {
            SceneManager.LoadSceneAsync("EndingScene");
        }
        else
        {
            SceneManager.LoadSceneAsync("DemoEndScene");
        }
    }
}
