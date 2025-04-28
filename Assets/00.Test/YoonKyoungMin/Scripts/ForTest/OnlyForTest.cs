using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnlyForTest : MonoBehaviour
{
    //스테이지1 빠른 테스트를 위한 임시 마스터 코드

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("종료");
            Application.Quit();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("스테이지1로 바로 이동");
            GoToStage1();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            GoToStage2();
        }
    }

    void GoToStage1()
    {
        //이벤트 모두 실행
        foreach (var _event in DataManager.Instance._events)
        {
            string chapterIndex = _event.Key;
            if (chapterIndex[6] == 'A')
            {
                _event.Value.isExecuted = true;
                //증거물 모두 수집
                if (!string.IsNullOrEmpty(_event.Value.evidenceId))
                {
                    EvidenceStructure evidence = DataManager.Instance._evidences[_event.Value.evidenceId];

                    if (evidence.acquisitionType == 'Y')
                    {
                        InventoryManager.Instance.AddEvidence(evidence);
                    }
                    else if (evidence.acquisitionType == 'N')
                    {
                        evidence.accessCnt = 3;
                    }
                }
            }
        }

        //퀴즈 모두 정답
        DataManager.Instance._quiz["Quiz_001"].isSolved = true;
        //서브 증거물
        DataManager.Instance._evidences["Evidence_008"].accessCnt = 3;

        DialogueManager.Instance.OnDialogueEnd?.Invoke();
        UIManager.Instance.CloseAllUI();

        //씬 스테이지1로 이동
        SceneManager.LoadScene("Stage1Map_real");
        //현재 스테이지 변경
        EventManager.Instance.curRoomInfo = EventManager.RoomInfo.Room_102;
        EventManager.Instance.currentEventID = "Event_A031";
        EventManager.Instance.nextEventID = "";

        //플레이어 찾기
        FindObjectOfType<PlayerInteract>().transform.position = new Vector3(-7, 1, 3);
        FindObjectOfType<PlayerInteract>().isInsideTrigger = false;
        FindObjectOfType<PlayerInteract>().curTrigger = null;
        EventManager.Instance.nextEventID = "";
    }
    
    void GoToStage2()
    {
        //이벤트 모두 실행
        foreach (var _event in DataManager.Instance._events)
        {
            string chapterIndex = _event.Key;
            if (chapterIndex[6] == 'A')
            {
                _event.Value.isExecuted = true;
                //증거물 모두 수집
                if (!string.IsNullOrEmpty(_event.Value.evidenceId))
                {
                    EvidenceStructure evidence = DataManager.Instance._evidences[_event.Value.evidenceId];

                    if (evidence.acquisitionType == 'Y')
                    {
                        InventoryManager.Instance.AddEvidence(evidence);
                    }
                    else if (evidence.acquisitionType == 'N')
                    {
                        evidence.accessCnt = 3;
                    }
                }
            }
        }

        //퀴즈 모두 정답
        DataManager.Instance._quiz["Quiz_001"].isSolved = true;
        //서브 증거물
        DataManager.Instance._evidences["Evidence_008"].accessCnt = 3;
        
        //이벤트 모두 실행
        foreach (var _event in DataManager.Instance._events)
        {
            string chapterIndex = _event.Key;
            if (chapterIndex[6] == 'B')
            {
                _event.Value.isExecuted = true;
                //증거물 모두 수집
                if (!string.IsNullOrEmpty(_event.Value.evidenceId))
                {
                    EvidenceStructure evidence = DataManager.Instance._evidences[_event.Value.evidenceId];

                    if (evidence.acquisitionType == 'Y')
                    {
                        InventoryManager.Instance.AddEvidence(evidence);
                    }
                    else if (evidence.acquisitionType == 'N')
                    {
                        evidence.accessCnt = 3;
                    }
                }
            }
        }

        //퀴즈 모두 정답
        DataManager.Instance._quiz["Quiz_003"].isSolved = true;
        DataManager.Instance._quiz["Quiz_004"].isSolved = true;
        DataManager.Instance._quiz["Quiz_005"].isSolved = true;
        DataManager.Instance._quiz["Quiz_006"].isSolved = true;
        DataManager.Instance._quiz["Quiz_007"].isSolved = true;

        DialogueManager.Instance.OnDialogueEnd?.Invoke();
        UIManager.Instance.CloseAllUI();

        //씬 스테이지1로 이동
        SceneManager.LoadScene("Lounge");
        //현재 스테이지 변경
        EventManager.Instance.curRoomInfo = EventManager.RoomInfo.Room_104;
        EventManager.Instance.currentEventID = "Event_C063";
        EventManager.Instance.nextEventID = "";

        //플레이어 찾기
        FindObjectOfType<PlayerInteract>().transform.position = new Vector3(-7.61f, 0f, 4.96f);
        FindObjectOfType<PlayerInteract>().isInsideTrigger = false;
        FindObjectOfType<PlayerInteract>().curTrigger = null;
        EventManager.Instance.nextEventID = "";
    }

}