using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = NooSphere.Debug;
public class RoomInfoManager : MonoBehaviour
{
    public EventManager.RoomInfo roomInfo;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            EventManager.Instance.curRoomInfo = this.roomInfo;

            if (SceneManager.GetActiveScene().name == "Lounge")
            {
                if (EventManager.Instance.curChapterInfo == EventManager.ChapterInfo.Stage1)
                {
                    EventManager.Instance.curRoomInfo = EventManager.RoomInfo.Room_102;
                }
                else if (EventManager.Instance.curChapterInfo == EventManager.ChapterInfo.Stage2)
                {
                    EventManager.Instance.curRoomInfo = EventManager.RoomInfo.Room_104;
                }
            }
            Debug.LogWarning($"현재 방 정보 : {roomInfo}");
        }
    }
}
