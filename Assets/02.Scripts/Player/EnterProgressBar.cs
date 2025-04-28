using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnterProgressBar : MonoBehaviour
{
    [SerializeField] private float _fillSpeed;
    [SerializeField] private Image bar;
    
    void OnEnable()
    {
        bar.fillAmount = 0;
    }

    public void InitFillAmount()
    {
        bar.fillAmount = 0;
    }

    public float FillAmount()
    {
        bar.fillAmount += _fillSpeed * Time.deltaTime;
        float value = Mathf.Clamp(bar.fillAmount, 0f, 1f);
        bar.fillAmount = value;
        return value;
    }

    public float DrainAmount()
    {
        bar.fillAmount -= _fillSpeed * Time.deltaTime;
        float value = Mathf.Clamp(bar.fillAmount, 0f, 1f);
        bar.fillAmount = value;
        return value;
    }
}
