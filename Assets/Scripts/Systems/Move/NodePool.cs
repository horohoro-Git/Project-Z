using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodePool
{
  
    static Queue<Node> pool = new Queue<Node>(1000);
    public static void CreateNodes(int num)
    {
        for(int i= 0; i < num; i++)
        {
            Node node = new Node();
            pool.Enqueue(node);
        }
    }
    public static Node GetNode(int X, int Y)
    {
        if (pool.Count == 0)
        {
            for (int i = 0; i < 10; i++)
            {
                Node node = new Node();
                pool.Enqueue(node);
            }
        }

        Node returnNode = pool.Dequeue();
        returnNode.X = X; returnNode.Y = Y;
        return returnNode;
    }

    public static void RemoveNode(Node node)
    {
        if (pool.Count > 1000)
        {
            node.ResetNode();
            node = null;
        }
        else
        {
            node.ResetNode();
            pool.Enqueue(node); 
        }
    }
}
