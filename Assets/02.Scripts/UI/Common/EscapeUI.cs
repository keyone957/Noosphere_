using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Debug = NooSphere.Debug;

public class EscapeUI : Singleton<EscapeUI>, IPointerClickHandler
{
    [SerializeField] private GameObject _escapeBtn;
    public void OnPointerClick(PointerEventData eventData)
    {
        //클릭한 오브젝트가 챕터인지 파악
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (FindObjectOfType<HintImage>() != null)
            {
                FindObjectOfType<HintImage>().gameObject.SetActive(false);
                SoundManager.Instance.PlaySFX("Soundresource_036");
                DisActive();
                UIManager.Instance.cctvFrame.SetActive(true);
                UIManager.Instance.inventoryIcon.SetActive(true);
                UIManager.Instance.UnLockPlayer();
                
                return;
            }
            Debug.LogWarning("나가기 버튼 누름");
            UIManager.Instance.CloseTopUI();
            //_escapeBtn.SetActive(false);
        }
    }

    public void Active()
    {
        //Debug.LogWarning("나가기 버튼 활성화");
        _escapeBtn.SetActive(true);
    }
    
    public void DisActive()
    {
        //Debug.LogWarning("나가기 버튼 비활성화");
        _escapeBtn.SetActive(false);
    }
}
