using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
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

    [NonSerialized]
    public bool lookAround;

    public Vector3 moveDir;
    public float moveSpeed;
    [NonSerialized] 
    public float currentMoveSpeed;
    [NonSerialized]
    public float currentDir;
    public  Vector3 viewDir = Vector3.zero;
    public bool turn = false;
    public bool longturn = false;
    [NonSerialized]
    public float velocity;
    [NonSerialized]
    public float viewVelocity;
    // float smoothTime = 0.5f;
    public bool sprint;

    [NonSerialized]
    public int canMove;
    [NonSerialized]
    public float viewSpeed;

    float lastDot;
    Vector3 lastVectors;
    [NonSerialized]
    public float around;

    [NonSerialized]
    public bool directionPlayer;

    [NonSerialized]
    public float moveSpeedMutiplier;
    void FixedUpdate()
    {
        if (!lookAround) Rotate();
        else
        {
            LookAround();
        }


        if (canMove > 0)
        {
            viewVelocity = 0;
            currentMoveSpeed = Mathf.SmoothDamp(currentMoveSpeed, 0, ref velocity, 0.2f);
            Rigid.velocity = viewDir * Time.fixedDeltaTime * currentMoveSpeed * moveSpeedMutiplier;
            viewSpeed = currentMoveSpeed;
            return;
        }
        if (moveDir.magnitude > 0)
        {
            viewDir = moveDir;
            viewSpeed = currentMoveSpeed;
            currentMoveSpeed = Mathf.SmoothDamp(currentMoveSpeed, moveSpeed * moveSpeedMutiplier, ref velocity, 0.2f);
            Rigid.velocity = viewDir * Time.fixedDeltaTime * currentMoveSpeed * moveSpeedMutiplier;

        }
        else
        {
            viewVelocity = 0;
            currentMoveSpeed = Mathf.SmoothDamp(currentMoveSpeed, 0, ref velocity, 0.2f);
            Rigid.velocity = viewDir * Time.fixedDeltaTime * currentMoveSpeed * moveSpeedMutiplier;
            viewSpeed = currentMoveSpeed;
        }

        if(currentMoveSpeed < 50) viewDir = Vector3.zero;
       
     //   Debug.Log(viewDir);

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

    void LookAround()
    {
       
        
        Vector3 mousePosition = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            Vector3 worldMousePosition = hitInfo.point;
            Vector3 direction = worldMousePosition - Transforms.position;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            targetRotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);
            Transforms.rotation = Quaternion.Slerp(Transforms.rotation, targetRotation, 5 * Time.deltaTime * moveSpeedMutiplier);

            float dot = Vector3.Dot(transform.forward, viewDir);
            Vector3 cross = Vector3.Cross(transform.forward, viewDir);
            float side = cross.y;
          /*  if (dot > -0.2f)
            {
                viewDir = Vector3.Normalize(direction);
                Debug.Log("¾Õ");
            }
            else
            {
                viewDir = -Vector3.Normalize(direction);

                Debug.Log("µÚ");
            }
*/
       //     viewDir = Vector3.Normalize(direction);
        }
    }
}
