using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : Controller, IIdentifiable, IDamageable
{
    RightHand rightHand;

    public RightHand GetRightHand { get { if (rightHand == null) rightHand = GetComponentInChildren<RightHand>(); return rightHand; } }

    public GameObject model;
    enum EnemyType
    {
        Roaming,
        TargetDetect,
        TargetDetect_Obstacles,
        TargetDetect_Far,
        Battle
    
    }
    public bool theta;
    
    Vector3 enemyDir = Vector3.zero;
    [SerializeField]
    NavMeshAgent agent;
    PlayerController target;
    bool hunting=false;
  //  float detectTimer = 0;
    MoveCalculator moveCalculator = new MoveCalculator();
  //  EnemyType enemyType= EnemyType.Roaming;
    Coroutine coroutine;
    List<Vector3> destinations = new List<Vector3>();
    public CapsuleCollider capsuleCollider;

    Animator animator;
    public Animator modelAnimator { get { if (animator == null) { animator = model.GetComponent<Animator>(); animator.applyRootMotion = true; } return animator; } }


    int animationWorking;
    public Collider attackColider;

    public EnemyStruct enemyStruct;

    [NonSerialized]
    public bool bDead;

    //플레이어기반 전용
    [NonSerialized]
    public List<ItemStruct> itemStructs = new List<ItemStruct>();

    [NonSerialized]
    public int playerType = 0; //감염된 플레이어가 아니면 0

    List<PlayerController> pcs = new List<PlayerController>(10);
    public string ID { get; set; }

    private void Awake()
    {
        bDead = false;
     //   if (theta) MoveCalculator.SetBlockArea();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        moveSpeed = 100;
      //  agent.speed = moveSpeed;
    }

    private void Start()
    {
        agent.speed = 1.5f;
        agent.angularSpeed = 500f;
        //agent.autoRepath = true;
        //agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
       // Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Enemy"), true);
        //  agent.radius = 0.3f;
        //  agent.acceleration = 0;
        //  GetComponent<Rigidbody>().isKinematic = true;
    }
    float last;
    float next;
    // Update is called once per frame
    void Update()
    {
        if (animationWorking > 0) return;
        if (bDead) return;
        //   if (LastPosition != Transforms.position)
        /*    {
           //     LastPosition = Transforms.position;
                DetectPlayer(GameInstance.Instance.worldGrids.FindPlayersInGrid(Transforms, ref pcs));

            }*/
        if (next + last < Time.time)
        {
            last = Time.time;
            next = UnityEngine.Random.Range(0.3f, 0.8f);

            if (target == null)
            {
                FindPlayer(GameInstance.Instance.worldGrids.FindPlayersInGrid(Transforms, ref pcs));
                return;
            }

            float distance = Vector3.Distance(transform.position, target.transform.position);


            if (distance <= 1.5f)
            {
                StartAnimation("scratch", 2);
                //   GetRightHand.Attack(enemyStruct.attack);
                agent.isStopped = true;
            }
            else if (distance > 20)
            {
                agent.isStopped = true;
            }
            else
            {
                agent.isStopped = false;
                agent.SetDestination(target.Transforms.position);
            }

        }
      
        modelAnimator.SetFloat("speed", agent.velocity.magnitude);
        
    }


    void FindPlayer(List<PlayerController> players, bool skipAngle = false)
    {
        int index = 0;

        while (players.Count > index)
        {
            PlayerController pc = players[index++];
            float distance = Vector3.Distance(pc.Transforms.position, Transforms.position);

            Vector3 dir = (pc.Transforms.position - Transforms.position).normalized;
            float angle = Vector3.Angle(Transforms.forward, dir);   // 적의 전면부에서 110도 까지 탐지 

            if (angle < 110 / 2 || skipAngle)
            {
                if (!target) target = pc;
                else
                {
                    float dis = Vector3.Distance(target.Transforms.position, Transforms.position);
                    if (distance < dis)
                    {
                        target = pc;
                    }
                }
            }
        }
    }

    public void Setup()
    {
        gameObject.layer = 0b1010;
        ChangeTagLayer(Transforms, "Enemy", 0b1010);
     //   modelAnimator = GetComponentInChildren<Animator>();
    }
    void ChangeTagLayer(Transform parent, string newTag, int layerName)
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
    //탐색한 플레이어들 중 최적의 플레이어를 찾고 추적
    void DetectPlayer(List<PlayerController> players, bool skipAngle = false)
    {
        if (animationWorking > 0) return;
        int index = 0;
        while (players.Count > index)
        {
            PlayerController pc = players[index++];
            float distance = Vector3.Distance(pc.Transforms.position, Transforms.position);

        //    if (distance < 20)
            {
                Vector3 dir = (pc.Transforms.position - Transforms.position).normalized;
                float angle = Vector3.Angle(Transforms.forward, dir);   // 적의 전면부에서 110도 까지 탐지 


                if (angle < 110 / 2 || skipAngle)
                {
                    if (!target) target = pc;
                    else
                    {
                        float dis = Vector3.Distance(target.Transforms.position, Transforms.position);
                        if (distance < dis)
                        {
                            target = pc;
                        }
                    }
                }
            }
        }

        if(target != null)
        {
            float distance = Vector3.Distance(target.Transforms.position, Transforms.position);
            if (distance <= 1.5f)
            {
                StartAnimation("scratch", 2);
             //   GetRightHand.Attack(enemyStruct.attack);
                agent.isStopped = true;
            }
            else if(distance > 20)
            {
                agent.isStopped = true;
            }
            else
            {
                Debug.Log(target.Transforms.position);
                agent.isStopped = false;
                agent.SetDestination(target.Transforms.position);
            }
        }
        else
        {
            Debug.Log("L");
            agent.isStopped = true;
        }
    }

    Vector3 LineOfSight(Stack<Vector3> stack)
    {
        Vector3 size = new Vector3(0.5f, 0.5f, 0.5f);
        Vector3 start = Transforms.position;
        Vector3 pos = start;
        Debug.Log(stack.Count);
        stack.Pop();
        while (stack.Count > 0)
        {

            Vector3 elemnet = stack.Pop();
            Debug.Log(elemnet);
            Vector3 dir = elemnet - start;
            float distance = dir.magnitude;
            dir = Vector3.Normalize(dir);
            
             pos = elemnet;
        /*    if(!Physics.BoxCast(start, size, dir, Quaternion.identity, distance, 1 << 6))
            {
            }
            else
            {
                break;
            }*/
           
        }
        Debug.Log(pos);
        return pos;
    }

    public void BeAttacked(int damage)
    {
        if(bDead) return;

        Debug.Log($"Damaged {damage}");
        modelAnimator.SetTrigger("damaged");
        enemyStruct.health -= damage;
        if(enemyStruct.health <= 0)
        {
            enemyStruct.health = 0;
            bDead = true;

            agent.isStopped = true;

            capsuleCollider = GetComponent<CapsuleCollider>();
            capsuleCollider.excludeLayers = 0b1000;
           
         
            //   Destroy(Rigid);
            GameInstance.Instance.worldGrids.RemoveObjects(ID, MinimapIconType.Enemy);
            
            modelAnimator.SetTrigger("zombieDead");
            Reward();
        }
    }

    void Reward()
    {
        if (playerType != 0)
        {
            GameInstance.Instance.worldGrids.AddObjects(this.gameObject, MinimapIconType.ItemBox, false);
            Debug.Log("player Zombie Dead");
            if (itemStructs.Count > 0)
            {
                Debug.Log("Inventory Recovery");
            }
        }
        else
        {
            for (int i = 0; i < enemyStruct.dropStruct.Count; i++)
            {
                int random = UnityEngine.Random.Range(1, 101);
                if (random <= enemyStruct.dropStruct[i].item_chance)
                {
                    int index = enemyStruct.dropStruct[i].item_index - 1;
                    GameObject item = Instantiate(AssetLoader.loadedAssets[AssetLoader.itemAssetkeys[index].ID]);
                    item.transform.position = new Vector3(Transforms.position.x, Transforms.position.y + 1, Transforms.position.z);
                    GettableItem gettableItem = item.AddComponent<GettableItem>();

                    Rigidbody itemRigid = item.AddComponent<Rigidbody>();
                    itemRigid.AddForce(Vector3.up * 10f);
                }
            }

        }
    }

    void OpenInventorySystem(PlayerController pc)
    {
        GameInstance.Instance.uiManager.SwitchUI(UIType.BoxInventory, false);
        GameInstance.Instance.boxInventorySystem.GetOpponentInventory(itemStructs);

    }

    void StartAnimation(string animationName, float timer)
    {
        if (animationWorking > 0) return;
        animationWorking++;
        modelAnimator.SetTrigger(animationName);

        canMove--;
        Invoke("StopAnimation", timer);
    }
    void StopAnimation()
    {
        canMove++;
        animationWorking--;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (bDead && playerType != 0 && itemStructs.Count > 0)
        {
            PlayerController pc = other.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.RegisterAction(OpenInventorySystem);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (bDead && playerType != 0 && itemStructs.Count > 0)
        {
            PlayerController pc = other.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.RemoveAction(OpenInventorySystem);
            }
        }
    }
    private void OnDrawGizmos()
    {
        if (pcs.Count == 0) return; 
        if(!hunting) Gizmos.color = Color.yellow;
        else Gizmos.color = Color.red;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(Transforms.position, 6);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(Transforms.position, 1.5f);
        Gizmos.color = Color.yellow;
        Vector3 leftBoundary = Quaternion.Euler(0, -110 / 2, 0) * Transforms.forward * 110;
        Vector3 rightBoundary = Quaternion.Euler(0, 110 / 2, 0) * Transforms.forward * 110;

        Gizmos.DrawLine(Transforms.position, Transforms.position + leftBoundary);
        Gizmos.DrawLine(Transforms.position, Transforms.position + rightBoundary);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(Transforms.position, 20);
    }

    public bool Damaged(int damage, int layer)
    {
        if (layer != gameObject.layer) BeAttacked(damage);
        else return false;
        return true;
    }
    private void OnDestroy()
    {
        for (int i = 0; i < itemStructs.Count; i++)
        {
            itemStructs[i].Clear();
        }
    }

    public void SetController(GameObject model)
    {
        this.model = model;
        Invoke("ZombieSet", 0.5f);
       /* modelAnimator.SetLayerWeight(1, 0);
        modelAnimator.SetLayerWeight(2, 0);
        modelAnimator.SetLayerWeight(3, 0);
        modelAnimator.SetLayerWeight(4, 1);
        modelAnimator.SetFloat("zombieTimer", 1);
        modelAnimator.SetTrigger("standup");*/
    }

    void ZombieSet()
    {
        // Debug.Log("AA");
        modelAnimator.SetFloat("zombieTimer", 1);
        modelAnimator.SetTrigger("standup");
        modelAnimator.SetLayerWeight(1, 0);
        modelAnimator.SetLayerWeight(2, 0);
        modelAnimator.SetLayerWeight(3, 0);
        modelAnimator.SetLayerWeight(4, 1);
        //  modelAnimator.SetLayerWeight(0, 0);
    }
}
