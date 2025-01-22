using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;


public class MoveCalculator
{

    float minX, maxX, minY, maxY;
    Queue<Node> usingNodes = new Queue<Node>();
    MinHeap<Node> openNode = new MinHeap<Node>(40);
    HashSet<Node> closedNode = new HashSet<Node>(40);
    Dictionary<Vector2, Node> nodes = new Dictionary<Vector2, Node>(40);
   // static Dictionary<int, bool> blockArea = new Dictionary<int, bool>();
    bool[,] blockArea = new bool[41, 41];
    //int[] moveX = new int[8] { 1, 1, 0, -1, -1, -1, 0, 1 };
    // int[] moveY = new int[8] { 0, -1, -1, -1, 0, 1, 1, 1 };
    int[] moveX = { 1, -1, 0, 0, 1, 1, -1, -1 };
    int[] moveY = { 0, 0, 1, -1, 1, -1, 1, -1 };
    public void Setup(Vector3 position)
    {
       
        minX =  position.x - 10;
        maxX =  position.x + 10;
        minY =  position.z - 10;
        maxY =  position.z + 10;
        SetBlockArea();
    }

    public void SetBlockArea()
    {

        //blockArea.Clear();
        Vector3 size = new Vector3(0.5f, 0.5f, 0.5f);
      /*  for (int i = -60; i < 60; i++)
        {
            for (int j = -60; j < 60; j++)
            {
                if (Physics.CheckBox(new Vector3(i/2, 0, j/2), size, Quaternion.identity, 1 << 6))
                {
             
                   

                    blockArea[j * 120 + i] = true;
                }
                else
                {
                    blockArea[j * 120 + i] = false;

                }
            }
        }*/
        for (int i = 0; i <= 40; i++)
        {
            for (int j = 0; j <= 40; j++)
            {
                if (Physics.CheckBox(new Vector3(i / 2f + minX, 0, j / 2f + minY), size, Quaternion.identity, 1 << 6))
                {
                    //Debug.Log((i / 2f + minX) + " " + (j / 2f + minY) + $"({i},{j})");
                    blockArea[i,j] = true;
                }
                else
                {
                    blockArea[i,j] = false;

                }
            }
        }
    }
    public Stack<Vector3> Calculate(Vector3 startPos, Vector3 endPos)
    {
        Setup(startPos);

        ResetNodes();


        Node node = NodePool.GetNode(20,20);
   //     Node start = NodePool.GetNode((int)(startPos.x - minX) * 2, (int)(startPos.z - minY) * 2);
        Node end = NodePool.GetNode((int)((endPos.x - minX) * 2), (int)((endPos.z - minY)*2));

     //   Debug.Log(startPos);
     //   Debug.Log(endPos);
    //    Debug.Log(end.X + " " + end.Y);
        usingNodes.Enqueue(node);
       // usingNodes.Enqueue(start);
        usingNodes.Enqueue(end);
        openNode.Add(node);
        // int get = start.X / 2 + minX + (start.Y / 2 + minY) * 120;
       // blockArea[start.X + start.Y * 40] = false;
       // blockArea[start.X + start.Y * 40] = false;
       // if (blockArea[get]) Debug.Log("GG");

        while (openNode.Count > 0)
        {
            Node currentNode = openNode.PopMin();
            nodes.Add(new Vector2(currentNode.X, currentNode.Y), currentNode);  
            closedNode.Add(currentNode);
            for (int i = 0; i < 8; i++)
            {
                int posX = currentNode.X + moveX[i];
                int posY = currentNode.Y + moveY[i];
                if(ValidCheck(posX, posY))
                {
                    bool containsKey = nodes.ContainsKey(new Vector2(posX, posY));
                   // Node neighbor = containsKey ? nodes[new Vector2(posX, posY)] : NodePool.GetNode(posX,posY);
                    Node neighbor = NodePool.GetNode(posX, posY);
                    usingNodes.Enqueue((neighbor));
                    //int gety = posX / 2 + minX + (posY / 2 + minY) * 120; 

                    if (!closedNode.Contains(neighbor) && (!blockArea[posX,posY]))
                    {
                        if(end.X == posX && end.Y == posY)
                        {
                            end.parentNode = currentNode;
                            return ReturnNodes(end);
                        }

                        bool getContains = openNode.Contains(neighbor);
                        bool lineOfSight = LineOfSight(currentNode.parentNode, neighbor, 0.5f);

                        if (lineOfSight)
                        {
                            int g = currentNode.parentNode.G + GetCost(currentNode.parentNode, neighbor);

                            if (g < neighbor.G || !getContains)
                            {
                                neighbor.G = g;
                                neighbor.parentNode = currentNode.parentNode;
                            }
                        }
                        else
                        {
                            int g = currentNode.G + GetCost(currentNode, neighbor);

                            if(g < neighbor.G || !getContains)
                            {
                                neighbor.G = g;
                                neighbor.parentNode = currentNode;
                            }
                        }
                        
                        neighbor.H = GetCost(neighbor, end);
                        if(!getContains) openNode.Add(neighbor);
                    }
                 
                }
            }
        }
     //   Debug.Log("GG");
        return null;
    }
    int GetMapIndex(float coordinate, float currentPos)
    {
        return Mathf.FloorToInt((coordinate - currentPos) / 0.5f) + 20;
    }
    Vector3 GetPositionFromMapIndex(int mapIndex, Vector3 currentPos)
    {
        int xIndex = mapIndex % 40;
        int yIndex = (mapIndex / 40) % 40;
        int zIndex = mapIndex / (40 * 40);

        float xPos = currentPos.x + (xIndex - 20) * 0.5f;
        float yPos = currentPos.y + (yIndex - 20) * 0.5f;
        float zPos = currentPos.z + (zIndex - 20) * 0.5f;

        return new Vector3(xPos, yPos, zPos);
    }
    Stack<Vector3> ReturnNodes(Node find)
    {
        Stack<Vector3> returnNodes = new Stack<Vector3>();

        Node current = find;
        while(current != null)
        {
          
            returnNodes.Push(new Vector3(current.X / 2f + minX , 0, current.Y /2f + minY));
          
            current = current.parentNode;
        }
        return returnNodes;
    }

    int GetCost(Node a, Node b)
    {
        int dstX = Mathf.Abs(a.X - b.X);
        int dstY = Mathf.Abs(a.Y - b.Y);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
/*
        int aa = Mathf.Abs(a.X - b.X);
        int bb = Mathf.Abs(a.Y - b.Y);
        int cc = Mathf.Abs(aa - bb);
        if (aa < bb) return aa * 14 + cc;
        else return bb * 14 + cc;*/
    }

    bool LineOfSight(Node start, Node end, float size)
    {
        /*if(start == null || end == null) return false;

        Vector3 sizeVector = new Vector3(size, size, size);
        Vector3 startPos = new Vector3(start.X / 2f + minX, 0, start.Y / 2f + minY);
        Vector3 endPos = new Vector3(end.X / 2f + minX, 0, end.Y / 2f + minY);


        Vector3 dir = endPos - startPos; 
        float distance = dir.magnitude;
        dir = Vector3.Normalize(dir);

        return !Physics.BoxCast(startPos, sizeVector, dir, Quaternion.identity, distance, 1 << 6);*/
        if (start == null || end == null) return false;

        Vector3 startPos = new Vector3(
            start.X * 0.5f + minX,
            0,
            start.Y * 0.5f + minY
        );
        Vector3 endPos = new Vector3(
            end.X * 0.5f + minX,
            0,
            end.Y * 0.5f + minY
        );

        Vector3 direction = (endPos - startPos); // 시작점에서 끝점으로 가는 방향
        float distance = Vector3.Distance(startPos, endPos); // 시작점과 끝점 간 거리
        if (distance > Mathf.Epsilon)
        {
            direction.x /= distance;
            direction.y /= distance;
            direction.z /= distance;
        }
        // SphereCast를 사용하여 충돌 검사
        return !Physics.BoxCast(startPos, new Vector3(size,size,size), direction, Quaternion.identity, distance, 1 << 6);
    }

    bool ValidCheck(int a, int b)
    {
        return a >= 0 && a <= 40 && b >= 0 && b <= 40;
    }

    void ResetNodes()
    {
        nodes.Clear();
        while (usingNodes.Count > 0)
        {
            NodePool.RemoveNode(usingNodes.Dequeue());
        }
        
        openNode.Clear();
        closedNode.Clear();
    }
}
