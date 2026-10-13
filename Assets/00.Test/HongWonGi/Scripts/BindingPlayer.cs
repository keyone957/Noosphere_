using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine;
public class BindingPlayer : MonoBehaviour
{
    [SerializeField] PlayableDirector director;
    [SerializeField] private string trackName;

    void Start()
    {
        director.played += BindingPlayerObj;
    }

    private void BindingPlayerObj(PlayableDirector obj)
    {
        Animator playerAnim = PlayerController.Instance.GetComponent<Animator>();
        TimelineBindUtil.BindAnimatorToTrackName(director, trackName, playerAnim);
    }

}
