using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using NooSphere;
using Debug = NooSphere.Debug;  
public enum ClockHandType
{
    Hour,   // 시침
    Minute  // 분침
}
public class ClockHandBtn :  MonoBehaviour, IDragHandler,IEndDragHandler, IBeginDragHandler
{
   [Header("Clock Hand Settings")]
    [SerializeField] private ClockHandType handType;
    
    private RectTransform clockHand;
    [SerializeField]private float _degreePerUnit;//스냅용 각도
    [SerializeField]private int _maxUnits;//

    private void OnEnable()
    {
        clockHand.rotation=Quaternion.Euler(0, 0, 0);
    }

    private void Awake()
    {
        clockHand = GetComponent<RectTransform>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 direction = eventData.position - (Vector2)clockHand.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        clockHand.rotation = Quaternion.Euler(0, 0, angle);
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        SoundManager.Instance.StopAllSFX();
        if (handType == ClockHandType.Hour)
        {
            SoundManager.Instance.PlayLoopingSound("Soundresource_093");
        }
        else
        {
            SoundManager.Instance.PlayLoopingSound("Soundresource_094");
        }
        
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        float currentAngle = clockHand.rotation.eulerAngles.z;
        float normalizedAngle = (360 - currentAngle) % 360;
        
        int currentUnit = Mathf.RoundToInt(normalizedAngle / _degreePerUnit) % 12;
        SoundManager.Instance.StopAllSFX();
        // 시침이나 분침이 0일 때의 처리
        if (currentUnit == 0)
        {
            if (handType == ClockHandType.Hour)
                currentUnit = 12;
            else
                currentUnit = 0; //분침 0분
        }
        
        float snapAngle = (360 - (currentUnit * _degreePerUnit)) % 360;
        clockHand.rotation = Quaternion.Euler(0, 0, snapAngle);
        
        // 현재 값 로그 출력
        if (handType == ClockHandType.Hour)
        {
            Debug.Log($"현재 시간: {currentUnit}시");
            ClockManager.Instance._hour = currentUnit;
        }
        else
        {
            Debug.Log($"현재 분: {currentUnit * 5}분");
            ClockManager.Instance._minute = currentUnit * 5;
        }
        ClockManager.Instance.CheckAnswerWithDelay().Forget();
    }
    
}
