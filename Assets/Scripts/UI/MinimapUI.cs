using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapUI : MonoBehaviour
{
    Camera camera;
    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if(GameInstance.Instance.GetPlayers.Count > 0 )
        {
            Transform transforms = GameInstance.Instance.GetPlayers[0].Transforms;


        }
    }
}
