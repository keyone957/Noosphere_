using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class QuizManager : Singleton<QuizManager>
{
    private QuizStructure _curQuiz;
    public InputFieldManager inputFieldManager;
    public Action OnQuizEnd;
    
    //퀴즈 설정
    public void SetQuiz(string quizID)
    {
        //quiz type이 input인 경우랑, ui인 경우 나눠서 생각
        _curQuiz = DataManager.Instance._quiz[quizID];
        Debug.Log($"현재 퀴즈 아이디 : {quizID}, 참조 : {_curQuiz}");

        if (_curQuiz.quizType == "input")
        {
            inputFieldManager.SetQuestionField(quizID);
        }
        else if (_curQuiz.quizType == "ui")
        {
            //현재 씬에서 quizId랑 동일한 이름을 갖고 있는 오브젝트가 있는지 파악
            //찾으면 해당 오브젝트의 UIBase 컴포넌트 가져와서 열기
            GameObject quizObject = GameObject.Find(_curQuiz.quizId);
            Debug.Log($"quiz obejct : {quizObject}, 아이디 : {_curQuiz.quizId}");
            UIBase script = quizObject.GetComponent<UIBase>();
            PlayerInteract.Instance.HideInteractionMark();
            UIManager.Instance.OpenUI(script,_curQuiz.quizId);
        }
    }
    
    public async UniTask DoCorrectResult(QuizStructure quiz)
    {
        await EventManager.Instance.DoResult(quiz.quizCorrects);
    }

    public async UniTask DoWrongResult(QuizStructure quiz)
    {
        string[] results = new string[1];
        results[0] = quiz.quizWrong;
        await EventManager.Instance.DoResult(results);
    }
}
