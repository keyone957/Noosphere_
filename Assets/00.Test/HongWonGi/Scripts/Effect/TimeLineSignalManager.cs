using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using DG.Tweening;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Cinemachine;

public class TimeLineSignalManager : MonoBehaviour
{
    private Animator _player;
    [SerializeField] private Animator _npc;
    [SerializeField] private Animator _friendNpc;
    [SerializeField] private SkinnedMeshRenderer _freindMaterial;
    [SerializeField] private Material _friendMaterial;
    [SerializeField] private List<string> footSteps;
    [SerializeField] CinemachineVirtualCamera _farLoungCam;
    private void Start() 
    {
        _player = GameObject.Find("Player").GetComponent<Animator>();
        _mainCamera = Camera.main;
        if (_mainCamera != null)
        {
            _cameraData = _mainCamera.GetComponent<UnityEngine.Rendering.Universal.UniversalAdditionalCameraData>();
        }
    }

    public void StartPlayerAnim(string playerAnim)
   {
       _player.SetBool(playerAnim,true);
   }

   public void EndPlayerAnim(string playerAnim)
   {
       _player.SetBool(playerAnim,false);
   }

   public void StartNpcAnim(string npcAnim)
   {
       _npc.SetBool(npcAnim,true);
   }

   public void EndNpcAnim(string npcAnim)
   {
       _npc.SetBool(npcAnim,false);
   }
   public void StartFriendNpcAnim(string npcAnim)
   {
       _friendNpc.SetBool(npcAnim,true);
       _freindMaterial.material = _friendMaterial;
   }

   public void EndFriendNpcAnim(string npcAnim)
   {
       _friendNpc.SetBool(npcAnim,false);
   }

   public void SetMaterial(Material material)
   {
       _freindMaterial.material = material;
   }

   /// <summary>
   /// ///////////////////////////////////////////////////
   /// </summary>
   [Header("정신세계 이펙트")]
   [SerializeField] private RawImage _vhsImage;
   [SerializeField] private Volume _vhsVolume;
   [SerializeField] private GameObject _vhsObj;
   [SerializeField] private float _duration = 5f;
   private Camera _mainCamera;
   private UnityEngine.Rendering.Universal.UniversalAdditionalCameraData _cameraData;
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
       //돌아오는 정보 저장
       _player.gameObject.GetComponent<PlayerInteract>().isInMental = true;
       _player.gameObject.GetComponent<MentalEnterProcess>().SetCombackEventId("Event_C075");
       SceneManager.LoadSceneAsync(SceneName);
   }

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

   public void StopFootSteps()
   {
       isPlayingFootsteps = true;
   }

   public void CameraON()
   {
       _farLoungCam.Priority = 12;
   }
}
