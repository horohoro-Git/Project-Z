using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets;
using UnityEngine;
using System.IO;

public class GetBundleNameEditor : EditorWindow
{
    string selectedFileName = "Home";
    private string[] fileNames = { "Home", "Map1", "Map2" };


    [MenuItem("Tools/GetBundleNameEditor")]
    public static void ShowWindow()
    {
        GetWindow<GetBundleNameEditor>("Get Bundle Name");
    }
    private void OnGUI()
    {
        int selectedIndex = ArrayIndexOf(fileNames, selectedFileName);
        selectedIndex = EditorGUILayout.Popup("Select Profile", selectedIndex, fileNames);
        selectedFileName = fileNames[selectedIndex];
   
        if(GUILayout.Button("Show Bundles Name"))
        {
            string path = $"ServerData/{selectedFileName}/StandaloneWindows64";
            if (Directory.Exists(path))
            {
                string[] files = Directory.GetFiles(path);
                foreach (string filePath in files)
                {
                    string fileName = Path.GetFileName(filePath);
                    Debug.Log(fileName);
                }
            }
        }
    }

    private int ArrayIndexOf(string[] array, string value)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] == value)
            {
                return i;
            }
        }
        return -1;
    }
}
