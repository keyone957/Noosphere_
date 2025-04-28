using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TutorialInfo : MonoBehaviour
{
    private Animator _animator;

    void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    void Start()
    {
        ShowInfo().Forget();
    }

    async UniTask ShowInfo()
    {
        await UniTask.Delay(3000);
        _animator.SetTrigger("End");
    }

    public void DestroyTutorial()
    {
        Destroy(gameObject);
    }
}
