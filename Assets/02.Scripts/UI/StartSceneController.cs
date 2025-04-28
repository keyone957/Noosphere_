using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class StartSceneController : MonoBehaviour
{
    private int _curSelectedBtnNum = 0;
    private GameObject _curSelectedBtn;
    [SerializeField] private List<GameObject> _startUIBtns = new List<GameObject>();
    
    void Awake()
    {
        _curSelectedBtnNum = 0;
        _curSelectedBtn = _startUIBtns[_curSelectedBtnNum];
        SetBtnSelected(_curSelectedBtn);
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            MoveUp();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            MoveDown();
        }

        if (_curSelectedBtn != null && Input.GetKeyDown(KeyCode.Space))
        {
            ExecuteEvents.Execute<ISelectHandler>(
                _curSelectedBtn, 
                new BaseEventData(EventSystem.current), (x, y) => x.OnSelect(y)
            );
        }
    }
    
    void MoveDown()
    {
        _curSelectedBtnNum += 1;
        if (_curSelectedBtnNum >= _startUIBtns.Count) _curSelectedBtnNum = 0;
        _curSelectedBtn = _startUIBtns[_curSelectedBtnNum];
        SetBtnSelected(_curSelectedBtn);
    }

    void MoveUp()
    {
        _curSelectedBtnNum -= 1;
        if (_curSelectedBtnNum < 0) _curSelectedBtnNum = _startUIBtns.Count - 1;
        _curSelectedBtn = _startUIBtns[_curSelectedBtnNum];
        SetBtnSelected(_curSelectedBtn);
    }
    
    void SetBtnSelected(GameObject btn)
    {
        //btn.GetComponent<TextMeshProUGUI>().color = UnityExtension.HexColor(UIManager.GreenColor);
        foreach (var _btn in _startUIBtns)
        {
            //if (_btn != btn) _btn.GetComponent<TextMeshProUGUI>().color = UnityExtension.HexColor(UIManager.BlackColor);
        }
    }

    public void SelectStartBtn()
    {
        SceneManager.LoadScene("LoadingScene");
    }

    public void SelectContinueBtn()
    {
        
    }

    public void SelectExitBtn()
    {
        
    }
    
}
