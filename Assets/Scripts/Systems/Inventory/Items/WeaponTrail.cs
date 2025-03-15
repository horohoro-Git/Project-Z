using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponTrail : MonoBehaviour
{
    TrailRenderer lineRenderer;
    public TrailRenderer GetTrailRenderer { get { if (lineRenderer == null) lineRenderer = GetComponent<TrailRenderer>(); return lineRenderer; } }

    public Transform weaponTransform;
    public float trailDuration = 0.5f; 
    private float trailTime = 0.1f;
    bool trail;

    private void Start()
    {
        GetTrailRenderer.enabled = false;
        GetTrailRenderer.time = trailTime;
        GetTrailRenderer.startWidth = 0.1f;
        GetTrailRenderer.endWidth = 0.05f;

    }

    public void Trail(bool on)
    {
        trail = on;

        GetTrailRenderer.enabled = true;
        Invoke("TrailOff", 0.5f);
    }

    void TrailOff()
    {

    }
}
