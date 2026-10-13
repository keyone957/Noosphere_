using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ES3Types;
using TMPro;
using DG.Tweening;

public class SaveManager : Singleton<SaveManager>
{
   [SerializeField] private TextMeshProUGUI _loadingText;
   private void Update()
   {
      if (Input.GetKeyDown(KeyCode.B))
      {
         Save();
      }

      // if (Input.GetKeyDown(KeyCode.C))
      // {
      //    Load();
      // }
   }
   public void Save()
   {
      _loadingText.alpha = 0f;
      ES3.Save("curRoomInfo", EventManager.Instance.curRoomInfo);
      ES3.Save("nextEventID", EventManager.Instance.nextEventID);
      ES3.Save("startEventID", EventManager.Instance.startEventID);
      // ES3.Save("chapterInventories", InventoryManager.Instance.chapterInventories);
      /*TODO
       *inventorymanager inventoryslot [syste,.serializable]
       * chapterinventory[system.serializable]  chapterinventoris public으로 변경
       */
      _loadingText.DOFade(1f, 0.5f).OnComplete(() =>
      {
         // 2초 후 Fade Out
         DOVirtual.DelayedCall(2f, () =>
         {
            _loadingText.DOFade(0f, 0.5f);
         });
      });
   }
   
   // public void Load()
   // {
   //    // 이벤트 매니저 정보 로드
   //    if(ES3.KeyExists("curStageInfo"))
   //    {
   //       EventManagerYKM.Instance.curStageInfo = ES3.Load<EventManagerYKM.ChapterInfo>("curStageInfo");
   //       Debug.Log($"Loaded curStageInfo: {EventManagerYKM.Instance.curStageInfo}");
   //    }
   //
   //    if(ES3.KeyExists("nextEventID"))
   //    {
   //       EventManagerYKM.Instance.nextEventID = ES3.Load<string>("nextEventID");
   //       Debug.Log($"Loaded nextEventID: {EventManagerYKM.Instance.nextEventID}");
   //    }
   //     
   //    if(ES3.KeyExists("startEventID"))
   //    {
   //       EventManagerYKM.Instance.startEventID = ES3.Load<string>("startEventID");
   //       Debug.Log($"Loaded startEventID: {EventManagerYKM.Instance.startEventID}");
   //    }
   //
   //    // 인벤토리 정보 로드
   //    if(ES3.KeyExists("chapterInventories"))
   //    {
   //       InventoryManager.Instance.chapterInventories = ES3.Load<Dictionary<int, ChapterInventory>>("chapterInventories");
   //     
   //       // 로드된 인벤토리 데이터 확인
   //       var inventory = InventoryManager.Instance.chapterInventories;
   //       foreach(var chapter in inventory)
   //       {
   //          Debug.Log($"Loaded Chapter {chapter.Key}:");
   //          Debug.Log($"Loaded Real World Evidence Count: {chapter.Value.realWorldEvidences.Count}");
   //          foreach(var evidence in chapter.Value.realWorldEvidences)
   //          {
   //             Debug.Log($"- Loaded Evidence ID: {evidence.evidence_id}, Name: {evidence.evidence_name}");
   //          }
   //         
   //          Debug.Log($"Loaded Mental World Evidence Count: {chapter.Value.mentalWorldEvidences.Count}");
   //          foreach(var evidence in chapter.Value.mentalWorldEvidences)
   //          {
   //             Debug.Log($"- Loaded Evidence ID: {evidence.evidence_id}, Name: {evidence.evidence_name}");
   //          }
   //       }
   //    }
   //    Debug.Log("Load Complete");
   // }

 
}