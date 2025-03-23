using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{

    //public GameObject camera;
    Transform transforms;
    PlayerController pc;
    Camera cam;

    [NonSerialized]
    public bool lookAround;
    public Transform Transforms
    {
        get
        {
            if (transforms == null) transforms = transform;
            return transforms;
        }
    }

    public Camera GetCamera
    {
        get
        {
            if(cam == null)
            {
                cam = Camera.main;
            }

            return cam;
        }
    }

    public PlayerController PC
    {
        get
        {
            if (pc == null)
            {
                if (GameInstance.Instance.GetPlayers.Count > 0)
                {
                    pc = GameInstance.Instance.GetPlayers[0];//GameObject.FindObjectOfType<PlayerController>();
                    pc.playerCamera = this;
                }
            }
            return pc;
        }
    }

    Vector3 lastPos;
    Vector3 targetPos;
    private Vector3 velocity = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
      
    }

    void Update()
    {
        if (lookAround)
        {
            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - lastPos);
            Vector3 preMove = new Vector3(pos.x * 20, 0, pos.y * 20);
            Vector3 move = Quaternion.Euler(0, 45, 0) * preMove;

            Vector3 checkPosition = Quaternion.Euler(0,-45,0) * (targetPos + move);
            if(checkPosition.x <= 6f && checkPosition.x >= -6f && checkPosition.z >= -4f && checkPosition.z <= 2.5f)
            {
                targetPos += move;
            }

        //    Debug.Log(checkPosition);
        }

    }

    // Update is called once per frame
    private void LateUpdate()
    {
       
        if (PC)
        {
            //if (!lookAround)
            {
                if (!lookAround) targetPos = GetCamera.transform.localPosition;
                Vector3 target = PC.Transforms.position;
               // Transforms.position =
                Vector3 targetLoc = new Vector3(target.x - 40, 100, target.z - 40);

                 Transforms.position = targetLoc;
                //Transforms.position = Vector3.SmoothDamp(Transforms.position, targetLoc, ref velocity, 0.3f);
                if(lookAround) GetCamera.transform.localPosition = Vector3.SmoothDamp(GetCamera.transform.localPosition, targetPos, ref velocity, 0.3f);
                else GetCamera.transform.localPosition = Vector3.SmoothDamp(GetCamera.transform.localPosition, Vector3.zero, ref velocity, 0.3f);
            }
           // else
            {
          //      camera.transform.localPosition = Vector3.SmoothDamp(camera.transform.localPosition, targetPos, ref velocity, 0.3f);
            }
        }
        lastPos = Input.mousePosition;
    }
 
    public void ResetPlayer()
    {
        pc = null;
    }
    void SmoothMovement()
    {

    }
}
