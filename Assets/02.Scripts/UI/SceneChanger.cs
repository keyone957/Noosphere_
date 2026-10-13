using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class SceneChanger : Singleton<SceneChanger>
{
    public CanvasGroup _fadeImg;
    float fadeDuration = 1.5f;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _fadeImg = transform.GetComponentInChildren<CanvasGroup>();
        _fadeImg.DOFade(0, fadeDuration)
            .OnComplete(() =>
            {
                _fadeImg.blocksRaycasts = false;
                if (FindAnyObjectByType<PlayerController>() != null)
                    PlayerController.Instance.canMove =true;
            });
    }

    public async UniTaskVoid ChangeScene(string sceneName)
    {
        _fadeImg = transform.GetComponentInChildren<CanvasGroup>();
        await _fadeImg.DOFade(1, fadeDuration)
            .OnStart(() => {
                _fadeImg.blocksRaycasts = true;
            if (FindAnyObjectByType < PlayerController>()!=null)
                {
                    PlayerController.Instance.canMove = false;
                }
                SoundManager.Instance.StopBGM(1.5f);
            })
            .AsyncWaitForCompletion();
        DefaultUIController.Instance.CloseAllUI();
        await LoadScene(sceneName);
    }
    
    private async UniTask LoadScene(string sceneName)
    {
        SceneTracker.previousMentalState = PlayerInteract.Instance.isInMental;
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
       
        while (!async.isDone)
        {
            await UniTask.Yield();
        }
    }
}