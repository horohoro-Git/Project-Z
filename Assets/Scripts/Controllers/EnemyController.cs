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


    void DetectPlayer(bool skipAngle)
    {
        if (destinations.Count == 0)
        {
          /*  float distance = Vector3.Distance(GameInstance.Instance.player.Transforms.position, Transforms.position);


            if (distance < 10)
            {
                Vector3 dir = (GameInstance.Instance.player.Transforms.position - Transforms.position).normalized;
                float angle = Vector3.Angle(transform.forward, dir);

                if (angle < 110 / 2 || skipAngle)
                {
                    hunting = true;
                    agent.isStopped = false;
                    enemyType = EnemyType.TargetDetect;
                

                }
            }
            else if (distance >= 20)
            {
                moveDir = Vector3.zero;
                enemyType = EnemyType.Roaming;
                agent.isStopped = true;
                hunting = false;
            }*/
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
