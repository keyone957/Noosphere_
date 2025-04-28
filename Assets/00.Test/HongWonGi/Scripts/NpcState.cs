using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NPCState
{
    Idle,
    IsTalking,
    IsWalking
}
public class NpcState : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    private Transform _player;
    private NPCState _currentState;
    private Quaternion _originalRotation; // 초기 회전값 저장

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _originalRotation = transform.rotation;
        SetState(NPCState.Idle);
    }

    public void SetState(NPCState newState)
    {
        _currentState = newState;
      
        switch(_currentState)
        {
            case NPCState.Idle:
                _animator.SetBool("IsTalking", false);
                // transform.rotation = _originalRotation; 
                break;
              
            case NPCState.IsTalking:
                _animator.SetBool("IsTalking", true);
                LookAtPlayer();
                break;
            case NPCState.IsWalking:
                _animator.SetBool("IsWalking",true);
                break;
        }
    }
    public NPCState GetCurrentState()
    {
        return _currentState;
    }
    
    private void LookAtPlayer()
    {
        if(_player != null)
        {
            Vector3 direction = _player.position - transform.position;
            direction.y = 0;
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}
