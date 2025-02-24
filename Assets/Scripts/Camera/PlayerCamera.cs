using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    Transform transforms;
    PlayerController pc;

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

    public PlayerController PC
    {
        get
        {
            if (pc == null)
            {
                if (GameInstance.Instance.GetPlayers.Count > 0)
                {
                    pc = GameInstance.Instance.GetPlayers[0];//GameObject.FindObjectOfType<PlayerController>();
                    pc.camera = this;
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
            Vector3 move = new Vector3(pos.x * 20, 0, pos.y * 20); // y축은 0으로 고정
            move = Quaternion.Euler(0, 45, 0) * move;
            targetPos += move;
        }

    }

    // Update is called once per frame
    private void LateUpdate()
    {
       
        if (PC)
        {
            if (!lookAround)
            {
                targetPos = Transforms.position;
                Vector3 target = PC.Transforms.position;
               // Transforms.position =
                Vector3 targetLoc = new Vector3(target.x - 40, 100, target.z - 40);
                Transforms.position = Vector3.SmoothDamp(Transforms.position, targetLoc, ref velocity, 0.3f);
            }
            else
            {
                Transforms.position = Vector3.SmoothDamp(Transforms.position, targetPos, ref velocity, 0.3f);
            }
        }
        lastPos = Input.mousePosition;
    }
 
    void SmoothMovement()
    {

    }
}
