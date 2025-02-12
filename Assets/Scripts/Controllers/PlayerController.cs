using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using UnityEditor;
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

    public event Action<PlayerController> InteractionEvent;
    Action<PlayerController> lastAction;
    int lastGridX = 0; 
    int lastGridY = 0;

    [NonSerialized]
    Animator modelAnimator;

    PlayerState state = PlayerState.Default;
    Coroutine combatMotion;

    bool inputEnabled = false;
    bool destroyed = false;
    private void Awake()
    {
        GameInstance.Instance.playerController = this;
        Input.defaultActionMap = "OnMove";
        moveSpeed = 200f;
        /*modelAnimator = GetComponentInChildren<Animator>();
   */
    }

    public void SetController(GameObject go)
    {
        modelAnimator = go.GetComponent<Animator>();
    }
    private void OnEnable()
    {
       
        AddAction();
    }

    private void OnDisable()
    {
        RemoveAction();
       // lastHandler.Clear();
    }

    private void OnDestroy()
    {
        destroyed = true;
    }
    public void AddAction()
    {
        if (destroyed) return;
        if (!Application.isPlaying) return;
        if (inputEnabled) return;
        inputEnabled = true;
        Input.actions["WASD"].performed += MoveHandle;
        Input.actions["WASD"].canceled += MoveStop;
        Input.actions["Run"].performed += Run;
        Input.actions["Run"].canceled += RunStop;
        Input.actions["Interaction"].performed += Interact;
        Input.actions["Combat"].performed += ChangeCombatMode;
        Input.actions["Attack"].performed += Attack;
        Input.actions["Attack"].canceled += EndAttack;
        Input.actions["OpenInventory"].performed += OpenInventory;
    }
    public void RemoveAction()
    {
        if (destroyed) return;
        if (!inputEnabled) return;
        inputEnabled = false;
        Input.actions["WASD"].performed -= MoveHandle;
        Input.actions["WASD"].performed -= MoveStop;
        Input.actions["Run"].performed -= Run;
        Input.actions["Run"].canceled -= RunStop;
        Input.actions["Interaction"].performed -= Interact;
        Input.actions["Combat"].performed -= ChangeCombatMode;
        Input.actions["Attack"].performed -= Attack;
        Input.actions["Attack"].canceled -= EndAttack;
        Input.actions["OpenInventory"].performed -= OpenInventory;
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
    
    //   if(currentDir < 0) modelAnimator.SetFloat("speed", 0.f);
      // else  modelAnimator.SetFloat("speed", s /200);
          modelAnimator.SetFloat("speed", currentMoveSpeed / 200);
          modelAnimator.SetFloat("dir", viewVelocity);
        // modelAnimator.speed = velocity;

        if(attack)
        {
            Punch();
        }
    }
    
    Vector2 lastVector = Vector2.zero;
    void MoveHandle(InputAction.CallbackContext callback)
    {
        Vector2 dir = callback.ReadValue<Vector2>();
        
        moveDir = VectorUtility.RotateY(new Vector3(dir.x, 0, dir.y), 45);
        modelAnimator.SetInteger("state", 1);
        if (lastVector != dir)
        {
            lastVector = dir;
            float dot = Vector3.Dot(Transforms.forward.normalized, moveDir);
            if (dot < -0.1f) longturn = true; else longturn = false;
        }
    }
    void MoveStop(InputAction.CallbackContext callback)
    {
        modelAnimator.SetInteger("state", 0);
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

    void ChangeCombatMode(InputAction.CallbackContext callback)
    {
        if(state == PlayerState.Default)
        {
            modelAnimator.SetBool("combat", true);
            
            if (combatMotion != null) StopCoroutine(combatMotion);
            combatMotion = StartCoroutine(ChangeMode(true));
        }
        else if(state == PlayerState.Combat)
        {
          
            if (combatMotion != null) StopCoroutine(combatMotion);
            combatMotion = StartCoroutine(ChangeMode(false));
        }
    }

    IEnumerator ChangeMode(bool on)
    {
        if (on)
        {
           
            float offset = modelAnimator.GetFloat("combatMotion");
            while (offset < 0.2f)
            {
                offset += Time.deltaTime;
                modelAnimator.SetFloat("combatMotion", offset);
                yield return null;
            }
            state = PlayerState.Combat;
            offset = 0.2f;
            modelAnimator.SetFloat("combatMotion", offset);
        }
        else
        {
            float offset = modelAnimator.GetFloat("combatMotion");
            while (offset > 0f)
            {
                offset -= Time.deltaTime;
                modelAnimator.SetFloat("combatMotion", offset);
                yield return null;
            }
            offset = 0;
            state = PlayerState.Default;
            modelAnimator.SetFloat("combatMotion", offset);
            modelAnimator.SetBool("combat", false);
        }
    }

    bool reinput = false;
    bool attack = false;
    void Attack(InputAction.CallbackContext callback)
    {
        attack = true;
       
    }
    void EndAttack(InputAction.CallbackContext callback)
    {
        attack = false;
    }

    void Punch()
    {
        if (state == PlayerState.Combat)
        {
            reinput = false;
            int i = UnityEngine.Random.Range(0, 2);
            modelAnimator.SetTrigger("a");
            modelAnimator.SetInteger("type", i);
            Invoke("AttackTimer", 1f);
            Invoke("InputTimer", 0.5f);
            modelAnimator.SetFloat("combatMotion", 0);
            state = PlayerState.Default;
        }
        else if (reinput)
        {
            reinput = false;
            attackAgain = true;
        }

    }

    void InputTimer()
    {
        reinput = true;
    }
    bool attackAgain;
    void AttackTimer()
    {
        if (!attackAgain)
        {
            modelAnimator.SetBool("attack", false);
            if (combatMotion != null) StopCoroutine(combatMotion);
            combatMotion = StartCoroutine(ChangeMode(true));
        }
        else
        {
            attackAgain = false;
            int i = UnityEngine.Random.Range(0, 2);
            modelAnimator.SetTrigger("a");
            modelAnimator.SetInteger("type", i);
            Invoke("AttackTimer", 1f);
            Invoke("InputTimer", 0.5f);
            modelAnimator.SetFloat("combatMotion", 0);
            state = PlayerState.Default;
        }
    }
    void Interact(InputAction.CallbackContext callback)
    {
        if (lastAction == null) return;
        lastAction.Invoke(this);
    }

    public void RegisterAction(Action<PlayerController> action)
    {
        InteractionEvent += action;
        Delegate[] actions = InteractionEvent.GetInvocationList();
        lastAction = (Action<PlayerController>)actions[0];
    }


    public void RemoveLastAction(Action<PlayerController> action)
    {
           InteractionEvent.GetInvocationList();
        InteractionEvent -= action;
        if (lastAction == action)
        {
            if (InteractionEvent != null)
            { 
                Delegate[] actions = InteractionEvent.GetInvocationList();
                lastAction = (Action<PlayerController>)actions[0];
            }
            else
            {
                lastAction = null;
            }
        }
        else
        {
            InteractionEvent -= action;
        }
    }

    void OpenInventory(InputAction.CallbackContext callback)
    {
        InventorySystem inventorySystem = GameInstance.Instance.inventorySystem;
        if (inventorySystem == null) return;
       
        if(inventorySystem.gameObject.activeSelf) GameInstance.Instance.inventorySystem.gameObject.SetActive(false);
        else GameInstance.Instance.inventorySystem.gameObject.SetActive(false);
    }
}
