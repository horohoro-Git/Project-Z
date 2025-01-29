using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GridManager : MonoBehaviour
{
  
    static Queue<LineRenderer> lineRenderers = new Queue<LineRenderer>(200);

    static LineRenderer line;
    static GameObject lineRenderRoot;
    public static void CreateLines(LineRenderer lineRenderer, int num)
    {
        if (lineRenderRoot == null)
        {
            lineRenderRoot = new GameObject();
            lineRenderRoot.transform.position = Vector3.zero;
            lineRenderRoot.name = "LineRoot";
        }
        if (line == null) { line = lineRenderer; }

        for (int i = 0; i < num; i++)
        {
            LineRenderer lineRender = GameObject.Instantiate(lineRenderer, lineRenderRoot.transform.parent);
            lineRender.gameObject.transform.parent = lineRenderRoot.transform;
            lineRender.gameObject.SetActive(false);

            lineRenderers.Enqueue(lineRender);
        }
    }


    public static LineRenderer GetLine()
    {
        if (lineRenderers.Count > 0)
        {
            LineRenderer lineRenderer = lineRenderers.Dequeue();
            lineRenderer.gameObject.SetActive(true);
            return lineRenderer;
        }
        else
        {
            for (int i = 0; i < 10; i++)
            {
                LineRenderer newLineRenderer = GameObject.Instantiate(line, lineRenderRoot.transform.parent);
                newLineRenderer.gameObject.transform.parent = lineRenderRoot.transform;
                newLineRenderer.gameObject.SetActive(false);
                lineRenderers.Enqueue(newLineRenderer);
            }

            LineRenderer lineRenderer = lineRenderers.Dequeue();
            lineRenderer.gameObject.SetActive(true);
            return lineRenderer;
        }
    }

    public static void RemoveLine(LineRenderer lineRenderer)
    {
        if(lineRenderers.Count < 300)
        {
            lineRenderer.gameObject.SetActive(false);
            lineRenderers.Enqueue(lineRenderer);

        }
        else
        {
            Destroy(lineRenderer.gameObject);
        }
    }

    private void OnApplicationQuit()
    {
        while(lineRenderers.Count >0)
        {
            LineRenderer lineRenderer = lineRenderers.Dequeue();
            Destroy(lineRenderer.gameObject);
        }
    }
}
