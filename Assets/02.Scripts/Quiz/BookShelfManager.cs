using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Debug = NooSphere.Debug;

public class BookShelfManager : UIBase
{
    [SerializeField] private string _curQuizID;
    private QuizStructure _curQuiz;
    public bool isBookClear=false;
    [SerializeField] private List<int> _answer; // 정답 순서
    [SerializeField] private Transform _bookParent;

    private List<Transform> _initialBookOrder;//맨처음 책 위치 저장하기 위한 리스트

    public override async void OnOpen(string quizID)
    {
        base.OnOpen(quizID);
        _curQuizID = "";
        _curQuiz = null;
        _curQuizID = quizID;
        _curQuiz = DataManager.Instance._quiz[_curQuizID];
        Debug.Log($"# quiz id : {quizID}, _curQuiz : {_curQuiz}");
        
        transform.GetChild(0).gameObject.SetActive(true);
        
        if (_initialBookOrder == null)//맨처음에만 책위치 저장
        {
            _initialBookOrder = new List<Transform>();
            foreach (Transform child in _bookParent)
            {
                if (child.TryGetComponent<BookDrag>(out _))
                    _initialBookOrder.Add(child);
            }
        }
        ResetBookOrder();
    }
    private void ResetBookOrder()
    {
        for (int i = 0; i < _initialBookOrder.Count; i++)
        {
            Transform left = _initialBookOrder[i];
            left.SetSiblingIndex(i);
            if (left.TryGetComponent<BookDrag>(out var drag) && drag._syncBook != null)
            {
                drag._syncBook.SetSiblingIndex(i);
            }
        }
    }
    public override void OnClose()
    {
        base.OnClose();
        transform.GetChild(0).gameObject.SetActive(false);
        UIManager.Instance.cctvFrame.SetActive(true);
        QuizManager.Instance.OnQuizEnd?.Invoke();
    }
    
    public async void CheckBookOrder()
    {
        List<int> currentOrder = new List<int>();
        
        foreach(Transform child in _bookParent)
        {
            // 활성화된 오브젝트이고 BookDrag 컴포넌트가 있는 경우만 체크
            if(child.gameObject.activeSelf && child.TryGetComponent<BookDrag>(out BookDrag book))
            {
                currentOrder.Add(book.bookIdx);
            }
        }

        
        // 정답 체크
        if(IsCorrectOrder(currentOrder))
        {
            //퀴즈 해결되었다고 표시
            _curQuiz.isSolved = true;
            SoundManager.Instance.PlaySFX("Soundresource_083");

            //트리거 삭제
            if (!DataManager.Instance._events["Event_B045"].isExecuted)
            {
                PlayerInteract.Instance.curTrigger = null;
                PlayerInteract.Instance.isInsideTrigger = false;
            }
            
            //PlayerInteract.Instance.HideInteractionMark();
            DataManager.Instance._events["Event_B044"].repeatType = false;
            DataManager.Instance._events["Event_B044"].isExecuted = true;
            await UniTask.Yield();
            UIManager.Instance.CloseTopUI();
        }
    }

    private bool IsCorrectOrder(List<int> currentOrder)
    {
        if(currentOrder.Count != _answer.Count) 
            return false;

        for(int i = 0; i < currentOrder.Count; i++)
        {
            if(currentOrder[i] != _answer[i])
                return false;
        }
        return true;
    }
    
    private async UniTask WaitForInvestigateEndAsync()
    {
        UIManager.Instance.OnSelectEnd = null;
        var tcs = new UniTaskCompletionSource();
        UIManager.Instance.OnSelectEnd += () => tcs.TrySetResult();
        await tcs.Task;
    }
    
    private async UniTask GetMirrorPiece()
    {
        /*
        UIManager.Instance.OpenUI(UIManager.Instance.investigateUI,DataManager.Instance._evidences["Evidence_019"]);
        //yes, no 선택 기다리기
        await WaitForInvestigateEndAsync();
        Debug.LogWarning($"Investigate UI 버튼 선택 다 기다림.");
                
        //yes라면
        if (UIManager.Instance.isYesClicked)
        {
            Debug.LogWarning("Investigate UI에서 YES를 선택함.");
                    
            //증거물 상세 ui가 닫힐 때까지 기다리기
            await UniTask.WaitUntil(() => !UIManager.Instance.IsAnyUIOpen());
            Debug.LogWarning("창 닫힐 때까지 다 기다림.");
        }
        else
        {
            //no라면
            Debug.LogWarning("Investigate UI에서 NO를 선택함.");
        }
        */
        DialogueManager.Instance.SetDialogue("Dialogue_0065");
        UIManager.Instance.cctvFrame.SetActive(true);
        DataManager.Instance._evidences["Evidence_019"].AcquireEvidence();
        MirrorPuzzleManager.Instance.GetMirrorPiece("Evidence_019");
        
        await UniTask.Yield();
        PlayerInteract.Instance.curTrigger = null;
        PlayerInteract.Instance.isInsideTrigger = false;
        //PlayerInteract.Instance.HideInteractionMark();
        await UniTask.Yield();
        DataManager.Instance._events["Event_B044"].repeatType = false;
    }
}
