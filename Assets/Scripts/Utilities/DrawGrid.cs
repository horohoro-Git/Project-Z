using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class DrawGrid : MonoBehaviour
{
    [SerializeField]
    Material mat;
    [SerializeField]
    LineRenderer lineRenderer;
   // int min = -10;
   // int max = 10;
    LineRenderer[,] lineRenderers = new LineRenderer[20, 20];
    LineRenderer currentLineRender;
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
       // Debug.Log(selectedCell);

   //     GameObject GO = new GameObject();
       // LineRenderer line = GO.AddComponent<LineRenderer>();
        if(currentLineRender!= null) GridManager.RemoveLine(currentLineRender);

        currentLineRender = GridManager.GetLine();
        int cellX = Mathf.FloorToInt(selectedCell.x / 2);
        int cellY = Mathf.FloorToInt(selectedCell.z / 2);
        Vector3[] corners = new Vector3[5];
        // 셀의 4개의 꼭지점 좌표 계산
        corners[0] = new Vector3(cellX * 2, 0.1f, cellY * 2);               // 왼쪽 아래
        corners[1] = new Vector3((cellX + 1) * 2, 0.1f, cellY * 2);         // 오른쪽 아래
        corners[2] = new Vector3((cellX + 1) * 2, 0.1f, (cellY + 1) * 2);   // 오른쪽 위
        corners[3] = new Vector3(cellX * 2, 0.1f, (cellY + 1) * 2);         // 왼쪽 위
        corners[4] = new Vector3(cellX * 2, 0.1f, cellY * 2);               // 왼쪽 아래
        
        // LineRenderer를 사용해 셀의 경계선 그리기
        currentLineRender.positionCount = 5;  // 4개의 꼭지점 + 시작점
        currentLineRender.SetPositions(corners);

        // 선 두께 설정
        currentLineRender.startWidth = 0.2f;  // 선의 두께
        currentLineRender.endWidth = 0.2f;
        currentLineRender.material = mat;
        currentLineRender.textureMode = LineTextureMode.Stretch;

        currentLineRender.numCapVertices = 10; // 끝부분의 버텍스를 설정하여 선 끝이 부드럽게 처리됨
        currentLineRender.numCornerVertices = 10;
    }
}
