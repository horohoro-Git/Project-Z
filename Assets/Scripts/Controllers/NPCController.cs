using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;
using UnityEngine.UIElements;
using Unity.VisualScripting;
public class NPCController : MonoBehaviour, IDamageable, IIdentifiable
{
    UMACharacterAvatar characterAvatar;
    public UMACharacterAvatar GetCharacterAvatar {  get { if (characterAvatar == null) characterAvatar = GetComponent<UMACharacterAvatar>(); return characterAvatar; } }

    Transform transforms;
    public Transform GetTransform {  get { if (transforms == null) transforms = transform;  return transforms; } }
    Animator animator;
    public Animator modelAnimator { get { if (animator == null) { animator = GetComponentInChildren<Animator>(); animator.applyRootMotion = true; } return animator; } }

    NavMeshAgent agent;
    public NavMeshAgent GetAgent { get { if (agent == null) { agent = GetComponent<NavMeshAgent>(); } return agent; } }

    RightHand righthand;
    public RightHand GetRightHand { get { if (righthand == null) righthand = GetComponentInChildren<RightHand>(); return righthand; } }

    LeftHand leftHand;
    public LeftHand GetLeftHand { get { if (leftHand == null) leftHand = GetComponentInChildren<LeftHand>(); return leftHand; } }

    Rigidbody rigid;
    public Rigidbody GetRigidbody { get { if (rigid == null) rigid = GetComponent<Rigidbody>(); return rigid; } }

    // 애니메이션 인스턴싱 
    AnimationInstancingController animationInstancingController;
    public AnimationInstancingController GetInstancingController { get { if (animationInstancingController == null) animationInstancingController = GetComponentInParent<AnimationInstancingController>(); return animationInstancingController; } }

    public int ID { get; set; }

    Quaternion targetRotation;
    [NonSerialized]
    public bool bDead;
    int animationWorking;
    public NPCCombatStruct npcStruct;
    public NPCEventStruct eventStruct;
    float last;
    float next;
    float equip;
    float another;
    GameObject target;
    bool moveStop;
    List<PlayerController> playerControllers = new List<PlayerController>();
    List<EnemyController> enemyControllers = new List<EnemyController>();

    [NonSerialized]
    public List<ItemStruct> itemStructs = new List<ItemStruct>();

    bool focus;
    public bool isAnimationInstancing;

    private void Start()
    {
    //    ChangeTagLayer(GetTransform, "NPC", 0b1110);
       
        GetAgent.acceleration = 4;
        GetAgent.angularSpeed = 360;
     
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
            next = 0.1f;
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
                case NPCDispositionType.Infected:
                    ZombieAction();
                    return;
            }
        }

        UpdateAnimation();
    }


    void UpdateAnimation()
    {
        if (npcStruct.infected)
        {
            modelAnimator.SetFloat("speed", GetAgent.velocity.magnitude);
        }
        else
        {
            modelAnimator.SetFloat("speed", GetAgent.velocity.magnitude);
            modelAnimator.SetFloat("equip", equip);
            modelAnimator.SetFloat("another", another);
        }
        if(isAnimationInstancing)
        {
            if(GetInstancingController.GetAnimation.GetCurrentAnimationInfo().animationName != "Zombie@Walk01")
            {
                GetInstancingController.PlayerAnimation("Zombie@Walk01");
            }
          
        }
    }


    public void ChangeTagLayer(Transform parent, string newTag, int layerName)
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
        Dictionary<int, GameObject> list = GameInstance.Instance.worldGrids.FindEnemiesInGrid();
        foreach (KeyValuePair<int, GameObject> kvp in list)
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
            Dictionary<int, GameObject> list = GameInstance.Instance.worldGrids.FindEnemiesInGrid();
            float minDistance = 999;
            foreach (KeyValuePair<int, GameObject> kvp in list)
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

        Dictionary<int, GameObject> list = GameInstance.Instance.worldGrids.FindEnemiesInGrid();
       
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

        foreach(KeyValuePair<int, GameObject> kvp in list)
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


    void ZombieAction()
    {
        Dictionary<int, PlayerController> players = GameInstance.Instance.worldGrids.FindPlayerDictinary();
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
        if (target == null) return;
        float targetDistance = Vector3.Distance(transform.position, target.transform.position);
        if (NPCBehavior_Attack(targetDistance, true)) return;
        if (NPCBehavior_Follow(targetDistance, false)) return;
        if (NPCBehavior_Pause(targetDistance, false)) return;
    }

    bool NPCBehavior_Attack(float distance, bool hostile)
    {
        if (distance <= npcStruct.attack_range)
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
                    if(eventStruct.npc_disposition == NPCDispositionType.Infected)
                    {
                        StartAnimation("scratch", 2);
                    }
                    else
                    {

                        StartAnimation("attack", 2);
                    }
                    if(isAnimationInstancing)
                    {
                        Debug.Log("A");
                        GetInstancingController.PlayerAnimation("Zombie@Attack");
                    }
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
        if (layer == 0b0011 && eventStruct.npc_disposition != NPCDispositionType.Infected)
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

    public void Setup(NPCCombatStruct npcStruct, bool init)
    {
        this.npcStruct = npcStruct;
        GetAgent.speed = npcStruct.speed;
        if (!isAnimationInstancing)
        {
            if (npcStruct.infected)
            {
                modelAnimator.runtimeAnimatorController = AssetLoader.animators[GameInstance.Instance.assetLoader.animatorKeys[2].animator_name];
                gameObject.layer = 0b1010;
                gameObject.tag = "Enemy";
                ChangeTagLayer(GetTransform, "Enemy", 0b1010);
            }
            else
            {
                modelAnimator.runtimeAnimatorController = AssetLoader.animators[GameInstance.Instance.assetLoader.animatorKeys[1].animator_name];
                gameObject.layer = 0b1110;
                gameObject.tag = "NPC";
                ChangeTagLayer(GetTransform, "NPC", 0b1110);
            }
        }
        else
        {
            GetInstancingController.gameObject.layer = 0b1010; 
            GetInstancingController.gameObject.tag = "Enemy";
            ChangeTagLayer(GetInstancingController.transform, "Enemy", 0b1010);
        }
        GetAgent.stoppingDistance = npcStruct.attack_range;
        if (init) this.npcStruct.health = npcStruct.max_health;
        modelAnimator.cullingMode = AnimatorCullingMode.CullUpdateTransforms;
    }

}
