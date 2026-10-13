using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cinemachine;

public class BrokeMirrorEffect : MonoBehaviour
{
    [SerializeField] private GameObject _brokeMirror;
    [SerializeField] private GameObject _mirror;
    [SerializeField] private GameObject _pointEffects;
    [SerializeField] private Animator _mirrorAnim;
    [SerializeField] private CinemachineVirtualCamera _vcam;
    private CinemachineBasicMultiChannelPerlin _perlin;

    private Vector3 _originalPos;
    private Quaternion _originalRot;
    private float _seed;
    private Coroutine _shakeCoroutine;

    void Awake()
    {
        _perlin = _vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        _perlin.m_FrequencyGain = 0f; 
    }

    private void OnEnable()
    {
        MirrorPuzzleManager.Instance.isMirrorBroke = true;
        _pointEffects.SetActive(true);
        if (DataManager.Instance._events["Event_B099"].isExecuted) return;
        SoundManager.Instance.PlaySFXNoEffect("Soundresource_054");
        _mirrorAnim.SetBool("IsBroke", true);
        _mirror.SetActive(false);
        _brokeMirror.SetActive(true);
        DOTween.To(
                () => _perlin.m_FrequencyGain,
                x => _perlin.m_FrequencyGain = x,
                1f,
                0.5f 
            )
            .SetEase(Ease.Linear)
            .SetLoops(2, LoopType.Yoyo);
    }
    public void OnAnimEnd()
    {
        // EffectManager.Instance.OnEffectEnd?.Invoke();
    }
}