using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

public class PersistantDataEditor : EditorWindow
{
    //파일 열기
    [MenuItem("Tools/OpenPersistantData %`")]
    static void OpenFolder()
    {
        string folderPath = Application.persistentDataPath;
        Application.OpenURL(folderPath);
    }

    [MenuItem("Tools/OpenPersistantData %`", validate = true)]
    static bool CheckPlayingOpenPersistantData()
    {

        return !EditorApplication.isPlaying && Directory.Exists(Application.persistentDataPath);

    }

    //세이브 파일 제거
    [MenuItem("Tools/RemoveSaveData &`")]
    static void Remove()
    {
        string url = Path.Combine(Application.persistentDataPath, "Save");
        DeleteFolder(url);
    }

    [MenuItem("Tools/RemoveSaveData &`", validate = true)]
    static bool CheckPlayingRemoveSaveData()
    {

        return !EditorApplication.isPlaying && Directory.Exists(Application.persistentDataPath);

    }
    static void DeleteFolder(string folderPath)
    {
        // 폴더 내 모든 파일 삭제
        foreach (string file in Directory.GetFiles(folderPath))
        {
            File.Delete(file);
        }

        foreach (string subDirectory in Directory.GetDirectories(folderPath))
        {
            DeleteFolder(subDirectory);  // 재귀적으로 폴더 내부를 삭제
        }

        Directory.Delete(folderPath, true); //파일 삭제

        UnityEngine.Debug.Log("Save data folder deleted.");
    }
}
