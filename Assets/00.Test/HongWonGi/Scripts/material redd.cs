using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class materialredd : MonoBehaviour
{
    public Material retroMaterial;
   
    public void PlayPixelateAnimation()
    {
        DOTween.Sequence()
            .Append(DOTween.To(() => 370f, x => retroMaterial.SetFloat("_Pixelate", x), 0f, 1.5f))
            .Append(DOTween.To(() => 0f, x => retroMaterial.SetFloat("_Pixelate", x), 370f, 1.5f))
            .OnComplete(() => Debug.Log("Pixelate Animation Complete"));
    }


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.C)) 
        {
            PlayPixelateAnimation();
        }
    }
}
