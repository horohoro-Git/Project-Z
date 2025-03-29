using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.XR;
using UnityEngine.UIElements;

public class EnemySpawner : MonoBehaviour
{
    public int num;
    bool[,] objectChecks = new bool[100, 100];
    int[] sizeCheckX = new int[8] { 0, 1, -1, 0, 1, 1, -1, 1 };
    int[] sizeCheckY = new int[8] { 1, 0, 0, -1, 1, -1, -1, 1 };

    [NonSerialized]
    public bool loaded = false;
    private void Awake()
    {
        GameInstance.Instance.enemySpawner = this;
    }

    public void Setup()
    {
        loaded = true;
    }

    public void SpawnEnemies()
    {
        AssetLoader assetLoader = GameInstance.Instance.assetLoader;
        for (int i = 0; i < num; i++)
        {
            int x = UnityEngine.Random.Range(0, 100);
            int y = UnityEngine.Random.Range(0, 100);

            int check = 0;
            for (int k = 0; k < 8; k++)
            {
                if (CheckSize(x + sizeCheckX[k], y + sizeCheckY[k]))
                {
                    check++;
                }
            }

            if (check == 8)
            {
                for (int k = 0; k < 8; k++) objectChecks[x + sizeCheckX[k], y + sizeCheckY[k]] = true;
                objectChecks[x, y] = true;
                int type = UnityEngine.Random.Range(1, 2);
                EnemyController enemy = Instantiate(assetLoader.loadedAssets[AssetLoader.enemykeys[type - 1]]).GetComponent<EnemyController>();
                enemy.Transforms.position = new Vector3(x - 49, 0, y - 49);
                enemy.enemyStruct = assetLoader.enemies[type - 1];
                GameInstance.Instance.worldGrids.AddObjects(enemy.gameObject, MinimapIconType.Enemy, true);
                enemy.Setup();

            }
        }
        GameInstance.Instance.minimapUI.ChangeList(MinimapIconType.Enemy);
    }

    public void LoadEnemies(Vector3 position, Quaternion rotation, int type, List<ItemStruct> itemStructs, int modelType, string helmet = "", string chest = "", string arms = "", string legs = "", string feet = "", string cape = "")
    {
        if(modelType > 0)
        {
            //감염된 플레이어 
            PlayerController pc = Instantiate(GameInstance.Instance.gameManager.player);

            pc.RemoveAction();
            pc.Rigid.interpolation = RigidbodyInterpolation.None;
            pc.Rigid.constraints = RigidbodyConstraints.FreezeAll;
            pc.state = PlayerState.Dead;
            GameObject model = Instantiate(GameInstance.Instance.assetLoader.loadedAssets[AssetLoader.humankeys[modelType - 1]]);
            //GameObject model = Instantiate(GameInstance.Instance.gameManager.test);
            //Destroy(model.GetComponentInChildren<Animator>());
            model.transform.SetParent(pc.Transforms);
            model.transform.localPosition = Vector3.zero;
            pc.SetController(model, true, false);

            //위치 조정
            pc.Transforms.position = position;
            pc.Transforms.rotation = rotation;

            //적의 타입으로 충돌 변경
            pc.gameObject.tag = "Enemy";
            pc.gameObject.layer = 0b1010;
            pc.Rigid.excludeLayers = 0;
            pc.ChangeTagLayer(pc.Transforms, "Enemy", 0b1010);
            pc.GetComponent<CapsuleCollider>().excludeLayers = 0;
            pc.GetLeftHand.boxCollider.excludeLayers = 0b10000000000;
            pc.GetRightHand.boxCollider.excludeLayers = 0b10000000000;

            //애니메이션 적으로 변환
         
            pc.modelAnimator.SetLayerWeight(1, 0);
            pc.modelAnimator.SetLayerWeight(2, 0);
            //죽은 시체에 인벤토리의 아이템 적용
            pc.gameObject.AddComponent<NavMeshAgent>();
            EnemyController enemyController = pc.gameObject.AddComponent<EnemyController>();
            enemyController.SetController(model);
            enemyController.capsuleCollider = GetComponent<CapsuleCollider>();
            enemyController.itemStructs = itemStructs;
            enemyController.enemyStruct = GameInstance.Instance.assetLoader.enemies[0];
            GameInstance.Instance.worldGrids.AddObjects(enemyController.gameObject, MinimapIconType.Enemy, true);

            enemyController.playerType = modelType;

            enemyController.Setup();
            if(helmet.Length > 0) pc.GetAvatar.AddCloth(helmet);
            if(chest.Length > 0) pc.GetAvatar.AddCloth(chest);
            if(arms.Length > 0) pc.GetAvatar.AddCloth(arms);
            if(legs.Length > 0) pc.GetAvatar.AddCloth(legs);
            if(feet.Length > 0) pc.GetAvatar.AddCloth(feet);
            if(cape.Length > 0) pc.GetAvatar.AddCloth(cape);
            
            Destroy(pc);
        }
        else
        {
            EnemyController enemyController = Instantiate(GameInstance.Instance.assetLoader.loadedAssets[AssetLoader.enemykeys[type - 1]]).GetComponent<EnemyController>();
            enemyController.transform.position = position;
            enemyController.transform.rotation = rotation;
            enemyController.itemStructs = itemStructs;
            enemyController.enemyStruct = GameInstance.Instance.assetLoader.enemies[type - 1];
            GameInstance.Instance.worldGrids.AddObjects(enemyController.gameObject, MinimapIconType.Enemy, true);

            enemyController.Setup();
        }
     
    }

    bool CheckSize(int a, int b)
    {
        return a >= 0 && a < 100 && b >= 0 && b < 100;
    }
}
