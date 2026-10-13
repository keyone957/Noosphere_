using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine.Audio;
using Debug = NooSphere.Debug;

public class SoundManager : Singleton<SoundManager>
{
    // 딕셔너리로 오디오 관리
    private Dictionary<string, SoundData> _bgmDictionary = new Dictionary<string, SoundData>();
    private Dictionary<string, SoundData> _sfxDictionary = new Dictionary<string, SoundData>();
    [SerializeField] private List<SoundData> _bgmList = new List<SoundData>();
    [SerializeField] private List<SoundData> _sfxList = new List<SoundData>();

    [SerializeField] private AudioMixerGroup _SFXGroup;
    [SerializeField]
    public AudioSource _bgmSource = null;
    [SerializeField]
    private List<AudioSource> _sfxSources = new List<AudioSource>(); // SFX를 재생하는 AudioSource 리스트
    [SerializeField] private int _maxSFXPoolSize = 10;
    private const string BGM_FADE_ID = "BGM_FADE";
    private Tween _bgmFadeTween;
    public float sfxVolume;
    public float bgmVolume;
    /* 사운드 Data 시트에 따라 업데이트 하고 싶으면, 이 주석 제거한 뒤 실행하면 생성됨.
    async void Awake()
    {
        //데이터 매니저의 사운드 리소스들을 읽어서 자동으로 soundData로 변환해서 저장하기
        foreach (var sound in DataManager.Instance._sound)
        {
            await CreateSoundData(sound.Value);
        }
        //저장한 sound Data를 딕셔너리에 저장하기
        LoadSoundData("Assets/02.Scripts/Sounds/SoundData/BGM", _bgmDictionary);
        LoadSoundData("Assets/02.Scripts/Sounds/SoundData/SFX", _sfxDictionary);
    }
    */
    void Awake()
    {
        base.Awake();
        // bgm 데이터 초기화
        foreach (var bgmData in _bgmList)
        {
            if (bgmData != null && bgmData.soundClip != null)
            {
                _bgmDictionary[bgmData.soundID] = bgmData;
            }
        }
        
        // sfx 데이터 초기화
        foreach (var sfxData in _sfxList)
        {
            if (sfxData != null && sfxData.soundClip != null)
            {
                _sfxDictionary[sfxData.soundID] = sfxData;
            }
        }

        if (!ES3.KeyExists("BgmVolume","Setting.es3"))
        {
            bgmVolume = 0.5f;
        }
        else
        {
            bgmVolume = ES3.Load<float>("BgmVolume","Setting.es3");
        }
        
        if (!ES3.KeyExists("SoundVolume","Setting.es3"))
        {
            sfxVolume = 0.5f;
        }
        else
        {
            sfxVolume = ES3.Load<float>("SoundVolume","Setting.es3");
        }
    }
    
    /*
    // 특정 폴더 내 Sound Data 로드
    public void LoadSoundData(string folderPath, Dictionary<string, SoundData> dictionary)
    {
        dictionary.Clear();

        // 특정 폴더 내 모든 에셋 검색
        string[] guids = AssetDatabase.FindAssets("t:SoundData", new[] { folderPath });

        foreach (string guid in guids)
        {
            // 에셋 경로 가져오기
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);

            // SoundData 로드
            SoundData soundData = AssetDatabase.LoadAssetAtPath<SoundData>(assetPath);

            if (soundData != null)
            {
                // 딕셔너리에 추가 (키는 SoundData의 이름)
                dictionary[soundData.name] = soundData;
                Debug.Log($"Loaded SoundData: {soundData.name}");
            }
        }

        Debug.Log($"총 {dictionary.Count}개의 SoundData를 로드했습니다.");
    }
    */
    /*
    public AudioClip LoadAudioClip(string path)
    {
        // Resources 폴더 내의 경로에서 AudioClip 로드
        AudioClip clip = Resources.Load<AudioClip>(path);

        if (clip == null)
        {
            Debug.LogError($"AudioClip을 찾을 수 없습니다: {path}");
        }

        return clip;
    }
*/
    /*
    async UniTask CreateSoundData(SoundResourceStructure data)
    {
        SoundData soundData = ScriptableObject.CreateInstance<SoundData>();

        string _folderPath = "";
        if (data.soundresourceType == "Sound")
        {
            _folderPath = "Assets/02.Scripts/Sounds/SoundData/SFX";

            // 폴더가 없으면 생성
            if (!AssetDatabase.IsValidFolder(_folderPath))
            {
                AssetDatabase.CreateFolder("Assets/02.Scripts/Sounds/SoundData", "SFX");
            }
        }
        else if(data.soundresourceType == "BGM")
        {
            _folderPath = "Assets/02.Scripts/Sounds/SoundData/BGM";

            // 폴더가 없으면 생성
            if (!AssetDatabase.IsValidFolder(_folderPath))
            {
                AssetDatabase.CreateFolder("Assets/02.Scripts/Sounds/SoundData", "BGM");
            }
        }
        
        // Sound Data 정보 반영
        soundData.soundID = data.soundresourceId;
        soundData.soundClip = LoadAudioClip(data.FilePath);
        soundData.loopCnt = data.loopCount;
        soundData.volume = data.volume;
        
        // ScriptableObject 저장 경로 설정
        string assetPath = $"{_folderPath}/{data.soundresourceId}.asset";
        AssetDatabase.CreateAsset(soundData, assetPath);

        // 변경 사항 저장 및 갱신
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // 에디터 작업 후 프레임 대기
        await UniTask.Yield(PlayerLoopTiming.Update);

        Debug.Log($"SoundData created at {assetPath}");
    }
*/
    public void PlayBGM(string id)
    {
        // 🔴 이전 씬에서 걸린 페이드 트윈 제거(DDOL이라 꼭 필요)
        DOTween.Kill(BGM_FADE_ID, complete: false);
        if (_bgmFadeTween != null && _bgmFadeTween.IsActive()) _bgmFadeTween.Kill();
        _bgmFadeTween = null;

        if (!_bgmDictionary.TryGetValue(id, out var data) || data?.soundClip == null)
        {
            Debug.LogWarning("SoundData가 유효하지 않습니다."); return;
        }

        // 같은 곡 재생 최적화는 '트윈 킬' 이후에 검사해야 안전
        if (_bgmSource.clip == data.soundClip && _bgmSource.isPlaying)
        {
            // 혹시 볼륨이 0으로 남았을 수 있으니 복구
            _bgmSource.volume = data.volume * bgmVolume;
            return;
        }

        _bgmSource.Stop();                 // 이전 클립 정리
        SetAudioSource(_bgmSource, data);  // 클립/설정 반영
        _bgmSource.loop = true;
        _bgmSource.volume = 0f;
        _bgmSource.Play();

        DOTween.To(() => _bgmSource.volume, x => _bgmSource.volume = x,
            data.volume * bgmVolume, 1f);
    }

    public void StopForceBGM() => _bgmSource.Stop();
    // public void StopBGM(float duration)
    // {
    //     if (_bgmSource.isPlaying)
    //     {
    //         DOTween.To(() => _bgmSource.volume, x => _bgmSource.volume = x, 0f, duration)
    //             .OnComplete(() => _bgmSource.Stop());
    //     }
    //     
    // }
    
    public void StopBGM(float duration)
    {
        if (_bgmFadeTween != null && _bgmFadeTween.IsActive()) _bgmFadeTween.Kill();

        if (_bgmSource.isPlaying)
        {
            _bgmFadeTween = DOTween
                .To(() => _bgmSource.volume, x => _bgmSource.volume = x, 0f, duration)
                .SetId(BGM_FADE_ID)
                .OnComplete(() =>
                {
                    _bgmSource.Stop();
                    _bgmFadeTween = null;
                });
        }
    }
    public void PlaySFX(string id)
    {
        SoundData soundData = _sfxDictionary[id];
        if (soundData == null || soundData.soundClip == null)
        {
            Debug.LogWarning("SoundData 유효하지 않습니다.");
            return;
        }

        // 재사용 가능한 AudioSource 가져오기
        AudioSource source = GetAvailableSFXSource();
        SetAudioSource(source, soundData);
        source.volume = soundData.volume*sfxVolume;
        source.loop = false;
        for(int i = 0; i < soundData.loopCnt; i++)
        {
            source.time = 0f;
            source.Play();
            // AudioClip의 길이만큼 대기 후 오디오 소스 중지 및 반환
            float clipLength = soundData.soundClip.length; // 클립의 길이 가져오기
            StartCoroutine(StopAndReleaseSourceAfterDelay(source, clipLength));
        }
        EffectManager.Instance.OnEffectEnd?.Invoke();
    }
    public void PlaySFXNoEffect(string id)
    {
        SoundData soundData = _sfxDictionary[id];
        if (soundData == null || soundData.soundClip == null)
        {
            Debug.LogWarning("SoundData 유효하지 않습니다.");
            return;
        }
        // 재사용 가능한 AudioSource 가져오기
        AudioSource source = GetAvailableSFXSource();
        SetAudioSource(source, soundData);
        // source.clip = soundData.soundClip;
        source.volume = soundData.volume*sfxVolume;
        source.loop = false;

        for(int i = 0; i < soundData.loopCnt; i++)
        {
            source.time = 0f;
            source.Play();
            // AudioClip의 길이만큼 대기 후 오디오 소스 중지 및 반환
            float clipLength = soundData.soundClip.length; // 클립의 길이 가져오기
            StartCoroutine(StopAndReleaseSourceAfterDelay(source, clipLength));
        }
    }
    private AudioSource GetAvailableSFXSource()
    {
        // 사용 가능한 오디오 소스 찾기
        foreach (AudioSource source in _sfxSources)
        {
            if (!source.isPlaying)
            {
                source.outputAudioMixerGroup = _SFXGroup;
                return source; // 재사용 가능한 소스를 반환
            }
        }

        // 새 오디오 소스를 생성 (최대 개수 제한)
        if (_sfxSources.Count < _maxSFXPoolSize)
        {
            AudioSource newSource = gameObject.AddComponent<AudioSource>();
            newSource.outputAudioMixerGroup = _SFXGroup;
            _sfxSources.Add(newSource);
            return newSource;
        }

        // 풀이 꽉 찼을 때 가장 오래된 소스를 재사용
        return _sfxSources[0];
    }
    
    public void StopSFX(string id)
    {
        SoundData soundData = _sfxDictionary[id];
        if (soundData == null || soundData.soundClip == null)
        {
            Debug.LogWarning("SoundData 유효하지 않습니다.");
            return;
        }
        
        foreach (var source in _sfxSources)
        {
            if (source.clip == soundData.soundClip && source.isPlaying)
            {
                source.clip = null;
                source.Stop();
                Debug.Log($"{source}의 SFX가 중지되었습니다.");
                return;
            }
        }
    }

    public void StopSFXWithFade(string id, float duration)
    {
        SoundData soundData = _sfxDictionary[id];
        if (soundData == null || soundData.soundClip == null)
        {
            Debug.LogWarning("SoundData 유효하지 않습니다.");
            return;
        }
        
        foreach (var source in _sfxSources)
        {
            if (source.clip == soundData.soundClip && source.isPlaying)
            {
                DOTween.To(() => source.volume, x => source.volume = x, 0f, duration)
                    .OnComplete(() =>
                    {
                        source.clip = null;
                        source.Stop();
                    });
                Debug.Log($"{source}의 SFX가 중지되었습니다.");
                return;
            }
        }
    }
    
    private System.Collections.IEnumerator StopAndReleaseSourceAfterDelay(AudioSource source, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (source != null && source.isPlaying)
        {
            source.clip = null;
        }
    }
    
    public void StopAllSFX()
    {
        foreach (AudioSource source in _sfxSources)
        {
            source.clip = null;
        }
    }

    public void PlayLoopingSound(string id)
    {
        SoundData soundData = _sfxDictionary[id];
        if (soundData == null || soundData.soundClip == null)
        {
            Debug.LogWarning("SoundData 유효하지 않습니다.");
            return;
        }

        // 재사용 가능한 AudioSource 가져오기
        AudioSource source = GetAvailableSFXSource();

        source.clip = soundData.soundClip;
        source.volume = soundData.volume*sfxVolume;
        source.loop = true; // 루프 활성화
        source.Play();
    }
    
   

    public void SetAudioSource(AudioSource audioSource, SoundData data)
    {
        // 오디오 일반 설정
        audioSource.clip = data.soundClip;
        audioSource.volume = data.volume;
        audioSource.priority = data.priority;
        audioSource.pitch = data.pitch;
        audioSource.panStereo = data.stereoPan;
        audioSource.spatialBlend = data.spatialBlend;
        audioSource.reverbZoneMix = data.reverbZoneMix;
        
        // bypass 관련 설정
        audioSource.bypassEffects = data.bypassEffects;
        audioSource.bypassListenerEffects = data.bypassListenerEffects;
        audioSource.bypassReverbZones = data.bypassReverbZones;
        
        // 3d 공간 설정
        audioSource.dopplerLevel = data.dopplerLevel;
        audioSource.spread = data.spread;
        audioSource.rolloffMode = data.volumeRolloff;
        audioSource.minDistance = data.minDistance;
        audioSource.maxDistance = data.maxDistance;
    }

    public SoundData GetSoundData(string id, bool isBGM)
    {
        if (isBGM)
        {
            if (_bgmDictionary.TryGetValue(id, out SoundData bgmData))
            {
                return bgmData;
            }
        }
        else
        {
            if (_sfxDictionary.TryGetValue(id, out SoundData sfxData))
            {
                return sfxData;
            }
        }
        Debug.LogWarning("SoundData를 찾을 수 없습니다.");
        return null;
    }
}