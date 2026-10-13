using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndingScene : MonoBehaviour
{
    [SerializeField] private GameObject _friendObj;
    [SerializeField] private Animator _playerAnimator;
    [SerializeField] private Animator _friendAnimator;
    [SerializeField] private Image _fadeImage;
    [SerializeField] private float _fadeDuration;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        SoundManager.Instance.PlaySFXNoEffect("Soundresource_130");
        InventoryManager.Instance.canOpenInventory = true;
       
        Destroy(PlayerController.Instance.gameObject);
        //PlayerController.Instance.gameObject.SetActive(false);
        if (DataManager.Instance._events["Event_D103"].isExecuted)
        {
            _friendObj.SetActive(false);
        }
        
    }

    public void SetAnim()
    {
        _playerAnimator.speed = 0;
    }

    public void StopAnim()
    {
        _friendAnimator.speed = 0;
    }

    public void StartFadeIn()
    {
        FadeIn().Forget();
    }

    private async UniTask FadeIn()
    {
        _fadeImage.color = new Color(0, 0, 0, 1);
        UIManager.Instance.inventoryIcon.SetActive(false);
        await _fadeImage.DOFade(0, _fadeDuration).AsyncWaitForCompletion();
    }
    public void StartFadeOut()
    {
        FadeOut().Forget();
    }

    private async UniTask FadeOut()
    {
        _fadeImage.color = new Color(0, 0, 0, 0);
        await _fadeImage.DOFade(1, _fadeDuration).AsyncWaitForCompletion();
    }

    public void LoadScene(string scene)
    {
        SceneManager.LoadSceneAsync(scene);
    }
}