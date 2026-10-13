using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;
using DG.Tweening;
using Cinemachine;
public class SirenEffect : MonoBehaviour
{
    [Header("사이렌 효과")] 
    [SerializeField] private Light _sirenLight;

    [Tooltip("최저 intensity (여기서는 1)")] 
    [SerializeField] private float minIntensity;//최소 빛 밝기
    [SerializeField] private float maxIntensity;//불 밝기 
    [SerializeField] private float upDuration;//깜빡거림 시간
    Tween _sirenTween;
    [SerializeField] private CinemachineVirtualCamera _vcam;
    private CinemachineBasicMultiChannelPerlin _perlin;
    private void Awake()
    {
        _perlin = _vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    private void OnEnable()
    {
        StartSiren();
        // DOTween.To(
        //         () => _perlin.m_FrequencyGain,
        //         x => _perlin.m_FrequencyGain = x,
        //         1f,
        //         2f 
        //     )
        //     .SetEase(Ease.Linear)
        //     .SetLoops(2, LoopType.Yoyo);
         DOTween.Kill(_perlin);
        
            // 시퀀스 생성
            Sequence seq = DOTween.Sequence();
            seq.Append(
                DOTween.To(
                    () => _perlin.m_FrequencyGain,
                    x => _perlin.m_FrequencyGain = x,
                    1f,
                    2f // 0→1
                ).SetEase(Ease.Linear)
            );
            seq.Append(
                DOTween.To(
                    () => _perlin.m_FrequencyGain,
                    x => _perlin.m_FrequencyGain = x,
                    2f,
                    2f // 1→2
                ).SetEase(Ease.Linear).From(1f)
            );
            seq.Append(
                DOTween.To(
                    () => _perlin.m_FrequencyGain,
                    x => _perlin.m_FrequencyGain = x,
                    0f,
                    2f // 2→0
                ).SetEase(Ease.Linear).From(2f)
            );
            seq.Play();
        
    }
    private void StartSiren()
    {
        _sirenLight.intensity = minIntensity;
        _sirenTween = _sirenLight
            .DOIntensity(maxIntensity, upDuration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.Linear);
    }
    public void StopBlink()
    {
        _sirenTween.Kill();
    }
    
 
}