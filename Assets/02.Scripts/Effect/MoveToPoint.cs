using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MoveToPoint : MonoBehaviour
{
    [SerializeField] private Transform _point;
    [SerializeField] private float _walkDuration;
    [SerializeField] private float _rotate;
    [SerializeField] private bool _isRotate;
    private Animator _playerAnim;
    private GameObject _player;

    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _playerAnim = _player.GetComponent<Animator>();
    }

    private void OnEnable()
    {
        PlayerController.Instance.canMove = false;
        InventoryManager.Instance.canOpenInventory = true;
        UIManager.Instance.LockPlayer();
        UIManager.Instance.inventoryIcon.SetActive(false);
        _playerAnim.SetBool("IsWalk", true);
        if (_isRotate)
        {
            MoveWithRotate();
        }
        else
        {
            NormalMove();
        }

    }

    private void NormalMove()
    {
        _player.transform.LookAt(_point.position);
        _player.transform.DOMove(_point.transform.position, _walkDuration).OnComplete(() =>
        {
            PlayerController.Instance.canMove = true;
            _playerAnim.SetBool("IsWalk", false);
            EffectManager.Instance?.OnEffectEnd.Invoke();
            gameObject.SetActive(false);
        });
    }

    private void MoveWithRotate()
    {
        Sequence seq = DOTween.Sequence();

        seq.Join(_player.transform.DOMove(_point.position, _walkDuration).SetEase(Ease.Linear));

        seq.Join(_player.transform.DORotate(new Vector3(0, _rotate, 0), _walkDuration).SetEase(Ease.Linear));

        seq.OnComplete(() =>
        {
            PlayerController.Instance.canMove = true;
            _playerAnim.SetBool("IsWalk", false);
            EffectManager.Instance?.OnEffectEnd.Invoke();
            gameObject.SetActive(false);
        });
    }
}