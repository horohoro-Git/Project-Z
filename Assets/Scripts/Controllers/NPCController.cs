using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;
using UnityEngine.UIElements;
using Unity.VisualScripting;
public class NPCController : MonoBehaviour, IDamageable
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
    Quaternion targetRotation;
  [NonSerialized]
    public bool bDead;
    int animationWorking;
    public EnemyStruct npcStruct;
    public NPCEventStruct eventStruct;
    float last;
    float next;
    float equip;
    float another;
    GameObject target;
    bool moveStop;
    List<PlayerController> playerControllers = new List<PlayerController>();
    List<EnemyController> enemyControllers = new List<EnemyController>();

    public RuntimeAnimatorController newAnimator;

    bool focus;
    float attackRange = 1.2f;
    private void Start()
    {
        ChangeTagLayer(GetTransform, "NPC", 0b1110);
        GetAgent.speed = 3f;
        GetAgent.acceleration = 4;
        GetAgent.angularSpeed = 360;
        modelAnimator.runtimeAnimatorController = newAnimator;
        GetAgent.stoppingDistance = 1.2f;
    }
    //  List<Vector3> positions = new List<Vector3>();
    private void Update()
    {
        if (bDead) return;
        if(moveStop)
        {
            agent.velocity = Vector3.Lerp(agent.velocity, Vector3.zero, Time.deltaTime * 10);
        }

        targetRotation = new Quaternion();

        if (animationWorking > 0) return;

        if (target != null &&  focus)
        {
            Vector3 direction = target.transform.position - GetTransform.position;
            targetRotation = Quaternion.LookRotation(direction);
            targetRotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);
            GetTransform.rotation = Quaternion.Slerp(GetTransform.rotation, targetRotation, 5 * Time.deltaTime);
        }
        if (next + last < Time.time)
        {
            last = Time.time;
            //  next = UnityEngine.Random.Range(0.3f, 0.8f);
            next = 0.1f * Time.deltaTime;
            target = null;

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

        if (target == null) return;
        float targetDistance = Vector3.Distance(transform.position, target.transform.position);

        if (NPCBehavior_Attack(targetDistance, false)) return;
        if (NPCBehavior_Follow(targetDistance, false)) return;
        if (NPCBehavior_Pause(targetDistance, false)) return;
    }

    void FriendlyAction()
    {
        Dictionary<int, PlayerController> players = GameInstance.Instance.worldGrids.FindPlayerDictinary();

        float d = 0;

        foreach (KeyValuePair<int, PlayerController> player in players)
        {
            PlayerController pc = player.Value;
            float distance = Vector3.Distance(pc.Transforms.position, GetTransform.position);

            if (!target)
            {
                target = pc.gameObject;
            }
            else
            {
                float dis = Vector3.Distance(target.transform.position, GetTransform.position);
                if (distance < dis)
                {
                    target = pc.gameObject;

                }
            }
        }

        Vector3 playerVector = Vector3.zero;
        if (target != null)
        {
            playerVector = target.transform.position;
            d = (target.transform.position - GetTransform.position).magnitude;
        }
        if (d < 10)
        {
            Dictionary<string, GameObject> list = GameInstance.Instance.worldGrids.FindEnemiesInGrid();
            float minDistance = 999;
            foreach (KeyValuePair<string, GameObject> kvp in list)
            {
                GameObject go = kvp.Value;
                float distance = Vector3.Distance(go.transform.position, GetTransform.position);

                if (!target)
                {
                    target = go;
                    minDistance = distance;
                }
                else
                {
                  //  float dis = Vector3.Distance(target.transform.position, transform.position);
                    float dis2 = Vector3.Distance(go.transform.position, playerVector);
                    if (minDistance > distance && dis2 < 10)
                    {
                        minDistance = distance;
                        target = go;
                    }
                }
            }
        }

        if (target == null) return;
        float targetDistance = Vector3.Distance(transform.position, target.transform.position);
        if (NPCBehavior_Attack(targetDistance, false)) return;
        if (NPCBehavior_Follow(targetDistance, target.layer == 0b0011)) return;
        if (NPCBehavior_Pause(targetDistance, target.layer == 0b0011)) return;


    }

    void HositleAction()
    {
        Dictionary<int, PlayerController> players = GameInstance.Instance.worldGrids.FindPlayerDictinary();

        Dictionary<string, GameObject> list = GameInstance.Instance.worldGrids.FindEnemiesInGrid();
       
        foreach (KeyValuePair<int, PlayerController> player in players)
        {
            PlayerController pc = player.Value;
            float distance = Vector3.Distance(pc.Transforms.position, GetTransform.position);

            if (!target) target = pc.gameObject;
            else
            {
                float dis = Vector3.Distance(target.transform.position, GetTransform.position);
                if (distance < dis)
                {
                    target = pc.gameObject;
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
        if (target == null) return;
        float targetDistance = Vector3.Distance(transform.position, target.transform.position);
        if (NPCBehavior_Attack(targetDistance, true)) return;
        if (NPCBehavior_Follow(targetDistance, false)) return;
        if (NPCBehavior_Pause(targetDistance, false)) return;
    }

    bool NPCBehavior_Attack(float distance, bool hostile)
    {
        if (distance <= attackRange)
        {
            if (target.layer != 0b0011 || hostile)
            {
                GetAgent.isStopped = true;
                focus = true;
                moveStop = true;

                float angledifference = Quaternion.Angle(GetTransform.rotation, targetRotation);

                if (angledifference < 10)
                {
                    another = UnityEngine.Random.Range(0, 2);
                    StartAnimation("attack", 2);
                   
                }
                return true;
            }
            
        }
        return false;
    }
    bool NPCBehavior_Follow(float distance, bool friendly)
    {
        if(distance > 1.2f && distance <= 40 || friendly && distance > 1.2f)
        {
            focus = false;
            GetAgent.isStopped = false;
            moveStop = false;
            GetAgent.SetDestination(target.transform.position);
            return true;
        }
        else
        {
            GetAgent.isStopped = true;
            focus = false;
            moveStop = true;
            //agent.velocity = Vector3.zero;
            return true;
        }
    //    return false;
    }
    bool NPCBehavior_Pause(float distance, bool friendly)
    {
        if(distance > 40 && !friendly)
        {
            GetAgent.isStopped = true;
            moveStop = true;
            focus = false;
            return true;
        }
        return false;
    }


    void GetDamage(int damage, int layer)
    {
        if (layer == 0b0011)
        {
            Debug.Log("Hostile");
            NPCEventHandler.Publish(1000003, this);
        }
    }

    public bool Damaged(int damage, int layer)
    {
        if (gameObject.layer != layer) GetDamage(damage , layer);
        else return false;
        return true;
    }

    public void Setup(EnemyStruct npcStruct)
    {
        this.npcStruct = npcStruct;
        this.npcStruct.health = npcStruct.max_health;
    }

}
