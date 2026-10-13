using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

public class IntroSkipBtn : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private PlayableDirector _playableDirector;
    [SerializeField] private double skipTime;
    [SerializeField] private Image _fadeImage;

    void Start()
    {
        _button.onClick.AddListener(skip);
    }

    private void skip()
    {
        _fadeImage.color = new Color(0, 0, 0, 1);
        _playableDirector.time = skipTime;
        _playableDirector.Evaluate();
        SoundManager.Instance.StopAllSFX();
        gameObject.SetActive(false);
    }
}