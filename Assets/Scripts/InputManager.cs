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
    PC playerController;
    PlayerCamera camera;
    Vector3 movements;
    public void Setup(PC playerController)
    {
        if (this.playerController == null) this.playerController = playerController; 
        if (camera == null) camera = Camera.main.GetComponent<PlayerCamera>();
    }

    private void Awake()
    {

        playerInput = GetComponent<PlayerInput>();
        playerInput.defaultActionMap = "OnMove";
    }
    private void OnEnable()
    {
        playerInput.actions["WASD"].performed += HandleMove;
        playerInput.actions["WASD"].canceled += OnMoveCanceled;
    }
    private void OnDisable()
    {
        playerInput.actions["WASD"].performed -= HandleMove;
        playerInput.actions["WASD"].canceled -= OnMoveCanceled;

    }
    private void Start()
    {
        Moves += OnMove;
    }
    private void HandleMove(InputAction.CallbackContext context)
    {
        if(!playerController) return;
        // 입력된 값으로 이동 벡터 받아오기
        Vector2 movement = context.ReadValue<Vector2>();
       
        movements = VectorUtility.RotateY45(new Vector3(movement.x, 0, movement.y),45);
  
       // Moves?.Invoke(movement);
    }
    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        // 키를 뗄 때 이동을 멈춤
        movements = Vector3.zero;
    }

   

    private void OnMove(Vector2 movement)
    {
      //  playerController.Transforms.Translate(movement * Time.deltaTime * 5f);
    }
    public Vector3 check;
    Vector3 latestDir;
    float moveSpeed = 0;
    public void Update()
    {
        if (playerController && movements != Vector3.zero && camera)
        {
            // 이동하려는 방향과 현재 방향이 얼마나 일치하는지
            float dotProduct = Vector3.Dot(playerController.Transforms.forward, movements);

            // 일정 각도 이상 방향이 일치하면 이동
            if (dotProduct >= 0.35f)
            {
                // 이동 속도 증가
                moveSpeed += 3f * Time.deltaTime;
                moveSpeed = Mathf.Clamp(moveSpeed, 2f, 5f); // 이동 속도가 2f와 5f 사이에 유지되도록 설정
              //  moveSpeed = moveSpeed == 2f ? 0 : moveSpeed;
                float speed = moveSpeed == 2f ? 0 : moveSpeed;
                // 회전 속도와 이동 속도에 비례해서 부드럽게 회전
                float rotationStep = moveSpeed * Time.deltaTime; // 이동 속도에 비례하여 회전 속도 조정
                Vector3 dir = Vector3.RotateTowards(playerController.Transforms.forward, movements, rotationStep, 0f);
                playerController.Transforms.rotation = Quaternion.LookRotation(dir);

                // 이동 및 카메라 이동
                playerController.Transforms.Translate(movements * Time.deltaTime * speed, Space.World);
                camera.Transforms.Translate(Quaternion.Euler(0, camera.Transforms.rotation.eulerAngles.y, 0) * VectorUtility.RotateY45(movements, -45) * Time.deltaTime * speed, Space.World);
            }
            else // 목표 방향과 일정 각도 이상 차이가 나면 회전
            {
                moveSpeed += 3f * Time.deltaTime;
                moveSpeed = Mathf.Clamp(moveSpeed, 2f, 5f);
                float speed = moveSpeed == 2f ? 0 : moveSpeed;
                // 회전 속도 설정
                float rotationStep = speed * Time.deltaTime;
                Vector3 dir = Vector3.RotateTowards(playerController.Transforms.forward, movements, rotationStep, 0f);
                playerController.Transforms.rotation = Quaternion.LookRotation(dir);

                // 조금만 이동 (회전만)
                Vector3 rotateMove = playerController.Transforms.forward;
                playerController.Transforms.Translate(rotateMove * Time.deltaTime * speed, Space.World);
                camera.Transforms.Translate(Quaternion.Euler(0, camera.Transforms.rotation.eulerAngles.y, 0) * VectorUtility.RotateY45(rotateMove, -45) * Time.deltaTime * speed, Space.World);
            }
        }
        else if (movements == Vector3.zero && moveSpeed != 0)
        {
            // 멈추는 과정에서 이동 속도 감소
            moveSpeed = Mathf.Lerp(moveSpeed, 0f, 10f * Time.deltaTime); // 부드럽게 속도가 줄어들도록 설정

            // 이동하며, 카메라가 플레이어와 동기화되도록
            Vector3 rotateMove = playerController.Transforms.forward;
            playerController.Transforms.Translate(rotateMove * Time.deltaTime * moveSpeed, Space.World);
            camera.Transforms.Translate(Quaternion.Euler(0, camera.Transforms.rotation.eulerAngles.y, 0) * VectorUtility.RotateY45(rotateMove, -45) * Time.deltaTime * moveSpeed, Space.World);

          
        }
    }
}
