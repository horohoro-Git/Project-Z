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

    protected bool turnRotate;
    protected Vector3 rotateVector;
    protected Queue<InputBuffer> inputBuffer = new Queue<InputBuffer>();
    protected float delayTimer = 0f; 
    protected float inputDelay = 0.02f;
    float inputTime;
    Vector3 latest;
    bool stopped;
    Vector3 realVector;

    public struct InputBuffer
    {
        public Vector3 vector;
        public float timer;
        public InputBuffer(Vector3 vector, float timer)
        {
            this.vector = vector;
            this.timer = timer;
        }
    };

    void FixedUpdate()
    {
        if (inputBuffer.Count > 0)
        {
            InputBuffer buffer = inputBuffer.Peek();
            if (realVector == Vector3.zero || inputTime >= buffer.timer + inputDelay)
            {
                realVector = buffer.vector;
                inputTime = buffer.timer;
                inputBuffer.Dequeue();
            }
            else
            {
                inputTime = Time.time;
            }
        }
        else
        {
            inputTime = Time.time;
        }

        if (moveDir == Vector3.zero)
        {
            realVector = Vector3.zero;
            inputBuffer.Clear();
        }
        if (realVector == moveDir)
        {
            inputBuffer.Clear();
        }

        if (turnRotate)
        {
            Vector3 r = (rotateVector - Transforms.position);

            Quaternion quaternion = Quaternion.LookRotation(r);
            quaternion = Quaternion.Euler(0, quaternion.eulerAngles.y, 0);
            Transforms.rotation = Quaternion.Slerp(Transforms.rotation, quaternion, 10 * Time.fixedDeltaTime * moveSpeedMutiplier);

            float angle = Vector3.Dot(Transforms.forward, r.normalized);

            if (Vector3.Dot(Transforms.forward, r.normalized) > 0.999f)
            {
                viewDir = Vector3.zero;
                turnRotate = false;
            }
        }
        else
        {
            if (!lookAround) Rotate();
            else
            {
                LookAround();
            }
        }

        if (canMove > 0)
        {
            viewVelocity = 0;
            currentMoveSpeed = Mathf.SmoothDamp(currentMoveSpeed, 0, ref velocity, 0.2f);
            Rigid.velocity = viewDir * Time.fixedDeltaTime * currentMoveSpeed * moveSpeedMutiplier;
            viewSpeed = currentMoveSpeed;
            return;
        }
        if (realVector.magnitude > 0)
        {
            //if (realVector.magnitude < 0.1f) return;
            viewDir = realVector;
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

        if (currentMoveSpeed < 50) viewDir = Vector3.zero;
    }
    private Vector3 lastValidDirection;
    private bool wasMoving;
    int rotateTick = 2;
    void Rotate()
    {
        if (viewDir != Vector3.zero)
        {
            float dot = Vector3.Dot(Transforms.forward.normalized, viewDir);

            if (dot < -0.1f) turn = true; else turn = false;
            Quaternion targetRotation = Quaternion.LookRotation(viewDir);
            currentDir = dot;


            if (longturn)
            {
                Transforms.rotation = Quaternion.RotateTowards(Transforms.rotation, targetRotation, 400 * Time.fixedDeltaTime);
            }
            if (!longturn)
            {
                if (sprint) Transforms.rotation = Quaternion.RotateTowards(Transforms.rotation, targetRotation, 300 * Time.fixedDeltaTime);
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
