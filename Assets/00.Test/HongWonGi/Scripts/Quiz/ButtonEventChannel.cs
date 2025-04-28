using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "ButtonEventChannel", menuName = "SO/Button Event Channel")]
public class ButtonEventChannel : ScriptableObject
{
    public UnityAction<string> OnButtonClicked;
    public UnityAction<string> OnResetCardState;

    public void RaiseEvent(string buttonId)
    {
        OnButtonClicked?.Invoke(buttonId);
    }

    public void RaiseReturnEvent(string buttonId)
    {
        OnResetCardState?.Invoke(buttonId);
    }
}
