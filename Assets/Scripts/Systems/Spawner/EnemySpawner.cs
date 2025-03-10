using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class EnemySpawner : MonoBehaviour
{
    public int num;
    bool[,] objectChecks = new bool[100, 100];
    int[] sizeCheckX = new int[8] { 0, 1, -1, 0, 1, 1, -1, 1 };
    int[] sizeCheckY = new int[8] { 1, 0, 0, -1, 1, -1, -1, 1 };

    bool loaded = false;
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
                int type = UnityEngine.Random.Range(0, 1);
                EnemyController enemy = Instantiate(assetLoader.loadedAssets[AssetLoader.enemykeys[type]]).GetComponent<EnemyController>();
                enemy.Transforms.position = new Vector3(x - 49, 0, y - 49);
                enemy.enemyStruct = assetLoader.enemies[type];
                GameInstance.Instance.worldGrids.AddLives(enemy.gameObject);
                enemy.Setup();
                //EnvironmentObject tree = Instantiate(GameInstance.Instance.assetLoader.loadedAssets[LoadURL.Tree], terrain.transform).GetComponent<EnvironmentObject>();
                /*  tree.transform.position = new Vector3(x - 49, 0, y - 49);
                  tree.environmentType = EnvironmentType.Tree;
                  tree.X = x;
                  tree.Y = y;
                  environmentObjects[x, y] = tree;*/
            }
        }
    }

    public void LoadEnemies(Vector3 position, Quaternion rotation, int type, List<ItemStruct> itemStructs)
    {
        EnemyController enemyController = Instantiate(GameInstance.Instance.assetLoader.loadedAssets[AssetLoader.enemykeys[type - 1]]).GetComponent<EnemyController>();
        enemyController.transform.position = position;
        enemyController.transform.rotation = rotation;
        enemyController.itemStructs = itemStructs;

        GameInstance.Instance.worldGrids.AddLives(enemyController.gameObject);
        enemyController.Setup();
    }

    bool CheckSize(int a, int b)
    {
        return a >= 0 && a < 100 && b >= 0 && b < 100;
    }

    private void OnApplicationQuit()
    {
        if(loaded) SaveLoadSystem.SaveEnemyInfo();
    }
}
