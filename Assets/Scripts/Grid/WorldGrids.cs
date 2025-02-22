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
    int[] findX = new int[9] {0, 1, 1, 0,-1,-1,-1,0,1 };
    int[] findY = new int[9] {0, 0, -1, -1,-1, 0, 1, 1, 1 };
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

    //플레이어의 위치를 기록
    public void UpdatePlayerInGrid(PlayerController pc, ref int refX, ref int refY)
    {
        //10f 간격으로 배열에 배치
        Vector3 playerLoc = pc.Transforms.position; 
        int x = Mathf.FloorToInt(playerLoc.x / 10) - indexingMinX; 
        int y = Mathf.FloorToInt(playerLoc.y / 10) - indexingMinY;

        if (x == refX && y == refY) return; // 인덱스상 위치가 달라졌을 때에만 배열의 내용을 변경

        players[refX, refY].Remove(pc); //기존 위치에 있던 플레이어 참조 제거

        refX = x;
        refY = y;
        players[x,y].Add(pc);   //새로운 위치에 플레이어 참조
    }

    //적의 위치를 기반으로 그리드 탐색
    public List<PlayerController> FindPlayersInGrid(Transform transforms)
    {
        Vector3 currentPosition = transforms.position;   
        int x = Mathf.FloorToInt(currentPosition.x /10)- indexingMinX;
        int y = Mathf.FloorToInt(currentPosition.y /10)- indexingMinY;

        List<PlayerController> returnPlayers = new List<PlayerController>();

        for (int i = 0; i < 9; i++) // 현재 위치부터 8방향 탐색 
        {
            int posX = x + findX[i];
            int posY = y + findY[i];

            if (ValidCheck(posX, posY)) //인덱스 유효성 체크
            {
                List<PlayerController> controllers = players[posX, posY];
                for (int j = 0; j < controllers.Count; j++)
                {
                    PlayerController controller = controllers[j];
                    Vector3 dir = controller.Transforms.position - currentPosition;
                    float distance = dir.magnitude;
                    dir = Vector3.Normalize(dir);
                    if (Physics.Raycast(currentPosition,dir,distance,1 << 3)) returnPlayers.Add(controller); // 플레이어와 적 사이에 장애물이 있지 않을 때에만
                }
            }
        }
        return returnPlayers;
    }

    bool ValidCheck(int x, int y)
    {
     
        return x >= 0 && x < sizeX && y >=0 && y < sizeY;

    }
}
