using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
public class InputManager : MonoBehaviour
{
    PlayerInput playerInput;
    public delegate void MoveAction(Vector2 movement);
    public event MoveAction Moves;
    private void Awake()
    {

        playerInput = GetComponent<PlayerInput>();
        playerInput.defaultActionMap = "Move";
    }
    private void OnEnable()
    {
       playerInput.actions["WASD"].performed += HandleMove;
    }
    private void OnDisable()
    {
        playerInput.actions["WASD"].performed -= HandleMove;
        
    }
    private void Start()
    {
        Moves += Move;
    }
    private void HandleMove(InputAction.CallbackContext context)
    {
        // 입력된 값으로 이동 벡터 받아오기
        Vector2 movement = context.ReadValue<Vector2>();
        Debug.Log(movement);
        // 이동 이벤트 발생
        Moves?.Invoke(movement);
    }
    private void Move(Vector2 movement)
    {
      //  Debug.Log("AA");
    }
}
