using NooSphere;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DestroyFadeImg : MonoBehaviour
{
    [SerializeField] private string _eventID;
    private void Start()
    {
        if (DataManager.Instance._events[_eventID].isExecuted)
        {
            Destroy(this.gameObject);
        }
        //SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        //SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        
    }
}
