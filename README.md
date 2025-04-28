# NoosPhere

진행 기간: 2024. 09 ~ 진행 중
사용한 기술 스택: C#, Unity
개발 인원(역할): 개발자 2명 + 기획자 2명 + 디자이너 1명 + 사운드 1명
한 줄 설명: 3D 추리 게임
비고: 팀 프로젝트/최우수상 수상작 / 진행중인 프로젝트

## 게임 플레이 풀 영상

---

[https://www.youtube.com/watch?v=5mgx3IzGAn8](https://www.youtube.com/watch?v=5mgx3IzGAn8)

- 깃허브

[https://github.com/keyone957/Noosphere_](https://github.com/keyone957/Noosphere_)

## 프로젝트 소개

---

![image.png](image.png)

## 나의 역할

---

1. 게임 데이터 관리
    - 구글 스프레드시트를 이용한 각종 이벤트를 관리하는 이벤트 매니저 및 게임 플로우 관리
    - 시트 내 데이터들을 리플렉션을 사용하여 객체화한 DataManager 구현
    - 대화 시스템 구성
2. UGUI 기반 게임 기믹(미니게임) 구현
    - 게임 내 라디오 게임, 책장 순서 맞추기 미니게임 등 모든 기믹들 구현
3. 연출 및 기능 구현
    - 포스트 프로세싱을 이용한 플레이어 정신세계 이동 연출 및 기능 구현
    - Unity Timeline 기능을 이용한 연출
    - 각종 게임 내 카메라 연출 구현
4. 플레이어 이동 및 물체 상호작용 구현
5. 게임 내 UI 구현
    - 대화 시스템, 타이틀 메뉴 등 다양한 UI 구현
    - Scriptable Object 기반 이벤트 채널 패턴을 활용한 UI 구현

### [구글 스프레드 시트를 이용한 게임 이벤트 관리]

![image.png](image%201.png)

=⇒ 이벤트 동작 방식 플로우 차트 

![image.png](image%202.png)

![image.png](image%203.png)

⇒ 구글 스프레드시트를 이용하여 다른 부서와 개발자 사이의 협업을 원활하게 진행

- 구글시트 CSV 파일을 파싱하여 실시간으로 데이터를 가져오도록 하였습니다.
- 추리 게임의 특성상 다양한 루트가 존재하므로 각 행의 이벤트를 객체화하여 여러 루트로 이어지게 하여 다양한 엔딩을 구현하였습니다.
- 기획자가 실시간으로 스프레드시트를 수정하면 바로 게임에 적용되게 구현하였습니다
- 모든 게임에 필요한 데이터(이벤트, 기믹, 증거물 상세 정보 및 속성, 대화, 소리, 사진 등…)를 모두 스프레드시트로 관리하여 다른 부서 간 협업이 원활하게 진행되도록 하였습니다.
- Eventmanager.cs
  https://github.com/keyone957/Noosphere_/blob/main/Assets/02.Scripts/Manager/EventManager.cs
    
- EventTrigger.cs
    
    ```csharp
    using System.Collections;
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using UnityEngine;
    using Debug = NooSphere.Debug;
    
    public class EventTrigger : MonoBehaviour
    {
        [Header("이벤트 목록")]
        public List<string> eventIdList = new List<string>();
        private EventStructure _curEvent;
        [Space(5)] [Header("트리거 설정")] 
        [SerializeField] private bool _canDestroy = false;
        [Space(5)][Header("NPC 관련")]
        public GameObject npcCameraPoint;
        public bool isNpc;
        
        
        //플레이어가 트리거 내에 진입한다면
        protected void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && PlayerInteract.Instance.canInteract)
            {
                CheckTriggerAvail();
                PlayerInteract.Instance.isInsideTrigger = true;
                PlayerInteract.Instance.curTrigger = this;
                
                if (isNpc)
                {
                    PlayerController.Instance.npcCam = npcCameraPoint;
                    PlayerController.Instance.npcState = transform.GetChild(0).GetComponent<NpcState>();
                }
            }
        }
        
        //플레이어가 트리거 내에서 나간다면
        protected void OnTriggerExit(Collider other)
        {
            //플레이어에게 ? 없애기
            PlayerInteract.Instance.HideInteractionMark();
            PlayerInteract.Instance.isInsideTrigger = false;
            PlayerInteract.Instance.curTrigger = null;
            
            //현재 이벤트 초기화하기
            _curEvent = null;
            
            //npc 변수 초기화
            PlayerController.Instance.npcCam = null;
            PlayerController.Instance.npcState = null;
            
            //플레이어에게 할당된 액션 다 초기화
            PlayerInteract.Instance.OnInteract = null;
            PlayerInteract.Instance.OnMentalInteract = null;
        }
    
        public void CheckTriggerAvail()
        {
            bool canExecute = false;
            foreach (var eventID in eventIdList)
            {
                if (EventManager.Instance.CheckExecutable(eventID))
                {
                    _curEvent = DataManager.Instance._events[eventID];
                    canExecute = true;
                    break;
                }
            }
    
            if (canExecute)
            {
                string[] results = EventManager.Instance.CheckConditions(_curEvent);
    
                //결과 실행
                if (results != null && results.Length > 0)
                {
                    if (!EventManager.Instance.IsConditionMet() &&
                        string.IsNullOrEmpty(_curEvent.conditionFalseResults[0]))
                    {
                        Debug.LogWarning($"{_curEvent.eventId}는 조건을 만족하지 못했으나 conditionFalseResult도 존재하지 않아 할당하지 않음");
                        return;
                    }
                    //트리거 종류에 따라 할당하기
                    switch (tag)
                    {
                        case "EventTrigger":
                            //바로 실행 메소드 호출
                            EventManager.Instance.ExecuteEvent(_curEvent.eventId).Forget();
                            Debug.Log("바로 실행");
                            break;
                        case "EventInteractionTrigger":
                            //플레이어에게 ? 띄우기
                            PlayerInteract.Instance.ShowInteractionMark();
                            PlayerInteract.Instance.OnInteract = null;
                            Debug.Log("OnInteract 할당");
                            //플레이어의 OnInteract 액션에 실행 메소드 할당
                            PlayerInteract.Instance.OnInteract += () =>
                            {
                                EventManager.Instance.ExecuteEvent(_curEvent.eventId).Forget();
                            };
                            break;
                        case "EventMentalEnterTrigger":
                            //플레이어에게 ? 띄우기
                            PlayerInteract.Instance.ShowInteractionMark();
                            PlayerInteract.Instance.OnMentalInteract = null;
                            Debug.Log("OnMentalInteract에 할당");
                            //정신세계 진입 프로세스의 OnInteract 액션에 실행 메소드 할당
                            PlayerInteract.Instance.OnMentalInteract += () =>
                            {
                                EventManager.Instance.ExecuteEvent(_curEvent.eventId).Forget();
                            };
                            break;
                    }
                }
            }
        }
    }
    ```
    

## 개발 내용 및 플레이 영상

---

### [기본 시스템]

![증거물획득.gif](%25EC%25A6%259D%25EA%25B1%25B0%25EB%25AC%25BC%25ED%259A%258D%25EB%2593%259D.gif)

특정한 위치 or 조건을 충족하여 상호작용하면 다양한 이벤트가 실행이 된다.  

![능력사용.gif](%25EB%258A%25A5%25EB%25A0%25A5%25EC%2582%25AC%25EC%259A%25A9.gif)

능력을 사용하여 정신세계로 진입.

![인벤토리.mp4_20250309_153131.gif](%EC%9D%B8%EB%B2%A4%ED%86%A0%EB%A6%AC.mp4_20250309_153131.gif)

인벤토리를 열어 게임 플레이 도중 얻은 증거물을 상세하게 살펴보거나 특정 위치에서 사용할 수 있다.

### [기믹]

![라디오기믹.gif](%25EB%259D%25BC%25EB%2594%2594%25EC%2598%25A4%25EA%25B8%25B0%25EB%25AF%25B9.gif)

올바른 주파수를 입력하여 정답을 맞춘다.

![벽시계.gif](%25EB%25B2%25BD%25EC%258B%259C%25EA%25B3%2584.gif)

올바른 시간을 맞춰서 정답을 맞춘다

![책장기믹.gif](%25EC%25B1%2585%25EC%259E%25A5%25EA%25B8%25B0%25EB%25AF%25B9.gif)

올바른 순서로 책을 배치하여 정답을 맞춘다

![거울기믹.gif](%25EA%25B1%25B0%25EC%259A%25B8%25EA%25B8%25B0%25EB%25AF%25B9.gif)

각종 기믹들을 풀고 맵 내에 숨겨진 거울 조각을 획득하고 올바른 위치에 조각을 배치하여 정답을 맞춘다

### [연출]

![컷씬연출.gif](%25EC%25BB%25B7%25EC%2594%25AC%25EC%2597%25B0%25EC%25B6%259C.gif)

게임 플레이 도중 컷씬을 실행 시키는 연출

![감금연출.gif](%25EA%25B0%2590%25EA%25B8%2588%25EC%2597%25B0%25EC%25B6%259C.gif)

카메라를 흔드는 연출

![new.mp4_20250309_154140.gif](new.mp4_20250309_154140.gif)

![카메라 연출.mp4_20250309_154301.gif](%EC%B9%B4%EB%A9%94%EB%9D%BC_%EC%97%B0%EC%B6%9C.mp4_20250309_154301.gif)

NPC에 따른 고유한 카메라 위치 이동 연출

## 프로젝트 사용기술

---

### ⚒️ 클라이언트

- Unity
- C#

### ⚒️ 버전 관리 및 협업

- Git
- Notion
- Figma
- Google Sheet

### ⚒️ 개발 환경

- Visual Studio
