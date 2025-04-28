using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TutorialData", menuName = "Tutorial/Create Tutorial Data")]
public class TutorialData : ScriptableObject
{
    public string tutorialName;
    public int heightSize;
    public Sprite tutorialImg;
}
