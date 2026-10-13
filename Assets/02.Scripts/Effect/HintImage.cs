using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintImage : MonoBehaviour
{
   private void OnEnable()
   {
      UIManager.Instance.cctvFrame.SetActive(false);
      //인벤토리 아이콘 비활성화
      UIManager.Instance.inventoryIcon.SetActive(false);
      //플레이어 Lock
      UIManager.Instance.LockPlayer();
   }

   private void Update()
   {
      if (InputRouter.Instance.ConsumeEscape())
      {
         gameObject.SetActive(false);
         EscapeUI.Instance.DisActive();
         UIManager.Instance.cctvFrame.SetActive(true);
         UIManager.Instance.inventoryIcon.SetActive(true);
         UIManager.Instance.UnLockPlayer();
      }
   }
}
