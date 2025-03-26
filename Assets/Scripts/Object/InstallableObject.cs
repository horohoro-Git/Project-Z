using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstallableObject : MonoBehaviour, IBuildMaterials
{
    public StructureState structureState { get; set; }

    public Renderer GetRenderer;
    Renderer IBuildMaterials.renderer { get { return GetRenderer; } }

    public FurnitureType furnitureType;
    public MaterialsType materialType;
    public BuildWallDirection buildWallDirection;
    public int assetID;
    public void Clear()
    {
      
    }

}
