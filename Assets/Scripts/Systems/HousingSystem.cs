using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;
using static InputManager;
using UnityEngine.InputSystem.HID;
using static HousingSystem;

public class HousingSystem : MonoBehaviour
{
    struct DoorSturct
    {
        public int indexX;
        public int indexY;
        public int indexZ;
        public DoorSturct(int indexX, int indexY, int indexZ)
        {
            this.indexX = indexX;
            this.indexY = indexY;
            this.indexZ = indexZ;
        }
    }

    public enum BuildWallDirection
    {
        None,
        Left,
        Right,
        Top,
        Bottom,
    }

    [NonSerialized]
    public int minx = -50 / 2;
    [NonSerialized]
    public int miny = -50 / 2;
    GameObject[,] floors = new GameObject[100,100];//0 == -15 
    Wall[,,] walls = new Wall[100,100,2];
    Roof[,] roofs = new Roof[100, 100];
    int floorCount;
    List<DoorSturct> doors = new List<DoorSturct>();
    bool[,,] visit = new bool[100, 100,2];
    bool[,] visitCheckHouse = new bool[100, 100];
    private void Awake()
    {
        GameInstance.Instance.housingSystem = this;
    }
    // Start is called before the first frame update
    void Start()
    {
      // Invoke("st", 2);
       
    }

    void st()
    {
        for (int i = -25; i < 25; i++)
        {
            for (int j = -25; j < 25; j++)
            {
                 GameInstance.Instance.assetLoader.LoadFloor(i, j);

                /*           HousingSystem.BuildWallDirection buildWallDirection = GameInstance.Instance.housingSystem.GetWallDirection(hit.point, x, y);
                           if (GameInstance.Instance.editMode == GameInstance.EditMode.CreativeMode) GameInstance.Instance.assetLoader.LoadWall(buildWallDirection, x, y);
                           else if (GameInstance.Instance.editMode == GameInstance.EditMode.DestroyMode) GameInstance.Instance.housingSystem.RemoveWall(buildWallDirection, x, y);
                           break;*/

            }

        }
  
        for (int i = -25;i < 25;i++)
        {
            for(int j = -25; j < 25;j++)
            {
                bool d = false;
                if (i == -25 || i == 24) d = true;
                BuildWallDirection buildWallDirections = BuildWallDirection.Left;
                GameInstance.Instance.assetLoader.LoadWall(buildWallDirections, i, j,d);
            }
        }
        for (int i = -25; i < 25; i++)
        {
            for (int j = -25; j < -24; j++)
            {
             
                BuildWallDirection buildWallDirections = BuildWallDirection.Bottom;
                GameInstance.Instance.assetLoader.LoadWall(buildWallDirections, i, j);
            }
            {  
                BuildWallDirection buildWallDirections = BuildWallDirection.Top;
                GameInstance.Instance.assetLoader.LoadWall(buildWallDirections, i, 24);
            }
        }
        for (int j = -25; j < -24; j++) GameInstance.Instance.assetLoader.LoadWall(BuildWallDirection.Left, -25, j);
        for (int j = -25; j < -24; j++) GameInstance.Instance.assetLoader.LoadWall(BuildWallDirection.Right, 24, j);
        GameInstance.Instance.assetLoader.LoadWall(BuildWallDirection.Bottom, 15, 8, false);
    }

    public bool CheckFloor(int x, int y)
    {
       
        return floors[x - minx,y - miny] != null;
    }
    public void BuildFloor(int x, int y, GameObject floor)
    {
        floorCount++;
        floors[x - minx,y - miny] = floor;
        //  CheckHouse(x-minx, y-miny);
        //CheckRoofInWorld();
    }

    int a = 0;
    public void CheckRoofInWorld()
    {
      //  a = 0;
        /*  for (int i = 0; i < 100; i++)
          {
              for (int j = 0; j < 100; j++)
              {
                  if(walls[i,j,0] != null || walls[i,j,1] != null) CheckHouse(i, j);
              }
          }*/
        Queue<int> q = new Queue<int>();
        for (int i = 0; i < doors.Count; i++)
        {
            int indexX = doors[i].indexX;
            int indexY = doors[i].indexY;
            int indexZ = doors[i].indexZ;
            if (!visitCheckHouse[indexX, indexY])
            {
                {
                    visitCheckHouse[indexX, indexY] = true;

                    int index = indexY * 100 + indexX;
                    q.Enqueue(index);
                }
                if(indexX + 1 < 100) {
                    visitCheckHouse[indexX + 1, indexY] = true;

                    int index = indexY * 100 + (indexX + 1);
                    q.Enqueue(index);
                }
                if (indexX - 1 >= 0)
                {
                    visitCheckHouse[indexX - 1, indexY] = true;

                    int index = (indexY) * 100 + (indexX - 1);
                    q.Enqueue(index);
                }
                if (indexY + 1 < 100)
                {
                    visitCheckHouse[indexX, indexY + 1] = true;

                    int index = indexY * 100 + indexX;
                    q.Enqueue(index);
                }
                if (indexY - 1 >= 0)
                {
                    visitCheckHouse[indexX, indexY - 1] = true;

                    int index = indexY * 100 + indexX;
                    q.Enqueue(index);
                }
                if (DoorWithOutSideValidation(indexX, indexY, indexZ))
                {
                    CheckHouse(indexX, indexY, false);
                }
          
            }
        }

        while (q.Count > 0)
        {
            int index = q.Dequeue();
            int indexX = index % 100;
            int indexY = index / 100;
            visitCheckHouse[indexX, indexY] = false;
        }
        Debug.Log(a);

        for (int l = 0; l < 100; l++)
        {
            for (int k = 0; k < 100; k++)
            {
                for (int p = 0; p < 2; p++)
                {
                   if(visit[l, k, p])
                    {
                        Debug.Log(l + " " + k + " " + p);
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
                    Destroy(roofs[i,j].gameObject);
                    
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
    public void BuildWall(int x, int y, Wall wall, BuildWallDirection build, bool justWall = true)
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
            {
                DoorSturct doorSturct = new DoorSturct(indexX, indexY, indexZ);

                doors.Add(doorSturct);
            }
            {
           //     DoorSturct doorSturct = new DoorSturct(indexX + dirX, indexY + dirY, indexZ);

             //   if()     doors.Add(doorSturct);
            }
        }
       // if(CheckFloor(x, y)) CheckHouse(x - minx, y - miny);
       // if(CheckFloor(x + dirX, y + dirY)) CheckHouse(x - minx + dirX, y - miny + dirY);
      //  CheckRoofInWorld();
    }

    int[] moveX = new int[4] { 1, -1,0,0 };
    int[] moveY = new int[4] { 0, 0, 1, -1 };
    Queue<Node> usingNodes = new Queue<Node>();

    void ClearNodes()
    {
        while (usingNodes.Count > 0)
        {
            NodePool.RemoveNode(usingNodes.Dequeue());
        }
    }
    HashSet<Node> closedNode = new HashSet<Node>();
    bool CheckHouse(int x, int y, bool remove = false)
    {
        Node  current = NodePool.GetNode(x, y);
      
        current.roof = true;
        if (floors[x, y] == null)
        {
            ClearNodes();
            return false;
        }
        usingNodes.Enqueue(current);
        List<Node> openNode = new List<Node>();
        closedNode.Clear();
      
        bool isDoor = true;
        openNode.Add(current);
        while (openNode.Count > 0)
        {
            Node node = openNode[openNode.Count - 1];
          //  usingNodes.Enqueue(node);
            closedNode.Add(node);
            openNode.RemoveAt(openNode.Count - 1);
          //  Debug.Log("벽을 지음1" + node.X + " " + node.Y + " " + closedNode.Count);
            for (int i = 0; i < 4; i++)
            {
                int xx = node.X + moveX[i];
                int yy = node.Y + moveY[i];

                if (xx >= 0 && xx < 100 && yy >=  0 && yy < 100)
                {
                    Node newNode = NodePool.GetNode(xx, yy);
                    usingNodes.Enqueue(newNode);
                    int newX = node.X;
                    int newY = node.Y;
                    int type = i < 2 ? 0 : 1;
                    if (i == 0) newX += 1;
                    if (i == 2) newY += 1;

                    /* if (walls[newX, newY, type])
                     {
                         if (walls[newX, newY, type].isDoor)
                         {
                             if(!isDoor) isDoor = DoorWithOutSideValidation(newX, newY, type); // 문이 바깥 연결 확인
                         }
                         continue;
                     }*/
                    if (walls[newX, newY, type] != null)
                    {
                      
                        continue;
                    }
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
                a++;
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
                            GameObject go = Instantiate(GameInstance.Instance.assetLoader.roof);
                            go.transform.parent = GameInstance.Instance.assetLoader.root.transform;
                            go.transform.position = new Vector3((xx + minx) * 2 + 1, 3.4f, (yy + miny) * 2 + 1);
                            roofs[xx, yy] = go.GetComponent<Roof>();
                        }
                    }
                    else
                    {
                        xx -= minx;
                        yy -= miny;
                        if (roofs[xx, yy] != null)
                        {
                            Roof r = roofs[xx, yy];
                            Destroy(r.gameObject);
                            roofs[xx, yy] = null;
                        }
                    }
                }
            }
            ClearNodes();
            return true;
            //      Debug.Log(a + "칸 지붕 생성");
        }
        else
        {
            ClearNodes();
            return false;
      //      Debug.Log("문이 없음");
        }
    }

    int[] ttx = new int[4] { 0, 1,0,1 };
    int[] tty = new int[4] { 0, 0,-1,-1 };
    bool DoorWithOutSideValidation(int newX, int newY, int type)
    {
      /*  for (int l = 0; l < 100; l++)
        {
            for (int k = 0; k < 100; k++)
            {
                for (int p = 0; p < 2; p++)
                {
                    visit[l, k, p] = false;
                }

            }
        }*/
        bool returnVal = false;
        for (int i = 0; i < 4; i++)
        {
          //  if (i < 4)
            {
                int t = i < 2 ? 0 : 1;
                int checkX = newX;
                int checkY = newY;
                if (t == 0)
                {
                    if(type == t)
                    {
                        int checkXX = newX + moveX[i];
                        int checkYY = newY + moveY[i];
                        if (ValidSize(checkXX, checkYY))
                        { 
                            returnVal = DoorWithOutSiedeValidationLoop(checkXX, checkYY, t);
                            visit[checkXX, checkYY, t] = false;
                            if (returnVal) return returnVal;
                        }
                        //visit[checkX, checkY] = false;
                     /*   for (int l = 0; l < 100; l++)
                        {
                            for (int k = 0; k < 100; k++)
                            {
                                for (int p = 0; p < 2; p++)
                                {
                                    visit[l, k, p] = false;
                                }

                            }
                        }*/

                      
                    }
                    else
                    {
                        for(int j = 0; j < 4; j++)
                        {
                       //     checkX = newX + ttx[j];
                           // checkY = newY + tty[j];
                            int checkXX = newX + ttx[j];
                            int checkYY = newY + tty[j];

                            if (ValidSize(checkXX, checkYY))
                            {
                                returnVal = DoorWithOutSiedeValidationLoop(checkXX, checkYY, t);
                                visit[checkXX, checkYY, t] = false;
                                if (returnVal) return returnVal;
                            }
                            //visit[checkX, checkY] = false;
                            //for (int l = 0; l < 100; l++)
                            //{
                            //    for (int k = 0; k < 100; k++)
                            //    {
                            //        for (int p = 0; p < 2; p++)
                            //        {
                            //            visit[l, k, p] = false;
                            //        }

                            //    }
                            //}
                          
                        }
                       
                    }
                   
                }
                else
                {
                    if (type == t)
                    {
                        int checkXX = newX + moveX[i];
                        int checkYY = newY + moveY[i];
                        if (ValidSize(checkXX, checkYY))
                        {
                            returnVal = DoorWithOutSiedeValidationLoop(checkXX, checkYY, t);
                            visit[checkXX, checkYY, t] = false;
                            if (returnVal) return returnVal;
                        }
                        //visit[checkX, checkY] = false;
                       /* for (int l = 0; l < 100; l++)
                        {
                            for (int k = 0; k < 100; k++)
                            {
                                for (int p = 0; p < 2; p++)
                                {
                                    visit[l, k, p] = false;
                                }

                            }
                        }*/
                       
                    }
                    else
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            int checkXX = newX + ttx[j];
                            int checkYY = newY + tty[j];
                            if (ValidSize(checkXX, checkYY))
                            {
                                returnVal = DoorWithOutSiedeValidationLoop(checkXX, checkYY, t);
                                visit[checkXX, checkYY, t] = false;
                                if (returnVal) return returnVal;
                            }
                            //visit[checkX, checkY] = false;
                            /* for (int l = 0; l < 100; l++)
                             {
                                 for (int k = 0; k < 100; k++)
                                 {
                                     for (int p = 0; p < 2; p++)
                                     {
                                         visit[l, k, p] = false;
                                     }

                                 }
                             }*/
                            
                        }

                    }
                }
             
              
            }
         
        }
        
        return returnVal;
    }

    bool DoorWithOutSiedeValidationLoop(int newX, int newY, int type)
    { 
        if (newX >= 0 && newX < 100 && newY >= 0 && newY < 100)
        {
            if (visit[newX, newY,type]) return false;

            visit[newX, newY, type] = true;
            int other = 1 - type;
            if (walls[newX, newY, type] != null && !walls[newX, newY, type].isDoor)
            {
                //   visit[newX, newY, type] = false;
                visit[newX, newY, type] = false;
                return false;
            }

            for (int i = 0; i < 4; i++)
            {
                int t = i < 2 ? 0 : 1;
                int checkX = newX;
                int checkY = newY;
                if (t == 0)
                {
                    if (type == t)
                    {
                        int checkXX = newX + moveX[i];
                        int checkYY = newY + moveY[i];


                        if (DoorWithOutSiedeValidationLoop(checkXX, checkYY, t))
                        {
                            return true;
                         
                        }
                        if (ValidSize(checkXX, checkYY))
                        {

                            visit[checkXX, checkYY, t] = false;
                        }
                    }
                    else
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            int checkXX = newX + ttx[j];
                            int checkYY = newY + tty[j];

                            if (DoorWithOutSiedeValidationLoop(checkXX, checkYY, t))
                            {
                                return true;

                            }
                            if (ValidSize(checkXX, checkYY))
                            {

                                visit[checkXX, checkYY, t] = false;
                            }
                        }

                    }

                }
                else
                {
                    if (type == t)
                    {
                        int checkXX = newX + moveX[i];
                        int checkYY = newY + moveY[i];

                        if (DoorWithOutSiedeValidationLoop(checkXX, checkYY, t))
                        {
                            return true;

                        }
                        if (ValidSize(checkXX, checkYY))
                        {

                            visit[checkXX, checkYY, t] = false;
                        }
                    }
                    else
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            int checkXX = newX + ttx[j];
                            int checkYY = newY + tty[j];

                            if (DoorWithOutSiedeValidationLoop(checkXX, checkYY, t))
                            {
                                return true;

                            }
                            if (ValidSize(checkXX, checkYY))
                            {

                                visit[checkXX, checkYY, t] = false;
                            }
                        }

                    }
                }
             
              //  newX = newX - moveX[i];
               // newY = newY - moveY[i];
            }
        }
        else
        {
            return true;
        }
        visit[newX, newY, type] = false;
        return false;
    }

    //바닥 제거
    public void RemoveFloor(int x, int y)
    {
        if(CheckFloor(x, y))
        {
            //인덱스 위치 조정
            int xx = x - minx;  
            int yy = y - miny;

            //바닥위에 설치된 벽 확인
            if (walls[xx, yy, 0] != null) return;   
            else if (walls[xx, yy, 1] != null) return;
            else if (xx + 1 < 100 && walls[xx + 1, yy, 0] != null) return;
            else if (yy + 1 < 100 && walls[xx, yy + 1, 1] != null) return;

           // RemoveRoofInWorld();
          //  CheckHouse(xx, yy, true);
            //CheckRoofInWorld();
            Destroy(floors[xx, yy].gameObject);
            floors[xx, yy] = null;
        }
    }

    //벽, 문 제거
    public void RemoveWall(BuildWallDirection build, int x, int y)
    {
        if(CheckWall(x,y, build))
        {
            Wall w = GetBuildedWall(x, y, build);
            //집에 설치된 천장제거
        //   CheckHouse(w.x, w.y, true);
       //     CheckRoofInWorld();

            //제거할 벽의 선택되지 않은 셀
            int indexX = w.x;
            int indexY = w.y;
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
                    dirX = 1;
                    break;
                case BuildWallDirection.Top:
                    dirY = 1;
                    break;
                case BuildWallDirection.Bottom:
                     dirY = -1;
                    break;
            }
            //    CheckRoofInWorld();
            //  CheckHouse(w.x + dirX, w.y + dirY, true);
           // RemoveRoofInWorld();
            int indexZ = (int)build <= 2 ? 0 : 1; 
            walls[indexX, indexY, indexZ] = null;
            Destroy(w.gameObject);
          //  CheckRoofInWorld();
           // CheckHouse(indexX, indexY, false);
           // CheckHouse(indexX + dirX, indexY + dirY, false);
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
