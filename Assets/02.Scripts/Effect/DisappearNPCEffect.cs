using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DisappearNPCEffect : MonoBehaviour
{
    [SerializeField] private GameObject _npcObj;
    [Tooltip("npc 스킨렌더러")]
    [SerializeField] private List<SkinnedMeshRenderer> _skinRenderers;

    [Header("NPC Material")]
    [SerializeField] private Material _alphaMat;
    [SerializeField] private Material _originMat;
    [SerializeField] private float _fadeOutDuration = 1f;

    private void OnEnable()
    {
        DOTween.Kill(_alphaMat);

        foreach (var renderer in _skinRenderers)
            renderer.material = _alphaMat;
        
        Color startColor = _alphaMat.GetColor("_BaseColor");
        startColor.a = 1f;
        _alphaMat.SetColor("_BaseColor", startColor);

        Color targetColor = startColor;
        targetColor.a = 0f;
        
        _alphaMat.DOColor(targetColor, "_BaseColor", _fadeOutDuration)
            .OnComplete(() =>
            {
                _npcObj.SetActive(false);

                Color resetColor = _alphaMat.GetColor("_BaseColor");
                resetColor.a = 1f;
                _alphaMat.SetColor("_BaseColor", resetColor);

                var player = GameObject.FindWithTag("Player");
                if (player != null)
                {
                    int playerLayer = LayerMask.NameToLayer("Player");
                    SetLayerRecursively(player, playerLayer);
                }
                EffectManager.Instance.OnEffectEnd?.Invoke();
            });
    }

    private void OnDisable()
    {
        DOTween.Kill(_alphaMat);
    }

    void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
}
