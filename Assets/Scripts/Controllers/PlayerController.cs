using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Controller
{
    PlayerInput input;
    PlayerInput Input 
    {
        get
        {
            if(input == null) input = GetComponent<PlayerInput>();
            return input;
        }
    }
    
    public delegate void PlayerAction();
    public event PlayerAction Runs;
    private InputAction moveAction;  
    private InputAction sprintAction;
    private void Awake()
    {
        GameInstance.Instance.player = this;
        Input.defaultActionMap = "OnMove";
        moveSpeed = 200f;
      //  moveAction = Input.actions["OnMove"];
      //  sprintAction = Input.actions["OnSprint"];
    }

    private void OnEnable()
    {
        Input.actions["WASD"].performed += MoveHandle;
        Input.actions["WASD"].canceled += MoveStop;
        Input.actions["Run"].performed += Run;
        Input.actions["Run"].canceled += RunStop;
     //   Input.actions[]
    }

    private void OnDisable()
    {
        Input.actions["WASD"].performed -= MoveHandle;
        Input.actions["WASD"].performed -= MoveStop;
        Input.actions["Run"].performed -= Run;
        Input.actions["Run"].canceled -= RunStop;
    }

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    Vector2 lastVector = Vector2.zero;
    void MoveHandle(InputAction.CallbackContext callback)
    {
        Vector2 dir = callback.ReadValue<Vector2>();
        
        moveDir = VectorUtility.RotateY(new Vector3(dir.x, 0, dir.y), 45);

        if (lastVector != dir)
        {
            lastVector = dir;
            float dot = Vector3.Dot(Transforms.forward.normalized, moveDir);
            if (dot < -0.1f) longturn = true; else longturn = false;
        }
    }
    void MoveStop(InputAction.CallbackContext callback)
    {
        moveDir = Vector3.zero;
    }

    void Run(InputAction.CallbackContext callback)
    {
        moveSpeed = 400;
        sprint = true;
    }
    void RunStop(InputAction.CallbackContext callback)
    {
        moveSpeed = 200;
        sprint = false;
    }
}
