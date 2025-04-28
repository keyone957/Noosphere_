using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintImage : MonoBehaviour
{
   private void Update()
   {
      if (Input.GetKeyDown(KeyCode.Escape))
      {
         gameObject.SetActive(false);
      }
   }
}
