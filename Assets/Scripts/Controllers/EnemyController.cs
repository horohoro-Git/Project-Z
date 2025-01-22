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
        TargetDetect_Obstacles,
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
        Debug.Log(GameInstance.Instance.player.Transforms.position);
        GetDestination(moveCalculator.Calculate(Transforms.position, GameInstance.Instance.player.Transforms.position));
    }

    // Update is called once per frame
    void Update()
    {
        if (GameInstance.Instance.player)
        {
            DetectPlayer();
        }


        if (enemyType == EnemyType.TargetDetect)
        {
            if (theta)
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
                  
                  //  moveDir = Vector3.zero;
                    enemyType = EnemyType.TargetDetect_Obstacles;
                
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
        else if (enemyType == EnemyType.TargetDetect_Obstacles)
        {
            if (destinations.Count == 0)
            {
                Debug.Log("A");
                GetDestination(moveCalculator.Calculate(Transforms.position, GameInstance.Instance.player.Transforms.position));
               // enemyType = EnemyType.Roaming;
            }
        }
    }

    void GetDestination(Stack<Vector3> vector3s)
    {
        
        if (vector3s == null) return;
        Vector3 loc = Transforms.position;
        Vector3 s = Transforms.position;
        while (vector3s.Count > 0)
        {
            Vector3 toLoc = vector3s.Pop();
            Vector3 dir = toLoc - loc;
            float dis = dir.magnitude;
            dir = Vector3.Normalize(dir);
            Quaternion rotation = Quaternion.LookRotation(dir);
           
            if (vector3s.Count == 0)
            {
                destinations.Add(toLoc);
                break;
            }
            if (!Physics.BoxCast(loc, new Vector3(0.5f, 0.5f, 0.5f), dir, Quaternion.identity, dis, 1 << 6))
            {
                Debug.DrawLine(loc, loc + dir * dis, Color.green, 5f);
            }
            else
            {
                Vector3 dir2 = toLoc - s;
                float dis2 = dir2.magnitude;
                dir2 = Vector3.Normalize(dir2);
                Debug.DrawLine(s, s + dir2 * dis2, Color.green, 5f);

                destinations.Add(s);
                loc = toLoc;
            }
            s = toLoc;
        }

        if(coroutine!=null) StopCoroutine(coroutine);
        coroutine = StartCoroutine(GoToMove());
    }

    IEnumerator GoToMove()
    {
        moveDir = Vector3.zero;
        Rigid.velocity = Vector3.zero;
        int index = 0;
       
        while (destinations.Count > index)
        {
            Debug.Log(destinations.Count);
            Vector3 target = destinations[index];

            while (true)
            {
                Vector3 dir = target - Transforms.position;
                float distance = dir.magnitude;
                dir = Vector3.Normalize(dir);
                Debug.Log(dir);
                if(distance < 1f)
                {
                 //   moveDir = Vector3.zero;
                 //   Rigid.velocity = Vector3.zero;
                     //Transforms.position = target;
                    break;
                }
                 
                Transforms.Translate(dir * 2 * Time.deltaTime, Space.World);
           //     float angle = Vector3.Angle(Vector3.up, dir);  // 현재 방향과 목표 방향의 각도 차이 계산

                // 각도가 1도 이상일 때만 회전
                /*if (angle > 1f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(dir);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
                }*/
               // Transforms.rotation = Quaternion.LookRotation(dir);
               // Rigid.MovePosition(Transforms.position + dir* Time.deltaTime);
                yield return null;
            }
            index++;
        }
        destinations.Clear();
    }


    void DetectPlayer()
    {
        if (destinations.Count == 0)
        {
            float distance = Vector3.Distance(GameInstance.Instance.player.Transforms.position, Transforms.position);


            if (distance < 10)
            {
                Vector3 dir = (GameInstance.Instance.player.Transforms.position - Transforms.position).normalized;
                float angle = Vector3.Angle(transform.forward, dir);

                if (angle < 110 / 2 || ((int)enemyType > 0 && (int)enemyType != 4))
                {
                    RaycastHit hit;
                    if (Physics.Raycast(Transforms.position, dir, out hit))
                    {
                        if (hit.collider.CompareTag("Player"))
                        {
                            target = hit.collider.GetComponent<PlayerController>();
                            //      Debug.Log("check");
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
            else if (distance >= 20)
            {
                moveDir = Vector3.zero;
                enemyType = EnemyType.Roaming;
                agent.isStopped = true;
                hunting = false;
            }
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
