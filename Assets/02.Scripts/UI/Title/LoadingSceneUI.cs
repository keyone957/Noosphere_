using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Debug = NooSphere.Debug;
public class LoadingSceneUI : MonoBehaviour
{
    [SerializeField] private Image _fadeImage;
    [SerializeField] private float _fadeDuration = 0.8f;
    [SerializeField] private float _minLoadingTime = 5f;

    private void Start()
    {
        SceneTracker.previousSceneName = SceneManager.GetActiveScene().name;
        StartLoading().Forget();
    }
    private async UniTaskVoid StartLoading()
    {
        Debug.LogWarning(NooSphere.SaveManager.Instance.CurrentLoadType);
        // 새 게임 로딩이면
        if (NooSphere.SaveManager.Instance.CurrentLoadType == NooSphere.GameLoadType.NewGame)
        {
            await DataManager.Instance.InitializeData();
            await UniTask.Delay(TimeSpan.FromSeconds(_minLoadingTime));
            await FadeOut();
            
            // 플레이타임 설정
            PlayTime.Instance.SetPlayTimeTracking(true); // 플레이타임 추적 가능하게 설정
            
            SceneManager.LoadScene("IntroScene");
        }
        else if(NooSphere.SaveManager.Instance.CurrentLoadType == NooSphere.GameLoadType.ContinueGame) // 이어하기 로딩이면
        {
            await DataManager.Instance.LoadSaveData();
            await UniTask.Delay(TimeSpan.FromSeconds(_minLoadingTime));
            await FadeOut();
            
            // 플레이타임 설정
            int index = NooSphere.SaveManager.Instance.selectSlotIndex;
            PlayTime.Instance.SetPlayTime(NooSphere.SaveManager.Instance.GetPlayTimeFloatData(index));
            PlayTime.Instance.SetPlayTimeTracking(true); // 플레이타임 추적 가능하게 설정
            
            string sceneName = NooSphere.SaveManager.Instance.GetSceneName(NooSphere.SaveManager.Instance.selectSlotIndex);
            SceneManager.LoadScene(sceneName);
        }
    }

    private async UniTask FadeOut()
    {
        _fadeImage.color = new Color(0, 0, 0, 0);
        SoundManager.Instance.StopBGM(1.5f);
        await _fadeImage.DOFade(1, _fadeDuration).AsyncWaitForCompletion();
    }
}
