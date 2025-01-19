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
        float distance = Vector3.Distance(GameInstance.Instance.player.Transforms.position, transform.position);
        if (distance < 10)
        {
            hunting = true;
            if (distance < 2)
            {
                agent.isStopped = true;
            }
            else
            {

                agent.isStopped = false;
                Vector3 dir = (GameInstance.Instance.player.Transforms.position - transform.position).normalized;
                float angle = Vector3.Angle(transform.forward, dir);

                if (angle < 110 / 2)
                {
                    RaycastHit hit;
                    if (Physics.Raycast(transform.position, dir, out hit))
                    {
                        if (hit.collider.CompareTag("Player"))
                        {
                            target = hit.collider.GetComponent<PlayerController>();
                            Debug.Log("°¨ÁöµÊ");
                            agent.destination = target.Transforms.position;
                        }
                    }
                }
            }
        }
        else
        {
            hunting=false;
        }
    }

    private void OnDrawGizmos()
    {
        
        if(!hunting) Gizmos.color = Color.yellow;
        else Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, 10);

        Vector3 leftBoundary = Quaternion.Euler(0, -110 / 2, 0) * transform.forward * 110;
        Vector3 rightBoundary = Quaternion.Euler(0, 110 / 2, 0) * transform.forward * 110;

        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);
    }
}
