using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Controller : MonoBehaviour
{
    Transform transforms;
    Rigidbody rigid;

    [NonSerialized]
    public Vector3 LastPosition;
    public Transform Transforms
    {
        get
        {
            if (transforms == null) transforms = transform;
            return transforms;
        }
    }
    public Rigidbody Rigid
    {
        get
        {
            if(rigid == null) rigid = GetComponent<Rigidbody>();
            return rigid;
        }
    }

    public Vector3 moveDir;
    public float moveSpeed;
    [NonSerialized] 
    public float currentMoveSpeed;
    [NonSerialized]
    public float currentDir;
    Vector3 viewDir = Vector3.zero;
    public bool turn = false;
    public bool longturn = false;
    [NonSerialized]
    public float velocity;
    [NonSerialized]
    public float viewVelocity;
    // float smoothTime = 0.5f;
    public bool sprint;

    [NonSerialized]
    public bool canMove = true;

    
    void FixedUpdate()
    {
        Rotate();
        if (!canMove)
        {

            currentMoveSpeed = Mathf.SmoothDamp(currentMoveSpeed, 0, ref velocity, 0.2f);
            Rigid.velocity = Transforms.forward * Time.fixedDeltaTime * currentMoveSpeed;
            return;
        }
       
        if (moveDir != Vector3.zero)
        {
            viewDir = moveDir;

           /* if (turn)
            {
                viewVelocity = 1;
              //  currentMoveSpeed = 0;
            }
            else
            {*/
                viewVelocity = 0;
                // moveSpeed에 따른 걷기/뛰기 애니메이션
                currentMoveSpeed = Mathf.SmoothDamp(currentMoveSpeed, moveSpeed, ref velocity, 0.5f);
                //Rigid.velocity = Transforms.forward * Time.fixedDeltaTime * currentMoveSpeed;
                Rigid.velocity = moveDir * Time.fixedDeltaTime * currentMoveSpeed;
           // }
        }
        else
        {
            viewVelocity = 0;
            // 감속 되는 애니메이션
            currentMoveSpeed = Mathf.SmoothDamp(currentMoveSpeed, 0, ref velocity, 0.2f);
            Rigid.velocity = Transforms.forward * Time.fixedDeltaTime * currentMoveSpeed;
        }
    }

    void Rotate()
    {
        if (viewDir != Vector3.zero)
        {
            float dot = Vector3.Dot(Transforms.forward.normalized, viewDir);
            if(dot < -0.1f) turn = true; else turn = false;
            Quaternion targetRotation = Quaternion.LookRotation(viewDir);
            currentDir = dot;
            if (longturn)
            {
                Transforms.rotation = Quaternion.RotateTowards(Transforms.rotation, targetRotation, 400 * Time.fixedDeltaTime);
            }
            if (!longturn)
            {
               if(sprint) Transforms.rotation = Quaternion.RotateTowards(Transforms.rotation, targetRotation, 300 * Time.fixedDeltaTime);
               else Transforms.rotation = Quaternion.RotateTowards(Transforms.rotation, targetRotation, 300 * Time.fixedDeltaTime);
            }
        }
    }

    
}
