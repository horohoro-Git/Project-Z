using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class ScreenCaptureEditor : EditorWindow
{
    [MenuItem("Tools/ScreenCapture")]
    public static void ShowWindow()
    {
        GetWindow<ScreenCaptureEditor>();
    }

    private void OnGUI()
    {
        GUILayout.Label("Press the button to take a screenshot");
        if (GUILayout.Button("Screenshot"))
        {
            TakeScreenshot();
        }
    }

    void TakeScreenshot()
    {
        string url = Path.Combine(Application.persistentDataPath, "screenshot.png"); // ��� ��ġ

        ScreenCapture.CaptureScreenshot(url); //������ ��� ���
        Debug.Log("Screenshot saved to: " + url);
    }
}
