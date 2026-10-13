using UnityEngine;
using UnityEngine.EventSystems;
using Debug = NooSphere.Debug;

public class DefaultUIBase : MonoBehaviour
{
    [SerializeField] private GameObject firstSelectable;
    public GameObject FirstSelectable => firstSelectable;
    public bool noSound = false;
    private void Update()
    {
        if (IsTopUI())
        {
            HandleKeyboardInput();
            HandleMouseInput();
        }
    }

    public virtual void OnOpen()
    {
        if(!noSound)
            SoundManager.Instance.PlaySFX("Soundresource_037");
        Debug.Log($"#{gameObject.name}이(가) 열렸습니다.");
        
        if (FirstSelectable != null)
        {
            Debug.Log($"{FirstSelectable.name}을 firstSelect");
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(FirstSelectable);
        }
        else
        {
            Debug.Log($"firstSelect을 비움");
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    public virtual void OnClose()
    {
        if(!noSound)
            SoundManager.Instance.PlaySFX("Soundresource_036");
        Debug.Log($"#{gameObject.name}이(가) 닫혔습니다.");
    }

    public virtual void HandleKeyboardInput() { }
    public virtual void HandleMouseInput() { }

    public bool IsTopUI()
    {
        return DefaultUIController.Instance.GetTopUI() == this;
    }
    public void OnClickSound()
    {
        SoundManager.Instance.PlaySFX("Soundresource_037");
    }

    public void OnHoverSound()
    {
        SoundManager.Instance.PlaySFX("Soundresource_035");
    }
}
