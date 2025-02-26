using System;
using System.Collections;
using System.Collections.Generic;
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
    public int canMove;
    [NonSerialized]
    public float viewSpeed;

    float lastDot;
    Vector3 lastVectors;
    [NonSerialized]
    public float around;
    void FixedUpdate()
    {
        if(!lookAround) Rotate();
        else
        {
            LookAround();
        }
        if (canMove > 0)
        {
            currentMoveSpeed = Mathf.SmoothDamp(currentMoveSpeed, 0, ref velocity, 0.2f);
            Rigid.velocity = Transforms.forward * Time.fixedDeltaTime * currentMoveSpeed;
            viewSpeed = currentMoveSpeed;
            return;
        }
        if (moveDir.magnitude > 0)
        {
            viewDir = moveDir;
            viewSpeed = currentMoveSpeed;
            currentMoveSpeed = Mathf.SmoothDamp(currentMoveSpeed, moveSpeed, ref velocity, 0.5f);
            Rigid.velocity = viewDir * Time.fixedDeltaTime * currentMoveSpeed;

        }
        else
        {
            viewVelocity = 0;
            currentMoveSpeed = Mathf.SmoothDamp(currentMoveSpeed, 0, ref velocity, 0.2f);
            Rigid.velocity = viewDir * Time.fixedDeltaTime * currentMoveSpeed;
            viewSpeed = currentMoveSpeed;
        }

       /* if (canMove > 0)
        {
            currentMoveSpeed = Mathf.SmoothDamp(currentMoveSpeed, 0, ref velocity, 0.2f);
            Rigid.velocity = Transforms.forward * Time.fixedDeltaTime * currentMoveSpeed;
            viewSpeed = currentMoveSpeed;
            return;
        }

        if (lookAround)
        {
            if (moveDir != Vector3.zero)
            {
               // Transforms.rotation
                viewDir = moveDir;

                viewVelocity = 0;
                currentMoveSpeed = Mathf.SmoothDamp(currentMoveSpeed, moveSpeed, ref velocity, 0.5f);
                Rigid.velocity = moveDir * Time.fixedDeltaTime * currentMoveSpeed;

                float dot = Vector3.Dot(Transforms.forward, moveDir);
                float cross = Vector3.Cross(Transforms.forward, moveDir).y;

                viewSpeed = currentMoveSpeed;
                if (dot > 0)
                {
                    Debug.Log("앞");
                }
                else if(dot < 0 )
                {
                    viewSpeed *= -1;
                    Debug.Log(" 뒤");
                }
                Debug.Log(currentMoveSpeed);
                float angle = Vector3.Angle(Transforms.forward, moveDir);
                float directionValue = Mathf.Clamp01(1 - angle / 100f) * 2f - 1;
                //Debug.Log(dot + " " + cross);
            }
            else
            {
                viewVelocity = 0;
                // 감속 되는 애니메이션
                currentMoveSpeed = Mathf.SmoothDamp(currentMoveSpeed, 0, ref velocity, 0.2f);
                Rigid.velocity = viewDir * Time.fixedDeltaTime * currentMoveSpeed;
                float dot = Vector3.Dot(Transforms.forward, viewDir);
                float cross = Vector3.Cross(Transforms.forward, viewDir).y;

                viewSpeed = currentMoveSpeed;
                if (dot > 0)
                {
                    Debug.Log("앞");
                }
                else if (dot < 0)
                {
                    viewSpeed *= -1;
                    Debug.Log(" 뒤");
                }

                float angle = Vector3.Angle(Transforms.forward, viewDir);
                float directionValue = Mathf.Clamp01(1 - angle / 100f) * 2f - 1;
                Debug.Log(dot + " " + cross);
            }
        }
        else
        {
            if (moveDir != Vector3.zero)
            {
                viewDir = moveDir;

                viewVelocity = 0;

                currentMoveSpeed = Mathf.SmoothDamp(currentMoveSpeed, moveSpeed, ref velocity, 0.5f);
                viewSpeed = currentMoveSpeed;
                //Rigid.velocity = Transforms.forward * Time.fixedDeltaTime * currentMoveSpeed;
                Rigid.velocity = moveDir * Time.fixedDeltaTime * currentMoveSpeed;
                // }
            }
            else
            {
                viewVelocity = 0;
                // 감속 되는 애니메이션
                currentMoveSpeed = Mathf.SmoothDamp(currentMoveSpeed, 0, ref velocity, 0.2f);
                Rigid.velocity = viewDir * Time.fixedDeltaTime * currentMoveSpeed;
                viewSpeed = currentMoveSpeed;
            }
        }*/
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
            Transforms.rotation = Quaternion.Slerp(Transforms.rotation, targetRotation, 5 * Time.deltaTime);


        }
    }
}
