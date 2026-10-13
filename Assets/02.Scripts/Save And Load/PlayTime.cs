using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayTime : Singleton<PlayTime>
{
    private bool canTrackPlayTime = false;
    [SerializeField] float _playTime = 0f;

    void Start()
    {
        canTrackPlayTime = false;
        _playTime = 0f;
    }
    private void Update()
    {
        if (canTrackPlayTime) _playTime += Time.deltaTime;
    }

    public string FormatPlayTime(float time)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(time);
        int totalHours = (int)timeSpan.TotalHours;
        string formatted = $"{totalHours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
        return formatted;
    }

    public float GetPlayTime()
    {
        return _playTime;
    }

    public void SetPlayTime(float value)
    {
        _playTime = value;
    }

    public void SetPlayTimeTracking(bool isEnabled)
    {
        canTrackPlayTime = isEnabled;
    }
}
