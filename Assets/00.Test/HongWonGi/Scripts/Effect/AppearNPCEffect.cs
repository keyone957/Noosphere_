using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.Universal;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
public class AppearNPCEffect : MonoBehaviour
{
  
    [SerializeField] private GameObject _npcObj;
    [Tooltip("npc 스킨렌더러")]
    [SerializeField] private List<SkinnedMeshRenderer> _skinRenderers;//몸,머리,안경 렌더러
    [Header("NPC Material")]
    [SerializeField] private Material _alphaMat;// Alpha가 설정된 머티리얼
    [SerializeField] private Material _originMat;// 돌아갈 원본 머티리얼
    [SerializeField] private float _fadeInDuration = 1f;
    private void OnEnable()
    {
        _npcObj.SetActive(true);
        // 알파값 초기화
        Color startColor = _alphaMat.GetColor("_BaseColor");
        startColor.a = 0f;
        _alphaMat.SetColor("_BaseColor", startColor);

        // 모든 렌더러에 알파 머티리얼 적용
        foreach (var renderer in _skinRenderers)
        {
            renderer.material = _alphaMat;
        }

        // 페이드 인 애니메이션
        Color targetColor = startColor;
        targetColor.a = 1f;

        _alphaMat.DOColor(targetColor, "_BaseColor", _fadeInDuration)
            .OnComplete(() => {
                foreach (var renderer in _skinRenderers)
                {
                    renderer.material = _originMat;
                }
                EffectManager.Instance.OnEffectEnd?.Invoke();
            });
    }

    private void OnDisable()
    {
        DOTween.Kill(_alphaMat);
    }
}
