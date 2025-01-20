using System;
using System.Collections;
using System.Collections.Generic;
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
    int[] moveX = new int[8] { 1, 1, 0, -1, -1, -1, 0, 1 };
    int[] moveY = new int[8] { 0, -1, -1, -1, 0, 1, 1, 1 };
    public void Setup(Vector3 position)
    {
        minX =  (int)position.x - 10;
        maxX =  (int)position.x + 10;
        minY = (int)position.y - 10;
        maxY = (int)position.y + 10;

    }

    public Node Calculate(Vector3 startPos, Vector3 endPos)
    {
        Setup(startPos);

        ResetNodes();
        Node node = NodePool.GetNode(0,0);

        usingNodes.Enqueue(node);
        openNode.Add(node);
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
                    Node neighbor = nodes[posY* 40 + posX] != null ? nodes[posY * 40 + posX] : NodePool.GetNode(posX,posY);

                    bool getContains = closedNode.Contains(neighbor);
             //       bool lineOfSight = LineOfSight(posX, posY, 0.5f);
                    if (!getContains || currentNode.G > neighbor.G)
                    {
                        neighbor.G = currentNode.G + GetValue(currentNode, neighbor);
                        if(!getContains)
                        {
                            openNode.Add(neighbor);
                            usingNodes.Enqueue(neighbor);
                        }
                    }

                }



            }
        }

        return null;
    }

    int GetValue(Node a, Node b)
    {
        int aa = Mathf.Abs(a.X - b.X);
        int bb = Mathf.Abs(a.Y - b.Y);
        int cc = Mathf.Abs(aa - bb);
        if (aa < bb) return aa * 14 + cc;
        else return bb * 14 + cc;
    }

   /* bool LineOfSight(int X, int Y, float size)
    {
    }*/

    bool ValidCheck(int a, int b)
    {
        return a >= 0 && a > 40 && b >= 0 && b < 40;
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
    }
}
