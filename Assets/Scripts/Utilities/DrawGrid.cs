using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class DrawGrid : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Draw();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Draw()
    {
        for (int i = 0; i <= 10; i++)
        {
            GameObject lineRender = new GameObject("Horizontal" + i);
            LineRenderer horizontalRender = lineRender.AddComponent<LineRenderer>();
           // horizontalRender.material = default;
           // horizontalRender.enabled = true;
            horizontalRender.startWidth = 0.05f;
            horizontalRender.endWidth = 0.05f;


            Vector3[] points = new Vector3[2];
            points[0] = new Vector3(-10, 0, i * 2 - 10);
            points[1] = new Vector3(10 , 0, i * 2 - 10);

            horizontalRender.positionCount = points.Length;
            horizontalRender.SetPositions(points);
        }

        for (int i = 0; i <= 10; i++)
        {
            GameObject verticalLine = new GameObject("VerticalLine" + i);
            LineRenderer verticalRenderer = verticalLine.AddComponent<LineRenderer>();
          //  verticalRenderer.material = lineMaterial;
            verticalRenderer.startWidth = 0.05f;
            verticalRenderer.endWidth = 0.05f;

            // 점 설정
            Vector3[] points = new Vector3[2];
            points[0] = new Vector3(i * 2 -10, 0, -10);  // 시작점
            points[1] = new Vector3(i * 2 -10, 0, 10);  // 끝점

            verticalRenderer.positionCount = points.Length;
            verticalRenderer.SetPositions(points);
        }
    }
}
