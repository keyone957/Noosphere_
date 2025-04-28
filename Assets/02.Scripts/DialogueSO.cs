using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Dialogue", menuName = "SO/Dialougue")]
public class DialogueSO : ScriptableObject
{
   public int dialogueCount;
   public string[] dialogueText;

   private void OnValidate()
   {
      if (dialogueCount < 0) dialogueCount = 0;
      if (dialogueText == null || dialogueText.Length != dialogueCount)
      {
         System.Array.Resize(ref dialogueText, dialogueCount);
      }
   }
}
