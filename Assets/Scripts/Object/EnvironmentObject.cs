using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentObject : MonoBehaviour
{
    public EnvironmentType environmentType;


    public int X;
    public int Y;
    public bool rotated;

    public void Clear()
    {
        if(gameObject != null)
        {
            GameObject.Destroy(gameObject);
          
        }
    }
}
