using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    Transform transforms;
    PlayerController pc;
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
            if(pc == null) pc = GameObject.FindObjectOfType<PlayerController>();
            return pc;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        if (PC)
        {
            Vector3 target = PC.Transforms.position;
            Transforms.position = new Vector3(target.x - 40, 100, target.z - 40);

        }
    }
   /* Update()
    {
        if (PC)
        {
            Vector3 target = PC.Transforms.position;
            Transforms.position = new Vector3(target.x - 40, 100, target.z - 40);

        }
    }*/
}
