using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class QuizStructure
{
   public string quizId;
   public string quizType; //input, ui 현재는 두가지 타입 존재
   public string questionText;
   public string correctAnswer;
   public string quizWrong;
   public string[] quizCorrects;

   public bool isSolved = false;
}
