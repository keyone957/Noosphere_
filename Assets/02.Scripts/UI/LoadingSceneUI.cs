using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
public class LoadingSceneUI : MonoBehaviour
{
    [SerializeField] private Slider _loadingSlider;
    [SerializeField] private TextMeshProUGUI _loadingText;
    [SerializeField] private Image _fadeImage;
    [SerializeField] private float _fadeDuration = 0.8f;
    
    private void Start()
    {
        StartLoading().Forget();
    }
    private async UniTaskVoid StartLoading()
    {
        await LoadData();
        // SceneChanger.Instance.ChangeScene("PrologueMap_real");
        await FadeOut();
        SceneManager.LoadScene("PrologueMap_real");
    }

    private async UniTask LoadData()
    {
        await DataManager.Instance.InitializeData(UpdateLoadingProgress);
    }

    private void UpdateLoadingProgress(float progress)
    {
        _loadingSlider.value = progress;
        _loadingText.text = $"{Mathf.RoundToInt(progress * 100)}%";
    }

   
    private async UniTask FadeOut()
    {
        _fadeImage.color = new Color(0, 0, 0, 0);
        SoundManager.Instance.StopBGM();
        await _fadeImage.DOFade(1, _fadeDuration).AsyncWaitForCompletion();
    }
}
