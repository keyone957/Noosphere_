using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Stage2SpiritManager : MonoBehaviour
{
    private void Awake()
    {  
        if (SceneTracker.previousSceneName !="Stage2Map_real"||SceneTracker.previousSceneName==String.Empty) return;
        
        EffectManager.Instance.ReverseProcessEffect().Forget();
    }
}
