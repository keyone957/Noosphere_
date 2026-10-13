using System.Collections;
using System.Collections.Generic;
using NooSphere;
using UnityEngine;
using Debug = NooSphere.Debug;

public class BgmStartForContinue : MonoBehaviour
{
    [SerializeField] private string bgmName;
    private void Start()
    {
        if(SoundManager.Instance._bgmSource.isPlaying) return;
        SoundManager.Instance.StopForceBGM();
        SoundManager.Instance.PlayBGM(bgmName);
    }
}
