using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Wall
{
    BoxCollider boxCollider;
    public BoxCollider BoxCol
    {
        get {
                if (boxCollider == null) boxCollider = GetComponent<BoxCollider>();
         
                return boxCollider; 
            } 
    }
    [NonSerialized]
    public bool isOpen;
}
