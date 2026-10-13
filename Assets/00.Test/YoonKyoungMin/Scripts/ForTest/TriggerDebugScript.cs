using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TriggerDebugScript : Singleton<TriggerDebugScript>
{
    public TextMeshProUGUI triggerText;
    public TextMeshProUGUI frontText;

    void Update()
    {
        if(PlayerInteract.Instance.curTrigger == null)
        {
            triggerText.text = "Cur Trigger : None";
        }
        else
        {
            triggerText.text = "Cur Trigger : " + PlayerInteract.Instance.curTrigger.name;
        }
    }

    public void WhenFront()
    {
        frontText.text = "Is Front : Front";
    }

    public void WhenNotFront()
    {
        frontText.text = "Is Front : Not Front";
    }
}
