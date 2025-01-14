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
        InputManager inputMGR = Instantiate(inputManager);  //�Է� �ý��� ����
        inputMGR.Setup(pc);
    }
}
