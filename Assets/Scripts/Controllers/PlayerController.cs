using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using System.Timers;
using Unity.VisualScripting;
using UnityEditor;
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
    PlayerInput Inputs
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
    
    //public delegate void PlayerAction();
  //  public event PlayerAction Runs;
    private InputAction moveAction;  
    private InputAction sprintAction;

    private Action<InputAction.CallbackContext>[] slotHandlers;

    public event Action<PlayerController> InteractionEvent;
    Action<PlayerController> lastAction;
    int lastGridX = 0; 
    int lastGridY = 0;
    float motion;

    [NonSerialized]
    public Animator modelAnimator;

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
    public PlayerCamera camera;

    [NonSerialized]
    public bool loaded;

    [NonSerialized]
    public bool noAction;
    private void Awake()
    {
    
    }

    public void SetController(GameObject go, bool load, bool human = true)
    {
        if (human)
        {
            
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
            Inputs.actions["OpenInventory"].performed += OpenInventory;
            Inputs.actions["ZoomIn"].performed += ZoomIn;
            Inputs.actions["ZoomOut"].performed += ZoomOut;

            //테스트
            Inputs.actions["TestInventory"].performed += JustTest;


            modelAnimator = go.GetComponent<Animator>();
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

                    PlayerStruct playerStruct = new PlayerStruct(100, 100, 100, 100, 0, 100, 1, 0, 0, 1, 1, 1);

                    GetPlayer.playerStruct = playerStruct;
                    GetPlayer.UpdatePlayer();
                }
            }
            else
            {
                //플레이어 초기 값
                PlayerStruct playerStruct = new PlayerStruct(100, 100, 100, 100, 0, 100, 1, 0, 0, 1, 1, 1);

                GetPlayer.playerStruct = playerStruct;
                GetPlayer.UpdatePlayer();
            }
        }
        else
        {
            modelAnimator = go.GetComponent<Animator>();
            model = go;
        }

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
            Inputs.actions[$"Item{slotNumber}"].performed += slotHandlers[i];
        }
        Inputs.actions["WASD"].performed += MoveHandle;
        Inputs.actions["WASD"].canceled += MoveStop;
        Inputs.actions["Run"].performed += Run;
        Inputs.actions["Run"].canceled += RunStop;
        Inputs.actions["Interaction"].performed += Interact;
        Inputs.actions["Attack"].performed += Attack;
        Inputs.actions["Attack"].canceled += EndAttack;
       
    }
    public void RemoveAction()
    {
        if (destroyed) return;
        if (!inputEnabled) return;
        inputEnabled = false;
        Inputs.actions["WASD"].performed -= MoveHandle;
        Inputs.actions["WASD"].performed -= MoveStop;
        Inputs.actions["Run"].performed -= Run;
        Inputs.actions["Run"].canceled -= RunStop;
        Inputs.actions["Interaction"].performed -= Interact;
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
        if (GameInstance.Instance.GetPlayers.Count == 0) GameInstance.Instance.AddPlayer(this);
        else GameInstance.Instance.GetPlayers[0] = this;
        Equipment(GameInstance.Instance.quickSlotUI.slots[0], 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (state == PlayerState.Dead) return;

        if (LastPosition != Transforms.position)
        {
            LastPosition = Transforms.position;
            GameInstance.Instance.worldGrids.UpdatePlayerInGrid(this, ref lastGridX, ref lastGridY);
        }

        if(combatTimer < Time.time)
        {
            state = PlayerState.Default;  
            
        }

        if(lookAround)
        {
            float dot = Vector3.Dot(transform.forward, moveDir);
            Vector3 cross = Vector3.Cross(transform.forward, moveDir);
            float side = cross.y;
            if (dot > -0.2f)
            {
                //앞
                modelAnimator.SetFloat("speed", viewSpeed / 200, 0.1f, Time.deltaTime);
            }
            else
            {
                // 뒤
                modelAnimator.SetFloat("speed", -viewSpeed / 200, 0.1f, Time.deltaTime);
            }
            modelAnimator.SetFloat("lookAround", side, 0.1f, Time.deltaTime);
        }
        else
        {
            modelAnimator.SetFloat("lookAround", around);
            modelAnimator.SetFloat("speed", viewSpeed / 200);
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

    bool reinput = false;
    bool attack = false;
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
                        weapon.Attack(0.35f, 0.6f);
                        /*    equipWeapon.GetComponent<Axe>().EndAttack();*/
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
        //attack = true;
     /*   if (equipWeapon != null)    //무기로 공격
        {
            switch(equipWeapon.type)
            {
                case WeaponType.None:
                    break;
                case WeaponType.Axe:
                    StartAnimation("cut", 1);
                    equipWeapon.Attack(0.35f, 0.6f);
                *//*    equipWeapon.GetComponent<Axe>().EndAttack();*//*
                    break;
            }
        }
        else //주먹으로 공격
        {
            Punch();
            CancelInvoke("StopMotion");
            Invoke("StopMotion", 1);
        }*/
        combatTimer = Time.time + 5f;
        state = PlayerState.Combat;
    }
    void EndAttack(InputAction.CallbackContext callback)
    {
        attack = false;
    }

    void Punch()
    {
        int r = UnityEngine.Random.Range(0, 2);
        if (r == 1)
        {
            GetLeftHand.Attack(0.8f, GetPlayer.playerStruct.attackDamage);
            StartAnimation("punchLeft", 0.8f);
        }
        else
        {
            GetRightHand.GetComponentInChildren<RightHand>().Attack(0.8f, GetPlayer.playerStruct.attackDamage);
            StartAnimation("punchRight", 0.8f);
        }
    }

    void InputTimer()
    {
        reinput = true;
    }
    bool attackAgain;
 
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

    void OpenInventory(InputAction.CallbackContext callback)
    {
        InventorySystem inventorySystem = GameInstance.Instance.inventorySystem;
        if (inventorySystem == null) return;


        GameInstance.Instance.uiManager.SwitchUI(UIType.Inventory, false);
    }

    void SelectInventorySlot(int index)
    {
        GameInstance.Instance.inventorySystem.UseItem(this ,index);
    }

    public void Equipment(Slot equipItem, int index)
    {
        if (animationWorking > 0) return;

        AssetLoader loader = GameInstance.Instance.assetLoader;
        //무기
        if (equipItem.item.item_type == ItemType.Equipmentable)
        {
            if(this.equipItem != null)
            {
                Destroy(this.equipItem.gameObject);
                this.equipItem = null;
            }
            equipSlotIndex = index;
            this.equipItem = Instantiate(loader.loadedAssets[AssetLoader.itemAssetkeys[equipItem.item.item_index - 1]]).GetComponent<Item>();  //equipItem.itemGO).GetComponent<Item>();
            this.equipItem.equippedPlayer = GetPlayer;
            AttachItem attachItem = GetComponentInChildren<AttachItem>();
            this.equipItem.transform.SetParent(attachItem.transform);
            this.equipItem.itemStruct = equipItem.item;
            this.equipItem.GetComponent<Weapon>().weaponStruct = equipItem.weapon;
            
            this.equipItem.transform.localPosition = Vector3.zero;
            this.equipItem.transform.localRotation = Quaternion.Euler(-90, 120, 0);
            modelAnimator.SetFloat("equip", 1);
            /*   if (equipWeapon != null)
               {
                   Destroy(equipWeapon.gameObject);
                   equipWeapon = null;
               }
               equipSlotIndex = index;
               equipWeapon = Instantiate(equipItem.itemGO).GetComponent<Weapon>();
               equipWeapon.equippedPlayer = GetPlayer;
               AttachItem attachItem = GetComponentInChildren<AttachItem>();
               equipWeapon.transform.SetParent(attachItem.transform);

               equipWeapon.transform.localPosition = new Vector3(0, 0, 0);
               equipWeapon.transform.localRotation = Quaternion.Euler(-90, 120, 0);
               modelAnimator.SetFloat("equip", 1);*/
        }
        else if (equipItem.item.item_type == ItemType.Consumable)     //음식을 손에 듬
        {
            if (this.equipItem != null)
            {
                Destroy(this.equipItem.gameObject);
                this.equipItem = null;
            }
            Debug.Log(equipItem.item.item_index);
            equipSlotIndex = index;
            this.equipItem = Instantiate(loader.loadedAssets[AssetLoader.itemAssetkeys[equipItem.item.item_index - 1]]).GetComponent<Item>();
            this.equipItem.equippedPlayer = GetPlayer;
            AttachItem attachItem = GetComponentInChildren<AttachItem>();
            this.equipItem.transform.SetParent(attachItem.transform);
            this.equipItem.itemStruct = equipItem.item;
            this.equipItem.GetComponent<ConsumptionItem>().consumtionStruct = equipItem.consumption;

            this.equipItem.transform.localPosition = Vector3.zero;
            this.equipItem.transform.localRotation = Quaternion.Euler(-90, 120, 0);
            modelAnimator.SetFloat("equip", 0);
        }
        else
        {
            if (this.equipItem != null)
            {
                Destroy(this.equipItem.gameObject);
                this.equipItem = null;
            }
            equipSlotIndex = index;
        }
    }

    public void Unequipment()
    {
        if (equipWeapon != null)
        {
            Destroy(equipWeapon.gameObject);
            equipWeapon = null;
        }
        Equipment(GameInstance.Instance.quickSlotUI.slots[equipSlotIndex], equipSlotIndex);
        
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

    public void GetDamage(int damage)
    {
        if (state == PlayerState.Dead) return;
        //GetPlayer.playerStruct.hp -= damage;
        GetPlayer.GetDamage(damage);
        if (GetPlayer.playerStruct.hp <= 0)
        {
            modelAnimator.SetTrigger("dead");
            state = PlayerState.Dead;
            GameInstance.Instance.worldGrids.RemovePlayer(this, ref lastGridX, ref lastGridY);
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

            for (int i = 0; i < GameInstance.Instance.inventorySystem.slotNum; i++)
            {
                for(int j= 0; j<10; j++)
                {
                    ItemStruct itemStruct = GameInstance.Instance.inventorySystem.inventoryArray[i, j].item;
                    if (itemStruct.item_index == 0) continue;
                    enemyController.itemStructs.Add(itemStruct);
                }
            }
            GameInstance.Instance.worldGrids.AddLives(enemyController.gameObject, false);
            //인벤토리 초기화
            GameInstance.Instance.inventorySystem.ResetInventory();


            //인벤토리와 적 데이터 저장
            SaveLoadSystem.SaveEnemyInfo();
            SaveLoadSystem.SaveInventoryData();

           // GameInstance.Instance.enemySpawner.enemies.Add(enemyController);


            Invoke("Infected",2f);
        }
        else
        {
            modelAnimator.SetTrigger("hit");
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
       // model.transform.localPosition = new Vector3(0, 0,0);
      //  modelAnimator.SetLayerWeight(1, 0);
     //   modelAnimator.SetLayerWeight(2, 0);
      //  modelAnimator.SetLayerWeight(3, 0);
     //   modelAnimator.SetLayerWeight(4, 1);
      //  modelAnimator.SetLayerWeight(0, 0);

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

        modelAnimator.SetLayerWeight(1, 0);
        modelAnimator.SetLayerWeight(2, 0);

       
        EnemyController enemyController = GetComponent<EnemyController>();
        enemyController.bDead = false;
        enemyController.enemyStruct = GameInstance.Instance.assetLoader.enemies[0];
        Destroy(GetComponent<PlayerInput>());

        RemoveAction();
        Inputs.actions["OpenInventory"].performed -= OpenInventory;
        GameInstance.Instance.gameManager.PlayerSettings(false);
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
            Camera cam = camera.GetComponentInChildren<Camera>();
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
            Camera cam = camera.GetComponentInChildren<Camera>();
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
 /*   private void OnApplicationQuit()
    {
        SaveLoadSystem.SavePlayerData(GetPlayer);
        
    }*/

    public void Damaged(int damage)
    {
        GetDamage(damage);
    }
}
