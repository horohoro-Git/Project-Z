using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class PlayerController : Controller, IDamageable
{

    RightHand rightHand;

    public RightHand GetRightHand { get { if (rightHand == null) rightHand = GetComponentInChildren<RightHand>(); return rightHand; } }

    LeftHand leftHand;

    public LeftHand GetLeftHand { get { if (leftHand == null) leftHand = GetComponentInChildren<LeftHand>(); return leftHand; } }

    PlayerInput input;
    public PlayerInput Inputs
    {
        get
        {
            if(input == null) input = GetComponent<PlayerInput>();
            return input;
        }
    }
    Player player;
    public Player GetPlayer
    {
        get { if(player == null) player = GetComponent<Player>(); return player;}
    }

    UMACharacterAvatar avatar;
    public UMACharacterAvatar GetAvatar
    {
        get
        {
            if(avatar == null) avatar = GetComponentInChildren<UMACharacterAvatar>();
            return avatar;
        }
    }
    
    //public delegate void PlayerAction();
  //  public event PlayerAction Runs;
    private InputAction moveAction;  
    private InputAction sprintAction;

    private Action<InputAction.CallbackContext>[] slotHandlers;

    public event Action<PlayerController> InteractionEvent;
    Action<PlayerController> lastAction;
    [NonSerialized]
    public int lastGridX; 
    [NonSerialized]
    public int lastGridY;
    float motion;

    //  [NonSerialized]
    public Animator animator;
    public Animator modelAnimator { get { if (animator == null) { animator = model.GetComponent<Animator>(); animator.applyRootMotion = true; Equipment(GameInstance.Instance.quickSlotUI.slots[0], 0); } return animator; } }

    [NonSerialized]
    public PlayerState state = PlayerState.Default;
    float combatTimer;
    Coroutine combatMotion;

    bool inputEnabled = false;
    bool destroyed = false;
    int animationWorking;
    [NonSerialized]
    public Weapon equipWeapon;
    [NonSerialized]
    public Item equipItem;
    [NonSerialized]
    public int equipSlotIndex = -1;
    Action<PlayerController> getItemAction;

    public Animator zombieAnimator;
    public GameObject model;

    [NonSerialized]
    public PlayerCamera playerCamera;

    [NonSerialized]
    public bool loaded;

    [NonSerialized]
    public bool noAction;

    public GameObject testEffect;


    public void SetController(GameObject go, bool load, bool human = true)
    {
        if (human)
        {
            if (GameInstance.Instance.GetPlayers.Count == 0) GameInstance.Instance.AddPlayer(this);
            else GameInstance.Instance.GetPlayers[0] = this;
            GameInstance.Instance.playerController = this;
            Inputs.defaultActionMap = "OnMove";
            moveSpeed = 200f;
            slotHandlers = new Action<InputAction.CallbackContext>[10];
            for (int i = 0; i < 10; i++)
            {
                int index = i;
                int slotNumber = i + 1;
                slotHandlers[i] = ctx => SelectInventorySlot(index);
            }

            //입력 설정
            AddAction();
            Inputs.actions["OpenInventory"].performed -= OpenInventory;
            Inputs.actions["ZoomIn"].performed -= ZoomIn;
            Inputs.actions["ZoomOut"].performed -= ZoomOut;
            Inputs.actions["Interaction"].performed -= Interact;
            //테스트
            Inputs.actions["TestInventory"].performed -= JustTest;
            Inputs.actions["OpenInventory"].performed += OpenInventory;
            Inputs.actions["ZoomIn"].performed += ZoomIn;
            Inputs.actions["ZoomOut"].performed += ZoomOut;
            Inputs.actions["Interaction"].performed += Interact;
            //테스트
            Inputs.actions["TestInventory"].performed += JustTest;

            model = go;
            model.tag = "Player";
            model.layer = 0b0011;


            ChangeTagLayer(model.transform, "Player", 0b0011);

            GetRightHand.boxCollider.excludeLayers = 0b1000;
            GetLeftHand.boxCollider.excludeLayers = 0b1000;
            loaded = load;
            if (load)
            {
                if (!SaveLoadSystem.LoadPlayerData(this))
                {

                    //플레이어 초기 값

                    PlayerStruct playerStruct = new PlayerStruct(100, 100, 100, 100, 0,0, 100, 1, 0, 0, 200, 20 , 0, 1, 1, 1, 0);
                    GetPlayer.playerStruct = playerStruct;
                    GetPlayer.UpdatePlayer();
                }
            }
            else
            {
                SaveLoadSystem.LoadPlayerData(this);
            }
            equipSlotIndex = 0;

            if(GetPlayer.playerStruct.weight < GameInstance.Instance.inventorySystem.inventoryWeight)
            {
                moveSpeedMutiplier = 0.5f;
            }
            else
            {
                moveSpeedMutiplier = 1;
            }
            GameInstance.Instance.characterProfileUI.CreateCharacter(load, go);
            GameInstance.Instance.inventorySystem.UseItem(this, equipSlotIndex);
            GameInstance.Instance.worldGrids.UpdatePlayerInGrid(this, ref lastGridX, ref lastGridY, true);
        }
        else
        {
            model = go;
        }
        modelAnimator.runtimeAnimatorController = AssetLoader.animators[GameInstance.Instance.assetLoader.animatorKeys[0].animator_name];

        Destroy(go.GetComponent<Rigidbody>());  
        Destroy(go.GetComponent<Collider>());
        /* GameInstance.Instance.inventorySystem.LoadInvetory(0, 0, ItemData.GetItem(10), new WeaponStruct(), new ConsumptionStruct(), GameInstance.Instance.assetLoader.armors[10]);
         GameInstance.Instance.boxInventorySystem.LoadInvetory(0, 0, ItemData.GetItem(10), new WeaponStruct(), new ConsumptionStruct(), GameInstance.Instance.assetLoader.armors[10]);
         GameInstance.Instance.inventorySystem.LoadInvetory(0, 1, ItemData.GetItem(9), new WeaponStruct(), new ConsumptionStruct(), GameInstance.Instance.assetLoader.armors[9]);
         GameInstance.Instance.boxInventorySystem.LoadInvetory(0, 1, ItemData.GetItem(9), new WeaponStruct(), new ConsumptionStruct(), GameInstance.Instance.assetLoader.armors[9]);

         GameInstance.Instance.inventorySystem.LoadInvetory(0, 2, ItemData.GetItem(8), new WeaponStruct(), new ConsumptionStruct(), GameInstance.Instance.assetLoader.armors[8]);
         GameInstance.Instance.boxInventorySystem.LoadInvetory(0,2, ItemData.GetItem(8), new WeaponStruct(), new ConsumptionStruct(), GameInstance.Instance.assetLoader.armors[8]);
 */
        /*GameInstance.Instance.inventorySystem.LoadInvetory(1, 0, ItemData.GetItem(4), new WeaponStruct(), new ConsumptionStruct(), GameInstance.Instance.assetLoader.armors[4]);
        GameInstance.Instance.boxInventorySystem.LoadInvetory(1, 0, ItemData.GetItem(4), new WeaponStruct(), new ConsumptionStruct(), GameInstance.Instance.assetLoader.armors[4]);

         GameInstance.Instance.inventorySystem.LoadInvetory(1, 1, ItemData.GetItem(5), new WeaponStruct(), new ConsumptionStruct(), GameInstance.Instance.assetLoader.armors[5]);
         GameInstance.Instance.boxInventorySystem.LoadInvetory(1, 1, ItemData.GetItem(5), new WeaponStruct(), new ConsumptionStruct(), GameInstance.Instance.assetLoader.armors[5]);

         GameInstance.Instance.inventorySystem.LoadInvetory(1, 2, ItemData.GetItem(6), new WeaponStruct(), new ConsumptionStruct(), GameInstance.Instance.assetLoader.armors[6]);
         GameInstance.Instance.boxInventorySystem.LoadInvetory(1, 2, ItemData.GetItem(6), new WeaponStruct(), new ConsumptionStruct(), GameInstance.Instance.assetLoader.armors[6]);

         GameInstance.Instance.inventorySystem.LoadInvetory(1, 3, ItemData.GetItem(7), new WeaponStruct(), new ConsumptionStruct(), GameInstance.Instance.assetLoader.armors[7]);
         GameInstance.Instance.boxInventorySystem.LoadInvetory(1, 3, ItemData.GetItem(7), new WeaponStruct(), new ConsumptionStruct(), GameInstance.Instance.assetLoader.armors[7]);

         GameInstance.Instance.inventorySystem.LoadInvetory(1, 4, ItemData.GetItem(8), new WeaponStruct(), new ConsumptionStruct(), GameInstance.Instance.assetLoader.armors[8]);
         GameInstance.Instance.boxInventorySystem.LoadInvetory(1, 4, ItemData.GetItem(8), new WeaponStruct(), new ConsumptionStruct(), GameInstance.Instance.assetLoader.armors[8]);

         GameInstance.Instance.inventorySystem.LoadInvetory(1, 5, ItemData.GetItem(9), new WeaponStruct(), new ConsumptionStruct(), GameInstance.Instance.assetLoader.armors[9]);
         GameInstance.Instance.boxInventorySystem.LoadInvetory(1, 5, ItemData.GetItem(9), new WeaponStruct(), new ConsumptionStruct(), GameInstance.Instance.assetLoader.armors[9]);*/
    }

    public void ChangeTagLayer(Transform parent, string newTag, int layerName)
    {
        if (parent != null)
        { 
            foreach (Transform child in parent)
            {
                if (child != null)
                {
                    child.gameObject.tag = newTag;
                    child.gameObject.layer = layerName;
                    ChangeTagLayer(child, newTag, layerName);
                }
            }
        }
    }

    public void SetPlayerData(Vector3 pos, Quaternion rot, PlayerStruct playerStruct)
    {
   //     Transforms.position = pos;
    //    Transforms.rotation = rot;
        Rigid.MovePosition(pos);
        Rigid.MoveRotation(rot);
        GetPlayer.playerStruct = playerStruct;
        GetPlayer.UpdatePlayer();
      //  SaveLoadSystem.SavePlayerData(GetPlayer);
    }

    private void OnEnable()
    {
    }

    private void OnDisable()
    {
        RemoveAction();
        Inputs.actions["OpenInventory"].performed -= OpenInventory;
        Inputs.actions["ZoomIn"].performed -= ZoomIn;
        Inputs.actions["ZoomOut"].performed -= ZoomOut;
        Inputs.actions["TestInventory"].performed -= JustTest;
        Inputs.actions["Interaction"].performed -= Interact;
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
        for (int i = 0; i < 10; i++)
        {
            int slotNumber = i + 1;
            Inputs.actions[$"Item{slotNumber}"].performed -= slotHandlers[i];
        }
        Inputs.actions["WASD"].performed -= MoveHandle;
        Inputs.actions["WASD"].canceled -= MoveStop;
        Inputs.actions["Run"].performed -= Run;
        Inputs.actions["Run"].canceled -= RunStop;
        Inputs.actions["Attack"].performed -= Attack;
        Inputs.actions["Attack"].canceled -= EndAttack;
        for (int i = 0; i < 10; i++)
        {
            int slotNumber = i + 1;
            Inputs.actions[$"Item{slotNumber}"].performed += slotHandlers[i];
        }
        Inputs.actions["WASD"].performed += MoveHandle;
        Inputs.actions["WASD"].canceled += MoveStop;
        Inputs.actions["Run"].performed += Run;
        Inputs.actions["Run"].canceled += RunStop;
        Inputs.actions["Attack"].performed += Attack;
        Inputs.actions["Attack"].canceled += EndAttack;
       
    }
    public void RemoveAction()
    {
        if (destroyed) return;
        if (!inputEnabled) return;
        inputEnabled = false;
        Inputs.actions["WASD"].performed -= MoveHandle;
        Inputs.actions["WASD"].canceled -= MoveStop;
        Inputs.actions["Run"].performed -= Run;
        Inputs.actions["Run"].canceled -= RunStop;
        Inputs.actions["Attack"].performed -= Attack;
        Inputs.actions["Attack"].canceled -= EndAttack;
        for (int i = 0; i < 10; i++)
        {
            int slotNumber = i + 1;
            Inputs.actions[$"Item{slotNumber}"].performed -= slotHandlers[i];
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
     
    }

    // Update is called once per frame
    void Update()
    {
       
        if (state == PlayerState.Dead) return;
      
        LastPosition = Transforms.position;
        GameInstance.Instance.worldGrids.UpdatePlayerInGrid(this, ref lastGridX, ref lastGridY, false);
    
       
        if(animationWorking > 0)
        {
            modelAnimator.SetFloat("lookAround", 0);
        }

        if(combatTimer < Time.time)
        {
            state = PlayerState.Default;  
            
        }
        if(lookAround)
        {
            float dot = Vector3.Dot(transform.forward, viewDir);
            Vector3 cross = Vector3.Cross(transform.forward, viewDir);
            float side = cross.y;
            if(viewDir == Vector3.zero) side = 0;
            if (dot > -0.2f)
            {
                //앞
                modelAnimator.SetFloat("speed", viewSpeed / moveSpeed, 0.1f, Time.deltaTime);
            }
            else
            {
                // 뒤
                modelAnimator.SetFloat("speed", -viewSpeed / moveSpeed, 0.1f, Time.deltaTime);
            }
            modelAnimator.SetFloat("lookAround", side, 0.1f, Time.deltaTime);
        }
        else
        {
            modelAnimator.SetFloat("lookAround", around);
            modelAnimator.SetFloat("speed", viewSpeed / moveSpeed);
            modelAnimator.SetFloat("dir", viewVelocity);
           
        }
     
        if (state == PlayerState.Combat)
        {
            motion += Time.deltaTime;
            motion = motion > 1 ? 1 : motion;
            modelAnimator.SetFloat("combatMotion", motion);
        }
        else if(state == PlayerState.Default)
        {
            motion -= Time.deltaTime;
            motion = motion < 0 ? 0 : motion;
            modelAnimator.SetFloat("combatMotion", motion);
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
        Debug.Log("MoveStop");
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

    PointerEventData eventData = new PointerEventData(EventSystem.current);
    List<RaycastResult> raycastResults = new List<RaycastResult>();
    void Attack(InputAction.CallbackContext callback)
    {
        if (animationWorking > 0) return;
        GraphicRaycaster raycaster = GameInstance.Instance.uiManager.graphicRaycaster;
        eventData.position = Input.mousePosition;
        raycastResults.Clear();
        raycaster.Raycast(eventData, raycastResults);
        if (raycastResults.Count > 0) return;

        Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(r, out RaycastHit h, float.MaxValue, 0b100000000000))
        {
            turnRotate = true;
            rotateVector = h.point;
        }
       

        if (equipItem != null)
        {
            if (equipItem.itemStruct.item_type == ItemType.Equipmentable)  //무기
            {
                Weapon weapon = equipItem.GetComponent<Weapon>();
                switch (weapon.weaponStruct.weapon_type)
                {
                    case WeaponType.None:
                        break;
                    case WeaponType.Axe:
                        StartAnimation("cut", 1);
                        break;
                }
            }
            else if(equipItem.itemStruct.item_type == ItemType.Consumable) //음식 소모
            {
                Debug.Log("Work");
                Invoke("Consumption", 1.7f);
                StartAnimation("eating", 1.8f);
            }
        }
        else //주먹으로 공격
        {
            Punch();
            CancelInvoke("StopMotion");
            Invoke("StopMotion", 1);
        }
        combatTimer = Time.time + 5f;
        state = PlayerState.Combat;
    }
    void EndAttack(InputAction.CallbackContext callback)
    {
     //   attack = false;
    }

    void Punch()
    {
        int r = UnityEngine.Random.Range(0, 2);
        if (r == 1)
        {
            StartAnimation("punchLeft", 0.8f);
            //GetLeftHand.GetWeaponTrail.Trail(true);
        }
        else
        {
            StartAnimation("punchRight", 0.8f);
          //  GetRightHand.GetWeaponTrail.Trail(true);
        }
    }
 
    void Interact(InputAction.CallbackContext callback)
    {
        if (lastAction == null) return;
        lastAction.Invoke(this);
    }

    public void GetItem_Animation(Action<PlayerController> action)
    {
        if (animationWorking > 0) return;

        getItemAction = action;
        StartAnimation("item", 0.8f);
        Invoke("StopMotion", 1.117f);
        Invoke("GetItem_Callback", 0.2f);
    }
    void GetItem_Callback()
    {
        getItemAction?.Invoke(this);
    }
    public void CutTree()
    {
        modelAnimator.SetTrigger("cut");
    }

    public void Consumption()
    {
        ConsumptionItem item = equipItem.GetComponent<ConsumptionItem>();
        if (item != null)
        {
            item.Consumption();
        }
        //equipItem.
    }


    public void RegisterAction(Action<PlayerController> action)
    {
        InteractionEvent += action;
        Delegate[] actions = InteractionEvent.GetInvocationList();
        lastAction = (Action<PlayerController>)actions[0];
    }


    public void RemoveAction(Action<PlayerController> action)
    {
       // InteractionEvent.GetInvocationList();
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

    public void OpenInventory(InputAction.CallbackContext callback)
    {
        if (animationWorking > 0) return;
        InventorySystem inventorySystem = GameInstance.Instance.inventorySystem;
        if (inventorySystem == null) return;


        GameInstance.Instance.uiManager.SwitchUI(UIType.Inventory, false);
    }

    void SelectInventorySlot(int index)
    {
        GameInstance.Instance.inventorySystem.UseItem(this ,index);
    }

    public void Equipment(Slot equipItem, int index, bool forcedEquip = false)
    {
        if (animationWorking > 0 && !forcedEquip) return;

        equipSlotIndex = index;

        if (AllEventManager.customEvents.TryGetValue(3, out var events)) ((Action<PlayerController, Slot, int, bool>)events)?.Invoke(this, equipItem, index, forcedEquip);
    }

    public void Unequipment()
    {
        if (equipWeapon != null)
        {
            Destroy(equipWeapon.gameObject);
            equipWeapon = null;
        }
        Equipment(GameInstance.Instance.quickSlotUI.slots[equipSlotIndex], equipSlotIndex);
        GameInstance.Instance.inventorySystem.UseItem(this, equipSlotIndex);
    }

    void StartAnimation(string animationName, float timer)
    {
        if (animationWorking > 0) return;
        animationWorking++;
        modelAnimator.SetTrigger(animationName);
       
        canMove++;
        Invoke("StopAnimation", timer);
    }

    void StopAnimation()
    {
        canMove--;
        animationWorking--;
    }

    public void GetDamage(int damage, int opponentLayer)
    {
        if (state == PlayerState.Dead) return;
        //GetPlayer.playerStruct.hp -= damage;
        GetPlayer.GetDamage(damage);
        if (GetPlayer.playerStruct.hp <= 0)
        {
            if (AllEventManager.customEvents.TryGetValue(4, out var events)) ((Action<PlayerController, int>)events)?.Invoke(this, opponentLayer);
           /* modelAnimator.SetTrigger("dead");
            GetPlayer.dead = true;
            state = PlayerState.Dead;
            playerCamera.lookAround = false;
            lookAround = false;
            GameInstance.Instance.worldGrids.RemovePlayer(this, ref lastGridX, ref lastGridY);*/

        /*    if (opponentLayer == 0b1010)
            {
                gameObject.tag = "Enemy";
                gameObject.layer = 0b1010;
                Rigid.excludeLayers = 0;
                ChangeTagLayer(Transforms, "Enemy", 0b1010);
                GetComponent<CapsuleCollider>().excludeLayers = 0;
                GetLeftHand.boxCollider.excludeLayers = 0b10000000000;
                GetRightHand.boxCollider.excludeLayers = 0b10000000000;
                Rigid.interpolation = RigidbodyInterpolation.None;
                //죽은 시체에 인벤토리의 아이템 적용
                gameObject.AddComponent<NavMeshAgent>();
                EnemyController enemyController = gameObject.AddComponent<EnemyController>();
                enemyController.playerType = 1;
                enemyController.capsuleCollider = GetComponent<CapsuleCollider>();
                enemyController.bDead = true;
                enemyController.enemyStruct = GameInstance.Instance.assetLoader.enemies[0];
                GameInstance.Instance.worldGrids.AddObjects(enemyController.gameObject, MinimapIconType.Enemy, false);
                for (int i = 0; i < GameInstance.Instance.inventorySystem.slotNum; i++)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        ItemStruct itemStruct = GameInstance.Instance.inventorySystem.inventoryArray[i, j].item;
                        if (itemStruct.item_index == 0) continue;
                        enemyController.itemStructs.Add(itemStruct);
                    }
                }

                SaveLoadSystem.SavePlayerData(GetPlayer);


                //인벤토리 초기화
                GameInstance.Instance.inventorySystem.ResetInventory();

                //인벤토리와 적 데이터 저장
                SaveLoadSystem.SaveEnemyInfo();
                SaveLoadSystem.SaveInventoryData();

                Invoke("Infected", 2f);
            }

            GameInstance.Instance.uiManager.SwitchUI(UIType.Dead);*/

        }
        else
        {
            modelAnimator.SetTrigger("hit");
           // modelAnimator.SetTrigger("dead");
            Invoke("StopAnimation", 0.5f);
            Invoke("StopMotion", 0.667f);
        }
        canMove++;
        animationWorking++;
    }

    void Infected()
    {
        //감염되어 일어나는 애니메이션
        modelAnimator.SetTrigger("infected");

        //일정시간 이후 기본 레이어 끄기, 좀비 애니메이션 레이어 설정
        Invoke("BecomeToZombie", 7);
    }

    void BecomeToZombie()
    {
        StartCoroutine(ZombieUpdate());
    }

    IEnumerator ZombieUpdate()    
    {
        Vector3 vector3 = model.transform.localPosition;
        Vector3 target = new Vector3(0, 0, 0);
        float timer = 0;
        while(timer < 1)
        {
            timer += Time.deltaTime;
            model.transform.localPosition = Vector3.Lerp(vector3, target, timer / 1);
            modelAnimator.SetFloat("zombieTimer", timer);
            yield return null;
        }
        model.transform.localPosition = target;
        modelAnimator.SetTrigger("standup");

      //  modelAnimator.SetLayerWeight(1, 0);
       // modelAnimator.SetLayerWeight(2, 0);

        Rigid.constraints = RigidbodyConstraints.FreezeAll;
       
        NPCController enemyController = GetComponent<NPCController>();
        enemyController.Setup(enemyController.npcStruct, false);
        NPCEventHandler.Publish(1000004, enemyController);
        enemyController.bDead = false;

      /*  Inputs.actions["OpenInventory"].performed -= OpenInventory;
        Inputs.actions["ZoomIn"].performed -= ZoomIn;
        Inputs.actions["ZoomOut"].performed -= ZoomOut;
        Inputs.actions["Interaction"].performed -= Interact;
        //테스트
        Inputs.actions["TestInventory"].performed -= JustTest;

        for (int i = 0; i < 10; i++)
        {
            int slotNumber = i + 1;
            Inputs.actions[$"Item{slotNumber}"].performed -= slotHandlers[i];
        }
        Inputs.actions["WASD"].performed -= MoveHandle;
        Inputs.actions["WASD"].canceled -= MoveStop;
        Inputs.actions["Run"].performed -= Run;
        Inputs.actions["Run"].canceled -= RunStop;
        Inputs.actions["Attack"].performed -= Attack;
        Inputs.actions["Attack"].canceled -= EndAttack;*/
        //   Inputs.DeactivateInput();
         Destroy(GetComponent<PlayerInput>());

         RemoveAction();
         Inputs.actions["OpenInventory"].performed -= OpenInventory;
      
        GameInstance.Instance.gameManager.PlayerSettings(false);


      

        if (playerCamera != null) playerCamera.ResetPlayer();
        Destroy(GetPlayer);
        Destroy(this);
    }
    
    void StopMotion()
    {
        motion = 0;
    }

    void ZoomIn(InputAction.CallbackContext callback)
    {
        if(lookAround)
        {
            Camera cam = playerCamera.GetComponentInChildren<Camera>();
            if (cam != null)
            {
                cam.orthographicSize -= 0.5f;
                if (cam.orthographicSize < 4) cam.orthographicSize = 4;
            }
        }
    }
    
    void ZoomOut(InputAction.CallbackContext callback)
    {
        if (lookAround)
        {
            Camera cam = playerCamera.GetComponentInChildren<Camera>();
            if (cam != null)
            {
                cam.orthographicSize += 0.5f;
                if (cam.orthographicSize > 10) cam.orthographicSize = 10;
            }
        }
    }

    void JustTest(InputAction.CallbackContext callback)
    {
        GameInstance.Instance.uiManager.SwitchUI(UIType.BoxInventory, false);
    }

    public bool Damaged(int damage, int layer)
    {
        if(gameObject.layer != layer) GetDamage(damage, layer);
        else return false;
        return true;
    }
}
