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
    Roof[,] roofs = new Roof[30, 30];
    int floorCount;

    bool[,] visit = new bool[30, 30];
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
        floorCount++;
        floors[x - minx,y - miny] = floor;
    }




    public bool CheckWall(int x, int y, BuildWallDirection build)
    {
        Wall w = GetBuildedWall(x,y,build);
        return w != null;
    }


    public void BuildWall(int x, int y, Wall wall, BuildWallDirection build)
    {
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

        if(CheckFloor(x, y)) CheckHouse(x - minx, y - miny);
        if(CheckFloor(x + dirX, y + dirY)) CheckHouse(x - minx + dirX, y - miny + dirY);

        Debug.Log(indexX + "  " + indexY);
    }

    int[] moveX = new int[4] { 1, -1,0,0 };
    int[] moveY = new int[4] { 0, 0, 1, -1 };
    void CheckHouse(int x, int y)
    {
        List<Node> doors = new List<Node>();
        Node  current = NodePool.GetNode(x, y);
        current.roof = true;
        List<Node> openNode = new List<Node>();
        HashSet<Node> closedNode = new HashSet<Node>();
        bool isDoor = false;
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


                if(xx >= 0 && xx < 15 && yy >=  0 && yy < 15)
                {
                    Node newNode = NodePool.GetNode(xx, yy);

                    int newX = node.X;
                    int newY = node.Y;
                    int type = i < 2 ? 0 : 1;
                    if (i == 0) newX += 1;
                    if (i == 2) newY += 1;


                
                    if (walls[newX, newY, type])
                    {
                        if (walls[newX, newY, type].isDoor)
                        {
                            if(!isDoor) isDoor = DoorWithOutSideValidation(newX, newY, type);
                        }
                        //closedNode.Add(newNode);
                  
                        continue;
                    }

                    if (!closedNode.Contains(newNode))
                    {
                        newNode.roof = true;
                        openNode.Add(newNode);

                        if (closedNode.Count > floorCount * 4)
                        {
                 //           Debug.Log("벽을 지음");
                            return;
                        }
                    }
                }
            }
        }

        if (isDoor)
        {
           // Debug.Log("집이 지어짐");

            int a = 0;
            foreach (Node node in closedNode)
            {
                if (node.roof)
                {
                    int xx = node.X + minx;
                    int yy = node.Y + miny;
                    GameObject go = Instantiate(GameInstance.Instance.assetLoader.roof);
                    go.transform.parent = GameInstance.Instance.assetLoader.root.transform;
                    go.transform.position = new Vector3(xx * 2 + 1, 3.4f, yy * 2 + 1);
                    xx -= minx;
                    yy -= miny;
                    roofs[xx, yy] = go.GetComponent<Roof>();
                    
                    a++;
                }
            }
      //      Debug.Log(a + "칸 지붕 생성");
        }
        else
        {
      //      Debug.Log("문이 없음");
        }
    }

    int[] ttx = new int[4] { 0, 1,0,1 };
    int[] tty = new int[4] { 0, 0,-1,-1 };
    bool DoorWithOutSideValidation(int newX, int newY, int type)
    {
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
                        checkX = newX + moveX[i];
                        checkY = newY + moveY[i];

                        returnVal = DoorWithOutSiedeValidationLoop(checkX, checkY, t);
                        if (returnVal) return returnVal;
                        //visit[checkX, checkY] = false;
                        for (int l = 0; l < 30; l++)
                        {
                            for (int k = 0; k < 30; k++)
                            {
                                visit[l, k] = false;
                            }
                        }
                    }
                    else
                    {
                        for(int j = 0; j < 4; j++)
                        {
                            checkX = newX + ttx[j];
                            checkY = newY + tty[j];

                            returnVal = DoorWithOutSiedeValidationLoop(checkX, checkY, t);
                            if (returnVal) return returnVal;
                            //visit[checkX, checkY] = false;
                            for (int l = 0; l < 30; l++)
                            {
                                for (int k = 0; k < 30; k++)
                                {
                                    visit[l, k] = false;
                                }
                            }
                        }
                       
                    }
                   
                }
                else
                {
                    if (type == t)
                    {
                        checkX = newX + moveX[i];
                        checkY = newY + moveY[i];

                        returnVal = DoorWithOutSiedeValidationLoop(checkX, checkY, t);
                        if (returnVal) return returnVal;
                        //visit[checkX, checkY] = false;
                        for (int l = 0; l < 30; l++)
                        {
                            for (int k = 0; k < 30; k++)
                            {
                                visit[l, k] = false;
                            }
                        }
                    }
                    else
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            checkX = newX + tty[j];
                            checkY = newY + ttx[j];

                            returnVal = DoorWithOutSiedeValidationLoop(checkX, checkY, t);
                            if (returnVal) return returnVal;
                            //visit[checkX, checkY] = false;
                            for (int l = 0; l < 30; l++)
                            {
                                for (int k = 0; k < 30; k++)
                                {
                                    visit[l, k] = false;
                                }
                            }
                        }

                    }
                }
             
              
            }
         
        }
        
        return returnVal;
    }

    bool DoorWithOutSiedeValidationLoop(int newX, int newY, int type)
    { 
        if (newX >= 0 && newX < 30 && newY >= 0 && newY < 30)
        {
            if (visit[newX, newY]) return false;

            visit[newX, newY] = true;
            int other = 1 - type;
            if(walls[newX, newY, type] != null) return false;

            for (int i = 0; i < 4; i++)
            {
                int t = i < 2 ? 0 : 1;
                int checkX = newX;
                int checkY = newY;
                if (t == 0)
                {
                    if (type == t)
                    {
                        checkX = newX + moveX[i];
                        checkY = newY + moveY[i];

                        if (DoorWithOutSiedeValidationLoop(checkX, checkY, t)) return true;
                        //visit[checkX, checkY] = false;
                    }
                    else
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            checkX = newX + ttx[j];
                            checkY = newY + tty[j];

                            if (DoorWithOutSiedeValidationLoop(checkX, checkY, t)) return true;
                          //  visit[checkX, checkY] = false;
                        }

                    }

                }
                else
                {
                    if (type == t)
                    {
                        checkX = newX + moveX[i];
                        checkY = newY + moveY[i];

                        if (DoorWithOutSiedeValidationLoop(checkX, checkY, t)) return true;
                       /// visit[checkX, checkY] = false;
                    }
                    else
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            checkX = newX + tty[j];
                            checkY = newY + ttx[j];

                            if (DoorWithOutSiedeValidationLoop(checkX, checkY, t)) return true;
                         //   visit[checkX, checkY] = false;
                        }

                    }
                }
             
              //  newX = newX - moveX[i];
               // newY = newY - moveY[i];
            }
        }
        else
        {
            Debug.Log(newX + " " + newY);
            return true;
        }

        return false;
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
