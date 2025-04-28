using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Unity.VisualScripting;

public class SoundManager : Singleton<SoundManager>
{
    // 딕셔너리로 오디오 관리
    private Dictionary<string, SoundData> _bgmDictionary = new Dictionary<string, SoundData>();
    private Dictionary<string, SoundData> _sfxDictionary = new Dictionary<string, SoundData>();
    [SerializeField] private List<SoundData> _bgmList = new List<SoundData>();
    [SerializeField] private List<SoundData> _sfxList = new List<SoundData>();
    
    [SerializeField]
    private AudioSource _bgmSource = null;
    [SerializeField]
    private List<AudioSource> _sfxSources = new List<AudioSource>(); // SFX를 재생하는 AudioSource 리스트
    [SerializeField] private int _maxSFXPoolSize = 10;
    
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
        SoundData soundData = _bgmDictionary[id];
        if (soundData == null || soundData.soundClip == null)
        {
            Debug.LogWarning("SoundData가 유효하지 않습니다.");
            return;
        }

        // 현재 재생 중인 BGM과 같다면 다시 재생할 필요 없음
        if (_bgmSource.clip == soundData.soundClip && _bgmSource.isPlaying)
        {
            Debug.Log("현재 재생 중인 BGM과 동일합니다.");
            return;
        }

        StopBGM();
        _bgmSource.clip = soundData.soundClip;
        _bgmSource.volume = 0f;
        _bgmSource.loop = true;
  
        _bgmSource.Play();
        DOTween.To(() => _bgmSource.volume, x => _bgmSource.volume = x, soundData.volume, 1f);
    }

    public void StopForceBGM() => _bgmSource.Stop();
    public void StopBGM()
    {
        if (_bgmSource.isPlaying)
        {
            DOTween.To(() => _bgmSource.volume, x => _bgmSource.volume = x, 0f, 1.5f)
                .OnComplete(() => _bgmSource.Stop());
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

        source.clip = soundData.soundClip;
        source.volume = soundData.volume;

        for(int i = 0; i < soundData.loopCnt; i++)
        {
            source.Play();
            // AudioClip의 길이만큼 대기 후 오디오 소스 중지 및 반환
            float clipLength = soundData.soundClip.length; // 클립의 길이 가져오기
            StartCoroutine(StopAndReleaseSourceAfterDelay(source, clipLength));
        }
        EffectManager.Instance.OnEffectEnd?.Invoke();
    }
    
    private AudioSource GetAvailableSFXSource()
    {
        // 사용 가능한 오디오 소스 찾기
        foreach (AudioSource source in _sfxSources)
        {
            if (!source.isPlaying)
            {
                return source; // 재사용 가능한 소스를 반환
            }
        }

        // 새 오디오 소스를 생성 (최대 개수 제한)
        if (_sfxSources.Count < _maxSFXPoolSize)
        {
            AudioSource newSource = gameObject.AddComponent<AudioSource>();
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
                source.Stop();
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
            source.Stop();
        }
    }
    
    public void StopAllSFX()
    {
        foreach (AudioSource source in _sfxSources)
        {
            source.Stop();
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
        source.volume = soundData.volume;
        source.loop = true; // 루프 활성화
        source.Play();
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

        source.clip = soundData.soundClip;
        source.volume = soundData.volume;

        for(int i = 0; i < soundData.loopCnt; i++)
        {
            source.Play();
            // AudioClip의 길이만큼 대기 후 오디오 소스 중지 및 반환
            float clipLength = soundData.soundClip.length; // 클립의 길이 가져오기
            StartCoroutine(StopAndReleaseSourceAfterDelay(source, clipLength));
        }
    }
}