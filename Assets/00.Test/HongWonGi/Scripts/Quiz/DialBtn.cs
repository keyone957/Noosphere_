using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DialBtn :MonoBehaviour, IDragHandler, IPointerDownHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] private RectTransform _handle;
    [SerializeField] private RectTransform _centerPoint;
    [SerializeField] private Canvas _canvas;
    [SerializeField] private GameObject _dialBtnImg;
    [SerializeField] private string _soundInfo;
    private int _maxChips = 360; // 최대 값 (100%에 해당)
    private float _radius;
    private float _maxAngle = 360f; // 최대 각도
    private float _currentAngle = 0f;
    public Action<int> OnValueChanged;
    private void OnEnable()
    {
        _radius = Vector2.Distance(_handle.anchoredPosition, _centerPoint.anchoredPosition);
        UpdateHandlePosition(_maxAngle);
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        UpdateHandlePosition(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        UpdateHandlePosition(eventData);
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        SoundManager.Instance.StopAllSFX();
        SoundManager.Instance.PlaySFX(_soundInfo);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        SoundManager.Instance.StopAllSFX();
    }

    private void UpdateHandlePosition(PointerEventData eventData)
    {
        // 마우스의 화면 좌표를 캔버스의 로컬 좌표로 변환
        Vector2 localMousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_centerPoint, eventData.position, _canvas.worldCamera,
            out localMousePosition);

        // 중심점에서 마우스 위치 사이의 벡터를 계산
        Vector2 direction = (localMousePosition - (Vector2)_centerPoint.anchoredPosition).normalized;

        // 마우스 위치로부터 각도 계산 (0~360도)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360; // 각도를 0~360도로 조정


        _currentAngle = Mathf.Clamp(angle, 0, _maxAngle);
        // 핸들의 새로운 위치 계산: 중심점에서 방향 벡터에 반지름을 곱한 거리만큼 이동
        Vector2 newPosition = (Vector2)_centerPoint.anchoredPosition +
                              new Vector2(Mathf.Cos(_currentAngle * Mathf.Deg2Rad),
                                  Mathf.Sin(_currentAngle * Mathf.Deg2Rad)) * _radius;

        // 핸들의 위치를 업데이트
        _handle.anchoredPosition = newPosition;
        _dialBtnImg.transform.rotation = Quaternion.Euler(0, 0, _currentAngle);
        UpdateRadioNum();
    }

    private void UpdateHandlePosition(float angle)
    {
        // 초기 위치 설정 함수
        _currentAngle = angle;
        Vector2 newPosition = (Vector2)_centerPoint.anchoredPosition +
                              new Vector2(Mathf.Cos(_currentAngle * Mathf.Deg2Rad),
                                  Mathf.Sin(_currentAngle * Mathf.Deg2Rad)) * _radius;
        _handle.anchoredPosition = newPosition;
        _dialBtnImg.transform.rotation = Quaternion.Euler(0, 0, _currentAngle);
        UpdateRadioNum();
    }

    private void UpdateRadioNum()
    {
        float normalizedAngle = _currentAngle / 360f; 
        float roundedValue = Mathf.Round(normalizedAngle * 10)/10;
        float num = Mathf.Clamp(1 - roundedValue, 0, 0.9f)*10;
        int curNum= Mathf.RoundToInt(num);
        OnValueChanged?.Invoke(curNum);
    }
}