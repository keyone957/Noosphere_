using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    public Action moveAction = null;
    public Action selectBtnAction = null;
    public Action exitBtnAction = null;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void OnUpdate()
    {
        // if(Input.anyKey==false)
        //     return;
        // moveAction?.Invoke();

        if (InputRouter.Instance.ConsumeEscape())
            exitBtnAction?.Invoke();

        if (InputRouter.Instance.ConsumeE())
            selectBtnAction?.Invoke();
    }
    public void FixedUpdate()
    {
        if(Input.anyKey==false)
            return;
        moveAction?.Invoke();
    }
}

