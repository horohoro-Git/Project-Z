using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;

public class HousingSystem : MonoBehaviour
{
    int minx = -15 / 2;
    int miny = -15 / 2;
    GameObject[,] floors = new GameObject[30,30];//0 == -15 
    GameObject[,] walls = new GameObject[30,30];
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




    public bool CheckWall(int x, int y)
    {
        return walls[x,y] == null;
    }


    public void BuildWall()
    {

    }
}
