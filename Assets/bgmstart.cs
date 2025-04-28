using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bgmstart : MonoBehaviour
{
    [SerializeField] private string bgmName;
    private void Start()
    {
        SoundManager.Instance.StopForceBGM();
        SoundManager.Instance.PlayBGM(bgmName);
    }
}
