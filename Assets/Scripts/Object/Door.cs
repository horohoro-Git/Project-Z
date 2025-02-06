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
                if (boxCollider == null) boxCollider = GetComponentInChildren<BoxCollider>();
         
                return boxCollider; 
            } 
    }
    [NonSerialized]
    public bool isOpen;

    Animator animator;
    public Animator GetAnimation
    {
        get
        {
            if(animator == null) animator = GetComponent<Animator>();
            return animator;
        }
    }
    public bool isHorizontal = false;
}
