using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using Cysharp.Threading.Tasks;

public class LoadTimeLine : MonoBehaviour
{
    [SerializeField] private PlayableDirector _timeLine;
    [SerializeField] private string _eventID;
    //일단 finalstage_spirit용 타임라인 강제 실행 버전
    void Start()
    {
        if (DataManager.Instance._events[_eventID].isExecuted)
        {
            ForceEndTimeline(_timeLine).Forget();
            SoundManager.Instance.PlayBGM("Soundresource_102");
            PlayerController.Instance.blockLeftRight = true;
        }
    }

    // private void ForceEndTimeline(PlayableDirector director)
    // {
    //     director.time = director.duration;
    //     director.Evaluate();
    // }
    private async UniTaskVoid ForceEndTimeline(PlayableDirector director)
    {
        director.time = Mathf.Max(0, (float)director.duration - 0.01f);
        director.Play();
        await UniTask.WaitUntil(() => director.state != PlayState.Playing);
    }
}
