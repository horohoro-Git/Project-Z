using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SkinnedMeshBakerEditor : EditorWindow
{
    static GameObject selectedObject;
    static string selectedObjectName;
    [MenuItem("Tools/Bake Selected SkinnedMeshRenderer")]
    static void SkinnedMeshBaker()
    {
        GetWindow<SkinnedMeshBakerEditor>();
    }
    private void OnGUI()
    {
        GUILayout.Label("Bake Selected SkinnedMeshRenderer");
        selectedObject = (GameObject)EditorGUILayout.ObjectField("Object", selectedObject, typeof(GameObject), true);
        selectedObjectName = EditorGUILayout.TextField("Name", selectedObjectName);
        if (GUILayout.Button("Bake"))
        {
            if (selectedObject != null)
            {
                BakeSelectedMesh();

            }
        }
    }
    static void BakeSelectedMesh()
    {
  
        SkinnedMeshRenderer smr = selectedObject.GetComponentInChildren<SkinnedMeshRenderer>();
        if (smr == null)
        {
            Debug.LogError("SkinnedMeshRenderer Not Found");
            return;
        }

        // �޽� ���� �� ����ũ
        Mesh bakedMesh = new Mesh();
        smr.BakeMesh(bakedMesh);
        bakedMesh.name = "Baked_" + smr.name;
        // ���� ��� ����
        string path = "Assets/Edited/test/Baked_" + smr.name + ".asset";
        AssetDatabase.CreateAsset(bakedMesh, path);
        AssetDatabase.SaveAssets();

        Debug.Log("Bake Complete: " + path);

        GameObject newObj = new GameObject("Baked_" + smr.name);
        SkinnedMeshRenderer newSmr = newObj.AddComponent<SkinnedMeshRenderer>();
        newSmr.sharedMesh = bakedMesh;
        newSmr.sharedMaterials = smr.sharedMaterials;
        newSmr.bones = smr.bones;
        newSmr.rootBone = smr.rootBone;

        // ������ ����
        string prefabPath = "Assets/Edited/test/Baked_" + smr.name + ".prefab";
        PrefabUtility.SaveAsPrefabAsset(newObj, prefabPath);
        DestroyImmediate(newObj);
    }
}
