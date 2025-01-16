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
    public App app;
    public DrawGrid drawGrid;
    public void Reset()
    {
        app = null;
        manager = null;
        inputManager = null;
        instance = null;
        drawGrid = null;
    }
}
