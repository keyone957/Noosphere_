using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Stage2SpiritManager : MonoBehaviour
{
    private void Awake()
    {
        EffectManager.Instance.ReverseProcessEffect().Forget();
    }
}
