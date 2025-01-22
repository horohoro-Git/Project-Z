using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinHeap<T> where T : IComparable<T> 
{
    List<T> nodes;
    HashSet<T> set;
    public MinHeap(int size)
    {
        nodes = new List<T>(size);
        set = new HashSet<T>(size);
    }

    public int Count {  get { return nodes.Count; } }

    public void Add(T addT)
    {
        if(!set.Contains(addT))
        {
            nodes.Add(addT);
            set.Add(addT);
            SortYUp();

        }
    }

    public void Clear()
    {
        nodes.Clear();
        set.Clear();
    }

    public bool Contains(T item)
    {
        return nodes.Contains(item);
    }

    public T PopMin()
    {
        T min = nodes[0];
        set.Remove(min);
        nodes[0] = nodes[nodes.Count - 1];
        nodes.RemoveAt(nodes.Count - 1);

        SortYDown();

        return min;
    }
    public T Peek()
    {
        return nodes[0];
    }
    void SortYUp()
    {
        int index =0;

        while (index > 0)
        {
            int parentIndex = index / 2; 
            if (nodes[index].CompareTo(nodes[parentIndex]) >= 0) return;
        

            Swap(index, parentIndex);

            index = parentIndex;
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
            if (leftChild <= maxSize && nodes[min].CompareTo(nodes[leftChild]) > 0)
            {
                min = leftChild;
            }
            if (rightChild <= maxSize && nodes[min].CompareTo(nodes[rightChild]) > 0)
            {
                min = rightChild;
            }
            if (index == min) break;

            Swap(index, min);

            index = min;
        }
    }

    void Swap(int itemAIndex, int itemBIndex)
    {
        T temp = nodes[itemAIndex];
        nodes[itemAIndex] = nodes[itemBIndex];
        nodes[itemBIndex] = temp;
    }
}
