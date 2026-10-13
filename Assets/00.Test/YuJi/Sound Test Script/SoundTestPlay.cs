using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundTestPlay : MonoBehaviour
{
    public void PlaySound(AudioSource audioSource)
    {
        if(audioSource.isPlaying) audioSource.Stop();
        audioSource.Play();
    }
}
