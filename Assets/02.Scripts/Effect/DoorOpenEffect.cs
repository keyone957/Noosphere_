using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Debug = NooSphere.Debug;

public class DoorOpenEffect : MonoBehaviour
{
   [SerializeField] private GameObject _closeDoor;
   [SerializeField] private GameObject _clearTrigger;
   [SerializeField] private GameObject _block;

   private void OnEnable()
   {
      _closeDoor.SetActive(false);
      _clearTrigger.SetActive(true);

      if(GetComponent<AudioSource>() != null)
      {
            Debug.Log("Door Open 3D Sound Play");
            AudioSource audioSource = GetComponent<AudioSource>();
            if (audioSource.isPlaying) audioSource.Stop();
            audioSource.Play();
            audioSource.volume = SoundManager.Instance.sfxVolume;
        }
      else
      {
        SoundManager.Instance.PlaySFX("Soundresource_082");
      }

      // 문 다 열리면 EventTrigger의 isDoorOpen
      if (transform.parent.GetComponentInChildren<EventTrigger>() != null)
      {
        transform.parent.GetComponentInChildren<EventTrigger>().isDoorOpen = true;
      }
   }

   public void OnAnimationEnd()
   {
      _block.SetActive(false);
      EffectManager.Instance.OnEffectEnd?.Invoke();
   }
}
