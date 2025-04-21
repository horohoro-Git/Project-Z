using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using System.Collections;
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

    [NonSerialized]
    public LayerType latestLayerType = LayerType.Default;
    public int ID { get; set; }
    public float DamagedTimer { get; set; }

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
    public List<ItemPackageStruct> itemStructs = new List<ItemPackageStruct>();

    bool focus;
    public bool isAnimationInstancing;

    HashSet<string> animationTriggers = new HashSet<string>();
    Coroutine upperCoroutine;
    Coroutine fullbodyCoroutine;

    private void Start()
    {
    //    ChangeTagLayer(GetTransform, "NPC", 0b1110);
       
        GetAgent.acceleration = 4;
        GetAgent.angularSpeed = 360;
        GetAgent.radius = 0.2f;
     
    }
    //  List<Vector3> positions = new List<Vector3>();
    private void Update()
    {
        if (bDead) return;
        if(moveStop)
        {
            GetAgent.velocity = Vector3.Lerp(GetAgent.velocity, Vector3.zero, Time.deltaTime * 10);
        }
        if(DamagedTimer > 0) SpasticityRecovery();

        targetRotation = new Quaternion();

        if (animationWorking > 0)
        {
            GetAgent.velocity = Vector3.Lerp(GetAgent.velocity, Vector3.zero, Time.deltaTime * 10);
            if(!isAnimationInstancing) UpdateAnimation();
            return;
        }
        if (target != null && focus)
        {
            Vector3 direction = target.transform.position - GetTransform.position;
            targetRotation = Quaternion.LookRotation(direction);
            targetRotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);
            GetTransform.rotation = Quaternion.Slerp(GetTransform.rotation, targetRotation, 10 * Time.deltaTime);
        }
        if (next + last < Time.time)
        {
            last = Time.time;
            //  next = UnityEngine.Random.Range(0.3f, 0.8f);
            next = 0.05f;
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
        if (!isAnimationInstancing)
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
        }
        if(isAnimationInstancing)
        {
            if(GetInstancingController.GetAnimation.GetCurrentAnimationInfo().animationName != "Zombie@Walk01")
            {
                GetInstancingController.PlayerAnimation("Zombie@Walk01");
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

    void StartAnimation(string animationName, float timer, bool forced = false)
    {
        if (animationWorking > 0 && !forced) return;
        animationWorking++;

        if (animationTriggers.TryGetValue(animationName, out var trigger))
        {
            if (!modelAnimator.enabled) modelAnimator.enabled = true;
            modelAnimator.SetTrigger(animationName);
        }
      

        Invoke("StopAnimation", timer);
    }
    void StopAnimation()
    {
        if(bDead) return;
        animationWorking--;
        if (isAnimationInstancing) modelAnimator.enabled = false;
        else
        {
            if (latestLayerType != LayerType.Default) StartCoroutine(ChangeSmooth((int)latestLayerType, 0));
            latestLayerType = LayerType.Default;
        }
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
        Dictionary<int, GameObject> players = GameInstance.Instance.worldGrids.FindPlayerDictinary();

        float d = 0;

        foreach (KeyValuePair<int, GameObject> player in players)
        {
            GameObject pc = player.Value;
            float distance = Vector3.Distance(pc.transform.position, GetTransform.position);

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
        Dictionary<int, GameObject> players = GameInstance.Instance.worldGrids.FindPlayerDictinary();

        Dictionary<int, GameObject> list = GameInstance.Instance.worldGrids.FindEnemiesInGrid();
       
        foreach (KeyValuePair<int, GameObject> player in players)
        {
            GameObject pc = player.Value;
            float distance = Vector3.Distance(pc.transform.position, GetTransform.position);

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
        Dictionary<int, GameObject> players = GameInstance.Instance.worldGrids.FindPlayerDictinary();
        foreach (KeyValuePair<int, GameObject> player in players)
        {
            GameObject pc = player.Value;
            float distance = Vector3.Distance(pc.transform.position, GetTransform.position);

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
                    if (!isAnimationInstancing)
                    {
                        another = UnityEngine.Random.Range(0, 2);
                        if (eventStruct.npc_disposition == NPCDispositionType.Infected)
                        {
                            if (latestLayerType != LayerType.Default) StartCoroutine(ChangeSmooth((int)latestLayerType, 0));
                            fullbodyCoroutine = StartCoroutine(ChangeSmooth((int)LayerType.Upper, 1));
                            latestLayerType = LayerType.Upper;
                            StartAnimation("scratch", 2);
                        }
                        else
                        {
                            if (latestLayerType != LayerType.Default) StartCoroutine(ChangeSmooth((int)latestLayerType, 0));
                            fullbodyCoroutine = StartCoroutine(ChangeSmooth((int)LayerType.Upper, 1));
                            latestLayerType = LayerType.Upper;
                            StartAnimation("attack", 0.8f);
                        }
                    }
                    if(isAnimationInstancing)
                    {
                        StartAnimation("scratch", 2);
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
        if(distance > 1f && distance <= 40 || friendly && distance > 1f)
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
        if (layer == 0b0011 && eventStruct.npc_disposition != NPCDispositionType.Infected && eventStruct.npc_disposition != NPCDispositionType.Hostile)
        {
            NPCEventHandler.Publish(1000003, this);
        }

        if (layer != gameObject.layer)
        {
            npcStruct.health -= damage;
            DamagedTimer = 0.5f;

            if (!isAnimationInstancing)
            {
                if (latestLayerType != LayerType.Default)
                {
                    if (upperCoroutine != null) StopCoroutine(upperCoroutine);
                    modelAnimator.SetLayerWeight((int)latestLayerType, 0);
                    //   upperCoroutine = StartCoroutine(ChangeSmooth((int)latestLayerType, 0));
                }
                if (fullbodyCoroutine != null) StopCoroutine(fullbodyCoroutine);
                modelAnimator.SetLayerWeight((int)LayerType.FullBody, 1);

                //    fullbodyCoroutine = StartCoroutine(ChangeSmooth((int)LayerType.FullBody, 1));
                latestLayerType = LayerType.FullBody;
            }
            if (npcStruct.health <= 0)
            {
                bDead = true;
                npcStruct.health = 0;
                if (AllEventManager.customEvents.TryGetValue(5, out var events)) ((Action<NPCController>)events)?.Invoke(this);
                if (npcStruct.infected) StartAnimation("zombieDead", 1f, true);
                else StartAnimation("dead", 10f, true);
                if(isAnimationInstancing)
                {
                    GetInstancingController.PlayerAnimation("Zombie@Death01_A");
                }
                GetAgent.isStopped = true;
                GetAgent.enabled = false;
              
            }
            else
            {

                if (isAnimationInstancing) GetInstancingController.PlayerAnimation("Zombie@Damage01");
                StartAnimation("damaged", 0.5f, true);
            }
        }
    }

    public bool Damaged(int damage, int layer)
    {
        damage = 50;
        if (gameObject.layer != layer) GetDamage(damage , layer);
        else return false;
        return true;
    }
    public void SpasticityRecovery()
    {
        DamagedTimer -= Time.deltaTime;
        if(DamagedTimer <= 0) DamagedTimer = 0;
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
                Utility.ChangeTagLayer(GetTransform, "Enemy", 0b1010);
            }
            else
            {
                modelAnimator.runtimeAnimatorController = AssetLoader.animators[GameInstance.Instance.assetLoader.animatorKeys[1].animator_name];
                gameObject.layer = 0b1110;
                gameObject.tag = "NPC";
                Utility.ChangeTagLayer(GetTransform, "NPC", 0b1110);
            }
        }
        else
        {
            GetInstancingController.gameObject.layer = 0b1010; 
            GetInstancingController.gameObject.tag = "Enemy";
            Utility.ChangeTagLayer(GetInstancingController.transform, "Enemy", 0b1010);

        }
    //    modelAnimator.SetLayerWeight(1, 0);
     //   modelAnimator.SetLayerWeight(2, 0);
        GetAgent.stoppingDistance = npcStruct.attack_range;
        if (init) this.npcStruct.health = npcStruct.max_health;
        modelAnimator.cullingMode = AnimatorCullingMode.CullUpdateTransforms;


        foreach (var param in modelAnimator.parameters)
        {
            if (param.type == AnimatorControllerParameterType.Trigger)
            {
                animationTriggers.Add(param.name);
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (bDead && npcStruct.infected_player && itemStructs.Count > 0)
        {
            PlayerController pc = other.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.RegisterAction(OpenInventorySystem);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (bDead && npcStruct.infected_player && itemStructs.Count > 0)
        {
            PlayerController pc = other.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.RemoveAction(OpenInventorySystem);
            }
        }
    }
    void OpenInventorySystem(PlayerController pc)
    {
        GameInstance.Instance.uiManager.SwitchUI(UIType.BoxInventory, false);
        GameInstance.Instance.boxInventorySystem.GetOpponentInventory(itemStructs);
    }
    void UpdateController()
    {
        modelAnimator.runtimeAnimatorController = AssetLoader.animators[GameInstance.Instance.assetLoader.animatorKeys[2].animator_name];
        modelAnimator.SetLayerWeight(1, 0);
        modelAnimator.SetLayerWeight(2, 0);
    }
    IEnumerator ChangeSmooth(int layer, float target)
    {
        float timer = 1 - target;
        bool increase = timer > 0 ? false : true;
        while (true)
        {
            if (increase)
            {
                timer += Time.deltaTime * 4;
                modelAnimator.SetLayerWeight(layer, timer);
                if (timer > target) break;
            }
            else
            {
                timer -= Time.deltaTime * 4;
                modelAnimator.SetLayerWeight(layer, timer);
                if (timer < target) break;
            }
            yield return null;
        }
    }
}
