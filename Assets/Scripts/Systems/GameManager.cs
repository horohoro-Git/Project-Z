using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    const string playerName = "Player";
    const string inputManagerName = "InputManager";
    public PlayerController player;
    public InputManager inputManager;
    public Vector3 startPosition;
    private void Awake()
    {
        PlayerController pc = Instantiate(player); //�÷��̾� ����
        pc.name = playerName;
        pc.transform.position = startPosition; 
        InputManager inputMGR = Instantiate(inputManager);  //�Է� �ý��� ����
        inputMGR.name = inputManagerName;
        inputMGR.Setup(pc.GetComponent<PlayerInput>());
    }
}
