using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;
public class NPCController : MonoBehaviour
{
    Transform transforms;
    Transform GetTransform {  get { if (transforms == null) transforms = transform;  return transforms; } }
    Animator animator;
    public Animator modelAnimator { get { if (animator == null) { animator = GetComponent<Animator>(); animator.applyRootMotion = true; } return animator; } }

    NavMeshAgent agent;
    public NavMeshAgent GetAgent { get { if (agent == null) { agent = GetComponent<NavMeshAgent>(); } return agent; } }

    RightHand righthand;
    public RightHand GetRightHand { get { if (righthand == null) righthand = GetComponentInChildren<RightHand>(); return righthand; } }

    LeftHand leftHand;
    public LeftHand GetLeftHand { get { if (leftHand == null) leftHand = GetComponentInChildren<LeftHand>(); return leftHand; } }

    [NonSerialized]
    public bool bDead;
    int animationWorking;
    NPCEventStruct eventStruct;
    float last;
    float next;
    float equip;
    float another;
    GameObject target;
    List<PlayerController> playerControllers = new List<PlayerController>();
    List<EnemyController> enemyControllers = new List<EnemyController>();

    public RuntimeAnimatorController newAnimator;
    private void Start()
    {
        ChangeTagLayer(GetTransform, "NPC", 0b1110);
        GetAgent.speed = 3f;
        GetAgent.acceleration = 4;
        GetAgent.angularSpeed = 360;
        modelAnimator.runtimeAnimatorController = newAnimator;
    }
    //  List<Vector3> positions = new List<Vector3>();
    private void Update()
    {
        if (animationWorking > 0) return;
        if (bDead) return;
        //   if (LastPosition != Transforms.position)

        if (next + last < Time.time)
        {
            last = Time.time;
            next = UnityEngine.Random.Range(0.3f, 0.8f);

            if (target == null)
            {
                Debug.Log(eventStruct.npc_disposition);
                switch (eventStruct.npc_disposition)
                {
                    case NPCDispositionType.None:
                        break;
                    case NPCDispositionType.Netural:
                        NeturalAction();
                        break;
                    case NPCDispositionType.Friendly:
                        FriendlyAction(); 
                        break;
                    case NPCDispositionType.Hostile:
                        HositleAction();
                        break;
                }
                return;
            }

            float distance = Vector3.Distance(transform.position, target.transform.position);


            if (distance <= 1.5f)
            {

                StartAnimation("attack", 2);
                //   GetRightHand.Attack(enemyStruct.attack);
                GetAgent.isStopped = true;
            }
        /*    else if (distance > 20)
            {
                GetAgent.isStopped = true;
            }*/
            else
            {
                GetAgent.isStopped = false;
                GetAgent.SetDestination(target.transform.position);
            }

        }
        modelAnimator.SetFloat("speed", GetAgent.velocity.magnitude);
        modelAnimator.SetFloat("equip", equip);
        modelAnimator.SetFloat("another", another);
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
    public void ChangeEvent(NPCEventStruct eventStruct)
    {
        this.eventStruct = eventStruct;
    }
    void FindPlayer(List<PlayerController> players, bool skipAngle = false)
    {
        int index = 0;

        while (players.Count > index)
        {
            PlayerController pc = players[index++];
            float distance = Vector3.Distance(pc.Transforms.position, GetTransform.position);

            Vector3 dir = (pc.Transforms.position - GetTransform.position).normalized;
            float angle = Vector3.Angle(GetTransform.forward, dir);   // 적의 전면부에서 110도 까지 탐지 

            if (angle < 110 / 2 || skipAngle)
            {
                if (!target) target = pc.gameObject;
                else
                {
                    float dis = Vector3.Distance(target.transform.position, transform.position);
                    if (distance < dis)
                    {
                        target = pc.gameObject;
                    }
                }
            }
        }
    }

    void StartAnimation(string animationName, float timer)
    {
        if (animationWorking > 0) return;
        animationWorking++;
        modelAnimator.SetTrigger(animationName);

        Invoke("StopAnimation", timer);
    }
    void StopAnimation()
    {
        animationWorking--;
    }

    void NeturalAction()
    {
        Dictionary<string, GameObject> list = GameInstance.Instance.worldGrids.FindEnemiesInGrid();
        foreach (KeyValuePair<string, GameObject> kvp in list)
        {
            GameObject go = kvp.Value;
            float distance = Vector3.Distance(go.transform.position, GetTransform.position);

            if (!target) target = go;
            else
            {
                float dis = Vector3.Distance(target.transform.position, transform.position);
                if (distance < dis)
                {
                    target = go;
                }
            }
        }
    }

    void FriendlyAction()
    {
        Dictionary<string, GameObject> list = GameInstance.Instance.worldGrids.FindEnemiesInGrid();
        foreach (KeyValuePair<string, GameObject> kvp in list)
        {
            GameObject go = kvp.Value;
            float distance = Vector3.Distance(go.transform.position, GetTransform.position);

            if (!target) target = go;
            else
            {
                float dis = Vector3.Distance(target.transform.position, transform.position);
                if (distance < dis)
                {
                    target = go;
                }
            }
        }
    }

    void HositleAction()
    {
        GameInstance.Instance.worldGrids.FindPlayersInGrid(GetTransform, ref playerControllers);
        Dictionary<string, GameObject> list = GameInstance.Instance.worldGrids.FindEnemiesInGrid();
        FindPlayer(playerControllers);
        int index = 0;

        while (playerControllers.Count > index)
        {
            PlayerController pc = playerControllers[index++];
            float distance = Vector3.Distance(pc.Transforms.position, GetTransform.position);

            //Vector3 dir = (pc.Transforms.position - GetTransform.position).normalized;
            //float angle = Vector3.Angle(GetTransform.forward, dir);   // 적의 전면부에서 110도 까지 탐지 

          //  if (angle < 110 / 2)
            {
                if (!target) target = pc.gameObject;
                else
                {
                    float dis = Vector3.Distance(target.transform.position, transform.position);
                    if (distance < dis)
                    {
                        target = pc.gameObject;
                    }
                }
            }
        }
    
        foreach(KeyValuePair<string, GameObject> kvp in list)
        {
            GameObject go = kvp.Value;
            float distance = Vector3.Distance(go.transform.position, GetTransform.position);

            if (!target) target = go;
            else
            {
                float dis = Vector3.Distance(target.transform.position, transform.position);
                if (distance < dis)
                {
                    target = go;
                }
            }
        }

    }
}
