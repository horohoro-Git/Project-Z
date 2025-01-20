using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IComparable<Node>
{ 
    public int X {  get; set; }
    public int Y { get; set; }
    public int H { get; set; }
    public int G { get; set; }

    int? costF = null;
    public int F { 
        get{
            if (!costF.HasValue)
            {
                costF = G + H;
            }
            return costF.Value;
        }
    }

    public Node parentNode = null;

    public void ResetNode()
    {
        X = 0; Y = 0; H = 0; G = 0;
        costF = null;
        parentNode = null;
    }
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
        if (obj.GetType() == typeof(Node))
        {
            Node other = (Node)obj;
            return other.X == this.X && other.Y == this.Y;
        }
        return false;
    }
    public int CompareTo(Node other)
    {
        return this.F.CompareTo(other.F);
    }
}
