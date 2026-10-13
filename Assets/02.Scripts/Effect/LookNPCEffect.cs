using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LookNPCEffect : MonoBehaviour
{
    [SerializeField] private Transform _targetObj;
    private GameObject _player;
    private void OnEnable()
    {
        _player = FindObjectOfType<PlayerController>().gameObject;
        PlayerController.Instance.canMove = false;
        _player.transform.DOLookAt(_targetObj.position, 1f).OnComplete(() =>
        {
            PlayerController.Instance.canMove = true;
            EffectManager.Instance.OnEffectEnd?.Invoke();
        });
    }
}
