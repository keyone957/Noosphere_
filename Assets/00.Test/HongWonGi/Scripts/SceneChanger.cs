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

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _fadeImg.DOFade(0, fadeDuration)
            .OnComplete(() =>
            {
                _fadeImg.blocksRaycasts = false;
                PlayerController.Instance.canMove =true;
            });
    }

    public async UniTaskVoid ChangeScene(string sceneName)
    {
        await _fadeImg.DOFade(1, fadeDuration)
            .OnStart(() => {
                _fadeImg.blocksRaycasts = true;
                if (PlayerController.Instance!=null)
                {
                    PlayerController.Instance.canMove = false;
                }
                SoundManager.Instance.StopBGM();
            })
            .AsyncWaitForCompletion();

        await LoadScene(sceneName);
    }
    
    private async UniTask LoadScene(string sceneName)
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
       
        while (!async.isDone)
        {
            await UniTask.Yield();
        }
    }
}