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
    public const int Plane = 1 << 11;
    public const int Furniture = 1 << 13;
    PointerEventData eventData = new PointerEventData(EventSystem.current);
    List<RaycastResult> raycastResults = new List<RaycastResult>();

    [NonSerialized]
    public ItemStruct selectedInstallableItem;
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
        LivesHover();

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
            AssetLoader assetLoader = GameInstance.Instance.assetLoader;
            HousingSystem housing = GameInstance.Instance.housingSystem;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if(Physics.Raycast(ray, out RaycastHit hitFurniture, int.MaxValue, Furniture))
            {
               
                GameInstance.Instance.drawGrid.Select(hitFurniture.collider.gameObject.transform.position, ref x, ref y);
               
                if (!(x >= 24 || x < -24 || y >= 24 || y < -24) && GameInstance.Instance.editMode == EditMode.DestroyMode)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        GameInstance.Instance.housingSystem.RemoveFurniture(x, y);
                        return;
                    }
                    GameInstance.Instance.assetLoader.PreviewDestoryObject(hitFurniture, x, y);
                    return;
                }
            }

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
                                if (GameInstance.Instance.editMode != EditMode.None) assetLoader.PreLoadFloor(hit.point, x, y);
                                break;
                            case StructureState.Wall:
                                if (GameInstance.Instance.editMode != EditMode.None) assetLoader.PreLoadWall(hit.point, x, y, true);
                                break;
                            case StructureState.Door:
                                if (GameInstance.Instance.editMode != EditMode.None) assetLoader.PreLoadWall(hit.point, x, y, false);
                                break;
                            case StructureState.Furniture:
                                if (GameInstance.Instance.editMode != EditMode.None) assetLoader.PreviewFurniture(hit.point, x, y, "preview_" + selectedInstallableItem.asset_name);
                                 break;
                        }
                    }
                    else if (!(x >= 24 || x < -24 || y >= 24 || y < -24) && GameInstance.Instance.editMode == EditMode.DestroyMode)
                    {
                        BuildWallDirection buildWallDirection = housing.GetWallDirection(hit.point, x, y);
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
                        BuildWallDirection buildWallDirection = housing.GetWallDirection(hit.point, x, y);
                        GameInstance.Instance.housingSystem.RemoveMaterial(buildWallDirection, x, y);
                    }
                    else
                    {
                        switch (structureState)
                        {
                            case StructureState.None: break;
                            case StructureState.Floor:
                                if (GameInstance.Instance.editMode == EditMode.CreativeMode) assetLoader.LoadFloor(x, y);
                                else if (GameInstance.Instance.editMode == EditMode.DestroyMode) housing.RemoveFloor(x, y);
                                break;
                            case StructureState.Wall:
                                BuildWallDirection buildWallDirection = housing.GetWallDirection(hit.point, x, y);
                                if (GameInstance.Instance.editMode == EditMode.CreativeMode) assetLoader.LoadWall(buildWallDirection, x, y);
                                else if (GameInstance.Instance.editMode == EditMode.DestroyMode) housing.RemoveWall(buildWallDirection, x, y);
                                break;

                            case StructureState.Door:
                                BuildWallDirection buildDoorDirection = housing.GetWallDirection(hit.point, x, y);
                                if (GameInstance.Instance.editMode == EditMode.CreativeMode) assetLoader.LoadWall(buildDoorDirection, x, y, false);
                                else if (GameInstance.Instance.editMode == EditMode.DestroyMode) housing.RemoveWall(buildDoorDirection, x, y);
                                break;
                            case StructureState.Furniture:
                                BuildWallDirection buildfurnitureDirection = housing.GetWallDirection(hit.point, x, y);
                                if (GameInstance.Instance.editMode == EditMode.CreativeMode) assetLoader.LoadFurniture(buildfurnitureDirection, x, y, selectedInstallableItem.item_index, false);
                                else if (GameInstance.Instance.editMode == EditMode.DestroyMode) housing.RemoveFurniture(x, y);
                                break;
                        }
                    }
                }
            }
        }
    }


    //살아있는 오브젝트 호버
    float livesHoverTimer;

    void LivesHover()
    {
        if (livesHoverTimer + 0.2f < Time.time)
        {
            livesHoverTimer = Time.time;

            Ray hoverRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            Physics.Raycast(hoverRay, out RaycastHit hitInfo, float.MaxValue);

            if (Physics.Raycast(hoverRay, out RaycastHit hoverHit, float.MaxValue, 0b100010000000000))
            {
                if (Utility.TryGetComponentInParent<EnemyController>(hoverHit.collider, out EnemyController hoveredEnemy))
                {
                    EnemyStruct es = hoveredEnemy.enemyStruct;
                    string n = es.enemy_name;
                    int maxHP = es.max_health;
                    int hp = es.health;
                    if (AllEventManager.customEvents.TryGetValue(1, out var events)) ((Action<string, int, int>)events)?.Invoke(n, maxHP, hp);
                }
                if (Utility.TryGetComponentInParent<NPCController>(hoverHit.collider, out NPCController npcController))
                {
                    EnemyStruct ns = npcController.npcStruct;
                    string n = ns.enemy_name;
                    int maxHP = ns.max_health;
                    int hp = ns.health;
                    if (AllEventManager.customEvents.TryGetValue(1, out var events)) ((Action<string, int, int>)events)?.Invoke(n, maxHP, hp);
                }
            }
            else
            {
                if(AllEventManager.customEvents.TryGetValue(2, out var events)) ((Action)events)?.Invoke();
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
                PlayerCamera camera = pc.playerCamera;
                if (camera != null && pc.state != PlayerState.Dead)
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
                PlayerCamera camera = pc.GetComponent<PlayerCamera>();
                if (camera != null)
                {
                    pc.lookAround = false;
                    camera.lookAround = false;
                }
            }
        }
    }
}
