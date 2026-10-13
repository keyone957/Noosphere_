using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class PointEffect : MonoBehaviour
{
    [SerializeField] private float _frontAngle = 40f;
    [SerializeField] private EventTrigger _researchTrigger;
    [SerializeField] private Transform _researcher;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInteract.Instance.gameObject.transform.LookAt(_researcher);
            PlayerInteract.Instance.isInsideTrigger = true;
            PlayerInteract.Instance.curTrigger = _researchTrigger;
            
            EffectManager.Instance.OnEffectEnd?.Invoke();
            EventManager.Instance.ExecuteEvent(EventManager.Instance.nextEventID).Forget();
            gameObject.SetActive(false);
        }
    }
    
    // 플레이어가 트리거를 정면 방향으로 진입했는지 체크하는 함수
    bool IsPlayerFront(Transform player)
    {
        Vector3 triggerDirection = (transform.GetChild(0).position - player.position).normalized;
        float angle = Vector3.Angle(player.forward, triggerDirection);
        
        if (angle <= _frontAngle)
        {
            Debug.Log("플레이어가 정면을 바라보고 들어 옴.");
            return true;
        }
        Debug.Log("플레이어가 뒤돌거나 옆을 보고 들어오지 않음");
        return false;
    }
}
