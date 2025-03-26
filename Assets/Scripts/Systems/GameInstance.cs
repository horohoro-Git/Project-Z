using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInstance
{
    //singleton
    static GameInstance instance;

    public static GameInstance Instance
    {     
        get {
            if (instance == null) instance = new GameInstance();              
            return instance;
        }
    }

    public Vector2 currentDir;
    public GameManager gameManager;
    public InputManager inputManager;
    public UIManager uiManager;
    public PlayerController playerController;
    public App app;
    public DrawGrid drawGrid;
    public AssetLoader assetLoader;
    public HousingSystem housingSystem;
    public InventorySystem inventorySystem;
    public CreatableUISystem creatableUISystem;
    public QuickSlotUI quickSlotUI;
    public PlayerStatusUI playerStatusUI;
    public PlayerStatusDetailsUI playerStatusDetailsUI;
    public AbilityMenuUI abilityMenuUI;
    public BoxInventorySystem boxInventorySystem;
    public MinimapUI minimapUI;
    public CharacterProfileUI characterProfileUI;
    public EnvironmentSpawner environmentSpawner;
    public EditMode editMode;
    public EnemySpawner enemySpawner;
    public CraftingLearnSystem craftingLearnSystem;
    List<PlayerController> players = new List<PlayerController>();

    public List<PlayerController> GetPlayers { get { return players; } }
    public WorldGrids worldGrids;
    public bool quit = false;
    public void Reset()
    {
        playerStatusDetailsUI.enabled = false;
        characterProfileUI.enabled = false;
        abilityMenuUI.enabled = false;
        minimapUI.enabled = false;
        playerStatusUI.enabled = false;
        environmentSpawner.enabled = false;
        creatableUISystem.enabled = false;
        uiManager.enabled = false;
        inventorySystem.enabled = false;
        housingSystem.enabled = false;
        playerController.enabled = false;
        worldGrids.enabled = false;
        app.enabled = false;
        gameManager.enabled = false;
        inputManager.enabled = false;
        drawGrid.enabled = false;
        assetLoader.enabled = false;
        quickSlotUI.enabled = false;
        enemySpawner.enabled = false;
        boxInventorySystem.enabled = false;
        craftingLearnSystem.enabled = false;

        playerStatusDetailsUI = null;
        characterProfileUI = null;
        abilityMenuUI = null;
        minimapUI = null;
        playerStatusUI = null;
        environmentSpawner = null;
        creatableUISystem = null;
        uiManager = null;
        inventorySystem = null;
        housingSystem = null;
        playerController = null;
        worldGrids = null;
        players.Clear();
        app = null;
        gameManager = null;
        inputManager = null;
        instance = null;
        drawGrid = null;
        assetLoader = null;
        quickSlotUI = null;
        enemySpawner = null;
        boxInventorySystem = null;
        craftingLearnSystem = null;
    }

    public void AddPlayer(PlayerController pc)
    {
        players.Add(pc);
    }
}
