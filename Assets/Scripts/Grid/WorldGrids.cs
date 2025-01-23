using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGrids : MonoBehaviour
{
    
    public float minX;
    public float minY;
    public float maxX;
    public float maxY;
    private int indexingMinX;
    private int indexingMaxX;
    private int indexingMinY;
    private int indexingMaxY;
    int sizeX;
    int sizeY;
    List<PlayerController>[,] players;

    private void Awake()
    {
        GameInstance.Instance.worldGrids = this;
        indexingMinX = Mathf.FloorToInt(minX / 10f);
        indexingMinY = Mathf.FloorToInt(minY / 10f);
        indexingMaxX = Mathf.FloorToInt(maxX / 10f);
        indexingMaxY = Mathf.FloorToInt(maxY / 10f);

        sizeX = indexingMaxX - indexingMinX + 1;
        sizeY = indexingMaxY - indexingMinY + 1;
        players = new List<PlayerController>[sizeX, sizeY];
        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                players[i, j] = new List<PlayerController>();
            }
        }
        //    players = new List<PlayerController>[index]
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdatePlayerInGrids(PlayerController pc, ref int refX, ref int refY)
    {
        Vector3 playerLoc = pc.Transforms.position;
        int x = Mathf.FloorToInt(playerLoc.x / 10) - indexingMinX;
        int y = Mathf.FloorToInt(playerLoc.y / 10) - indexingMinY;

        if (x == refX && y == refY) return;

        players[refX, refY].Remove(pc);

        refX = x;
        refY = y;

        players[x,y].Add(pc);

        Debug.Log(x + " " + y);
    }
}
