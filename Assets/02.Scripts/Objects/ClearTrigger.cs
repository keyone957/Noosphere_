using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Debug = NooSphere.Debug;

public class ClearTrigger : MonoBehaviour
{
   [SerializeField] private string nextScene;
   [SerializeField] private string eventID;
   [SerializeField] private EventManager.ChapterInfo _chapterInfo;
   
   private void OnTriggerEnter(Collider other)
   {
      if (other.CompareTag("Player"))
      {
         Debug.Log("트리거 엔터");
         EventManager.Instance.ExecuteEvent(eventID).Forget();
         EventManager.Instance.curChapterInfo = _chapterInfo;
         SceneTracker.previousSceneName = SceneManager.GetActiveScene().name;
         
         // 자동 저장 트리거인데, 문과의 상호작용일 경우
         EventStructure _event = DataManager.Instance._events[eventID];

         if (_event.autoSave && _event.autoSaveDelay == 1 && !_event.autoSaveComplete)
         {
            Debug.Log(_event.eventId + " 문 자동 저장 트리거 발동");
            _event.autoSaveComplete = true;
            NooSphere.SaveManager.Instance.OnDoorAutoSave = true;
         }
         
         
         // SceneManager.LoadScene(nextScene);
         SceneChanger.Instance.ChangeScene(nextScene);
      }
   }
}
