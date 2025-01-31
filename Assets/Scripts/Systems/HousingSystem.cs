using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;

public class HousingSystem : MonoBehaviour
{
    public enum BuildWallDirection
    {
        None,
        Left,
        Right,
        Top,
        Bottom,
    }

    [NonSerialized]
    public int minx = -15 / 2;
    [NonSerialized]
    public int miny = -15 / 2;
    GameObject[,] floors = new GameObject[30,30];//0 == -15 
    Wall[,,] walls = new Wall[30,30,2];
    private void Awake()
    {
        GameInstance.Instance.housingSystem = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }


    public bool CheckFloor(int x, int y)
    {
        
        return floors[x - minx,y - miny] != null;
    }
    public void BuildFloor(int x, int y, GameObject floor)
    {
        floors[x - minx,y - miny] = floor;
    }




    public bool CheckWall(int x, int y, BuildWallDirection build)
    {
        Wall w = GetBuildedWall(x,y,build);
        return w != null;
     /*   switch (build)
        {
            case BuildWallDirection.None:
                return walls[x - minx, y - miny,0] != null;
            case BuildWallDirection.Left:
                return walls[x - minx, y - miny, 0] != null;
            case BuildWallDirection.Right:
                return walls[x - minx + 1, y - miny, 0] != null;
            case BuildWallDirection.Top:
                return walls[x - minx, y - miny + 1, 1] != null;
            case BuildWallDirection.Bottom:
                return walls[x - minx, y - miny, 1] != null;
            default: return false;
        }*/
    }


    public void BuildWall(int x, int y, Wall wall, BuildWallDirection build)
    {
        //  GameObject w = GetBuildedWall(x, y, build);
        //  w = wall;

        switch (build)
        {
            case BuildWallDirection.None:
                break;
            case BuildWallDirection.Left:
                walls[x - minx, y - miny, 0] = wall;
                break;
            case BuildWallDirection.Right:
                walls[x - minx + 1, y - miny, 0] = wall;
                break;
            case BuildWallDirection.Top:
                walls[x - minx, y - miny + 1, 1] = wall;
                break;
            case BuildWallDirection.Bottom:
                walls[x - minx, y - miny, 1] = wall;
                break;
        }
    }

    public void RemoveFloor(int x, int y)
    {
        if(CheckFloor(x, y))
        {
            Destroy(floors[x - minx, y - miny].gameObject);
            floors[x - minx, y - miny] = null;
        }
    }

    public void RemoveWall(BuildWallDirection build, int x, int y)
    {
        if(CheckWall(x,y, build))
        {
            Wall w = GetBuildedWall(x,y, build);
            // Destory(w.gameObject);
            int indexX = w.x;
            int indexY = w.y;
            int indexZ = (int)build <= 2 ? 0 : 1; 
            walls[indexX, indexY, indexZ] = null;
            Destroy(w.gameObject); 
        }
      /*  switch (build)
        {
            case BuildWallDirection.None:
                break;
            case BuildWallDirection.Left:
                walls[x - minx, y - miny, 0] = wall;
                break;
            case BuildWallDirection.Right:
                walls[x - minx + 1, y - miny, 0] = wall;
                break;
            case BuildWallDirection.Top:
                walls[x - minx, y - miny + 1, 1] = wall;
                break;
            case BuildWallDirection.Bottom:
                walls[x - minx, y - miny, 1] = wall;
                break;
        }*/
    }

    public BuildWallDirection GetWallDirection(Vector3 point, int x, int y)
    {
        float pointX = point.x;
        float pointY = point.z;

        //크기 확인
        float disA = pointX - x * 2;
        float disB = (x * 2 + 2) - pointX;
        float disC = pointY - y * 2;
        float disD = (y * 2 + 2) - pointY;
        float min = 2;
        if (disA < min) min = disA;
        if (disB < min) min = disB;
        if (disC < min) min = disC;
        if (disD < min) min = disD;

        BuildWallDirection buildWallDirection = BuildWallDirection.None;

        if (min == disA)
        {
            buildWallDirection = HousingSystem.BuildWallDirection.Left;
        }
        if (min == disB)
        {
            buildWallDirection = HousingSystem.BuildWallDirection.Right;
        }
        if (min == disC)
        {
            buildWallDirection = HousingSystem.BuildWallDirection.Bottom;
        }
        if (min == disD)
        {
            buildWallDirection = HousingSystem.BuildWallDirection.Top;
        }
        return buildWallDirection;
    }

    Wall GetBuildedWall(int x, int y, BuildWallDirection build)
    {
        switch (build)
        {
            case BuildWallDirection.None:
                return null;
            case BuildWallDirection.Left:
                Debug.Log((x - minx) + " " + (y - miny));
                return walls[x - minx, y - miny, 0];
            case BuildWallDirection.Right:
                return walls[x - minx + 1, y - miny, 0];
            case BuildWallDirection.Top:
                return walls[x - minx, y - miny + 1, 1];
            case BuildWallDirection.Bottom:
                return walls[x - minx, y - miny, 1];
            default: return null;
        }
    }
}
