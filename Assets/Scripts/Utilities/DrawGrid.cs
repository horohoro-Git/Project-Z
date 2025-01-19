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
    int min = -10;
    int max = 10;
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
       // Debug.Log(selectedCell);

   //     GameObject GO = new GameObject();
       // LineRenderer line = GO.AddComponent<LineRenderer>();
        if(currentLineRender!= null) GridManager.RemoveLine(currentLineRender);

        currentLineRender = GridManager.GetLine();
        int cellX = Mathf.FloorToInt(selectedCell.x / 2);
        int cellY = Mathf.FloorToInt(selectedCell.z / 2);
        Vector3[] corners = new Vector3[5];
        // ���� 4���� ������ ��ǥ ���
        corners[0] = new Vector3(cellX * 2, 0.1f, cellY * 2);               // ���� �Ʒ�
        corners[1] = new Vector3((cellX + 1) * 2, 0.1f, cellY * 2);         // ������ �Ʒ�
        corners[2] = new Vector3((cellX + 1) * 2, 0.1f, (cellY + 1) * 2);   // ������ ��
        corners[3] = new Vector3(cellX * 2, 0.1f, (cellY + 1) * 2);         // ���� ��
        corners[4] = new Vector3(cellX * 2, 0.1f, cellY * 2);               // ���� �Ʒ�
        
        // LineRenderer�� ����� ���� ��輱 �׸���
        currentLineRender.positionCount = 5;  // 4���� ������ + ������
        currentLineRender.SetPositions(corners);

        // �� �β� ����
        currentLineRender.startWidth = 0.2f;  // ���� �β�
        currentLineRender.endWidth = 0.2f;
        currentLineRender.material = mat;
        currentLineRender.textureMode = LineTextureMode.Stretch;

        currentLineRender.numCapVertices = 10; // ���κ��� ���ؽ��� �����Ͽ� �� ���� �ε巴�� ó����
        currentLineRender.numCornerVertices = 10;
        Debug.Log((cellX + 5) + " " + (cellY + 5));
    }
}
