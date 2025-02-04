using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MaterialsType
{
    None,
    Floor,
    Wall,
    Door
}
public struct HousingInfo
{
    public MaterialsType materialsType;
    public int x;
    public int y;
    public int z;

    public HousingInfo(int x, int y, int z, MaterialsType type)
    {
        this.materialsType = type;
        this.x = x;
        this.y = y;
        this.z = z;
    }
}
