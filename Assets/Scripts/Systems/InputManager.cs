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


    public const int BuildMaterials = 7;

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
        if (GameInstance.Instance.editMode != EditMode.None)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, int.MaxValue, ~(1 << BuildMaterials)))
            {
                if (GameInstance.Instance.drawGrid)
                {
                    if(GameInstance.Instance.drawGrid.Select(hit.point, ref x, ref y))
                    {
                       
                    }
                    if (!(x >= 24 || x < -24 || y >= 24 || y < -24))
                    {
                        switch (structureState)
                        {
                            case StructureState.None: break;
                            case StructureState.Floor:
                                if (GameInstance.Instance.editMode != EditMode.None) GameInstance.Instance.assetLoader.PreLoadFloor(x, y);
                                break;
                            case StructureState.Wall:
                                if (GameInstance.Instance.editMode != EditMode.None) GameInstance.Instance.assetLoader.PreLoadWall(hit.point, x, y, true);
                                break;
                            case StructureState.Door:
                                if (GameInstance.Instance.editMode != EditMode.None) GameInstance.Instance.assetLoader.PreLoadWall(hit.point, x, y, false);
                                break;
                        }
                    }
                }
            }
            if (!(x >= 25 || x < -25 || y >= 25 || y < -25))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    switch (structureState)
                    {
                        case StructureState.None: break;
                        case StructureState.Floor:
                            if(GameInstance.Instance.editMode == EditMode.CreativeMode) GameInstance.Instance.assetLoader.LoadFloor(x, y);
                            else if(GameInstance.Instance.editMode == EditMode.DestroyMode) GameInstance.Instance.housingSystem.RemoveFloor(x, y);
                            break;
                        case StructureState.Wall:
                            BuildWallDirection buildWallDirection = GameInstance.Instance.housingSystem.GetWallDirection(hit.point, x, y);
                            if (GameInstance.Instance.editMode == EditMode.CreativeMode) GameInstance.Instance.assetLoader.LoadWall(buildWallDirection, x, y);
                            else if (GameInstance.Instance.editMode == EditMode.DestroyMode) GameInstance.Instance.housingSystem.RemoveWall(buildWallDirection, x, y);
                            break;

                        case StructureState.Door:
                            BuildWallDirection buildDoorDirection = GameInstance.Instance.housingSystem.GetWallDirection(hit.point, x, y);
                            if (GameInstance.Instance.editMode == EditMode.CreativeMode) GameInstance.Instance.assetLoader.LoadWall(buildDoorDirection, x, y, false);
                            else if (GameInstance.Instance.editMode == EditMode.DestroyMode) GameInstance.Instance.housingSystem.RemoveWall(buildDoorDirection, x, y);
                            break;
                    }

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
