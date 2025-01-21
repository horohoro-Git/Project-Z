using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinHeap<T> where T : IComparable<T> 
{
    List<T> nodes;
    public MinHeap(int size)
    {
        nodes = new List<T>(size);
    }

    public int Count {  get { return nodes.Count; } }

    public void Add(T addT)
    {
        nodes.Add(addT);
        SortYUp();
    }

    public void RemoveAt(int indexz)
    {
        nodes.RemoveAt(indexz);
    }

    public bool Contains(T item)
    {
        return nodes.Contains(item);
    }

    public T PopMin()
    {
        T min = nodes[0];
        nodes[0] = nodes[nodes.Count - 1];
        nodes[nodes.Count - 1] = min;
        nodes.RemoveAt(nodes.Count - 1);

        SortYDown();

        return min;
    }

    void SortYUp()
    {
        int index = nodes.Count - 1;

        while (index > 0)
        {
            if (nodes[index].CompareTo(nodes[index - 1]) >= 0) return;
        

            Swap(nodes[index], nodes[index - 1]);

            index--;
        }
    }

    void SortYDown()
    {
        int index = 0;
        int maxSize = nodes.Count - 1;
        while(index < maxSize)
        {
            int min = index;
            int leftChild = index * 2 + 1;
            int rightChild = index * 2 + 2;
            if (leftChild < maxSize && nodes[min].CompareTo(nodes[leftChild]) > 0)
            {
                min = leftChild;
            }
            if (rightChild < maxSize && nodes[min].CompareTo(nodes[rightChild]) > 0)
            {
                min = rightChild;
            }
            if (index == min) break;

            Swap(nodes[index], nodes[min]);

            index = min;
        }
    }

    void Swap(T itemA, T itemB)
    {
        T temp = itemA;
        itemA = itemB;
        itemB = temp;
    }
}
