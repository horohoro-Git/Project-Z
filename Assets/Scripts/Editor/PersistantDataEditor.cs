using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

public class PersistantDataEditor : EditorWindow
{
    //���� ����
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

    //���̺� ���� ����
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
        // ���� �� ��� ���� ����
        foreach (string file in Directory.GetFiles(folderPath))
        {
            File.Delete(file);
        }

        foreach (string subDirectory in Directory.GetDirectories(folderPath))
        {
            DeleteFolder(subDirectory);  // ��������� ���� ���θ� ����
        }

        Directory.Delete(folderPath, true); //���� ����

        UnityEngine.Debug.Log("Save data folder deleted.");
    }
}
