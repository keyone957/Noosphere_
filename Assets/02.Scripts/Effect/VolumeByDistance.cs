using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Cysharp.Threading.Tasks;
using Debug = NooSphere.Debug;

public class VolumeByDistance : MonoBehaviour
{
    [Header("시작점과 도착점 설정")]
    [SerializeField] private Transform _startPoint;
    [SerializeField] private Transform _middlePoint;
    [SerializeField] private Transform _endPoint;
    [SerializeField] private Transform _playerTrans;

    [SerializeField] private Volume _volume;

    [SerializeField] private UniversalAdditionalCameraData _mainCam;
    [SerializeField] private GameObject _overlayCam;
    private Vignette _vignette;
    private bool _isTriggerEnd;
    private void OnEnable()
    {
        InitCamSetting();
    }

    private void InitCamSetting()
    {
        if (!_volume.profile.TryGet<Vignette>(out _vignette))
        {
            Debug.Log("volume효과 존재 x");
        }
        else
        {
            Debug.Log("volume효과 존재");
        }
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        _playerTrans = player.transform;
        _mainCam.renderPostProcessing = true;
        _overlayCam.SetActive(true);
        _isTriggerEnd = false;
    }

    void Update()
    {
        OnDark();
    }

    private void OnDark()
    {
        // 시작점과 도착점 사이의 전체 거리
        float startToMiddle = Vector3.Distance(_startPoint.position, _middlePoint.position);
        float middleToEnd = Vector3.Distance(_middlePoint.position, _endPoint.position);
        if (startToMiddle == 0f) return;

        // 대상이 시작점으로부터 떨어진 거리
        float currentDistance = Vector3.Distance(_startPoint.position, _playerTrans.position);

        if (currentDistance <= startToMiddle)
        {
            Vector3 startToMiddleDir = (_middlePoint.position - _startPoint.position).normalized;
            Vector3 startToPlayer = _playerTrans.position - _startPoint.position;
            float projection = Vector3.Dot(startToPlayer, startToMiddleDir); // 투영 길이 (음수 가능)
            // float t = Mathf.Clamp01(currentDistance / startToMiddle); // 0~1
            float t = Mathf.Clamp01(projection / startToMiddle);
            _volume.weight = t;

            // 비네트 색상도 점점 어둡게
            if (_vignette != null)
            {
                Color originalColor = _vignette.color.value;
                float h, s, v;
                Color.RGBToHSV(originalColor, out h, out s, out v);
                float newV = Mathf.Lerp(1f, 0f, t);
                Color newColor = Color.HSVToRGB(h, s, newV);
                newColor.a = originalColor.a;
                _vignette.color.value = newColor;
            }
        }
        // 2. middle ~ end: 완전 어둠 유지
        else
        {
            _volume.weight = 1f;
            if (_vignette != null)
            {
                Color originalColor = _vignette.color.value;
                float h, s, v;
                Color.RGBToHSV(originalColor, out h, out s, out v);
                Color newColor = Color.HSVToRGB(h, s, 0f);
                newColor.a = originalColor.a;
                _vignette.color.value = newColor;
            }
        }

        // 3. end 도달 체크
        float totalDistanceToEndCheck = Vector3.Distance(_startPoint.position, _endPoint.position);
        float endT = Mathf.Clamp01(currentDistance / totalDistanceToEndCheck);

        if (endT >= 1 && !_isTriggerEnd)
        {
            _isTriggerEnd = true;
            PlayerController.Instance.blockLeftRight = false;
            Debug.Log("끝에 도달");
            EffectManager.Instance.OnEffectEnd?.Invoke();
            EventManager.Instance.ExecuteEvent(EventManager.Instance.nextEventID).Forget();
        }
    }
}