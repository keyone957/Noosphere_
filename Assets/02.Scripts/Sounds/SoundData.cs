using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Sound Data", menuName = "Sound/Create Sound Data")]
public class SoundData : ScriptableObject
{
    public string soundID; //sound 아이디
    public AudioClip soundClip;  //sound clip
    [Space(5)][Header("Bypass 관련 설정")]
    public bool bypassEffects = false;
    public bool bypassListenerEffects = false;
    public bool bypassReverbZones = false;
    [Space(5)][Header("일반 설정")]
    public int loopCnt; //루프 횟수
    public float volume; //볼륨
    public int priority = 128; // 사운드 우선순위
    public float pitch = 1; // 피치
    public float stereoPan = 0; // 스테레오 팬
    public float spatialBlend = 0;
    public float reverbZoneMix = 1;
    [Space(5)] [Header("3D 공간 설정")] public int dopplerLevel = 1;
    public int spread = 0;
    public AudioRolloffMode volumeRolloff = AudioRolloffMode.Logarithmic;
    public int minDistance = 1;
    public int maxDistance = 500;
}
