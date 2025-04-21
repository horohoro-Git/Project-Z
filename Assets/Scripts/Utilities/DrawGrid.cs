using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class DrawGrid : MonoBehaviour
{
    [SerializeField]
    Material highlightMat;
    [SerializeField]
    Material gridMat;
    [SerializeField]
    LineRenderer lineRenderer;
    [SerializeField]
    Material planeMat;

    List<LineRenderer> lines = new List<LineRenderer>();
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
    public void Draw()
    {
        planeMat.SetFloat("_EnableGrid", 1);


     /*   for (int i = 0; i <= 50; i++)
        {
            LineRenderer horizontalRender = GridManager.GetLine();
            horizontalRender.name = string.Format("Horizontal {0}", i);

            horizontalRender.startWidth = 0.05f;
            horizontalRender.endWidth = 0.05f;


            Vector3[] points = new Vector3[2];
            points[0] = new Vector3(-50, 0, i * 2 - 50);
            points[1] = new Vector3(50, 0, i * 2 - 50);

            horizontalRender.positionCount = points.Length;
            horizontalRender.SetPositions(points);
            horizontalRender.material = null;
            lines.Add(horizontalRender);
        }

        for (int i = 0; i <= 50; i++)
        {
            LineRenderer verticalRenderer = GridManager.GetLine();
            verticalRenderer.name = string.Format("Vertical {0}", i);

            verticalRenderer.startWidth = 0.05f;
            verticalRenderer.endWidth = 0.05f;

            Vector3[] points = new Vector3[2];
            points[0] = new Vector3(i * 2 - 50, 0, -50);  // 시작점
            points[1] = new Vector3(i * 2 - 50, 0, 50);  // 끝점

            verticalRenderer.positionCount = points.Length;
            verticalRenderer.material = null;
            verticalRenderer.SetPositions(points);
            lines.Add(verticalRenderer);
        }*/
    }

    public void Remove()
    {
        planeMat.SetFloat("_EnableGrid", 0);
     /*   for (int i = lines.Count - 1; i >= 0; i--)
        {
            LineRenderer lineRenderer = lines[i];
            lines.RemoveAt(i);
            GridManager.RemoveLine(lineRenderer);
        }*/
        RemoveHighlight();
    }

    public void RemoveHighlight()
    {
        if (currentLineRender != null) GridManager.RemoveLine(currentLineRender);
        currentLineRender = null;
    }

    public bool Select(Vector3 selectedCell, ref int x, ref int y)
    {
        int cellX = Mathf.FloorToInt(selectedCell.x / 2);
        int cellY = Mathf.FloorToInt(selectedCell.z / 2);

        if (x == cellX && y == cellY && currentLineRender != null) return false;

        x= cellX; y= cellY;
      //  Debug.Log(x + " " + y); 
        if (currentLineRender!= null) GridManager.RemoveLine(currentLineRender);

        if(x >= 49 || x < -49 || y >= 49 || y< -49)
        {
            currentLineRender = null;
            return false;
        }

        currentLineRender = GridManager.GetLine();
       
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
        currentLineRender.material = highlightMat;
        currentLineRender.textureMode = LineTextureMode.Stretch;

        currentLineRender.numCapVertices = 10; // 끝부분의 버텍스를 설정하여 선 끝이 부드럽게 처리됨
        currentLineRender.numCornerVertices = 10;

        return true;
    }
}
