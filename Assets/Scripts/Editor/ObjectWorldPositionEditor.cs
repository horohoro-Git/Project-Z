using log4net.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ObjectWorldPositionEditor : EditorWindow
{
    GameObject selectedObject;
    Vector3[] corners = new Vector3[4];

    [MenuItem("Tools/Get Object's World Position")]
    public static void ShowWindow()
    {
        GetWindow<ObjectWorldPositionEditor>("World Position");
    }

    private void OnGUI()
    {
        GUILayout.Label("Select Object for View Min/Max WorldPosition");

        selectedObject = (GameObject)EditorGUILayout.ObjectField("Object", selectedObject,typeof(GameObject),true);

        if (GUILayout.Button("Show Positions"))
        {
            if (selectedObject != null)
            {
                ShowPositions();
             
            }
            else
            {
                Debug.LogWarning("Select HireArchy's Object");
            }
        }

        if(selectedObject != null) 
        {
            for (int i = 0; i < 4; i++)
            {
                GUILayout.Label($"Corner {i + 1} : {corners[i]}");
            }


        }


    }

    void ShowPositions()
    {
        MeshRenderer meshRenderer = selectedObject.GetComponent<MeshRenderer>();

        if (meshRenderer != null)
        {
            Bounds bounds = meshRenderer.bounds;

            Vector3 center = bounds.center;

            Vector3 extents = bounds.extents;


            corners[0] = new Vector3(center.x - extents.x, center.y, center.z - extents.z);
            corners[1] = new Vector3(center.x - extents.x, center.y, center.z + extents.z);
            corners[2] = new Vector3(center.x + extents.x, center.y, center.z + extents.z);
            corners[3] = new Vector3(center.x + extents.x, center.y, center.z - extents.z);

            /*
                        Vector3[] localCorners = new Vector3[4];
                        localCorners[0] = new Vector3(-extents.x, 0, -extents.z);
                        localCorners[1] = new Vector3(extents.x, 0, -extents.z); 
                        localCorners[2] = new Vector3(-extents.x, 0, extents.z);
                        localCorners[3] = new Vector3(extents.x, 0, extents.z); 

                        // 각 끝점을 회전된 방향에 맞게 변환 (회전 적용)
                        corners = new Vector3[4];
                        for (int i = 0; i < 4; i++)
                        {
                            // 회전 적용
                            corners[i] = selectedObject.transform.TransformPoint(localCorners[i]);
                            Debug.Log($"Corner {i + 1}: {corners[i]}");
                        }*/
           /* Vector3[] localCorners = new Vector3[4];
            localCorners[0] = new Vector3(-extents.x, 0, -extents.z);  // 좌측 하단
            localCorners[1] = new Vector3(extents.x, 0, -extents.z);   // 우측 하단
            localCorners[2] = new Vector3(-extents.x, 0, extents.z);   // 좌측 상단
            localCorners[3] = new Vector3(extents.x, 0, extents.z);    // 우측 상단

            // 객체의 회전된 방향에 맞게 각 코너들을 변환
            corners = new Vector3[4];
            for (int i = 0; i < 4; i++)
            {
                // 각 코너를 월드 좌표로 변환
                corners[i] = selectedObject.transform.TransformPoint(localCorners[i]);
                Debug.Log($"Corner {i + 1}: {corners[i]}");
            }*/

            // OBB 시각화 (기본적으로 4개의 코너만 시각화)
            for (int i = 0; i < 4; i++)
            {
                // 인접한 코너들 간의 선을 그려서 OBB의 경계를 시각화
                Vector3 start = corners[i];
                Vector3 end = corners[(i + 1) % 4];  // 마지막 코너는 첫 번째 코너와 연결
                Debug.DrawLine(start, end, Color.red, 10);  // 선 색은 빨간색으로 설정
            }
        }
        else
        {
            Debug.LogWarning("Selected object does not have a MeshRenderer component!");
        }
    }
  
}
