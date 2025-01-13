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
        PC pc = Instantiate(player); //�÷��̾� ����
        pc.transform.position = startPosition; 
        Instantiate(inputManager);  //�Է� �ý��� ����

    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
