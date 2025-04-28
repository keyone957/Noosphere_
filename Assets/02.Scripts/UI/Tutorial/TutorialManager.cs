using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Debug = NooSphere.Debug;
public class TutorialManager : Singleton<TutorialManager>
{
    public Dictionary<string, TutorialData> tutorialDictionary = new Dictionary<string, TutorialData>();

    [SerializeField]
    private List<TutorialData> tutorialDataList;

    [SerializeField] private GameObject _prefabHeight132;
    [SerializeField] private GameObject _prefabHeight217;

    void Awake()
    {
        // 리스트 딕셔너리 변환
        foreach (var tutorial in tutorialDataList)
        {
            if (tutorial != null && tutorial.tutorialImg != null)
            {
                tutorialDictionary[tutorial.tutorialName] = tutorial;
            }
        }
    }

    public void ShowTutorial(string tutorialName)
    {
        //유효성 검사
        if (!string.IsNullOrEmpty(tutorialName) && tutorialDictionary.ContainsKey(tutorialName))
        {
            TutorialData data = tutorialDictionary[tutorialName];
            GameObject prefab = _prefabHeight132;
            if (data.heightSize == 132)
            {
                prefab = _prefabHeight132;
            }
            else if (data.heightSize == 217)
            {
                prefab = _prefabHeight217;
            }

            prefab.GetComponent<Image>().sprite = data.tutorialImg;
            //자식오브젝트로 생성하기
            Instantiate(prefab, transform);
        }
    }
}
