using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using Cinemachine;
using TMPro;
using UnityEngine.Localization.Settings;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Steamworks;
using Debug = NooSphere.Debug;

public class TimeLineSignalManager : MonoBehaviour
{
    [Header("npc및 캐릭터 애니메이션 관련")] [SerializeField]
    private Animator _npc;

    [SerializeField] private Animator _friendNpc;
    private Animator _player;
    private SkinnedMeshRenderer _playerSkin;
    [SerializeField] private List<Material> _playerMaterial;
    [SerializeField] private SkinnedMeshRenderer _freindMaterial;
    [SerializeField] private Material _friendMaterial;
    [SerializeField] private List<string> footSteps;
    [SerializeField] CinemachineVirtualCamera _farLoungCam;
    [Header("정신세계 이펙트")] [SerializeField] private RawImage _vhsImage;
    [SerializeField] private Volume _vhsVolume;
    [SerializeField] private GameObject _vhsObj;
    [SerializeField] private float _duration = 5f;
    private Camera _mainCamera;
    private UnityEngine.Rendering.Universal.UniversalAdditionalCameraData _cameraData;
    [Header("문틈 대사 관련")] [SerializeField] private TextMeshProUGUI _realText;
    private float _fadeDuration = 0.3f;
    private float _displayDuration = 0.8f;
    private Dictionary<string, Animator> _animCacheDic = new Dictionary<string, Animator>();

    private void Start()
    {
        //_player = GameObject.Find("Player").GetComponent<Animator>();
        _mainCamera = Camera.main;
        if (_mainCamera != null)
        {
            _cameraData = _mainCamera.GetComponent<UnityEngine.Rendering.Universal.UniversalAdditionalCameraData>();
        }

        if (_fadeImage != null)
        {
            bool showFadeImage = PlayerPrefs.GetInt("FadeImage", 0) == 0;
            _fadeImage.gameObject.transform.parent.gameObject.SetActive(showFadeImage);
            
        }
        // DataManager.Instance.InitializeData().Forget();--> 테스트용
    }

    #region 애니메이션 관련

    public void StartPlayerAnim(string playerAnim)
    {
        _player = GameObject.FindWithTag("Player").GetComponent<Animator>();
        _player.SetBool(playerAnim, true);
    }

    public void EndPlayerAnim(string playerAnim)
    {
        _player = GameObject.FindWithTag("Player").GetComponent<Animator>();
        _player.SetBool(playerAnim, false);
    }

    public void PlayerAnimTrigger(string playerAnim)
    {
        _player = GameObject.FindWithTag("Player").GetComponent<Animator>();
        _player.SetTrigger(playerAnim);
    }

    public void StartNpcAnim(string npcAnim)
    {
        _npc.SetBool(npcAnim, true);
    }

    public void EndNpcAnim(string npcAnim)
    {
        _npc.SetBool(npcAnim, false);
    }

    public void StartFriendNpcAnim(string npcAnim)
    {
        _friendNpc.SetBool(npcAnim, true);
        _freindMaterial.material = _friendMaterial;
    }

    public void EndFriendNpcAnim(string npcAnim)
    {
        _friendNpc.SetBool(npcAnim, false);
    }

    private Animator GetAnimator(string npcObjectName)
    {
        if (!_animCacheDic.ContainsKey(npcObjectName))
        {
            Animator anim = GameObject.Find(npcObjectName)?.GetComponent<Animator>();
            _animCacheDic[npcObjectName] = anim;
        }

        return _animCacheDic[npcObjectName];
    }

    public void StartAnim(string command)
    {
        string objectName = command.Split(':')[0];
        string animName = command.Split(':')[1];
        Animator anim = GetAnimator(objectName);
        anim.SetBool(animName, true);
    }

    public void EndAnim(string command)
    {
        string objectName = command.Split(':')[0];
        string animName = command.Split(':')[1];
        Animator anim = GetAnimator(objectName);
        anim.SetBool(animName, false);
    }

    #endregion


    public void SetPlayerMaterial(Material material)
    {
        _playerSkin = GameObject.Find("Character_Main_Body").GetComponent<SkinnedMeshRenderer>();
        _playerSkin.material = material;
    }

    public void SetNpcMaterial(Material material)
    {
        _freindMaterial.material = material;
    }

    private async UniTaskVoid StartAutoEffect()
    {
        _vhsObj.SetActive(true);
        _cameraData.renderPostProcessing = true;

        float elapsedTime = 0f;
        SoundManager.Instance.PlaySFXNoEffect("Soundresource_029");
        // 5초동안 게이지 증가
        while (elapsedTime < _duration)
        {
            elapsedTime += Time.deltaTime;
            float value = elapsedTime / _duration;

            Color color = _vhsImage.color;
            color.a = value;
            _vhsImage.color = color;
            _vhsVolume.weight = value;

            await UniTask.Yield();
        }

        SoundManager.Instance.PlaySFXNoEffect("Soundresource_028");
        DontDestroyOnLoad(_player.gameObject);
    }

    public void OnStartMentalProcess()
    {
        StartAutoEffect().Forget();
    }

    public void OnLoadScene(string SceneName)
    {
        if (_player == null)
        {
            _player = GameObject.Find("Player").GetComponent<Animator>();
        }

        //돌아오는 정보 저장
        SceneTracker.previousSceneName = SceneManager.GetActiveScene().name;
        _player.gameObject.GetComponent<PlayerInteract>().isInMental = true;
        _player.gameObject.GetComponent<MentalEnterProcess>().SetCombackEventId("Event_C075");
        SceneManager.LoadSceneAsync(SceneName);
    }

    #region 사운드 관련

    public void StartVfx(string SoundResource)
    {
        SoundManager.Instance.PlaySFXNoEffect(SoundResource);
    }

    public void EndVfx()
    {
        SoundManager.Instance.StopAllSFX();
    }

    private bool isPlayingFootsteps = false;

    private async UniTask PlayRandomFootSteps(float duration)
    {
        if (footSteps == null || footSteps.Count == 0) return;

        isPlayingFootsteps = true;
        float elapsedTime = 0f;
        float interval = 0.4f;

        while (elapsedTime < duration && isPlayingFootsteps)
        {
            int randomIndex = Random.Range(0, footSteps.Count);
            SoundManager.Instance.PlaySFXNoEffect(footSteps[randomIndex]);

            await UniTask.Delay(System.TimeSpan.FromSeconds(interval));
            elapsedTime += interval;
        }

        isPlayingFootsteps = false;
    }

    public void StartRandomSound(float duration)
    {
        PlayRandomFootSteps(duration).Forget();
    }

    public void StartBgm(string soundResource)
    {
        StartBgmAsync(soundResource).Forget();
    }

    private async UniTaskVoid StartBgmAsync(string soundResource)
    {
        SoundManager.Instance.StopBGM(1.5f);
        await UniTask.Delay(TimeSpan.FromSeconds(1.6f)); // 페이드아웃 대기
        SoundManager.Instance.PlayBGM(soundResource);
    }

    private async UniTaskVoid StopBgm()
    {
        SoundManager.Instance.StopBGM(1.5f);
        await UniTask.Delay(TimeSpan.FromSeconds(1.6f));
    }

    public void StopBGM()
    {
        StopBgm().Forget();
    }

    #endregion


    public void StopFootSteps()
    {
        isPlayingFootsteps = true;
    }

    public void CameraON()
    {
        _farLoungCam.Priority = 12;
    }

    public void StartDialogueOnWorld(string ID)
    {
        ShowRealDialogue(ID).Forget();
    }

    private async UniTask ShowRealDialogue(string dialogueID)
    {
        DialogueStructure doorDialogue = DataManager.Instance._dialogue[dialogueID];
        DataManager.Instance._lockConditions["Lock_condition_003"].Lock();
        _realText.gameObject.SetActive(true);
        for (int i = 0; i < doorDialogue.Dialogue_Text_List.Count; i++)
        {
            DataManager.Instance._lockConditions["Lock_condition_003"].Lock();
            string text = LocalizationSettings.StringDatabase.GetLocalizedString(LocalConstants.DialogueTable,
                doorDialogue.Dialogue_Text_List[i].text, LocalizationSettings.SelectedLocale);
            string processedText = text.Replace("\\n", "<br>");
            _realText.text = $"<mark=#00000055>{processedText}</mark>";

            await _realText.DOFade(1f, _fadeDuration).AsyncWaitForCompletion();

            await UniTask.Delay(TimeSpan.FromSeconds(_displayDuration));

            await _realText.DOFade(0f, _fadeDuration).AsyncWaitForCompletion();

            if (i < doorDialogue.Dialogue_Text_List.Count - 1)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(_fadeDuration));
            }
        }

        _realText.gameObject.SetActive(false);
        DataManager.Instance._lockConditions["Lock_condition_003"].UnLock();
    }

    [SerializeField] private Image _fadeImage;

    public void StartFadeIn()
    {
        UIManager.Instance.inventoryIcon.SetActive(false);
        InventoryManager.Instance.canOpenInventory = true;
        // SoundManager.Instance.StopBGM(2f);
        FadeIn().Forget();
    }

    private async UniTask FadeIn()
    {
        _fadeImage.color = new Color(0, 0, 0, 1);
        await _fadeImage.DOFade(0, _fadeDuration).AsyncWaitForCompletion();
        _fadeImage.gameObject.transform.parent.gameObject.SetActive(false);
        PlayerPrefs.SetInt("FadeImage", 1);
        PlayerPrefs.Save();
    }

    public void StartLastEffect(string effectID)
    {
        EffectManager.Instance.SetEffect(effectID);
    }

    private async UniTaskVoid LookTarget(float duration)
    { 
        if (_player == null)
        {
            _player = GameObject.Find("Player").GetComponent<Animator>();
        }
        Transform playerTrans=_player.transform;
        Transform daugterTrans = GameObject.Find("Daughter").transform;
        float elapsed = 0f;
        while (elapsed < duration && playerTrans != null)
        {
            daugterTrans.LookAt(playerTrans.position);
            await UniTask.Yield();
            elapsed += Time.deltaTime;
        }
    }

    public void LookAtTarget(float second)
    {
        LookTarget(second).Forget();
    }

    public void LockLeftRight()
    {
        PlayerController.Instance.blockLeftRight = true;
    }

    public void ForceBlockPlayerMove()
    {
        PlayerController.Instance.canMove = false;
    }

    public void GetAchievements(string apiName)
    {
        if (SteamManager.Initialized)
        {
            Steamworks.SteamUserStats.GetAchievement(apiName, out bool isAchieved);
            if (!isAchieved)
            {
                SteamUserStats.SetAchievement(apiName);
                SteamUserStats.StoreStats();
            }
        }
        CheckAllAchievements();
    }

    private void CheckAllAchievements()
    {
        SteamUserStats.GetAchievement("Ach_All", out bool allAchieved);
        if (allAchieved)
            return;

        bool achTrue, achNormal, achBad;

        SteamUserStats.GetAchievement("Ach_True", out achTrue);
        SteamUserStats.GetAchievement("Ach_Normal", out achNormal);
        SteamUserStats.GetAchievement("Ach_Bad", out achBad);

        if (achTrue && achNormal && achBad)
        {
            SteamUserStats.SetAchievement("Ach_All");
            SteamUserStats.StoreStats();
        }
    }
}