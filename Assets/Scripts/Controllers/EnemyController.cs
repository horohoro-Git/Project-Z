using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : Controller, IIdentifiable
{

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
    [SerializeField]
    CapsuleCollider capsuleCollider;
    Animator modelAnimator;
    int animationWorking;
    public Collider attackColider;

    bool bDead;
    int attackDamage;
    int hp;
    public string ID { get; set; }

    private void Awake()
    {
        bDead = false;
        hp = 100;
        attackDamage = 50;
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

        Invoke("S", 0.5f);
    }

    void S()
    {
        GameInstance.Instance.worldGrids.AddLives(this.gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        if(bDead) return;
     //   if (LastPosition != Transforms.position)
        {
       //     LastPosition = Transforms.position;
            DetectPlayer(GameInstance.Instance.worldGrids.FindPlayersInGrid(Transforms));
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
        hp -= damage;
        if(hp <= 0)
        {
            hp = 0;
            bDead = true;

            agent.isStopped = true;

            capsuleCollider.excludeLayers = 0b1000;
         //   Destroy(Rigid);
            modelAnimator.SetTrigger("dead");

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
        if (bDead) return;
        if(other.gameObject.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            player.GetDamage(attackDamage);
            Debug.Log("Attack Hit");
        }
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
}
