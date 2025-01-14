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
        // �Էµ� ������ �̵� ���� �޾ƿ���
        Vector2 movement = context.ReadValue<Vector2>();
       
        movements = VectorUtility.RotateY45(new Vector3(movement.x, 0, movement.y),45);
  
       // Moves?.Invoke(movement);
    }
    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        // Ű�� �� �� �̵��� ����
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
            // �̵��Ϸ��� ����� ���� ������ �󸶳� ��ġ�ϴ���
            float dotProduct = Vector3.Dot(playerController.Transforms.forward, movements);

            // ���� ���� �̻� ������ ��ġ�ϸ� �̵�
            if (dotProduct >= 0.35f)
            {
                // �̵� �ӵ� ����
                moveSpeed += 3f * Time.deltaTime;
                moveSpeed = Mathf.Clamp(moveSpeed, 2f, 5f); // �̵� �ӵ��� 2f�� 5f ���̿� �����ǵ��� ����
              //  moveSpeed = moveSpeed == 2f ? 0 : moveSpeed;
                float speed = moveSpeed == 2f ? 0 : moveSpeed;
                // ȸ�� �ӵ��� �̵� �ӵ��� ����ؼ� �ε巴�� ȸ��
                float rotationStep = moveSpeed * Time.deltaTime; // �̵� �ӵ��� ����Ͽ� ȸ�� �ӵ� ����
                Vector3 dir = Vector3.RotateTowards(playerController.Transforms.forward, movements, rotationStep, 0f);
                playerController.Transforms.rotation = Quaternion.LookRotation(dir);

                // �̵� �� ī�޶� �̵�
                playerController.Transforms.Translate(movements * Time.deltaTime * speed, Space.World);
                camera.Transforms.Translate(Quaternion.Euler(0, camera.Transforms.rotation.eulerAngles.y, 0) * VectorUtility.RotateY45(movements, -45) * Time.deltaTime * speed, Space.World);
            }
            else // ��ǥ ����� ���� ���� �̻� ���̰� ���� ȸ��
            {
                moveSpeed += 3f * Time.deltaTime;
                moveSpeed = Mathf.Clamp(moveSpeed, 2f, 5f);
                float speed = moveSpeed == 2f ? 0 : moveSpeed;
                // ȸ�� �ӵ� ����
                float rotationStep = speed * Time.deltaTime;
                Vector3 dir = Vector3.RotateTowards(playerController.Transforms.forward, movements, rotationStep, 0f);
                playerController.Transforms.rotation = Quaternion.LookRotation(dir);

                // ���ݸ� �̵� (ȸ����)
                Vector3 rotateMove = playerController.Transforms.forward;
                playerController.Transforms.Translate(rotateMove * Time.deltaTime * speed, Space.World);
                camera.Transforms.Translate(Quaternion.Euler(0, camera.Transforms.rotation.eulerAngles.y, 0) * VectorUtility.RotateY45(rotateMove, -45) * Time.deltaTime * speed, Space.World);
            }
        }
        else if (movements == Vector3.zero && moveSpeed != 0)
        {
            // ���ߴ� �������� �̵� �ӵ� ����
            moveSpeed = Mathf.Lerp(moveSpeed, 0f, 10f * Time.deltaTime); // �ε巴�� �ӵ��� �پ�鵵�� ����

            // �̵��ϸ�, ī�޶� �÷��̾�� ����ȭ�ǵ���
            Vector3 rotateMove = playerController.Transforms.forward;
            playerController.Transforms.Translate(rotateMove * Time.deltaTime * moveSpeed, Space.World);
            camera.Transforms.Translate(Quaternion.Euler(0, camera.Transforms.rotation.eulerAngles.y, 0) * VectorUtility.RotateY45(rotateMove, -45) * Time.deltaTime * moveSpeed, Space.World);

          
        }
    }
}
