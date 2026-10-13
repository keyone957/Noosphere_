using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ExitGameUI : DefaultUIBase
{
    public override void OnOpen()
    {
        base.OnOpen();
        transform.GetChild(0).gameObject.SetActive(true);
    }

    public override void OnClose()
    {
        base.OnClose();

        transform.GetChild(0).gameObject.SetActive(false);
    }

    public void OnClickExitGame()
    {
        // 게임 종료
        Application.Quit();
    }

    public void OnClickCancel()
    {
        // UI 닫기
        DefaultUIController.Instance.CloseTopUI();
    }
}
