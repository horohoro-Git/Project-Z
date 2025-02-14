using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.UI.GridLayoutGroup;

public class Tree : MonoBehaviour
{
    public Plane cuttingPlane;
    public Transform upper;
    public GameObject spawnRewards;
    public List<GameObject> spawnLocations = new List<GameObject>();
    Animator animator;
    private void Start()
    {
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
            pc.RegisterAction(TreeInteraction);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController pc = other.GetComponent<PlayerController>();
            pc.RemoveAction(TreeInteraction);
        }
    }


    void TreeInteraction(PlayerController pc)
    {
        Vector3 dir = transform.position - pc.transform.position;

        float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        //angle += 180f;

        if (angle >= -22.5f && angle < 22.5f)
        {
            Debug.Log("³²");
            animator.SetInteger("state", 1);
        }
        else if (angle >= 22.5f && angle < 67.5f)
        {
            Debug.Log("³²¼­");
            animator.SetInteger("state", 2);
        }
        else if (angle >= 67.5f && angle < 112.5f)
        {
            Debug.Log("¼­");
            animator.SetInteger("state", 3);
        }
        else if (angle >= 112.5f && angle < 157.5f)
        {
            Debug.Log("ºÏ¼­");
            animator.SetInteger("state", 4);
        }
        else if (angle >= 157.5f)
        {
            Debug.Log("ºÏ");
            animator.SetInteger("state", 5);
        }
        else if (angle < -157.5f)
        {
            Debug.Log("ºÏ");
            animator.SetInteger("state", 5);
        }
        else if (angle >= -157.5f && angle < -112.5f)
        {
            Debug.Log("ºÏµ¿");
            animator.SetInteger("state", 6);
        }
        else if (angle >= -112.5f && angle < -67.5f)
        {
            Debug.Log("µ¿");
            animator.SetInteger("state", 7);
        }
        else if (angle >= -67.5f && angle < -22.5f)
        {
            Debug.Log("³²µ¿");
            animator.SetInteger("state", 8);
        }

       

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
                GameObject go = Instantiate(spawnRewards);
                go.transform.position = spawnLocations[i].transform.position; //new Vector3(corners[i].x, corners[i].y + 5f, corners[i].z);
            }
        }
    }
}
