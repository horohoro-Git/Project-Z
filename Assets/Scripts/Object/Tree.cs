using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.UI.GridLayoutGroup;

public class Tree : EnvironmentObject
{
    public Plane cuttingPlane;
    public Transform upper;
    public GameObject spawnRewards;
    public List<GameObject> spawnLocations = new List<GameObject>();
    int hp = 3;
    Animator animator;
    bool dead;
    [NonSerialized]
    public int exp;

    [NonSerialized]
    public Player rewardsPlayer;
    private void Start()
    {
        exp = 1;
        animator = GetComponent<Animator>();
        cuttingPlane = new Plane(Vector3.right, transform.position);
    }
    MeshFilter meshFilter;
    public MeshFilter GetMeshFilter {  get { 
            
            if(meshFilter == null) meshFilter = GetComponentInChildren<MeshFilter>();
            return meshFilter; } }

    MeshRenderer meshRenderer;
    public MeshRenderer GetMeshRenderer
    {
        get
        {

            if (meshRenderer == null) meshRenderer = GetComponentInChildren<MeshRenderer>();
            return meshRenderer;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController pc = other.GetComponent<PlayerController>();
         //   pc.RegisterAction(TreeInteraction);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController pc = other.GetComponent<PlayerController>();
       //     pc.RemoveAction(TreeInteraction);
        }
    }


    void TreeInteraction(PlayerController pc)
    {
        if(dead) return;
        pc.CutTree();

        hp--;


        Vector3 pcForward = pc.Transforms.forward;
        Vector3 toTarget = transform.position -  pc.Transforms.position;
        float dotProduct = Vector3.Dot(pcForward, toTarget);
        if (dotProduct <= 0) return;    //앞을 보고 있을 때만 동작
     
        Vector3 dir = transform.position - pc.transform.position;
        Debug.Log(dir);
        float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;

        if (angle >= -45f && angle < 45f) animator.SetInteger("state", 3);
        else if (angle >= 45f && angle < 135f) animator.SetInteger("state", 4);
        else if (angle >= 135f) animator.SetInteger("state", 1);
        else if (angle < -135f) animator.SetInteger("state", 1);
        else if (angle >= -135f && angle < -45f) animator.SetInteger("state", 2);
     
        Invoke("AddReward", 1.5f);
    }

    public void ChopDown(Transform transforms, Player player)
    {
        if (dead) return;
        dead = true;
        rewardsPlayer = player;

        rewardsPlayer.SpendEnergy(1);

        Vector3 dir = transform.position - transforms.position;
        Debug.Log(dir);
        float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;

        if (angle >= -45f && angle < 45f) animator.SetInteger("state", 3);
        else if (angle >= 45f && angle < 135f) animator.SetInteger("state", 4);
        else if (angle >= 135f) animator.SetInteger("state", 1);
        else if (angle < -135f) animator.SetInteger("state", 1);
        else if (angle >= -135f && angle < -45f) animator.SetInteger("state", 2);
        rewardsPlayer.GetExperience(exp);

        Invoke("AddReward", 1.5f);
    }

    void AddReward()
    {
        MeshRenderer meshRenderer = upper.GetComponent<MeshRenderer>();

        if (meshRenderer != null)
        {
            Bounds bounds = meshRenderer.bounds;

            Vector3 center = bounds.center;

            Vector3 extents = bounds.extents;

            Vector3[] corners = new Vector3[4];
            corners[0] = new Vector3(center.x - extents.x, center.y, center.z - extents.z);
            corners[1] = new Vector3(center.x - extents.x, center.y, center.z + extents.z);
            corners[2] = new Vector3(center.x + extents.x, center.y, center.z + extents.z);
            corners[3] = new Vector3(center.x + extents.x, center.y, center.z - extents.z);

            upper.gameObject.SetActive(false);


            for (int i = 0; i < 3; i++)
            {
                GameObject go = Instantiate(GameInstance.Instance.assetLoader.loadedAssets[LoadURL.Log]);
                go.transform.position = spawnLocations[i].transform.position; //new Vector3(corners[i].x, corners[i].y + 5f, corners[i].z);
                go.transform.rotation = upper.transform.rotation;
                go.GetComponent<Item>().itemIndex = 1;
            }

          
        }
    }
}
