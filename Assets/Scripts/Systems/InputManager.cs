using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;
using UnityEngine.UI;
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
  

    [NonSerialized]
    public StructureState structureState = StructureState.None;

    const string debugAction = "F3";
    const string aroundAction = "Viewaround";
    Camera mainCamera;
    PlayerInput playerInput;
    Debugging debugging;
    int x, y;

    [NonSerialized]
    public Vector3 worldPosition;


    public const int BuildMaterials = 7;
    public const int Plane = 11;
    PointerEventData eventData = new PointerEventData(EventSystem.current);
    List<RaycastResult> raycastResults = new List<RaycastResult>();
    public void Setup(PlayerInput playerInput)
    {
      
        this.playerInput = playerInput;
        if (debugging == null)
        {
            if (GameInstance.Instance.app != null && GameInstance.Instance.app.debugging != null)
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
        playerInput.actions[aroundAction].performed += StartAround;
    //    playerInput.actions[aroundAction].canceled += EndAround;
    }
 
  
    private void Update()
    {
        if (GameInstance.Instance.editMode != EditMode.None)
        {
            GraphicRaycaster raycaster = GameInstance.Instance.uiManager.graphicRaycaster;
            eventData.position = Input.mousePosition;
            raycastResults.Clear();
            raycaster.Raycast(eventData, raycastResults);
            if (raycastResults.Count > 0)
            {
                GameInstance.Instance.drawGrid.RemoveHighlight();
                GameInstance.Instance.assetLoader.RemovePreview();
                x = int.MaxValue;
                y = int.MaxValue;
                return;
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, int.MaxValue, Plane))
            {
                if (GameInstance.Instance.drawGrid)
                {
                    GameInstance.Instance.drawGrid.Select(hit.point, ref x, ref y);
                    if (!(x >= 24 || x < -24 || y >= 24 || y < -24) && GameInstance.Instance.editMode == EditMode.CreativeMode)
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
                    else if (!(x >= 24 || x < -24 || y >= 24 || y < -24) && GameInstance.Instance.editMode == EditMode.DestroyMode)
                    {
                        BuildWallDirection buildWallDirection = GameInstance.Instance.housingSystem.GetWallDirection(hit.point, x, y);
                        GameInstance.Instance.assetLoader.PreviewDestoryObject(buildWallDirection, hit, x, y);
                    }
                }
            }
            if (!(x >= 24 || x < -24 || y >= 24 || y < -24))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (GameInstance.Instance.editMode == EditMode.DestroyMode)
                    {
                        BuildWallDirection buildWallDirection = GameInstance.Instance.housingSystem.GetWallDirection(hit.point, x, y);
                        GameInstance.Instance.housingSystem.RemoveMaterial(buildWallDirection, x, y);
                    }
                    else
                    {
                        switch (structureState)
                        {
                            case StructureState.None: break;
                            case StructureState.Floor:
                                if (GameInstance.Instance.editMode == EditMode.CreativeMode) GameInstance.Instance.assetLoader.LoadFloor(x, y);
                                else if (GameInstance.Instance.editMode == EditMode.DestroyMode) GameInstance.Instance.housingSystem.RemoveFloor(x, y);
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
    }

    private void OnDisable()
    {
        if (playerInput != null)
        {
           if(debugging) playerInput.actions[debugAction].performed -= DebugOn;
            playerInput.actions[aroundAction].performed -= StartAround;
           // playerInput.actions[aroundAction].canceled -= EndAround;
        }
    }

    void DebugOn(InputAction.CallbackContext callback)
    {
        if (debugging.gameObject.activeSelf) debugging.gameObject.SetActive(false);
        else debugging.gameObject.SetActive(true);
    }
 

    void StartAround(InputAction.CallbackContext callback)
    {
        if (GameInstance.Instance.GetPlayers.Count > 0)
        {
            PlayerController pc = GameInstance.Instance.GetPlayers[0];
            if (pc != null)
            {
                PlayerCamera camera = pc.camera;
                if (camera != null)
                {
                    if (pc.lookAround)
                    {
                        pc.lookAround = false;
                        camera.lookAround = false;
                    }
                    else
                    {
                        pc.lookAround = true;
                        camera.lookAround = true;
                    }
                }
            }
        }
    }
    void EndAround(InputAction.CallbackContext callback)
    {
        if (GameInstance.Instance.GetPlayers.Count > 0)
        {
            PlayerController pc = GameInstance.Instance.GetPlayers[0];
            if (pc != null)
            {
                PlayerCamera camera = pc.camera;
                if (camera != null)
                {
                    pc.lookAround = false;
                    camera.lookAround = false;
                }
            }
        }
    }
}
