using System.Collections;
using System.Collections.Generic;
using NooSphere;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SetPlayerLoungePos : MonoBehaviour
{
    [SerializeField] private List<Transform> _loungeInitTrans;
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
        if (NooSphere.SaveManager.Instance.CurrentLoadType != GameLoadType.ContinueGame)
        {
            NooSphere.Debug.LogWarning("이어하기 이후 또는 새게임 시 실행되는 플레이어 위치 설정");
            GameObject player = GameObject.FindWithTag("Player");
            if (SceneTracker.previousSceneName == "PrologueMap_real")
            {
                player.transform.position = _loungeInitTrans[0].position;
                player.transform.rotation = _loungeInitTrans[0].rotation;
            }
            else if (SceneTracker.previousSceneName == "Stage1Map_real")
            {
                player.transform.position = _loungeInitTrans[1].position;
                player.transform.rotation = _loungeInitTrans[1].rotation;
            }
        
            if (NooSphere.SaveManager.Instance.OnDoorAutoSave)
            {
                NooSphere.Debug.LogWarning("문 상호작용 자동 저장");
                NooSphere.SaveManager.Instance.OnDoorAutoSave = false;
                NooSphere.SaveManager.Instance.DoAutoSaveDelay();
            }
        }
    }
}
