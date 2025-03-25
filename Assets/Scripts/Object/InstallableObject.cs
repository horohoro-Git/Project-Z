using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstallableObject : MonoBehaviour, IBuildMaterials
{
    public StructureState structureState { get; set; }

    public Renderer GetRenderer;
    Renderer IBuildMaterials.renderer { get { return GetRenderer; } }

    public void Clear()
    {
      
    }

}
