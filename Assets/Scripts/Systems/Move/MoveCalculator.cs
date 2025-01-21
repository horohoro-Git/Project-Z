using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;


public class MoveCalculator
{
    struct NodeStruct : IComparable<NodeStruct>
    {
        public int X;
        public int Y;
        public int F;
        public int G;
        public int H;

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 23 + X.GetHashCode();
            hash = hash * 23 + Y.GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj.GetType() == typeof(NodeStruct))
            {
                NodeStruct other = (NodeStruct)obj;
                return other.X == this.X && other.Y == this.Y;
            }
            return false;
        }

        public int CompareTo(NodeStruct other)
        {
            return this.F.CompareTo(other.F);
        }
    }

    int minX, maxX, minY, maxY;
    Queue<Node> usingNodes = new Queue<Node>();
    MinHeap<Node> openNode = new MinHeap<Node>(40);
    HashSet<Node> closedNode = new HashSet<Node>(40);
    Dictionary<int, Node> nodes = new Dictionary<int, Node>(40);
    static Dictionary<int, bool> blockArea = new Dictionary<int, bool>();
    int[] moveX = new int[8] { 1, 1, 0, -1, -1, -1, 0, 1 };
    int[] moveY = new int[8] { 0, -1, -1, -1, 0, 1, 1, 1 };
    public void Setup(Vector3 position)
    {
       
        minX =  (int)position.x - 10;
        maxX =  (int)position.x + 10;
        minY = (int)position.z - 10;
        maxY = (int)position.z + 10;
        SetBlockArea();
    }

    public void SetBlockArea()
    {

        blockArea.Clear();
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
        for (int i = 0; i < 40; i++)
        {
            for (int j = 0; j < 40; j++)
            {
                if (Physics.CheckBox(new Vector3(i / 2 + minX, 0, j / 2 + minY), size, Quaternion.identity, 1 << 6))
                {



                    blockArea[j * 40 + i] = true;
                }
                else
                {
                    blockArea[j * 40 + i] = false;

                }
            }
        }
    }
    public Stack<Vector3> Calculate(Vector3 startPos, Vector3 endPos)
    {
        Setup(startPos);

        ResetNodes();


        Node node = NodePool.GetNode(19,19);
        Node start = NodePool.GetNode((int)(startPos.x - minX) * 2, (int)(startPos.z - minY) * 2);
        Node end = NodePool.GetNode((int)(endPos.x-minX)*2, (int)(endPos.z-minY)*2);

     //   Debug.Log(startPos);
     //   Debug.Log(endPos);
    //    Debug.Log(end.X + " " + end.Y);
        usingNodes.Enqueue(node);
        usingNodes.Enqueue(start);
        usingNodes.Enqueue(end);
        openNode.Add(node);
       // int get = start.X / 2 + minX + (start.Y / 2 + minY) * 120;

       // if (blockArea[get]) Debug.Log("GG");

        while (openNode.Count > 0)
        {
            Node currentNode = openNode.PopMin();
            nodes.Add(currentNode.Y * 40 + currentNode.X, currentNode);  
            closedNode.Add(currentNode);
            for (int i = 0; i < 8; i++)
            {
                int posX = currentNode.X + moveX[i];
                int posY = currentNode.Y + moveY[i];
                if(ValidCheck(posX, posY))
                {
                    bool containsKey = nodes.ContainsKey(posY * 40 + posX);
                    Node neighbor = containsKey ? nodes[posY * 40 + posX] : NodePool.GetNode(posX,posY);

                    int gety = posX / 2 + minX + (posY / 2 + minY) * 120; 

                    if (!closedNode.Contains(neighbor) && (!blockArea[posX + posY * 40]))
                    {
                        if(end.X == posX && end.Y == posY)
                        {
                            end.parentNode = currentNode;
                            return ReturnNodes(end);
                        }

                        bool getContains = openNode.Contains(neighbor);
                        bool lineOfSight = LineOfSight(currentNode.parentNode, neighbor, 0.1f);

                        if (lineOfSight)
                        {
                            int g = currentNode.parentNode.G + GetCost(currentNode.parentNode, neighbor);

                            if (g < neighbor.G || !getContains)
                            {
                                neighbor.parentNode = currentNode.parentNode;
                                neighbor.G = g;
                            }
                        }
                        else
                        {
                            int g = currentNode.G + GetCost(currentNode, neighbor);

                            if(g < neighbor.G || !getContains)
                            {
                                neighbor.parentNode = currentNode;
                                neighbor.G = g;
                            }
                        }
                        
                        neighbor.H = GetCost(neighbor, end);
                        if(!getContains) openNode.Add(neighbor);
                    }
                 
                }
            }
        }
        Debug.Log("GG");
        return null;
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
        int aa = Mathf.Abs(a.X - b.X);
        int bb = Mathf.Abs(a.Y - b.Y);
        int cc = Mathf.Abs(aa - bb);
        if (aa < bb) return aa * 14 + cc;
        else return bb * 14 + cc;
    }

    bool LineOfSight(Node start, Node end, float size)
    {
        if(start == null || end == null) return false;

        Vector3 sizeVector = new Vector3(size, size, size);
        Vector3 startPos = new Vector3(start.X / 2f + minX, 0, start.Y / 2f + minY);
        Vector3 endPos = new Vector3(end.X / 2f + minX, 0, end.Y / 2f + minY);


        Vector3 dir = endPos - startPos; 
        float distance = dir.magnitude;
        dir = Vector3.Normalize(dir);

        return !Physics.BoxCast(startPos, sizeVector, dir, Quaternion.identity, distance, 1 << 6);

    }

    bool ValidCheck(int a, int b)
    {
        return a >= 0 && a < 40 && b >= 0 && b < 40;
    }

    void ResetNodes()
    {
        nodes.Clear();
        while (usingNodes.Count > 0)
        {
            NodePool.RemoveNode(usingNodes.Dequeue());
        }
        while(openNode.Count > 0)
        {
            openNode.RemoveAt(openNode.Count - 1);
        }
        closedNode.Clear();
    }
}
