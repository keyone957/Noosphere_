using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CustomEventData
{
    public string EventId;
    public string Description;
    public bool RepeatType;
    public string ConditionType;
    public List<string> Conditions = new List<string>();
    public List<string> Results = new List<string>();
    public string EvidenceId;
    public string LockConditionId;
    public string LocationId;
    public string NextEventId;
}