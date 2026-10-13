using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DemoScene : MonoBehaviour
{
    [SerializeField] private Button _restartBtn;

    private void Awake()
    {
        //dontdestroyonload에서 사운드매니저하고 dotween만 남겨놓기
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.scene.name == "DontDestroyOnLoad" &&
                obj.GetComponent<SoundManager>() == null &&
                !obj.name.Contains("[DOTween]"))
            {
                Destroy(obj);
            }
        }
    }

    private void Start()
    {
        _restartBtn.onClick.AddListener(OnClickStartBtn);
        if(SoundManager.Instance._bgmSource.isPlaying) return;
        if (DataManager.Instance._events["Event_D104"].isExecuted)
        {
            SoundManager.Instance.StopForceBGM();
            SoundManager.Instance.PlayBGM("Soundresource_056");
        }
        else
        {
            SoundManager.Instance.StopForceBGM();
            SoundManager.Instance.PlayBGM("Soundresource_124");
        }
    }

    private void OnClickStartBtn()
    {
        SceneChanger.Instance.ChangeScene("StartScene").Forget();
        SoundManager.Instance.PlaySFXNoEffect("Soundresource_037");
    }
}