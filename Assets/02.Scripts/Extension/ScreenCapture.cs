#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ScreenCapture : MonoBehaviour
{
    /// <summary>
    /// This adds an entry to the top main menu and a shortcut CTRL+ALT+S and stores files without transparency to Assets/{TimeStamp}.png
    /// </summary>
    [MenuItem("My Stuff/CaptureEditorScreenshot %&S")]
    private static void CaptureEditorScreenshot()
    {
        var path = Path.Combine(Application.dataPath, DateTime.Now.ToString("yyyy_MM_dd-hh_mm_ss") + ".png");

        CaptureEditorScreenshot(path);
    }

    public static void CaptureEditorScreenshot(string filePath)
    {
        var sw = SceneView.lastActiveSceneView;

        if (!sw)
        {
            Debug.LogError("Unable to capture editor screenshot, no scene view found");

            return;
        }

        var cam = sw.camera;

        if (!cam)
        {
            Debug.LogError("Unable to capture editor screenshot, no camera attached to current scene view");

            return;
        }

        var renderTexture = cam.targetTexture;

        if (!renderTexture)
        {
            Debug.LogError("Unable to capture editor screenshot, camera has no render texture attached");

            return;
        }

        var width = renderTexture.width;
        var height = renderTexture.height;

        var outputTexture = new Texture2D(width, height, TextureFormat.RGB24, false);

        RenderTexture.active = renderTexture;

        cam.Render();

        outputTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0);

        var pngData = outputTexture.EncodeToPNG();

        UnityEngine.Object.DestroyImmediate(outputTexture);

        RenderTexture.active = null;

        File.WriteAllBytes(filePath, pngData);

        AssetDatabase.Refresh();

        Debug.Log("Screenshot written to file " + filePath);
    }
}
#endif