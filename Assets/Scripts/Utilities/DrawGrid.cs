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

            // �� ����
            Vector3[] points = new Vector3[2];
            points[0] = new Vector3(i * 2 -10, 0, -10);  // ������
            points[1] = new Vector3(i * 2 -10, 0, 10);  // ����

            verticalRenderer.positionCount = points.Length;
            verticalRenderer.SetPositions(points);
        }
    }

    public void Select(Vector3 selectedCell)
    {
      /*  int cellX = Mathf.FloorToInt(worldPosition.x / cellWidth);
        int cellY = Mathf.FloorToInt(worldPosition.y / cellHeight);

        // ���� 4���� ������ ��ǥ ���
        corners[0] = new Vector3(cellX * cellWidth, cellY * cellHeight, 0);               // ���� �Ʒ�
        corners[1] = new Vector3((cellX + 1) * cellWidth, cellY * cellHeight, 0);         // ������ �Ʒ�
        corners[2] = new Vector3((cellX + 1) * cellWidth, (cellY + 1) * cellHeight, 0);   // ������ ��
        corners[3] = new Vector3(cellX * cellWidth, (cellY + 1) * cellHeight, 0);         // ���� ��

        // LineRenderer�� ����� ���� ��輱 �׸���
        lineRenderer.positionCount = 5;  // 4���� ������ + ������
        lineRenderer.SetPositions(new Vector3[] { corners[0], corners[1], corners[2], corners[3], corners[0] });

        // �� �β� ����
        lineRenderer.startWidth = 0.1f;  // ���� �β�
        lineRenderer.endWidth = 0.1f;*/
    }
}
