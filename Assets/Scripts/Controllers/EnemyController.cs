using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : Controller
{

    enum EnemyType
    {
        Roaming,
        TargetDetect,
        TargetDetect_Far,
        Battle
    
    }
    public bool theta;

    [SerializeField]
    NavMeshAgent agent;
    PlayerController target;
    bool hunting=false;
    float detectTimer = 0;
    MoveCalculator moveCalculator = new MoveCalculator();
    EnemyType enemyType= EnemyType.Roaming;
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
        if (GameInstance.Instance.player)
        {
            DetectPlayer();
        }


        if(enemyType == EnemyType.TargetDetect)
        {
            if(theta)
            {
                Vector3 dirs = GameInstance.Instance.player.Transforms.position - Transforms.position;
                float distance = dirs.magnitude;
                dirs = Vector3.Normalize(dirs);
                if (!Physics.BoxCast(Transforms.position, new Vector3(0.5f, 0.5f, 0.5f), dirs, Quaternion.identity, distance, 1 << 6))
                {

                    moveDir = dirs;
                }
                else
                {
                    
                    if (detectTimer + 1f < Time.time)
                    {
                        detectTimer = Time.time;
                    
                        Stack<Vector3> move = moveCalculator.Calculate(Transforms.position, GameInstance.Instance.player.Transforms.position);
                        if (move != null)
                        {
                            Vector3 destination = LineOfSight(move);
                            Vector3 dir = destination - Transforms.position;
                            dir = Vector3.Normalize(dir);

                            moveDir = dir;
                        }
                        //Rigid.velocity = dir * 100 * Time.deltaTime;
                    }
                  
                }
            }
            else
            {
                agent.angularSpeed = 300f;
                agent.updateRotation = false;

                Vector3 direction = (GameInstance.Instance.player.Transforms.position - transform.position).normalized;
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, 300f * Time.deltaTime);

                agent.isStopped = false;
                agent.destination = target.Transforms.position;
            }
        
        }

      /*  if (enemyType == EnemyType.TargetDetect_Far)
        {
            agent.angularSpeed = 300f;
            agent.updateRotation = false;

            Vector3 direction = (GameInstance.Instance.player.Transforms.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, 300f * Time.deltaTime);

            agent.isStopped = false;
            agent.destination = target.Transforms.position;
        
        }*/
    }

    
    void DetectPlayer()
    {
        float distance = Vector3.Distance(GameInstance.Instance.player.Transforms.position, Transforms.position);


        if (distance < 10)
        {
            Vector3 dir = (GameInstance.Instance.player.Transforms.position - Transforms.position).normalized;
            float angle = Vector3.Angle(transform.forward, dir);

            if (angle < 110 / 2 || ((int)enemyType > 0 && (int)enemyType != 3))
            {
                RaycastHit hit;
                if (Physics.Raycast(Transforms.position, dir, out hit))
                {
                    if (hit.collider.CompareTag("Player"))
                    {
                        target = hit.collider.GetComponent<PlayerController>();
                       
                        if (distance < 2)
                        {
                            moveDir = Vector3.zero;
                            agent.isStopped = true;
                            enemyType = EnemyType.Battle;
                        }
                        else if (distance < 6)
                        {

                            agent.isStopped = true;
                            enemyType = EnemyType.TargetDetect;


                        }
                        else
                        {
                            moveDir = Vector3.zero;
                            agent.isStopped = false;
                            enemyType = EnemyType.TargetDetect_Far;
                        }
                    }
                }
              
            }

         
          

          
        }
        else if(distance >= 20)
        {
            moveDir = Vector3.zero;
            enemyType = EnemyType.Roaming;
            agent.isStopped = true;
            hunting =false;
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
            if(!Physics.BoxCast(start, size, dir, Quaternion.identity, distance, 1 << 6))
            {
            }
            else
            {
                break;
            }
           
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
