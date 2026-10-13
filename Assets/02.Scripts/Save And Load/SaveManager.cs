using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NooSphere
{
    public enum GameLoadType
    {
        NewGame,
        ContinueGame,
        LoadComplete
    }
    public class SaveManager : Singleton<SaveManager>
    {
        public GameLoadType CurrentLoadType { get; private set; }
        public int selectSlotIndex { get; private set; }
        public bool OnDoorAutoSave = false;
        private string folderPath;
        [SerializeField] private GameObject _player;
        public List<GameObject> _essentialUIs = new List<GameObject>();
        [SerializeField] private float _autoSaveDelay;

        private void Awake()
        {
            CurrentLoadType = GameLoadType.NewGame;
            // 싱글톤 인스턴스가 이미 존재하는 경우, 중복 생성 방지
            if (Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            // 초기화 작업
            DontDestroyOnLoad(gameObject);
        }
        private void Start()
        {
            folderPath = Path.Combine(Application.persistentDataPath, "Saves");
            selectSlotIndex = -1;
        }

        public void SetLoadType(GameLoadType type)
        {
            CurrentLoadType = type;
        }

        public void SetSlotIndex(int index)
        {
            selectSlotIndex = index;
        }

        // 세이브 슬롯1에 현재 진행상황을 자동 저장 시 호출되는 함수
        // TODO : 상태 값이 모두 저장되는 동안 플레이어의 상호작용을 막은 채, 저장 중 UI를 띄우는 작업 연결 해야 함.
        public IEnumerator DoSave()
        {
            // Root 하위에 Saves 폴더로 이어지는 경로 찾기
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            SaveCurrentState();

            yield return null;
        }

        public IEnumerator DoAutoSave()
        {
            // Root 하위에 Saves 폴더로 이어지는 경로 찾기
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // 저장 시작 준비 -> 저장 중 UI 활성화 & 상호작용 막기
            PlayerInteract.Instance.canInteract = false;
            DefaultUIController.Instance.OpenUI(DefaultUIController.Instance.savingUI);

            SaveCurrentState();
            yield return new WaitForSeconds(_autoSaveDelay);

            // 저장 끝 -> 저장 중 UI 비활성화 & 상호작용 풀기
            DefaultUIController.Instance.CloseTopUI();
            PlayerInteract.Instance.canInteract = true;
        }

        // 자동 저장 혹은 사용자 임의 저장 시, 현재 상태를 저장하는 함수
        // 폴더 경로와, 저장하려는 파일 이름을 파라미터로 받음
        void SaveCurrentState()
        {
            // 저장할 파일 경로 찾기
            string filePath = Path.Combine(folderPath, $"slot{selectSlotIndex}.es3");

            // 현재 상태 저장하기, 저장할 데이터는 다음과 같음.
            // DataManager의 _events, _evidences, _quiz 리스트.
            // 저장 당시 플레이어의 Transform 정보
            // 저장 당시 플레이어가 위치한 공간 정보
            // 저장 일시(YYYY - MM - DD  HH: MM:SS 형식)
            // 저장 당시까지의 플레이 타임
            try
            {
                ES3.Save("EventDatas", DataManager.Instance._events, filePath);
                ES3.Save("EvidenceDatas", DataManager.Instance._evidences, filePath);
                ES3.Save("QuizDatas", DataManager.Instance._quiz, filePath);
                ES3.Save("PlayerTransform", PlayerController.Instance.transform, filePath);
                ES3.Save("SceneName", SceneManager.GetActiveScene().name, filePath);

                // 인벤토리 챕터 별로 저장
                List<string> evienceKeys = new List<string>();
                foreach(var chapter in InventoryManager.Instance.chapterInventories)
                {
                    foreach(var evidence in chapter.Value.evidences)
                    {
                        evienceKeys.Add(evidence.evidenceId);
                    }
                }
                ES3.Save("InventoryDatas", evienceKeys, filePath);

                // 현재 이벤트 상태 저장
                ES3.Save("CurrentEventID", EventManager.Instance.currentEventID, filePath);
                ES3.Save("NextEventID", EventManager.Instance.nextEventID, filePath);
                Debug.LogWarning("현재 이벤트, 나중 이벤트" + EventManager.Instance.currentEventID + "," + EventManager.Instance.nextEventID);

                // 정신세계 진입 관련 상태 저장
                MentalEnterProcess mentalInfo = PlayerController.Instance.GetComponent<MentalEnterProcess>();
                ES3.Save("CombackEventID", mentalInfo.GetComebackEventId(), filePath);
                ES3.Save("IsInMental", PlayerInteract.Instance.isInMental, filePath);
                ES3.Save("MentalInfo", mentalInfo.mentalInfo, filePath);
                


                if (FindObjectOfType<RoomInfoManager>() is RoomInfoManager roomInfoManager)
                {
                    ES3.Save("Location",roomInfoManager.roomName, filePath);
                }
                else
                {
                    Debug.LogError("플레이어의 현재 Location 값을 찾을 수 없습니다.");
                }

                string currentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                ES3.Save("DateTime", currentTime, filePath);

                // 이 곳에 플레이타임 불러와서 저장해야 함.
                float playTime = PlayTime.Instance.GetPlayTime();
                ES3.Save("PlayTime", playTime, filePath);
            }
            catch (System.IO.IOException)
            {
                Debug.LogError("파일이 열려있거나, 충분한 저장공간이 없습니다.");
            }
            catch (System.Security.SecurityException)
            {
                Debug.LogError("권한이 없는 사용자입니다.");
            }
        }

        // 슬롯에 저장된 데이터를 불러와 반영하는 함수
        // 슬롯에 세이브 파일이 있는지 HasSaveData 메소드로 체크했다는 가정.
        // TODO : 저장되어 있는 값을 반영하는 동안, 로딩 화면 띄워야 함.
        // TODO : 플레이타임 값을 반영하여, 다시 플레이타임을 재개해야 함.
        public void LoadSaveData()
        {
            // 슬롯에 해당하는 세이브 파일이 있는지 확인
            string filePath = Path.Combine(folderPath, $"slot{selectSlotIndex}.es3");
            if (File.Exists(filePath))
            {
                // 데이터들 반영
                DataManager.Instance._events = ES3.Load("EventDatas", filePath, DataManager.Instance._events);
                DataManager.Instance._evidences = ES3.Load("EvidenceDatas", filePath, DataManager.Instance._evidences);
                DataManager.Instance._quiz = ES3.Load("QuizDatas", filePath, DataManager.Instance._quiz);
            }
            else
            {
                Debug.LogError($"{filePath}에 파일이 존재하지 않습니다.");
            }
        }

        // 마지막 저장 씬 이름 가져오기
        public string GetSceneName(int index)
        {
            string filePath = Path.Combine(folderPath, $"slot{index}.es3");
            return ES3.Load<string>("SceneName", filePath);
        }

        // 슬롯에 해당하는 세이브 파일이 있는지 확인하는 함수
        public bool HasSaveData(int index)
        {
            string filePath = Path.Combine(folderPath, $"slot{index}.es3");
            bool value = File.Exists(filePath) ? true : false;
            return value;
        }

        // 위치, 플레이타임, 저장일시 등 텍스트 데이터 불러오기
        // 데이터 존재를 검증(HasSaveData) 후 호출하는 함수. 유효성 검증 완료되었다고 가정하에 작성.
        public string GetLocationData(int index)
        {
            string filePath = Path.Combine(folderPath, $"slot{index}.es3");
            return ES3.Load<string>("Location", filePath);
        }
        public string GetPlayTimeData(int index)
        {
            string filePath = Path.Combine(folderPath, $"slot{index}.es3");
            return PlayTime.Instance.FormatPlayTime(ES3.Load<float>("PlayTime", filePath));
        }

        public float GetPlayTimeFloatData(int index)
        {
            string filePath = Path.Combine(folderPath, $"slot{index}.es3");
            return ES3.Load<float>("PlayTime", filePath);
        }
        public string GetDateTimeData(int index)
        {
            string filePath = Path.Combine(folderPath, $"slot{index}.es3");
            return ES3.Load<string>("DateTime", filePath);
        }

        public Transform GetPlayerTransform(int index)
        {
            string filePath = Path.Combine(folderPath, $"slot{index}.es3");
            return ES3.Load<Transform>("PlayerTransform", filePath);
        }


        // 슬롯 데이터 삭제하는 함수
        public void DeleteSlotData(int index)
        {
            string filePath = Path.Combine(folderPath, $"slot{index}.es3");
            ES3.DeleteFile(filePath);
            selectSlotIndex = -1;
        }

        public void DoAutoSaveDelay()
        {
            NooSphere.SaveManager.Instance.SetSlotIndex(1);
            StartCoroutine(DoAutoSave());
        }

        public string GetCurrentLocation()
        {
            if (FindObjectOfType<RoomInfoManager>() is RoomInfoManager roomInfoManager)
            {
                return roomInfoManager.roomName;
            }
            else
            {
                Debug.LogError("플레이어의 현재 Location 값을 찾을 수 없습니다.");
                return "Unknown Location";
            }
        }

        public string GetCurrentPlayTime()
        {
            // 이 곳에 플레이타임 불러와서 저장해야 함.
            float playTime = PlayTime.Instance.GetPlayTime();
            return PlayTime.Instance.FormatPlayTime(playTime);
        }

        public string GetCurrentDateTime()
        {
            string currentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            return currentTime;
        }

        // 전체 세이브 데이터 삭제
        public void ResetAllSaveData()
        {
            DeleteSlotData(1);
            DeleteSlotData(2);
            DeleteSlotData(3);
        }

        public bool IsAnySaveDataExists()
        {
            return HasSaveData(1) || HasSaveData(2) || HasSaveData(3);
        }
    }
}