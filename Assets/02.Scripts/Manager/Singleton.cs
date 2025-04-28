using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// 싱글톤 제네릭 클래스
// 최초 작성자: 홍원기
// 수정자: 
// 최종 수정일: 2024-10-25
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    [SerializeField] bool dontDestroy = false;
    private static bool isQuitting = false;

    public static T Instance
    {
        get
        {
            if (isQuitting)
            {
                return null;
            }

            if (instance == null)
            {
                instance = (T)FindObjectOfType(typeof(T));

                if (instance == null)
                {
                    GameObject obj = new GameObject(typeof(T).Name, typeof(T));
                    instance = obj.GetComponent<T>();
                }
            }
            return instance;
        }
    }

    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
            if (dontDestroy)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void OnApplicationQuit()
    {
        isQuitting = true;
    }
}