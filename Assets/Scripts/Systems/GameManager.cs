using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{

    const string playerName = "Player";
    const string inputManagerName = "InputManager";
    public PlayerController player;
    public GameObject test;
    public Weapon testWeapon;
  
    public InputManager inputManager;
    public Vector3 startPosition;
    public GameMode gameMode;
    public bool loaded;
   

    private void Awake()
    {
        GameInstance.Instance.gameManager = this;
    }
    private void Start()
    {
        Invoke("PlayerSettings", 0.5f);
        Invoke("LoadBuilds", 0.5f);
    }

    void PlayerSettings()
    {
        PlayerController pc = Instantiate(player); //플레이어 생성
        pc.name = playerName;
        pc.transform.position = startPosition;

        InputManager inputMGR = Instantiate(inputManager);  //입력 시스템 생성
        inputMGR.name = inputManagerName;
        inputMGR.Setup(pc.GetComponent<PlayerInput>());

        //캐릭터 추가
        /* GameObject human = Instantiate(GameInstance.Instance.assetLoader.loadedAssets[LoadURL.Human_Male]);
         human.transform.SetParent(pc.Transforms);
         human.transform.position = startPosition;



         pc.SetController(human);*/
        GameObject human = Instantiate(test);
        human.transform.SetParent(pc.Transforms);
        human.transform.position = startPosition;
        pc.SetController(human);


        Weapon weapon = Instantiate(testWeapon);
        AttachItem attachItem = human.GetComponentInChildren<AttachItem>();
        weapon.transform.SetParent(attachItem.transform);

        weapon.transform.localPosition = new Vector3(0, 0, 0);
        weapon.transform.localRotation = Quaternion.Euler(-90, 120, 0);
        pc.equipWeapon = weapon;

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
