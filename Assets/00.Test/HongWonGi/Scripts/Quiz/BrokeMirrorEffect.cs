using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokeMirrorEffect : MonoBehaviour
{
    [SerializeField] private GameObject _brokeMirror;
    [SerializeField] private GameObject _mirror;
    [SerializeField] private GameObject _pointEffects;
    [SerializeField] private Animator _mirrorAnim;
    [SerializeField] private Camera _mainCamera;
    [Header("카메라 흔들림 효과 관련")] 
    [SerializeField] private float _decreaseShakeTiming;//흔들림 감소 속도
    [SerializeField] private float _maxAngle;//카메라 회전각도
    [SerializeField] private float _maxOffset;//카메라 이동거리
   

    private Vector3 _originalPos;
    private Quaternion _originalRot;
    private float _seed;
    private Coroutine _shakeCoroutine;

    void Awake()
    {
        if (_mainCamera != null)
        {
            _originalPos = _mainCamera.transform.localPosition;
            _originalRot = _mainCamera.transform.localRotation;
            _seed = UnityEngine.Random.value * 100f;
        }
    }

    private void OnEnable()
    {
        MirrorPuzzleManager.Instance.isMirrorBroke = true;
        _mirrorAnim.SetBool("IsBroke", true);
        _pointEffects.SetActive(true);
        _mirror.SetActive(false);
        _brokeMirror.SetActive(true);
        StartShake(0.6f);
    }

    public void StartShake(float trauma)
    {
        if (_shakeCoroutine != null)
        {
            StopCoroutine(_shakeCoroutine);
        }

        _shakeCoroutine = StartCoroutine(ShakeCoroutine(trauma));
    }

    private IEnumerator ShakeCoroutine(float trauma)
    {
        float currentTrauma = trauma;

        while (currentTrauma > 0 && _mainCamera != null)
        {
            float shake = currentTrauma * currentTrauma;

            float offsetX = _maxOffset * shake * (Mathf.PerlinNoise(_seed, Time.time * 10f) * 2f - 1f);
            float offsetY = _maxOffset * shake * (Mathf.PerlinNoise(_seed + 1f, Time.time * 10f) * 2f - 1f);
            float rotation = _maxAngle * shake * (Mathf.PerlinNoise(_seed + 2f, Time.time * 10f) * 2f - 1f);

            _mainCamera.transform.localPosition = _originalPos + new Vector3(offsetX, offsetY, 0f);
            _mainCamera.transform.localRotation = _originalRot * Quaternion.Euler(0f, 0f, rotation);

            currentTrauma = Mathf.Clamp01(currentTrauma - _decreaseShakeTiming * Time.deltaTime);
            yield return null;
        }

        // 카메라 위치 리셋
        if (_mainCamera != null)
        {
            _mainCamera.transform.localPosition = _originalPos;
            _mainCamera.transform.localRotation = _originalRot;
        }
    }

    private void OnDisable()
    {
        if (_shakeCoroutine != null)
        {
            StopCoroutine(_shakeCoroutine);
            if (_mainCamera != null)
            {
                _mainCamera.transform.localPosition = _originalPos;
                _mainCamera.transform.localRotation = _originalRot;
            }
        }
    }

    public void OnAnimEnd()
    {
        EffectManager.Instance.OnEffectEnd?.Invoke();
    }
}