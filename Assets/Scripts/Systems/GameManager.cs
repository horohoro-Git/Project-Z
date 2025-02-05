using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{


    const string playerName = "Player";
    const string inputManagerName = "InputManager";
    public PlayerController player;
    public InputManager inputManager;
    public Vector3 startPosition;
    public GameMode gameMode;
    public bool loaded;
    private void Awake()
    {
        GameInstance.Instance.gameManager = this;
        PlayerController pc = Instantiate(player); //플레이어 생성
        pc.name = playerName;
        pc.transform.position = startPosition; 
        InputManager inputMGR = Instantiate(inputManager);  //입력 시스템 생성
        inputMGR.name = inputManagerName;
        inputMGR.Setup(pc.GetComponent<PlayerInput>());
    }

    private void Start()
    {
        Invoke("LoadBuilds", 0.5f);
    }

    void LoadBuilds()
    {
        if (gameMode == GameMode.DefaultMode)
        {
            List<HousingInfo> infos = SaveLoadSystem.LoadBuildSystem();

            int minx = GameInstance.Instance.housingSystem.minx;
            int miny = GameInstance.Instance.housingSystem.miny;
            for (int i = 0; i < infos.Count; i++)
            {
                HousingInfo info = infos[i];
                if (info.materialsType == MaterialsType.Floor)
                {
                    GameInstance.Instance.assetLoader.LoadFloor(info.x + minx, info.y + miny, true);

                }
                else
                {
                    BuildWallDirection buildWallDirection = info.z == 0 ? BuildWallDirection.Left : BuildWallDirection.Bottom;
                    GameInstance.Instance.assetLoader.LoadWall(buildWallDirection, info.x + minx, info.y + miny, info.materialsType == MaterialsType.Wall ? true : false, true);
                }
            }

            GameInstance.Instance.housingSystem.CheckRoofInWorld();
        }

        loaded = true;
    }
}
