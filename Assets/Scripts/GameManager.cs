using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public PC player;
    public InputManager inputManager;
    public Vector3 startPosition;
    private void Awake()
    {
        PC pc = Instantiate(player); //플레이어 생성
        pc.transform.position = startPosition; 
        Instantiate(inputManager);  //입력 시스템 생성

    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
