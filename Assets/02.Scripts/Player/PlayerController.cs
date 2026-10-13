using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;
using FMODUnity;
using Debug = NooSphere.Debug;
// 플레이어 이동관련 함수
//플레이어 이동방식 변경
// 최초 작성자: 홍원기
// 수정자: 홍원기
// 최종 수정일: 2024-11-01
public class PlayerController : Singleton<PlayerController>
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
    [SerializeField] private EventReference _waterFootstepEvent;
    [SerializeField] private float stepInterval = 0.5f;
    [SerializeField] private float runInterval = 0.2f;
    private bool _isWater;
    private Vector3 _moveDirection;
    private Animator _animator;
    private float _defaultSpeed;
    private bool _timelineStarted = false; // 타임라인 시작시 canmove 함수 못바꾸게
    public bool IsTimelineLocked
    {
        get => _timelineStarted;
        set
        {
            _timelineStarted = value;
            if (_timelineStarted)
            {
                _canMove = false;
            }
        }
    }

    private bool _canMove = false;
    public bool canMove
    {
        get => _canMove;
        set
        {
            if (_timelineStarted && value)
            {
                _canMove = false;
                return;
            }

            if (_canMove == value) return;
            _canMove = value;
        }
    }
    public bool blockLeftRight=false;//좌우 input 막기
    public GameObject _currentNPC;
    public GameObject _swapNpc;
    private float lastStepTime = 0f;

    public GameObject npcCam;
    public NpcState npcState;

    private Vector3 _lastPos;
    private void Start()
    {
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
        _defaultSpeed = _moveSpeed;
        InputManager.Instance.moveAction += HandleInput;
        _lastPos = transform.position;
        if (_rigidbody != null)
        {
            _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            _rigidbody.useGravity = true;
        }
        if (_mainCamera == null)
            _mainCamera = Camera.main;
        
        // ui canvas 할당
        _uiCanvas = InventoryManager.Instance.transform.GetComponentInParent<Canvas>();
        if (SceneManager.GetActiveScene().name== "FinalStage_Spirit")
        {
            _isWater = true;
        }
        else
        {
            _isWater = false;
        }
    }

    private void Update()
    {
        _animator.SetFloat("MoveSpeed", _rigidbody.velocity.magnitude); 
    }
    private void FixedUpdate()
    {
        InputManager.Instance.FixedUpdate();
    }
    public void NpcCameraOn()
    {
        npcState.SetState(NPCState.IsTalking);
        _dialogueCamera.transform.gameObject.SetActive(true);
        _dialogueCamera.Follow = npcCam.transform;
    }

    public void ResetCamera()
    {
        if(npcState==null) return;
        npcState.SetState(NPCState.Idle);
        _dialogueCamera.transform.gameObject.SetActive(false);
    } 
  private void HandleInput()
    {
        if (!canMove)
        {
            _rigidbody.velocity = Vector3.zero;
            _animator.SetFloat("MoveSpeed", 0f);
            return;
        }
        
        float moveX = 0f;
        float moveY = 0f;

        if (Input.GetKey(KeyCode.W)) moveY = 1f;
        if (Input.GetKey(KeyCode.S)) moveY = -1f;
        if (Input.GetKey(KeyCode.A) && !blockLeftRight) moveX = -1f;
        if (Input.GetKey(KeyCode.D) && !blockLeftRight) moveX = 1f;

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

            float currentSpeed = _defaultSpeed;
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                currentSpeed *= _sprintMultiplier;
            }

            Move(_moveDirection, currentSpeed);
            
            // 발자국 소리 처리
            bool isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            float currentInterval = isRunning ? runInterval : stepInterval;
            
            float movedDistance = Vector3.Distance(transform.position, _lastPos);
            if (movedDistance > 0.01f && Time.time >= lastStepTime + currentInterval)
            {
                PlayFootstepSound(isRunning);
                lastStepTime = Time.time;
            }

            if (PlayerInteract.Instance.curTrigger != null && PlayerInteract.Instance.isInsideTrigger && PlayerInteract.Instance.canInteract)
            {
                PlayerInteract.Instance.curTrigger.GetComponent<EventTrigger>().OnTriggerEnter(GetComponent<CapsuleCollider>());
            }
        }
        else
        {
            Vector3 currentVelocity = _rigidbody.velocity;
            currentVelocity.x = 0f;
            currentVelocity.z = 0f;
            _rigidbody.velocity = currentVelocity;
        }
        
        _lastPos = transform.position; 
    }

    private void PlayFootstepSound(bool isRunning)
    {
        // 물속일 때는 FMOD 사용
        if (_isWater)
        {
            FMOD.Studio.EventInstance footstepInstance = RuntimeManager.CreateInstance(_waterFootstepEvent);
    
            FMOD.ATTRIBUTES_3D attributes = RuntimeUtils.To3DAttributes(transform.position);
    
            footstepInstance.set3DAttributes(attributes);
            
            footstepInstance.setVolume(SoundManager.Instance.sfxVolume);
    
            footstepInstance.start();
            footstepInstance.release();
        }
        else
        {
            // 일반 상태일 때는 AudioClip 사용
            List<AudioClip> currentSounds = isRunning ? runSounds : _walkSounds;
            int randomIndex = UnityEngine.Random.Range(0, currentSounds.Count);
            footstepSource.clip = currentSounds[randomIndex];
            footstepSource.volume = SoundManager.Instance.sfxVolume;
            footstepSource.Play();
        }
    }

    private void Move(Vector3 direction, float speed)
    {
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
            Vector3 targetVelocity = direction * speed;
            targetVelocity.y = _rigidbody.velocity.y;
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
    
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AssignMainCamera();
        if (scene.name == "FinalStage_Spirit")
        {
            _isWater = true;
        }
        else
        {
            _isWater = false;
        }
    }

    public void AssignMainCamera()
    {
        _mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        if (_mainCamera != null)
        {
            Debug.Log($"Main camera assigned: {_mainCamera.name}");
        }
        else
        {
            Debug.LogWarning("No main camera found in the current scene.");
        }

        if (_uiCanvas == null)
        {
            Debug.Log("UI 찾기1 :  " + UIManager.Instance.gameObject.name);
            Debug.Log("UI 찾기2 :  " + FindAnyObjectByType<HereIsUICanvas>().gameObject.name);
            _uiCanvas = UIManager.Instance.transform.GetChild(0).GetComponent<Canvas>();
            if (_uiCanvas.renderMode == RenderMode.ScreenSpaceCamera) _uiCanvas.worldCamera = _mainCamera;
        }
        else
        {
            if (_uiCanvas.renderMode == RenderMode.ScreenSpaceCamera) _uiCanvas.worldCamera = _mainCamera;
        }

        GameObject parent = GameObject.Find("-----[Cameras]");
        foreach (Transform child in parent.transform)
        {
            if (child.name == "Dialogue Camera") 
            {
                _dialogueCamera = child.GetComponent<CinemachineVirtualCamera>();
            }
            else
            {
                if (child.GetComponent<CinemachineVirtualCamera>() != null)
                {
                    CinemachineVirtualCamera abcCamera = child.GetComponent<CinemachineVirtualCamera>();
                    abcCamera.LookAt = transform;
                    Debug.Log($"Set {abcCamera.name} LookAt to player");
                }
            }
        }
    }

    public void SetDialogueCam(Cinemachine.CinemachineVirtualCamera cam)
    {
        _dialogueCamera = cam;
    }
}