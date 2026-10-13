using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ResetUI : DefaultUIBase
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

    public void ClickYesBtn()
    {
        NooSphere.SaveManager.Instance.ResetAllSaveData();
        ClickNoBtn();

        NooSphere.SaveManager.Instance.SetLoadType(NooSphere.GameLoadType.NewGame);
        SceneChanger.Instance.ChangeScene("LoadingScene").Forget();
    }

    public void ClickNoBtn()
    {
        DefaultUIController.Instance.CloseTopUI();
    }

    public override void HandleKeyboardInput()
    {
        base.HandleKeyboardInput();

        if (InputRouter.Instance.ConsumeEscape())
        {
            ClickNoBtn();
        }
    }
}
