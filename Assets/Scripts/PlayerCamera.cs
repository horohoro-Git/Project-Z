using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    Transform transforms;
    public Transform Transforms
    {
        get
        {
            if (transforms == null) transforms = transform;
            return transforms;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
