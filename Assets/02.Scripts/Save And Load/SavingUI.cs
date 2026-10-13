using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavingUI : DefaultUIBase
{
    public override void OnOpen()
    {
        base.OnOpen();
        transform.GetChild(0).gameObject.SetActive(true);
        SoundManager.Instance.PlaySFX("Soundresource_131");
    }

    public override void OnClose()
    {
        base.OnClose();
        transform.GetChild(0).gameObject.SetActive(false);
    }
}
