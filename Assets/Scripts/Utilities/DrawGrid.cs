using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class DrawGrid : MonoBehaviour
{
    [SerializeField]
    LineRenderer lineRenderer;
    int min = -10;
    int max = 10;
    LineRenderer[,] lineRenderers = new LineRenderer[20, 20]; 
    // Start is called before the first frame update
    private void Awake()
    {
        GameInstance.Instance.drawGrid = this;
        GridManager.CreateLines(lineRenderer, 200);
    }
    void Start()
    {
        Draw();

    }

    void Draw()
    {
        for (int i = 0; i <= 10; i++)
        {
            LineRenderer horizontalRender = GridManager.GetLine();
            horizontalRender.name = string.Format("Horizontal {0}", i); 

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
            LineRenderer verticalRenderer = GridManager.GetLine();
            verticalRenderer.name = string.Format("Vertical {0}", i);
          
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

    public void Select(Vector3 selectedCell)
    {
      /*  int cellX = Mathf.FloorToInt(worldPosition.x / cellWidth);
        int cellY = Mathf.FloorToInt(worldPosition.y / cellHeight);

        // 셀의 4개의 꼭지점 좌표 계산
        corners[0] = new Vector3(cellX * cellWidth, cellY * cellHeight, 0);               // 왼쪽 아래
        corners[1] = new Vector3((cellX + 1) * cellWidth, cellY * cellHeight, 0);         // 오른쪽 아래
        corners[2] = new Vector3((cellX + 1) * cellWidth, (cellY + 1) * cellHeight, 0);   // 오른쪽 위
        corners[3] = new Vector3(cellX * cellWidth, (cellY + 1) * cellHeight, 0);         // 왼쪽 위

        // LineRenderer를 사용해 셀의 경계선 그리기
        lineRenderer.positionCount = 5;  // 4개의 꼭지점 + 시작점
        lineRenderer.SetPositions(new Vector3[] { corners[0], corners[1], corners[2], corners[3], corners[0] });

        // 선 두께 설정
        lineRenderer.startWidth = 0.1f;  // 선의 두께
        lineRenderer.endWidth = 0.1f;*/
    }
}
