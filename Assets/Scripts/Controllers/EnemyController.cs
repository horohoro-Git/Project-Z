using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : Controller
{
    [SerializeField]
    NavMeshAgent agent;
    PlayerController target;
    bool hunting=false;
    private void Awake()
    {
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
    }

    
    void DetectPlayer()
    {
        float distance = Vector3.Distance(GameInstance.Instance.player.Transforms.position, Transforms.position);
        if (distance < 10)
        {
            hunting = true;
            if (distance < 2)
            {
                agent.isStopped = true;
            }
            else
            {
                agent.angularSpeed = 300f;
                agent.updateRotation = false; // 회전 처리를 수동으로 수행

                // 플레이어를 향한 방향 계산
                Vector3 direction = (GameInstance.Instance.player.Transforms.position - transform.position).normalized;
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, 300f * Time.deltaTime);



                agent.isStopped = false;
                Vector3 dir = (GameInstance.Instance.player.Transforms.position - Transforms.position).normalized;
                float angle = Vector3.Angle(transform.forward, dir);

                if (angle < 110 / 2)
                {
                    RaycastHit hit;
                    if (Physics.Raycast(Transforms.position, dir, out hit))
                    {
                        if (hit.collider.CompareTag("Player"))
                        {
                            target = hit.collider.GetComponent<PlayerController>();
                            Debug.Log("감지됨");
                            agent.destination = target.Transforms.position;
                        }
                    }
                }
            }
        }
        else if(distance < 20) 
        {
            agent.isStopped = true;
            hunting =false;
        }
    }

    private void OnDrawGizmos()
    {
        
        if(!hunting) Gizmos.color = Color.yellow;
        else Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(Transforms.position, 10);

        Vector3 leftBoundary = Quaternion.Euler(0, -110 / 2, 0) * Transforms.forward * 110;
        Vector3 rightBoundary = Quaternion.Euler(0, 110 / 2, 0) * Transforms.forward * 110;

        Gizmos.DrawLine(Transforms.position, Transforms.position + leftBoundary);
        Gizmos.DrawLine(Transforms.position, Transforms.position + rightBoundary);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(Transforms.position, 20);
    }
}
