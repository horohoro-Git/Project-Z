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
    public GameManager manager;
    public InputManager inputManager;
    public PlayerController playerController;
    public App app;
    public DrawGrid drawGrid;
    public AssetLoader assetLoader;
    public HousingSystem housingSystem;
    public bool creativeMode;
    List<PlayerController> players = new List<PlayerController>();

    public List<PlayerController> GetPlayers { get { return players; } }
    public WorldGrids worldGrids;
    public void Reset()
    {
        housingSystem = null;
        playerController = null;
        worldGrids = null;
        players.Clear();
        app = null;
        manager = null;
        inputManager = null;
        instance = null;
        drawGrid = null;
        assetLoader = null;
    }

    public void AddPlayer(PlayerController pc)
    {
        players.Add(pc);
    }
}
