using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cinemachine;

public class ShakeCamera : MonoBehaviour
{
    [SerializeField] 
    private CinemachineVirtualCamera _vcam;
    private CinemachineBasicMultiChannelPerlin _perlin;
    private void Awake()
    {
        _perlin = _vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public void ShakeCam()
    {
        DOTween.Kill(_perlin);
        
        // 시퀀스 생성
        Sequence seq = DOTween.Sequence();
        seq.Append(
            DOTween.To(
                () => _perlin.m_FrequencyGain,
                x => _perlin.m_FrequencyGain = x,
                1f,
                5f // 0→1
            ).SetEase(Ease.Linear)
        );
        seq.Append(
            DOTween.To(
                () => _perlin.m_FrequencyGain,
                x => _perlin.m_FrequencyGain = x,
                2f,
                5f // 1→2
            ).SetEase(Ease.Linear).From(1f)
        );
        seq.Append(
            DOTween.To(
                () => _perlin.m_FrequencyGain,
                x => _perlin.m_FrequencyGain = x,
                0f,
                5f // 2→0
            ).SetEase(Ease.Linear).From(2f)
        );
        seq.Play();
        
    }
}
