using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponTrail : MonoBehaviour
{
    //TrailRenderer lineRenderer;
    public TrailRenderer GetTrailRenderer;

    public Transform weaponTransform;
   // public float trailDuration = 0.5f; 
    private float trailTime = 0.5f;

    public float startWidth;
    public float endWidth;
    private List<Vector3> trailPositions = new List<Vector3>();
 //   private float lastSaveTime = 0f;
    public float saveInterval = 0.01f;
    public int trailLength = 100;

    public LineRenderer lineRenderer;
    bool trail;
    bool updated;
    private void Start()
    {
        GetTrailRenderer.enabled = false;
        GetTrailRenderer.time = trailTime;
        GetTrailRenderer.startWidth = 0.2f;
        GetTrailRenderer.endWidth = 0.2f;
     //   GetTrailRenderer.widthCurve.AddKey(0, 0.2f);
     //   GetTrailRenderer.widthCurve.AddKey(0.2f, 0.1f);
    //    GetTrailRenderer.minVertexDistance = 0.25f;
        GetTrailRenderer.alignment = LineAlignment.TransformZ;
        //GetTrailRenderer.emitting = false;

   //     lineRenderer.startWidth = 0.2f;
   //     lineRenderer.endWidth = 0.2f;
    }
 

    private void Update()
    {
        if (trail && lineRenderer)
        {
            if (updated)
            {
                updated = false;
                lineRenderer.enabled = true;
            }
            SavePosition();

            DrawTrail();
        }
        else if(lineRenderer)
        {
            if (updated)
            {
                updated = false;
                lineRenderer.enabled = false;
            }
        }
    }
    void SavePosition()
    {
        trailPositions.Add(lineRenderer.transform.position);
/*
        if (trailPositions.Count > trailLength)
        {
            trailPositions.RemoveAt(0);
        }*/
    }
    public void Trail(bool on)
    {
        updated = true;
        if(!on) DrawTrail();
        if (on)
        {
            trailPositions.Clear();
            lineRenderer.positionCount = 0;
        }
       // GetTrailRenderer.enabled = on;
       
        trail = on;
        lineRenderer.startWidth = 0.2f;
        lineRenderer.endWidth = 0.2f;
    }

    void DrawTrail()
    {
        if (trailPositions.Count < 2)
            return;

        List<Vector3> smoothedPositions = new List<Vector3>();

        // Catmull Rom º¸°£
        for (int i = 0; i < trailPositions.Count - 1; i++)
        {
            Vector3 p0 = i - 1 >= 0 ? trailPositions[i - 1] : trailPositions[i];
            Vector3 p1 = trailPositions[i];
            Vector3 p2 = trailPositions[i + 1];
            Vector3 p3 = i + 2 < trailPositions.Count ? trailPositions[i + 2] : trailPositions[i + 1];

            smoothedPositions.AddRange(CatmullRom(p0, p1, p2, p3, 5));
        }

        lineRenderer.positionCount = smoothedPositions.Count;
        for (int i = 0; i < smoothedPositions.Count; i++)
        {
            lineRenderer.SetPosition(i, smoothedPositions[i]);
        }
    }

    List<Vector3> CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, int segments)
    {
        List<Vector3> result = new List<Vector3>();

        for (int i = 0; i <= segments; i++)
        {
            float t = i / (float)segments;
            result.Add(CatmullRomFormula(p0, p1, p2, p3, t));
        }

        return result;
    }

    Vector3 CatmullRomFormula(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float t2 = t * t;
        float t3 = t2 * t;

        return 0.5f * (
            (2f * p1) +
            (-p0 + p2) * t +
            (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
            (-p0 + 3f * p1 - 3f * p2 + p3) * t3
        );
    }
}
