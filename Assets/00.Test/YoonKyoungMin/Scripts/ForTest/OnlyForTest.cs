using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnlyForTest : Singleton<OnlyForTest>
{
    ////스테이지1 빠른 테스트를 위한 임시 마스터 코드
    private bool _prologuePass = false;
    private bool _stage1Pass = false;
    private bool _stage2Pass = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            Debug.Log("종료");
            Application.Quit();
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            Debug.LogWarning("스테이지1로 바로 이동");
            SoundManager.Instance.StopAllSFX();
            PlayerInteract.Instance.OnInteract = null;
            PlayerInteract.Instance.OnMentalInteract = null;
            DialogueManager.Instance.OnDialogueEnd?.Invoke();
            EffectManager.Instance.OnEffectEnd?.Invoke();
            GoToStage1();
        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
            Debug.LogWarning("스테이지2로 바로 이동");
            SoundManager.Instance.StopAllSFX();
            PlayerInteract.Instance.OnInteract = null;
            PlayerInteract.Instance.OnMentalInteract = null;
            DialogueManager.Instance.OnDialogueEnd?.Invoke();
            EffectManager.Instance.OnEffectEnd?.Invoke();
            GoToStage2();
        }

        // 진엔딩
        if (Input.GetKeyDown(KeyCode.F4))
        {
            Debug.LogWarning("진엔딩으로 바로 이동");
            SoundManager.Instance.StopAllSFX();
            PlayerInteract.Instance.OnInteract = null;
            PlayerInteract.Instance.OnMentalInteract = null;
            DialogueManager.Instance.OnDialogueEnd?.Invoke();
            EffectManager.Instance.OnEffectEnd?.Invoke();
            GoToRealEnding();
        }

        // 일반엔딩 - 탈출 성공
        if (Input.GetKeyDown(KeyCode.F5))
        {
            Debug.LogWarning("일반엔딩 탈출 성공 으로 바로 이동");
            SoundManager.Instance.StopAllSFX();
            PlayerInteract.Instance.OnInteract = null;
            PlayerInteract.Instance.OnMentalInteract = null;
            DialogueManager.Instance.OnDialogueEnd?.Invoke();
            EffectManager.Instance.OnEffectEnd?.Invoke();
            GoToCommonEnding1();
        }

        // 일반엔딩 - 탈출 실패
        if (Input.GetKeyDown(KeyCode.F6))
        {
            Debug.LogWarning("일반엔딩 탈출 실패로 바로 이동");
            SoundManager.Instance.StopAllSFX();
            PlayerInteract.Instance.OnInteract = null;
            PlayerInteract.Instance.OnMentalInteract = null;
            DialogueManager.Instance.OnDialogueEnd?.Invoke();
            EffectManager.Instance.OnEffectEnd?.Invoke();
            GoToCommonEnding2();
        }
    }

    void GoToStage1()
    {
        UIManager.Instance.CloseAllUI();
        EventManager.Instance.nextEventID = "";
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

        InventoryManager.Instance.AddEvidence(DataManager.Instance._evidences["Evidence_011"]);
        //퀴즈 모두 정답
        DataManager.Instance._quiz["Quiz_001"].isSolved = true;
        //서브 증거물
        DataManager.Instance._evidences["Evidence_008"].accessCnt = 3;

        //씬 스테이지1로 이동
        SceneManager.LoadScene("Lounge");
        //현재 스테이지 변경
        EventManager.Instance.curRoomInfo = EventManager.RoomInfo.Room_102;
        EventManager.Instance.currentEventID = "Event_A031";
        EventManager.Instance.nextEventID = "";

        //플레이어 찾기
        FindObjectOfType<PlayerInteract>().transform.position = new Vector3(0, 0.7f, 5);
        FindObjectOfType<PlayerInteract>().isInsideTrigger = false;
        FindObjectOfType<PlayerInteract>().curTrigger = null;

        _prologuePass = true;
        EventManager.Instance.curChapterInfo = EventManager.ChapterInfo.Stage1;
    }

    void GoToStage2()
    {
        if (!_prologuePass)
        {
            EventManager.Instance.nextEventID = "";
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

            InventoryManager.Instance.AddEvidence(DataManager.Instance._evidences["Evidence_011"]);
            //퀴즈 모두 정답
            DataManager.Instance._quiz["Quiz_001"].isSolved = true;
            //서브 증거물
            DataManager.Instance._evidences["Evidence_008"].accessCnt = 3;

            _prologuePass = true;
        }

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

        InventoryManager.Instance.AddEvidence(DataManager.Instance._evidences["Evidence_016"]);
        InventoryManager.Instance.AddEvidence(DataManager.Instance._evidences["Evidence_010"]);
        //퀴즈 모두 정답
        DataManager.Instance._quiz["Quiz_003"].isSolved = true;
        DataManager.Instance._quiz["Quiz_004"].isSolved = true;
        DataManager.Instance._quiz["Quiz_005"].isSolved = true;
        DataManager.Instance._quiz["Quiz_006"].isSolved = true;
        DataManager.Instance._quiz["Quiz_007"].isSolved = true;
        DataManager.Instance._quiz["Quiz_009"].isSolved = true;

        DialogueManager.Instance.OnDialogueEnd?.Invoke();
        EffectManager.Instance.OnEffectEnd?.Invoke();
        UIManager.Instance.CloseAllUI();

        //씬 스테이지1로 이동
        SceneManager.LoadScene("Lounge");
        //현재 스테이지 변경
        EventManager.Instance.curRoomInfo = EventManager.RoomInfo.Room_104;
        EventManager.Instance.currentEventID = "Event_C063";
        DataManager.Instance._events["Event_C063"].isExecuted = true;
        EventManager.Instance.nextEventID = "";

        //플레이어 찾기
        FindObjectOfType<PlayerInteract>().transform.position = new Vector3(-7f, 0.7f, 5f);
        FindObjectOfType<PlayerInteract>().isInsideTrigger = false;
        FindObjectOfType<PlayerInteract>().curTrigger = null;
        EventManager.Instance.nextEventID = "";

        _stage1Pass = true;
        EventManager.Instance.curChapterInfo = EventManager.ChapterInfo.Stage2;
    }

    void GoToRealEnding()
    {
        if (!_prologuePass)
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

            InventoryManager.Instance.AddEvidence(DataManager.Instance._evidences["Evidence_011"]);
            //퀴즈 모두 정답
            DataManager.Instance._quiz["Quiz_001"].isSolved = true;
            //서브 증거물
            DataManager.Instance._evidences["Evidence_008"].accessCnt = 3;

            _prologuePass = true;
        }

        if (!_stage1Pass)
        {
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
            DataManager.Instance._quiz["Quiz_009"].isSolved = true;

            InventoryManager.Instance.AddEvidence(DataManager.Instance._evidences["Evidence_016"]);
            InventoryManager.Instance.AddEvidence(DataManager.Instance._evidences["Evidence_010"]);
            InventoryManager.Instance.AddEvidence(DataManager.Instance._evidences["Evidence_025"]);

            DialogueManager.Instance.OnDialogueEnd?.Invoke();
            UIManager.Instance.CloseAllUI();

            _stage1Pass = true;
        }


        if (!_stage2Pass)
        {
            //이벤트 모두 실행
            foreach (var _event in DataManager.Instance._events)
            {
                string chapterIndex = _event.Key;
                if (chapterIndex[6] == 'C')
                {
                    if (_event.Value.evidenceId == "Event_C067") continue;
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

            _stage2Pass = true;

            DialogueManager.Instance.OnDialogueEnd?.Invoke();
            EffectManager.Instance.OnEffectEnd?.Invoke();
            UIManager.Instance.CloseAllUI();

            SceneManager.LoadScene("FinalStage_Spirit");
            //현재 스테이지 변경
            EventManager.Instance.curRoomInfo = EventManager.RoomInfo.Room_103;

            //플레이어 찾기
            FindObjectOfType<PlayerInteract>().isInsideTrigger = false;
            FindObjectOfType<PlayerInteract>().curTrigger = null;
            EventManager.Instance.nextEventID = "";

            EventManager.Instance.curChapterInfo = EventManager.ChapterInfo.Final;
        }
    }

    // 일반 엔딩 - 탈출 성공
    void GoToCommonEnding1()
    {
        if (!_prologuePass)
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

            InventoryManager.Instance.AddEvidence(DataManager.Instance._evidences["Evidence_011"]);
            //퀴즈 모두 정답
            DataManager.Instance._quiz["Quiz_001"].isSolved = true;
            //서브 증거물
            DataManager.Instance._evidences["Evidence_008"].accessCnt = 3;

            _prologuePass = true;
        }

        if (!_stage1Pass)
        {
            //이벤트 모두 실행
            foreach (var _event in DataManager.Instance._events)
            {
                string chapterIndex = _event.Key;
                if (chapterIndex[6] == 'B')
                {
                    if (_event.Value.evidenceId == "Event_B066") continue;
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

            EvidenceStructure paper = DataManager.Instance._evidences["Evidence_016"];
            InventoryManager.Instance.AddEvidence(paper);

            //퀴즈 모두 정답
            DataManager.Instance._quiz["Quiz_003"].isSolved = true;
            DataManager.Instance._quiz["Quiz_004"].isSolved = true;
            DataManager.Instance._quiz["Quiz_005"].isSolved = true;
            DataManager.Instance._quiz["Quiz_006"].isSolved = true;
            DataManager.Instance._quiz["Quiz_007"].isSolved = true;
            DataManager.Instance._quiz["Quiz_009"].isSolved = true;

            DialogueManager.Instance.OnDialogueEnd?.Invoke();
            UIManager.Instance.CloseAllUI();

            _stage1Pass = true;
        }


        if (!_stage2Pass)
        {
            //이벤트 모두 실행
            foreach (var _event in DataManager.Instance._events)
            {
                string chapterIndex = _event.Key;
                if (chapterIndex[6] == 'C')
                {
                    if (_event.Value.evidenceId == "Event_C066") continue;
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

            _stage2Pass = true;

            InventoryManager.Instance.AddEvidence(DataManager.Instance._evidences["Evidence_010"]);
            InventoryManager.Instance.AddEvidence(DataManager.Instance._evidences["Evidence_025"]);

            DialogueManager.Instance.OnDialogueEnd?.Invoke();
            EffectManager.Instance.OnEffectEnd?.Invoke();
            UIManager.Instance.CloseAllUI();

            SceneManager.LoadScene("FinalStage_Spirit");
            //현재 스테이지 변경
            EventManager.Instance.curRoomInfo = EventManager.RoomInfo.Room_103;

            //플레이어 찾기
            FindObjectOfType<PlayerInteract>().isInsideTrigger = false;
            FindObjectOfType<PlayerInteract>().curTrigger = null;
            EventManager.Instance.nextEventID = "";

            EventManager.Instance.curChapterInfo = EventManager.ChapterInfo.Final;
            DataManager.Instance._events["Event_C066"].isExecuted = false;
            Debug.LogWarning($"Event_C066 : {DataManager.Instance._events["Event_C066"].isExecuted}");
        }
    }

    // 일반 엔딩 - 탈출 실패
    void GoToCommonEnding2()
    {
        if (!_prologuePass)
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

            InventoryManager.Instance.AddEvidence(DataManager.Instance._evidences["Evidence_011"]);
            //퀴즈 모두 정답
            DataManager.Instance._quiz["Quiz_001"].isSolved = true;
            //서브 증거물
            DataManager.Instance._evidences["Evidence_008"].accessCnt = 3;

            _prologuePass = true;
        }

        if (!_stage1Pass)
        {
            //이벤트 모두 실행
            foreach (var _event in DataManager.Instance._events)
            {
                string chapterIndex = _event.Key;
                if (chapterIndex[6] == 'B')
                {
                    if (_event.Value.evidenceId == "Event_B065") continue;
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
            DataManager.Instance._quiz["Quiz_009"].isSolved = true;

            DialogueManager.Instance.OnDialogueEnd?.Invoke();
            UIManager.Instance.CloseAllUI();

            _stage1Pass = true;
        }


        if (!_stage2Pass)
        {
            //이벤트 모두 실행
            foreach (var _event in DataManager.Instance._events)
            {
                string chapterIndex = _event.Key;
                if (chapterIndex[6] == 'C')
                {
                    if (_event.Value.evidenceId == "Event_C066") continue;
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

            _stage2Pass = true;

            InventoryManager.Instance.AddEvidence(DataManager.Instance._evidences["Evidence_010"]);
            InventoryManager.Instance.AddEvidence(DataManager.Instance._evidences["Evidence_025"]);

            DialogueManager.Instance.OnDialogueEnd?.Invoke();
            EffectManager.Instance.OnEffectEnd?.Invoke();
            UIManager.Instance.CloseAllUI();

            SceneManager.LoadScene("FinalStage_Spirit");
            //현재 스테이지 변경
            EventManager.Instance.curRoomInfo = EventManager.RoomInfo.Room_103;

            //플레이어 찾기
            FindObjectOfType<PlayerInteract>().isInsideTrigger = false;
            FindObjectOfType<PlayerInteract>().curTrigger = null;
            EventManager.Instance.nextEventID = "";

            EventManager.Instance.curChapterInfo = EventManager.ChapterInfo.Final;
            DataManager.Instance._events["Event_C066"].isExecuted = false;
            Debug.LogWarning($"Event_C066 : {DataManager.Instance._events["Event_C066"].isExecuted}");
        }
    }
}