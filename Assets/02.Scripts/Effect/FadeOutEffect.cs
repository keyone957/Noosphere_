using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class FadeOutEffect : MonoBehaviour
{
    [SerializeField] private Image _fadeImage;
    [SerializeField] private string _sceneName;
    private void OnEnable()
    {
        SoundManager.Instance.StopAllSFX();
        SoundManager.Instance.StopBGM(4);
        UIManager.Instance.inventoryIcon.SetActive(false);
        InventoryManager.Instance.canOpenInventory = true;
        PlayerController.Instance.canMove = false;
        _fadeImage.color = new Color(0, 0, 0, 0);
        _fadeImage.DOFade(1f, 5f)
            .SetEase(Ease.InOutQuad).OnComplete(()=>
            {
                
                EffectManager.Instance.OnEffectEnd?.Invoke();
                if (!string.IsNullOrEmpty(_sceneName))
                    SceneManager.LoadSceneAsync(_sceneName);
            });
        // todo 최종스테이지로
    }
}