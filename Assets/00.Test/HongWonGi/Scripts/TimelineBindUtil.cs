using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public static class TimelineBindUtil
{
    /// <summary>
    /// 타임라인의 AnimationTrack 중 이름이 trackName인 트랙을 찾아 target Animator로 바인딩
    /// </summary>
    public static bool BindAnimatorToTrackName(PlayableDirector director, string trackName, Animator target)
    {
        if (!director || target == null) return false;

        var tl = director.playableAsset as TimelineAsset;
        if (tl == null) return false;

        // 이름이 trackName인 AnimationTrack 하나 찾기
        var track = tl.GetOutputTracks()
            .OfType<AnimationTrack>()
            .FirstOrDefault(t => string.Equals(t.name, trackName, System.StringComparison.Ordinal));

        if (track == null)
        {
            Debug.LogError($"[TimelineBind] AnimationTrack '{trackName}'을(를) {tl.name}에서 찾지 못했습니다.");
            return false;
        }

        director.SetGenericBinding(track, target);
        director.RebuildGraph();

        return true;
    }
}