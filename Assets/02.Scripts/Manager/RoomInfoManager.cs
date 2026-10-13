using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = NooSphere.Debug;
public class RoomInfoManager : MonoBehaviour
{
    public string roomName;
    public EventManager.RoomInfo roomInfo;

    private void Start()
    {
        Debug.Log("이벤트 매니저 찾기!!" + FindObjectOfType<EventManager>().gameObject);
        Debug.Log(EventManager.Instance.curRoomInfo + "CurRoomInfo");
        Debug.Log(this.roomInfo + "roomInfo");
        EventManager.Instance.curRoomInfo = this.roomInfo;

        if (SceneManager.GetActiveScene().name == "Lounge")
        {
            if (DataManager.Instance._events.ContainsKey("Event_A031") && DataManager.Instance._events.ContainsKey("Event_B067"))
            {
                if(DataManager.Instance._events["Event_A031"].isExecuted)
                {
                    if (DataManager.Instance._events["Event_B067"].isExecuted)
                    {
                        EventManager.Instance.curChapterInfo = EventManager.ChapterInfo.Stage2;
                        EventManager.Instance.curRoomInfo = EventManager.RoomInfo.Room_104;
                    }
                    EventManager.Instance.curChapterInfo = EventManager.ChapterInfo.Stage1;
                    EventManager.Instance.curRoomInfo = EventManager.RoomInfo.Room_102;
                }
            }
        }
        else
        {
            StartCoroutine(ShowRoomNumber());
        }

        if(EventManager.Instance.currentEventID == "Event_D097")
        {
            DataManager.Instance._lockConditions["Lock_condition_001"].Lock();
        }
        Debug.LogWarning($"현재 방 정보 : {roomInfo}");
        InventoryManager.Instance.currentViewChapter = (int)EventManager.Instance.curRoomInfo;
    }

    IEnumerator ShowRoomNumber()
    {
        yield return new WaitForSeconds(2f);
        if (transform.GetChild(0) != null)
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }
    }
}
