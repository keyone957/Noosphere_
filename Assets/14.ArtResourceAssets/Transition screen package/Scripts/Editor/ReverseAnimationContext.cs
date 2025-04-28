using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;

public static class ReverseAnimationContext
{

    [MenuItem("Assets/Create Reversed Clip", false, 14)]
    private static void ReverseClip()
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        string directoryPath = Path.GetDirectoryName(path);
        string fileName = Path.GetFileName(path).Split('.')[0];
        string fileExtension = Path.GetExtension(path);
        string copiedFilePath = Path.Combine(directoryPath, $"{fileName}_Reversed{fileExtension}");
        AssetDatabase.CopyAsset(path, copiedFilePath);

        var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(copiedFilePath);

        if (clip == null)
        {
            return;
        }

        float clipLength = clip.length;
        var editorBindings = AnimationUtility.GetCurveBindings(clip);
        var curves = AnimationUtility.GetAllCurves(clip, true);
        
        foreach (AnimationClipCurveData curve in curves)
        {
            var keys = curve.curve.keys;
            int keyCount = keys.Length;
            var postWrapmode = curve.curve.postWrapMode;
            curve.curve.postWrapMode = curve.curve.preWrapMode;
            curve.curve.preWrapMode = postWrapmode;
            for (int i = 0; i < keyCount; i++)
            {
                Keyframe K = keys[i];
                K.time = clipLength - K.time;
                var tmp = -K.inTangent;
                K.inTangent = -K.outTangent;
                K.outTangent = tmp;
                keys[i] = K;
            }
            curve.curve.keys = keys;
            clip.SetCurve(curve.path, curve.type, curve.propertyName, curve.curve);
        }

        /*foreach (var binding in editorBindings)
        {
            var curve = AnimationUtility.GetEditorCurve(clip, binding);
            var keys = curve.keys;

            var postWrapmode = curve.postWrapMode;
            curve.postWrapMode = curve.preWrapMode;
            curve.preWrapMode = postWrapmode;

            for (int i = 0; i < keys.Length; i++)
            {
                var K = keys[i];
                K.time = clipLength - K.time;

                var tmp = -K.inTangent;
                K.inTangent = -K.outTangent;
                K.outTangent = tmp;

                keys[i] = K;
            }

            curve.keys = keys;
            clip.SetCurve(binding.path, binding.type, binding.propertyName, curve);
        }*/

        var events = AnimationUtility.GetAnimationEvents(clip);
        foreach (var @event in events)
        {
            @event.time = clipLength - @event.time;
        }

        AnimationUtility.SetAnimationEvents(clip, events);

        Debug.Log("Animation reversed!");
    }

    [MenuItem("Assets/Create Reversed Clip", true)]
    private static bool ReverseClipValidation() => Selection.activeObject is AnimationClip;

    private static AnimationClip SelectedClip =>
        Selection.GetFiltered<AnimationClip>(SelectionMode.Assets).FirstOrDefault();
}