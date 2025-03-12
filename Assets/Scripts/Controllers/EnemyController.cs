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

    RightHand GetRightHand { get { if (rightHand == null) rightHand = GetComponentInChildren<RightHand>(); return rightHand; } }


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
    float detectTimer = 0;
    MoveCalculator moveCalculator = new MoveCalculator();
    EnemyType enemyType= EnemyType.Roaming;
    Coroutine coroutine;
    List<Vector3> destinations = new List<Vector3>();
    public CapsuleCollider capsuleCollider;
    Animator modelAnimator;
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
        modelAnimator = GetComponentInChildren<Animator>();
        moveSpeed = 100;
      //  agent.speed = moveSpeed;
    }

    private void Start()
    {
        agent.speed = 1.5f;
        agent.angularSpeed = 500f;

    }

    // Update is called once per frame
    void Update()
    {
        if(bDead) return;
     //   if (LastPosition != Transforms.position)
        {
       //     LastPosition = Transforms.position;
            DetectPlayer(GameInstance.Instance.worldGrids.FindPlayersInGrid(Transforms, ref pcs));
            //Debug.Log(agent.velocity.magnitude);
            modelAnimator.SetFloat("speed", agent.velocity.magnitude);
        }
       /* if (GameInstance.Instance.player)
        {
            DetectPlayer(false);
        }
        if(enemyType == EnemyType.TargetDetect)
        {
            agent.SetDestination(GameInstance.Instance.player.Transforms.position);
        }
      */
    }

    public void Setup()
    {
        gameObject.layer = 0b1010;
        ChangeTagLayer(Transforms, "Enemy", 0b1010);
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


            if (distance < 10)  
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
                GetRightHand.Attack(2, enemyStruct.attack);
                agent.isStopped = true;
            }
            else if(distance < 6)
            {
                agent.isStopped = false;
                agent.SetDestination(target.Transforms.position);
            }
            else if(distance > 20)
            {
                agent.isStopped = true;
            }
        }
        else
        {
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

            if (itemStructs.Count > 0)
            {
                capsuleCollider.isTrigger = true;
            }
            else capsuleCollider.excludeLayers = 0b1000;
            //   Destroy(Rigid);
            GameInstance.Instance.worldGrids.RemoveLives(ID);
            modelAnimator.SetTrigger("dead");
            Reward();
        }
    }

    void Reward()
    {
        for (int i = 0; i < enemyStruct.dropStruct.Count; i++)
        {
            int random = UnityEngine.Random.Range(1, 101);
            if(random <= enemyStruct.dropStruct[i].item_chance)
            {
                int index = enemyStruct.dropStruct[i].item_index - 1;
                GameObject item = Instantiate(GameInstance.Instance.assetLoader.loadedAssets[AssetLoader.itemAssetkeys[index]]);
                item.transform.position = new Vector3(Transforms.position.x, Transforms.position.y + 1, Transforms.position.z);
                GettableItem gettableItem = item.AddComponent<GettableItem>();
                
                Rigidbody itemRigid = item.AddComponent<Rigidbody>();
                itemRigid.AddForce(Vector3.up * 10f);

            }
        }
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
        if (!bDead) return;
       /* if(other.gameObject.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            player.GetDamage(enemyStruct.attack);
            Debug.Log("Attack Hit");
        }*/
    }
    private void OnDrawGizmos()
    {
        
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

    public void Damaged(int damage)
    {
        BeAttacked(damage);
    }
    private void OnDestroy()
    {
        for (int i = 0; i < itemStructs.Count; i++)
        {
            itemStructs[i].Clear();
        }
    }
}
