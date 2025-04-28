# NoosPhere

진행 기간: 2024. 09 ~ 진행 중

사용한 기술 스택: C#, Unity

개발 인원(역할): 개발자 2명 + 기획자 2명 + 디자이너 1명 + 사운드 1명

한 줄 설명: 3D 추리 게임

비고: 팀 프로젝트/최우수상 수상작 / 진행중인 프로젝트

## 게임 플레이 풀 영상

---

[게임 플레이 풀영상](https://www.youtube.com/watch?v=5mgx3IzGAn8)


## 프로젝트 소개

---

![Image](https://github.com/user-attachments/assets/e2220397-9438-43ab-9d71-c634e22450b6)

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

![Image](https://github.com/user-attachments/assets/cd88a576-5cff-440c-8d4e-f42be3c9e403)

=⇒ 이벤트 동작 방식 플로우 차트 

![Image](https://github.com/user-attachments/assets/690efdab-3a76-416d-8f09-22624046cb4a)


![Image](https://github.com/user-attachments/assets/ab92bbe9-3487-49a5-b609-143161d5dc4f)

⇒ 구글 스프레드시트를 이용하여 다른 부서와 개발자 사이의 협업을 원활하게 진행

- 구글시트 CSV 파일을 파싱하여 실시간으로 데이터를 가져오도록 하였습니다.
- 추리 게임의 특성상 다양한 루트가 존재하므로 각 행의 이벤트를 객체화하여 여러 루트로 이어지게 하여 다양한 엔딩을 구현하였습니다.
- 기획자가 실시간으로 스프레드시트를 수정하면 바로 게임에 적용되게 구현하였습니다
- 모든 게임에 필요한 데이터(이벤트, 기믹, 증거물 상세 정보 및 속성, 대화, 소리, 사진 등…)를 모두 스프레드시트로 관리하여 다른 부서 간 협업이 원활하게 진행되도록 하였습니다.
- Eventmanager.cs
  [Eventmanager.cs](https://github.com/keyone957/Noosphere_/blob/main/Assets/02.Scripts/Manager/EventManager.cs)
    
- EventTrigger.cs
  [EventTrigger.cs](https://github.com/keyone957/Noosphere_/blob/main/Assets/00.Test/YoonKyoungMin/Scripts/EventTriggers/EventTrigger.cs)
## 개발 내용 및 플레이 영상

---

### [기본 시스템]

![Image](https://github.com/user-attachments/assets/c6e63205-8d3a-4c1b-b738-f2b764e78b7c)

특정한 위치 or 조건을 충족하여 상호작용하면 다양한 이벤트가 실행이 된다.  

![Image](https://github.com/user-attachments/assets/e2045f56-3ba2-4ff8-b95e-742331f66675)

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

![Image](https://github.com/user-attachments/assets/04b981eb-41b5-4d5c-b8cf-e1397ea7cc11)

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
