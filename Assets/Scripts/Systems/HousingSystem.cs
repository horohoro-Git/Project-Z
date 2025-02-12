using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;
public class HousingSystem : MonoBehaviour
{
    [NonSerialized]
    public int minx = -50 / 2;
    [NonSerialized]
    public int miny = -50 / 2;

    GameObject[,] floors = new GameObject[100,100];//0 == -15 
    GameObject[,] floors_Temp = new GameObject[100,100];

    public GameObject GetFloor(int x, int y) {
    
        return floors[x,y];
    }

    Wall[,,] walls = new Wall[100,100,2];
    Wall[,,] walls_Temp = new Wall[100,100,2];

    public Wall GetWall(int x, int y, int z)
    {

        return walls[x, y, z];
    }


    Roof[,] roofs = new Roof[100, 100];
    int floorCount;
    List<DoorSturct> doors = new List<DoorSturct>();
    bool[,,] visited = new bool[100, 100,2];


    List<HousingChangeInfo> changeLists = new List<HousingChangeInfo>();
    List<GameObject> removeMaterials = new List<GameObject>();
    private void Awake()
    {
        GameInstance.Instance.housingSystem = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        if(GameInstance.Instance.gameManager.gameMode == GameMode.TestMode) Invoke("st", 2);
       
    }

    void st()
    {
        BuildingMaterials.CreateRoofs();
        for (int i = -23; i < 23; i++)
        {
            for (int j = -23; j < 23; j++)
            {
                 GameInstance.Instance.assetLoader.LoadFloor(i, j);

                /*           HousingSystem.BuildWallDirection buildWallDirection = GameInstance.Instance.housingSystem.GetWallDirection(hit.point, x, y);
                           if (GameInstance.Instance.editMode == GameInstance.EditMode.CreativeMode) GameInstance.Instance.assetLoader.LoadWall(buildWallDirection, x, y);
                           else if (GameInstance.Instance.editMode == GameInstance.EditMode.DestroyMode) GameInstance.Instance.housingSystem.RemoveWall(buildWallDirection, x, y);
                           break;*/

            }

        }
  
        for (int i = -23;i < 23;i++)
        {
            for(int j = -23; j < 23;j++)
            {
                bool d = false;
                if (i == -23 || i == 22 || i == -22 || i == 21) d = true;
                if (j == -23 || j == -22 || j == 21 || j == 22) d = true;
                BuildWallDirection buildWallDirections = BuildWallDirection.Left;
                GameInstance.Instance.assetLoader.LoadWall(buildWallDirections, i, j,d);
            }
        }
        for (int i = -23; i < 23; i++)
        {
            for (int j = -23; j < -22; j++)
            {
             
                BuildWallDirection buildWallDirections = BuildWallDirection.Bottom;
                GameInstance.Instance.assetLoader.LoadWall(buildWallDirections, i, j);
                GameInstance.Instance.assetLoader.LoadWall(BuildWallDirection.Top, i, j);
            }
            {  
                BuildWallDirection buildWallDirections = BuildWallDirection.Top;
                GameInstance.Instance.assetLoader.LoadWall(buildWallDirections, i, 22);
                GameInstance.Instance.assetLoader.LoadWall(BuildWallDirection.Bottom, i, 22);
            }
        }
        for (int j = -23; j < -22; j++) GameInstance.Instance.assetLoader.LoadWall(BuildWallDirection.Left, -23, j);
        for (int j = -23; j < -22; j++) GameInstance.Instance.assetLoader.LoadWall(BuildWallDirection.Right, 22, j);
      //  GameInstance.Instance.assetLoader.LoadWall(BuildWallDirection.Bottom, 15, 8, false);
    }

    public bool CheckFloor(int x, int y)
    {
       
        return floors[x - minx,y - miny] != null;
    }
    public void BuildFloor(int x, int y, GameObject floor, bool isLoad)
    {
        int indexX = x - minx;
        int indexY = y - miny;
        floorCount++;
        floors[indexX, indexY] = floor;
        if (isLoad) return;

        HousingChangeInfo changeInfo = new HousingChangeInfo(floor, indexX, indexY, 0, ChangeInfo.Addition, StructureState.Floor);
        changeLists.Add(changeInfo);
    }

    void Make(int indexX, int indexY, int indexZ)
    {
        MakeHouse(indexX, indexY, false);
        if (indexZ == 0)
        {
            if (ValidSize(indexX - 1, indexY))
                MakeHouse(indexX - 1, indexY, false);
        }
        else
        {
            if (ValidSize(indexX, indexY - 1))
                MakeHouse(indexX, indexY - 1, false);
        }
    }
 
    //맵에 있는 건물들에 지붕 생성
    public void CheckRoofInWorld()
    {
        List<DoorSturct> DupulicatDoors = doors; //문들의 위치 값 리스트
        for (int i = 0; i < DupulicatDoors.Count; i++)
        {
            int indexX = DupulicatDoors[i].indexX;  //x축
            int indexY = DupulicatDoors[i].indexY;  //y축
            int indexZ = DupulicatDoors[i].indexZ;  //문과 벽의 방향
            List<DoorSturct> discovered = new List<DoorSturct>();

            bool checkWithOutSide = DoorWithOutSideValidation(indexX, indexY, indexZ, ref discovered); //문의 외부연결 확인

            if (checkWithOutSide)
            {
                //해당 위치 부터 지붕 생성
                Make(indexX, indexY, indexZ);    
                //탐색 하면서 같이 발견된 문의 지붕 생성
                for (int j=0; j< discovered.Count; j++) Make(discovered[j].indexX, discovered[j].indexY, discovered[j].indexZ);
            }
            //발견한 문 제거
            for (int j = DupulicatDoors.Count - 1; j > i; j--)
            {
                for (int k = 0; k < discovered.Count; k++)
                {
                    if (DupulicatDoors[j].indexX == discovered[k].indexX && DupulicatDoors[j].indexY == discovered[k].indexY &&
                        DupulicatDoors[j].indexZ == discovered[k].indexZ)
                    {
                        DupulicatDoors.RemoveAt(j);
                        break;
                    }
                }
            }

            //방문 인덱스 초기화
            for (int ii = 0; ii < 100; ii++)
            {
                for (int j = 0; j < 100; j++)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        if (visited[ii, j, k])
                        {
                            visited[ii, j, k] = false;
                        }
                    }
                }
            }

        }
    }
    public void RemoveRoofInWorld()
    {
        for (int i = 0; i < 100; i++)
        {
            for (int j = 0; j < 100; j++)
            {
                if (roofs[i, j] != null)// CheckHouse(i, j, true);
                {
                    Destroy(roofs[i, j].gameObject);

                }
                roofs[i, j] = null;
            }
        }
        
    }


    public bool CheckWall(int x, int y, BuildWallDirection build)
    {
        Wall w = GetBuildedWall(x,y,build);
        return w != null;
    }

    //벽 생성
    public void BuildWall(int x, int y, Wall wall, BuildWallDirection build, bool justWall = true, bool isLoad = false)
    {
//RemoveRoofInWorld();
        int indexX = x - minx;
        int indexY = y - miny;
        int indexZ = 0;
        int dirX = 0;
        int dirY = 0;
        switch (build)
        {
            case BuildWallDirection.None:
                break;
            case BuildWallDirection.Left:
                dirX = -1;
                break;
            case BuildWallDirection.Right:
                indexX += 1;
                dirX = 1;
                break;
            case BuildWallDirection.Top:
                indexY += 1; indexZ = 1; dirY = 1;
                break;
            case BuildWallDirection.Bottom:
                indexZ = 1; dirY = -1;
                break;
        }
        walls[indexX, indexY, indexZ] = wall;
        if (!justWall)
        {
            DoorSturct doorSturct = new DoorSturct(indexX, indexY, indexZ);

            doors.Add(doorSturct);
        }
        if (isLoad) return;

       // BuildMaterialsType buildMaterialsType = justWall ? BuildMaterialsType.Wall : BuildMaterialsType.
        HousingChangeInfo changeInfo = new HousingChangeInfo(wall.gameObject,indexX, indexY, indexZ, ChangeInfo.Addition, StructureState.Wall);
        changeLists.Add(changeInfo);
    }

    int[] moveX = new int[4] { 1, -1,0,0 };
    int[] moveY = new int[4] { 0, 0, 1, -1 };
    int[] moveWallX = new int[4] { 1, 0, 0, 0 };
    int[] moveWallY = new int[4] { 0, 0, 1, 0 };
    Queue<Node> usingNodes = new Queue<Node>();

    void ClearNodes()
    {
        while (usingNodes.Count > 0)
        {
            NodePool.RemoveNode(usingNodes.Dequeue());
        }
    }
    
    bool MakeHouse(int x, int y, bool remove = false)
    {
        Node  current = NodePool.GetNode(x, y);
      
        current.roof = true;
        if (floors[x, y] == null || roofs[x, y] != null)
        {
            ClearNodes();
            return false;
        }

        usingNodes.Enqueue(current);
        List<Node> openNode = new List<Node>();
        HashSet<Node> closedNode = new HashSet<Node>();

        bool isDoor = true;
        openNode.Add(current);
        while (openNode.Count > 0)
        {
            Node node = openNode[openNode.Count - 1];
            closedNode.Add(node);
            openNode.RemoveAt(openNode.Count - 1);
            for (int i = 0; i < 4; i++)
            {
                int xx = node.X + moveX[i];
                int yy = node.Y + moveY[i];

                if (xx >= 0 && xx < 100 && yy >= 0 && yy < 100)
                {
                    int newX = node.X + moveWallX[i];
                    int newY = node.Y + moveWallY[i];
                    int type = i < 2 ? 0 : 1;
                    if (walls[newX, newY, type] != null)
                    {
                        continue;
                    }
                    Node newNode = NodePool.GetNode(xx, yy);

                    if (!closedNode.Contains(newNode))
                    {
                        newNode.roof = true;

                        if (floors[newNode.X, newNode.Y] == null)
                        {

                            ClearNodes();
                            return false;
                        }
                        openNode.Add(newNode);

                        if (closedNode.Count > floorCount * 4)
                        {
                            ClearNodes();
                            return false;
                        }
                    }
                }
            }
        }

        if (isDoor || remove)
        { 
            //천장 생성 및 제거
            foreach (Node node in closedNode)
            {
                if (node.roof)
                {
                    int xx = node.X + minx;
                    int yy = node.Y + miny;
                    if (!remove)
                    {
                        xx -= minx;
                        yy -= miny;
                        if (roofs[xx, yy] == null)
                        {
                            Roof r = BuildingMaterials.GetRoof();
                            r.transform.position = new Vector3((xx + minx) * 2 + 1, 3.4f, (yy + miny) * 2 + 1);
                            roofs[xx, yy] = r;
                        }
                    }
                }
            }
            return true;
        }
            return false;
    }

    int[] offset = new int[2] { 1, -1 };
  
    int[] toVX = new int[4] { 0, 1, 0, 1 };
    int[] toVY = new int[4] { 0, 0, -1, -1 };

    int[] toHX = new int[4] { 0, 0, -1, -1 };
    int[] toHY = new int[4] { 0, 1, 0, 1 };
    bool DoorWithOutSideValidation(int newX, int newY, int type, ref List<DoorSturct> door)
    {
        bool returnVal = false;

        if (roofs[newX, newY] != null) return true;

        visited[newX, newY, type] = true;

        //현재의 반대 방향 4개의 벽
        int t = 1 - type;
        for (int i = 0; i < 4; i++)
        {

            int checkX = newX;
            int checkY = newY;
           

            if (t == 0)
            {
                checkX += toVX[i];
                checkY += toVY[i];

            }
            else
            {
                checkX += toHX[i];
                checkY += toHY[i];
            }
            returnVal = DoorWithOutSideValidationLoop(checkX, checkY, t, ref door);
            if (returnVal)
            {
       //         visited[newX, newY, type] = false;
         //       if (ValidSize(checkX, checkY)) visited[checkX, checkY, t] = false;
                return true;
            }
        }

        // 기존 방향 기준 앞, 뒤
        for (int i = 0; i < 2; i++)
        {
            int checkX = newX;
            int checkY = newY;

            if (type == 0)
            {
                checkX += offset[i];
            }
            else
            {
                checkY += offset[i];
            }

            returnVal = DoorWithOutSideValidationLoop(checkX, checkY, type, ref door);
            if (returnVal)
            {
        //        visited[newX, newY, type] = false;
       //         if (ValidSize(checkX, checkY)) visited[checkX, checkY, type] = false;
                return true;
            }
        }
       // visited[newX, newY, type] = false;
        return returnVal;
    }
    bool DoorWithOutSideValidationLoop(int newX, int newY, int type, ref List<DoorSturct> door)
    {
        bool returnVal = false;
        if (newX >= 0 && newX < 100 && newY >= 0 && newY < 100)
        {
            if (visited[newX, newY, type] == true) return false;

            if (walls[newX, newY, type] != null)
            {
                if(!walls[newX, newY, type].isDoor)
                {
                    return false;
                }
                else
                {
                    DoorSturct doorSturct = new DoorSturct(newX, newY, type);
                    door.Add(doorSturct);
                }
            }
           
            visited[newX, newY, type] = true;

            int t = 1 - type;
            for (int i = 0; i < 4; i++)
            {

                int checkX = newX;
                int checkY = newY;
               

                if (t == 0)
                {
                    checkX += toVX[i];
                    checkY += toVY[i];

                }
                else
                {
                    checkX += toHX[i];
                    checkY += toHY[i];
                }
                returnVal = DoorWithOutSideValidationLoop(checkX, checkY, t, ref door);
                if (returnVal)
                {
                  //  if (ValidSize(checkX, checkY)) visited[checkX, checkY, t] = false;
                    return true;
                }
            }

            for (int i = 0; i < 2; i++)
            {
                int checkX = newX;
                int checkY = newY;

                if (type == 0)
                {
                    checkX += offset[i];
                }
                else
                {
                    checkY += offset[i];
                }

                returnVal = DoorWithOutSideValidationLoop(checkX, checkY, type, ref door);
                if (returnVal)
                {
                //    if (ValidSize(checkX, checkY)) visited[checkX, checkY, type] = false;
                    return true;
                }
            }
        }
        else
        {
            return true;
        }

    //    visited[newX, newY, type] = false;
        return returnVal;
    }

    //바닥 제거
    public void RemoveFloor(int x, int y)
    {
        if(CheckFloor(x, y))
        {
            GameInstance.Instance.assetLoader.RemovePreview();
            //인덱스 위치 조정
            int xx = x - minx;  
            int yy = y - miny;

            //바닥위에 설치된 벽 확인
            if (walls[xx, yy, 0] != null) return;   
            else if (walls[xx, yy, 1] != null) return;
            else if (xx + 1 < 100 && walls[xx + 1, yy, 0] != null) return;
            else if (yy + 1 < 100 && walls[xx, yy + 1, 1] != null) return;

            floors[xx, yy].gameObject.SetActive(false);
            removeMaterials.Add(floors[xx, yy].gameObject);
          //  Destroy(floors[xx, yy].gameObject);
            HousingChangeInfo changeInfo = new HousingChangeInfo(floors[xx, yy].gameObject, xx, yy,0,ChangeInfo.Subtraction, StructureState.Floor);
            changeLists.Add(changeInfo);
            floors[xx, yy] = null;

        }
    }

    //벽, 문 제거
    public void RemoveWall(BuildWallDirection build, int x, int y)
    {
        if(CheckWall(x,y, build))
        {
            GameInstance.Instance.assetLoader.RemovePreview();
            Wall w = GetBuildedWall(x, y, build);
        
            int indexX = x - minx;
            int indexY = y - miny;
            bool isDoor = w.isDoor;
            
            int indexZ = (int)build <= 2 ? 0 : 1;
            switch (build)
            {
                case BuildWallDirection.None:
                    break;
                case BuildWallDirection.Left:
                    break;
                case BuildWallDirection.Right:
                    indexX += 1;
                    break;
                case BuildWallDirection.Top:
                    indexY += 1; indexZ = 1;
                    break;
                case BuildWallDirection.Bottom:
                    indexZ = 1;
                    break;
            }
                       
            Debug.Log(indexX + " " +  indexY + " " + indexZ);
            walls[indexX, indexY, indexZ] = null;
            //Destroy(w.gameObject);
            w.gameObject.SetActive(false);
            removeMaterials.Add(w.gameObject);
            HousingChangeInfo changeInfo = new HousingChangeInfo(w.gameObject, indexX, indexY, indexZ, ChangeInfo.Subtraction, StructureState.Wall);
            changeLists.Add(changeInfo);

            if (isDoor)
            {
                for (int i = 0; i < doors.Count; i++)
                {
                    if(indexX == doors[i].indexX && indexY == doors[i].indexY && indexZ == doors[i].indexZ)
                    {
                        doors.RemoveAt(i);
                        break;
                    }
                }
            }
        }
    }

    bool ValidSize(int x,int y)
    {
        return (x >= 0 && x <100 && y >= 0 && y <100);
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
            buildWallDirection = BuildWallDirection.Left;
        }
        if (min == disB)
        {
            buildWallDirection = BuildWallDirection.Right;
        }
        if (min == disC)
        {
            buildWallDirection = BuildWallDirection.Bottom;
        }
        if (min == disD)
        {
            buildWallDirection = BuildWallDirection.Top;
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

    //마지막 설치 취소
    public void Revert()
    {
        int count = changeLists.Count;
        if (count > 0)
        {
            HousingChangeInfo changeInfo = changeLists[count - 1];
            changeLists.RemoveAt(count - 1);
            if (!changeInfo.used) return;
            int indexX = changeInfo.indexX;
            int indexY = changeInfo.indexY;
            int indexZ = changeInfo.indexZ;

            if (changeInfo.change == ChangeInfo.Addition)
            {
                //추가했던 건물 자재를 제거
                if(changeInfo.type == StructureState.Floor)
                {
                    GameObject go = floors[indexX, indexY];
                    Destroy(go);
                    floors[indexX, indexY] = null;
                }
                else if (changeInfo.type == StructureState.Wall)
                {
                    Wall go = walls[indexX, indexY, indexZ];
                    Destroy(go.gameObject);
                    walls[indexX, indexY, indexZ] = null;
                }

                //비용 회득
            }
            else
            {
                //제거되었던 건물 자재를 추가
                if (changeInfo.type == StructureState.Floor) floors[indexX, indexY] = changeInfo.buildMat;
                else if (changeInfo.type == StructureState.Wall) walls[indexX, indexY, indexZ] = changeInfo.buildMat.GetComponent<Wall>();
                changeInfo.buildMat.SetActive(true);
                removeMaterials.RemoveAt(removeMaterials.Count - 1);
                //자재 비용 반납
            }
        }
    }

    //변경 사항 적용
    public void Apply()
    {
        if (changeLists.Count > 0)
        {
            changeLists.Clear();

            for (int i = removeMaterials.Count - 1; i >= 0; i--)
            {
                GameObject go = removeMaterials[i];
                if (go != null && !go.activeSelf) Destroy(go);
                removeMaterials.RemoveAt(i);
            }
            removeMaterials.Clear();

            TempStorage();

            if (GameInstance.Instance.gameManager.gameMode == GameMode.DefaultMode)
            {
                //게임 정보 저장
                SaveLoadSystem.SaveBuildSystem();
            }

        }
    }

    //초기 상태 저장
    public void TempStorage()
    {
        for (int i = 0; i < 100; i++)
        {
            for (int j = 0; j < 100; j++)
            {
                for (int k = 0; k < 2; k++)
                {
                    if(k == 0) floors_Temp[i,j] = floors[i,j];
                    walls_Temp[i,j,k] = walls[i,j,k];
                }
            }
        }
    }

    //변경 사항 초기화
    public void ResetMaterials()
    {
        if (changeLists.Count > 0)
        {
            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        if (k == 0)
                        {
                            GameObject floorTemp = floors_Temp[i, j];
                            GameObject floor = floors[i, j];
                            if (floorTemp != floor)
                            {
                                if (floor != null) Destroy(floor);
                                if (floorTemp != null) floorTemp.SetActive(true);
                                floors[i, j] = floors_Temp[i, j];
                            }
                        }
                        
                        Wall wallTemp = walls_Temp[i, j, k];
                        Wall wall = walls[i, j, k];
                        if (wallTemp != wall)
                        {
                            if (wall != null) Destroy(wall.gameObject);
                            if (wallTemp != null) wallTemp.gameObject.SetActive(true);
                            walls[i, j, k] = walls_Temp[i, j, k];
                        }
                    }
                }
            }
        }
        removeMaterials.Clear();
        changeLists.Clear();
    }
}
