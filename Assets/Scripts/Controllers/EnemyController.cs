using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : Controller
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
    private void Awake()
    {
     //   if (theta) MoveCalculator.SetBlockArea();
        moveSpeed = 100;
      //  agent.speed = moveSpeed;
    }
    // Start is called before the first frame update
    void Start()
    {
 
    }

    // Update is called once per frame
    void Update()
    {
     //   if (LastPosition != Transforms.position)
        {
       //     LastPosition = Transforms.position;
            DetectPlayer(GameInstance.Instance.worldGrids.FindPlayersInGrid(Transforms));
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


    private void FixedUpdate()
    {
        if(enemyDir != Vector3.zero)
        {
            Rigid.velocity = enemyDir * moveSpeed * Time.fixedDeltaTime;

        }
    }

    //탐색한 플레이어들 중 최적의 플레이어를 찾고 추적
    void DetectPlayer(List<PlayerController> players, bool skipAngle = false)
    {
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
            if(distance < 10)
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

    private void OnDrawGizmos()
    {
        
        if(!hunting) Gizmos.color = Color.yellow;
        else Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(Transforms.position, 6);

        Vector3 leftBoundary = Quaternion.Euler(0, -110 / 2, 0) * Transforms.forward * 110;
        Vector3 rightBoundary = Quaternion.Euler(0, 110 / 2, 0) * Transforms.forward * 110;

        Gizmos.DrawLine(Transforms.position, Transforms.position + leftBoundary);
        Gizmos.DrawLine(Transforms.position, Transforms.position + rightBoundary);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(Transforms.position, 20);
    }
}
