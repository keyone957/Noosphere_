using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;

// 플레이어 이동관련 함수
//플레이어 이동방식 변경
// 최초 작성자: 홍원기
// 수정자: 홍원기
// 최종 수정일: 2024-11-01
public class PlayerControllerTest : MonoBehaviour
{
    [Header("플레이어 설정")] [SerializeField] private float _moveSpeed;
    [SerializeField] private float _sprintMultiplier = 2f;
    private Rigidbody _rigidbody;
    public Camera _mainCamera;
    public Canvas _uiCanvas;
    [SerializeField] private CinemachineVirtualCamera _dialogueCamera;
    [Header("플레이어 사운드")] 
    [SerializeField] private List<AudioClip> _walkSounds;
    [SerializeField] private List<AudioClip> runSounds;
    [SerializeField] private List<AudioClip> _waterWalkSounds;
    [SerializeField] private AudioSource footstepSource;
    [SerializeField] private float stepInterval = 0.5f;
    [SerializeField] private float runInterval = 0.2f;
    private bool _isWater;
    private Vector3 _moveDirection;
    private Animator _animator;
    private float _defaultSpeed;
    public bool canMove = false; //대화시작
    // public bool isNpcRayOn=false;
    public GameObject _currentNPC;
    public GameObject _swapNpc;
    private float lastStepTime = 0f;

    public GameObject npcCam;
    public NpcState npcState;
    
    
    private void Start()
    {
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
        _defaultSpeed = _moveSpeed;
        InputManager.Instance.moveAction += HandleInput;
        if (_rigidbody != null)
        {
            _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            _rigidbody.useGravity = true;
        }
        if (_mainCamera == null)
            _mainCamera = Camera.main;
    }

    private void Update()
    {
        _animator.SetFloat("MoveSpeed", _rigidbody.velocity.magnitude); 
    }
    private void FixedUpdate()
    {
        InputManager.Instance.FixedUpdate();
    }
    private void HandleInput()
    {
        if (!canMove)
        {
            _rigidbody.velocity = Vector3.zero; //velocity
            _animator.SetFloat("MoveSpeed", 0f);
            return;
        }
        float moveX = 0f;
        float moveY = 0f;

        // WASD 키 입력 처리
        if (Input.GetKey(KeyCode.W)) moveY = 1f;
        if (Input.GetKey(KeyCode.S)) moveY = -1f;
        if (Input.GetKey(KeyCode.A)) moveX = -1f;
        if (Input.GetKey(KeyCode.D)) moveX = 1f;

        Vector3 inputDirection = new Vector3(moveX, 0f, moveY).normalized;
        
        if (inputDirection != Vector3.zero)
        {
            Vector3 cameraForward = _mainCamera.transform.forward;
            cameraForward.y = 0;
            cameraForward.Normalize();

            Vector3 cameraRight = _mainCamera.transform.right;
            cameraRight.y = 0;
            cameraRight.Normalize();

            _moveDirection = (cameraForward * inputDirection.z + cameraRight * inputDirection.x).normalized;

            // 이동 속도 설정
            float currentSpeed = _defaultSpeed;
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                currentSpeed *= _sprintMultiplier;
            }

            Move(_moveDirection, currentSpeed);
            
            // 발소리 처리
            bool isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            float currentInterval = isRunning ? runInterval : stepInterval;
            List<AudioClip> currentSounds;
            if (_isWater)
            {
                currentSounds = _waterWalkSounds;
            }
            else
            {
                currentSounds = isRunning ? runSounds : _walkSounds;
            }

            if (Time.time >= lastStepTime + currentInterval)
            {
                int randomIndex = UnityEngine.Random.Range(0, currentSounds.Count);
                footstepSource.clip = currentSounds[randomIndex];
                footstepSource.Play();
                lastStepTime = Time.time;
            }
        }
        else
        {
            Vector3 currentVelocity = _rigidbody.velocity;
            currentVelocity.x = 0f;
            currentVelocity.z = 0f;
            _rigidbody.velocity = currentVelocity;
        }
    }

    private void Move(Vector3 direction,float speed)
    {
      
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);

            // velocity를 사용한 이동
            Vector3 targetVelocity = direction * speed;
            targetVelocity.y = _rigidbody.velocity.y; // 현재 수직 속도 유지
            _rigidbody.velocity = targetVelocity;
        }
    }

    private void OnDestroy()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.moveAction -= HandleInput;
        }
    }
}