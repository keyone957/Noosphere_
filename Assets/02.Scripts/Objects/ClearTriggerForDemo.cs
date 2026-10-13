using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class ClearTriggerForDemo : MonoBehaviour
{
    [SerializeField] private string nextScene;
    [SerializeField] private string eventID;
    [SerializeField] private EventManager.ChapterInfo _chapterInfo;
   
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("트리거 엔터");
           
            if (DataManager.Instance._events["Event_B067"].isExecuted)
            {
                SceneChanger.Instance.ChangeScene("DemoEndScene");
            }
            else
            {
                EventManager.Instance.ExecuteEvent(eventID).Forget();
                EventManager.Instance.curChapterInfo = _chapterInfo;
                SceneTracker.previousSceneName = SceneManager.GetActiveScene().name;
                SceneChanger.Instance.ChangeScene(nextScene);
            }
        }
    }
}
