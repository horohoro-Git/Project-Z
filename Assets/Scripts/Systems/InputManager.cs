using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

using UnityEngine.InputSystem.HID;
public class InputManager : MonoBehaviour
{
    PlayerController pc;
    public PlayerController PC
    {
        get
        {
            if (pc == null) pc = GetComponent<PlayerController>();
            return pc;
        }
    }
    public enum StructureState
    {
        None,
        Floor,
        Wall,
        Door
    }

    [NonSerialized]
    public StructureState structureState = StructureState.None;

    const string debugAction = "F3";
    Camera mainCamera;
    PlayerInput playerInput;
    Debugging debugging;
    int x, y;

    [NonSerialized]
    public Vector3 worldPosition;




    public void Setup(PlayerInput playerInput)
    {
        this.playerInput = playerInput;

        if (debugging == null)
        {
            if (GameInstance.Instance.app != null)
            {
                debugging = GameInstance.Instance.app.debugging;
                debugging.gameObject.SetActive(false);
            }
        }
    }

    private void Start()
    {
        GameInstance.Instance.inputManager = this;
        mainCamera = Camera.main;   
        if(debugging) playerInput.actions[debugAction].performed += DebugOn;
    }

    private void Update()
    {
        if (GameInstance.Instance.creativeMode)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, int.MaxValue))
            {
                if (GameInstance.Instance.drawGrid)
                {
                    GameInstance.Instance.drawGrid.Select(hit.point, ref x, ref y);

                }
            }
            if (Input.GetMouseButtonDown(0))
            {
                switch (structureState)
                {
                    case StructureState.None: break;
                    case StructureState.Floor:
                        GameInstance.Instance.assetLoader.LoadFloor(x, y);
                        break;
                    case StructureState.Wall:

                        break;

                    case StructureState.Door:
                        break;
                }

               
            }
        }
    }

    private void OnDisable()
    {
        if (playerInput != null && debugging)
        {
            playerInput.actions[debugAction].performed -= DebugOn;
        }
    }

    void DebugOn(InputAction.CallbackContext callback)
    {
        if (debugging.gameObject.activeSelf) debugging.gameObject.SetActive(false);
        else debugging.gameObject.SetActive(true);
    }
 
}
