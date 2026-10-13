using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Debug = NooSphere.Debug;

public class DefaultUIController : Singleton<DefaultUIController>
{
    public DefaultUIBase resumeUI;
    public DefaultUIBase slotDeleteUI;
    public DefaultUIBase settingUI;
    public DefaultUIBase creditUI;
    public DefaultUIBase inGameOptionUI;
    public DefaultUIBase exitGameUI;
    public DefaultUIBase saveUI;
    public DefaultUIBase slotSaveUI;
    public DefaultUIBase slotOverwriteUI;
    public DefaultUIBase slotCompleteUI;
    public DefaultUIBase resetUI;
    public DefaultUIBase savingUI;

    public List<string> excludedOptionScenes = new List<string>();

    //여러 UI 창을 관리하기 위해 스택 이용
    private Stack<DefaultUIBase> uiStack = new Stack<DefaultUIBase>();
    public DefaultUIBase topUI;

    private void Update()
    {
        foreach (string sceneName in excludedOptionScenes)
        {
            if (SceneManager.GetActiveScene().name == sceneName)
            {
                return; // 현재 씬이 제외된 씬 중 하나라면 UI를 열지 않음
            }
        }
    }

    public void OpenUI(DefaultUIBase ui)
    {
        if (ui == null) return;

        uiStack.Push(ui);
        topUI = ui;

        ui.OnOpen();
    }

    public void CloseTopUI()
    {
        if (uiStack.Count == 0) return;

        DefaultUIBase topUI = uiStack.Pop();

        topUI.OnClose();

        if (uiStack.Count > 0)
        {
            this.topUI = uiStack.Peek();
            EventSystem.current.SetSelectedGameObject(null);
            if (this.topUI.FirstSelectable != null)
            {
                Debug.Log($"{this.topUI.FirstSelectable.name}을 firstSelect");
                EventSystem.current.SetSelectedGameObject(this.topUI.FirstSelectable);
            }
        }
        else
        {
            this.topUI = null;
            //EscapeUI.Instance.DisActive();
        }
    }

    public void CloseAllUI()
    {
        while (uiStack.Count > 0)
        {
            CloseTopUI();
        }
    }

    public DefaultUIBase GetTopUI()
    {
        if (uiStack.Count == 0) return null;
        return uiStack.Peek();
    }

    public bool IsAnyUIOpen()
    {
        return uiStack.Count > 0;
    }

    public bool IsUIOpen(DefaultUIBase ui)
    {
        return uiStack.Contains(ui);
    }
}
