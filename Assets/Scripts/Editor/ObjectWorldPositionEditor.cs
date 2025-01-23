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

                        // �� ������ ȸ���� ���⿡ �°� ��ȯ (ȸ�� ����)
                        corners = new Vector3[4];
                        for (int i = 0; i < 4; i++)
                        {
                            // ȸ�� ����
                            corners[i] = selectedObject.transform.TransformPoint(localCorners[i]);
                            Debug.Log($"Corner {i + 1}: {corners[i]}");
                        }*/
           /* Vector3[] localCorners = new Vector3[4];
            localCorners[0] = new Vector3(-extents.x, 0, -extents.z);  // ���� �ϴ�
            localCorners[1] = new Vector3(extents.x, 0, -extents.z);   // ���� �ϴ�
            localCorners[2] = new Vector3(-extents.x, 0, extents.z);   // ���� ���
            localCorners[3] = new Vector3(extents.x, 0, extents.z);    // ���� ���

            // ��ü�� ȸ���� ���⿡ �°� �� �ڳʵ��� ��ȯ
            corners = new Vector3[4];
            for (int i = 0; i < 4; i++)
            {
                // �� �ڳʸ� ���� ��ǥ�� ��ȯ
                corners[i] = selectedObject.transform.TransformPoint(localCorners[i]);
                Debug.Log($"Corner {i + 1}: {corners[i]}");
            }*/

            // OBB �ð�ȭ (�⺻������ 4���� �ڳʸ� �ð�ȭ)
            for (int i = 0; i < 4; i++)
            {
                // ������ �ڳʵ� ���� ���� �׷��� OBB�� ��踦 �ð�ȭ
                Vector3 start = corners[i];
                Vector3 end = corners[(i + 1) % 4];  // ������ �ڳʴ� ù ��° �ڳʿ� ����
                Debug.DrawLine(start, end, Color.red, 10);  // �� ���� ���������� ����
            }
        }
        else
        {
            Debug.LogWarning("Selected object does not have a MeshRenderer component!");
        }
    }
  
}
