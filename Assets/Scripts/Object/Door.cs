using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
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

    Animator animator;
    public Animator GetAnimation
    {
        get
        {
            if(animator == null) animator = GetComponentInParent<Animator>();
            return animator;
        }
    }
    public bool isHorizontal = false;
}
