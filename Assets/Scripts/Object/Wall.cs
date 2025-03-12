using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour, IBuildMaterials
{
    [NonSerialized]
    public int x;
    [NonSerialized] 
    public int y;
    public bool isDoor;
    public Renderer GetRenderer;
    Renderer IBuildMaterials.renderer { get { return GetRenderer; } }
    public StructureState structureState { get; set; }

    private void OnDestroy()
    {
        Clear();
    }
    public void Clear()
    {
        GetRenderer = null;
    }
}
