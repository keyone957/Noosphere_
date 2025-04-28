using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueData
{
    public string DialogueId;
    public string CharacterId;
    public List<string> DialogueTextLines = new List<string>();
    public string NextDialogueId;
}