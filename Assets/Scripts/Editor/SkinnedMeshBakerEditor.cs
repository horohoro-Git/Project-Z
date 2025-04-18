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

        // 메쉬 생성 및 베이크
        Mesh bakedMesh = new Mesh();
        smr.BakeMesh(bakedMesh);
        bakedMesh.name = "Baked_" + smr.name;
        // 저장 경로 설정
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

        // 프리팹 저장
        string prefabPath = "Assets/Edited/test/Baked_" + smr.name + ".prefab";
        PrefabUtility.SaveAsPrefabAsset(newObj, prefabPath);
        DestroyImmediate(newObj);
    }
}
