using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
public class InitPlayerPos : MonoBehaviour
{
    [SerializeField] private string _eventID;
    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

        if (!DataManager.Instance._events[_eventID].isExecuted)
        {
            Transform player = GameObject.FindWithTag("Player").transform;
            player.position = gameObject.transform.position;
            player.rotation = gameObject.transform.rotation;    
        }
        
        if(scene.name == "FinalStage_Spirit") EventManager.Instance.ExecuteEvent("Event_D085").Forget();
    }
}
