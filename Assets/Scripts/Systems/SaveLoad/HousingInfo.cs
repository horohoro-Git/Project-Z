using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MaterialsType
{
    None,
    Floor,
    Wall,
    Door,
    Furniture
}
public struct HousingInfo
{
    public MaterialsType materialsType;
    public int x;
    public int y;
    public int z;
    public int id;

    public HousingInfo(int x, int y, int z, MaterialsType type, int id)
    {
        this.materialsType = type;
        this.x = x;
        this.y = y;
        this.z = z;
        this.id = id;
    }
}
