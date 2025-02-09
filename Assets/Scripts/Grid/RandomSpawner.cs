using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpawner : MonoBehaviour
{
    [SerializeField]
    GameObject miniGrass;
    [SerializeField]
    GameObject flower1;
    [SerializeField]
    GameObject flower2;
    [SerializeField]
    GameObject flower3;
    [SerializeField]
    GameObject Tree;
    [SerializeField]
    int treeNum;
    GameObject terrain;
    bool[,] grasses = new bool[100, 100];
    int[] sizeCheckX = new int[8] { 0, 1, -1, 0, 1, 1, -1,1 };
    int[] sizeCheckY = new int[8] { 1, 0, 0, -1, 1, -1, -1,1 };
    // Start is called before the first frame update
    void Start()
    {
        terrain = new GameObject();
        terrain.name = "terrain";

        Invoke("LateStart", 0.5f);
    }

    void LateStart()
    {
        SpawnTrees();
        SpawnGrasses();
    }


    void SpawnTrees()
    {
        for (int i = 0; i < treeNum; i++)
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
            if(check == 8)
            {
                for (int k = 0; k < 8; k++) grasses[x + sizeCheckX[k], y + sizeCheckY[k]] = true;
                grasses[x,y] = true;
                GameObject tree = Instantiate(GameInstance.Instance.assetLoader.tree03, terrain.transform);
                tree.transform.position = new Vector3(x - 49, 0.1f, y - 49);
            }

        }
    
    }

    void SpawnGrasses()
    {
        for (int i = 0; i < 99; i++)
        {
            for (int j = 0; j < 99; j++)
            {
                int check = 0;
                for (int k = 0; k < 4; k++)
                {
                    if (CheckSize(i + sizeCheckX[k], j + sizeCheckY[k]))
                    {
                        if (grasses[i + sizeCheckX[k], j + sizeCheckY[k]])
                        {
                            check++;
                        }
                    }
                }

                if (UnityEngine.Random.Range(0, 100) > 90 + check * 3 )
                {
                    grasses[i, j] = true;
                    int rand = UnityEngine.Random.Range(0, 100);
                    if(rand < 70)
                    {
                        GameObject grass = Instantiate(GameInstance.Instance.assetLoader.grasses, terrain.transform);
                        grass.transform.position = new Vector3(i - 49f, 0.1f, j - 49f);
                    }
                    else if(rand < 80)
                    {
                        GameObject flower = Instantiate(GameInstance.Instance.assetLoader.flowerOrange, terrain.transform);
                        flower.transform.position = new Vector3(i - 49f, 0.1f, j - 49f);
                        flower.transform.rotation = Quaternion.Euler(0, -90, 0);
                    }
                    else if(rand < 90)
                    {
                        GameObject flower = Instantiate(GameInstance.Instance.assetLoader.flowerPink, terrain.transform);
                        flower.transform.position = new Vector3(i - 49f, 0.1f, j - 49f);
                        flower.transform.rotation = Quaternion.Euler(0, -90, 0);
                    }
                    else if(rand < 100)
                    {
                        GameObject flower = Instantiate(GameInstance.Instance.assetLoader.flowerYellow, terrain.transform);
                        flower.transform.position = new Vector3(i - 49f, 0.1f, j - 49f);
                        flower.transform.rotation = Quaternion.Euler(0, -90, 0);
                    }
                }
            }

        }
    }
    bool CheckSize(int a, int b)
    {
        return a >= 0 && a < 100 && b >= 0 && b < 100;
    }
}
