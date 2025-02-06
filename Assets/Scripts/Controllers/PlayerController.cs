using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro.EditorUtilities;
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
    
    //public delegate void PlayerAction();
  //  public event PlayerAction Runs;
    private InputAction moveAction;  
    private InputAction sprintAction;

    public event Action<Vector3> InteractionEvent;
    public List<Action<Vector3>> lastHandler = new List<Action<Vector3>>();

    int lastGridX = 0; 
    int lastGridY = 0;
   
    private void Awake()
    {
        GameInstance.Instance.playerController = this;
        Input.defaultActionMap = "OnMove";
        moveSpeed = 200f;
        //  moveAction = Input.actions["OnMove"];
        //  sprintAction = Input.actions["OnSprint"];
    
    }

    private void OnEnable()
    {
        AddAction();
    }

    private void OnDisable()
    {
        RemoveAction();
    }

    public void AddAction()
    {
        Input.actions["WASD"].performed += MoveHandle;
        Input.actions["WASD"].canceled += MoveStop;
        Input.actions["Run"].performed += Run;
        Input.actions["Run"].canceled += RunStop;
        Input.actions["Interaction"].performed += Interact;
    }
    public void RemoveAction()
    {
        Input.actions["WASD"].performed -= MoveHandle;
        Input.actions["WASD"].performed -= MoveStop;
        Input.actions["Run"].performed -= Run;
        Input.actions["Run"].canceled -= RunStop;
        Input.actions["Interaction"].performed -= Interact;
    }
    // Start is called before the first frame update
    void Start()
    {
        GameInstance.Instance.AddPlayer(this);
       
    }

    // Update is called once per frame
    void Update()
    {
        if(LastPosition != Transforms.position)
        {
            LastPosition = Transforms.position;
            GameInstance.Instance.worldGrids.UpdatePlayerInGrid(this, ref lastGridX, ref lastGridY);
        }
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

    void Interact(InputAction.CallbackContext callback)
    {
        if(lastHandler.Count == 0) return;
        lastHandler[lastHandler.Count - 1]?.Invoke(Transforms.position);
    }

    public void RegisterAction(Action<Vector3> action)
    {
        InteractionEvent += action;
        lastHandler.Add(action);
    }


    public void RemoveLastAction(Action<Vector3> action)
    {
        if(lastHandler[lastHandler.Count -1] == action)
        {
            lastHandler.RemoveAt(lastHandler.Count - 1);
            InteractionEvent -= action;
        }
        else
        {
            lastHandler.Remove(action);
            InteractionEvent -= action;
        }

    }
}
