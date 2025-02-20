using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentSpawner : MonoBehaviour
{
 /*   [SerializeField]
    GameObject miniGrass;
    [SerializeField]
    GameObject flower1;
    [SerializeField]
    GameObject flower2;
    [SerializeField]
    GameObject flower3;
    [SerializeField]
    GameObject Tree;*/
    [SerializeField]
    int treeNum;
    GameObject terrain;
    bool[,] objectChecks = new bool[100, 100];
    public EnvironmentObject[,] environmentObjects = new EnvironmentObject[100, 100];
    int[] sizeCheckX = new int[8] { 0, 1, -1, 0, 1, 1, -1,1 };
    int[] sizeCheckY = new int[8] { 1, 0, 0, -1, 1, -1, -1,1 };
    // Start is called before the first frame update

    private void Awake()
    {
        GameInstance.Instance.environmentSpawner = this;
    }
    void Start()
    {
        terrain = new GameObject();
        terrain.name = "terrain";

    //    Invoke("LateStart", 0.5f);
    }

/*    void LateStart()
    {
        SpawnTrees();
        SpawnGrasses();
    }
*/
    public void NewEnvironment()
    {
        if (GameInstance.Instance.assetLoader.assetLoadSuccessful)
        {
            SpawnTrees();
            SpawnGrasses();
        }
        else
        {
            Invoke("NewEnvironment", 0.5f);
        }
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
            if (check == 8)
            {
                for (int k = 0; k < 8; k++) objectChecks[x + sizeCheckX[k], y + sizeCheckY[k]] = true;
                objectChecks[x, y] = true;
                EnvironmentObject tree = Instantiate(GameInstance.Instance.assetLoader.loadedAssets[LoadURL.Tree], terrain.transform).GetComponent<EnvironmentObject>();
                tree.transform.position = new Vector3(x - 49, 0, y - 49);
                tree.environmentType = EnvironmentType.Tree;
                tree.X = x;
                tree.Y = y;
                environmentObjects[x, y] = tree;
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
                        if (objectChecks[i + sizeCheckX[k], j + sizeCheckY[k]])
                        {
                            check++;
                        }
                    }
                }

                if (UnityEngine.Random.Range(0, 100) > 90 + check * 3 )
                {
                    objectChecks[i, j] = true;
                    int rand = UnityEngine.Random.Range(0, 100);
                    if(rand < 70)
                    {
                        EnvironmentObject grass = Instantiate(GameInstance.Instance.assetLoader.loadedAssets[LoadURL.Grasses], terrain.transform).GetComponent<EnvironmentObject>();
                        grass.transform.position = new Vector3(i - 49f, 0, j - 49f);
                        grass.environmentType = EnvironmentType.Grasses;
                        grass.X = i;
                        grass.Y = j;
                        environmentObjects[i, j] = grass;
                    }
                    else if(rand < 80)
                    {
                        EnvironmentObject flower = Instantiate(GameInstance.Instance.assetLoader.loadedAssets[LoadURL.Flower_Orange], terrain.transform).GetComponent<EnvironmentObject>();
                        flower.transform.position = new Vector3(i - 49f, 0, j - 49f);
                        flower.transform.rotation = Quaternion.Euler(0, -90, 0);
                        flower.environmentType = EnvironmentType.Flower_Orange;
                        flower.rotated = true;
                        flower.X = i;
                        flower.Y = j;
                        environmentObjects[i, j] = flower;
                    }
                    else if(rand < 90)
                    {
                        EnvironmentObject flower = Instantiate(GameInstance.Instance.assetLoader.loadedAssets[LoadURL.Flower_Pink], terrain.transform).GetComponent<EnvironmentObject>();
                        flower.transform.position = new Vector3(i - 49f, 0, j - 49f);
                        flower.transform.rotation = Quaternion.Euler(0, -90, 0);
                        flower.environmentType = EnvironmentType.Flower_Pink;
                        flower.rotated = true;
                        flower.X = i;
                        flower.Y = j;
                        environmentObjects[i, j] = flower;
                    }
                    else if(rand < 100)
                    {
                        EnvironmentObject flower = Instantiate(GameInstance.Instance.assetLoader.loadedAssets[LoadURL.Flower_Yellow], terrain.transform).GetComponent<EnvironmentObject>();
                        flower.transform.position = new Vector3(i - 49f, 0, j - 49f);
                        flower.transform.rotation = Quaternion.Euler(0, -90, 0);
                        flower.environmentType = EnvironmentType.Flower_Yellow;
                        flower.rotated = true;
                        flower.X = i;
                        flower.Y = j;
                        environmentObjects[i,j] = flower;
                    }
                }
            }

        }
    }
    bool CheckSize(int a, int b)
    {
        return a >= 0 && a < 100 && b >= 0 && b < 100;
    }

    public void LoadObject(EnvironmentObjectInfo info)
    {
        switch (info.type)
        {
            case EnvironmentType.None:
                break;
            case EnvironmentType.Flower_Pink:
                EnvironmentObject flower_Pink = Instantiate(GameInstance.Instance.assetLoader.loadedAssets[LoadURL.Flower_Pink], terrain.transform).GetComponent<EnvironmentObject>();
                LoadObjectSetting(flower_Pink, info);
                break;
            case EnvironmentType.Flower_Orange:
                EnvironmentObject flower_Orange = Instantiate(GameInstance.Instance.assetLoader.loadedAssets[LoadURL.Flower_Orange], terrain.transform).GetComponent<EnvironmentObject>();
                LoadObjectSetting(flower_Orange, info);
                break;
            case EnvironmentType.Flower_Yellow:
                EnvironmentObject flower_Yellow = Instantiate(GameInstance.Instance.assetLoader.loadedAssets[LoadURL.Flower_Yellow], terrain.transform).GetComponent<EnvironmentObject>();
                LoadObjectSetting(flower_Yellow, info);
                break;
            case EnvironmentType.Grasses:
                EnvironmentObject grasses = Instantiate(GameInstance.Instance.assetLoader.loadedAssets[LoadURL.Grasses], terrain.transform).GetComponent<EnvironmentObject>();
                LoadObjectSetting(grasses, info);
                break;
            case EnvironmentType.Tree:
                EnvironmentObject tree = Instantiate(GameInstance.Instance.assetLoader.loadedAssets[LoadURL.Tree], terrain.transform).GetComponent<EnvironmentObject>();
                LoadObjectSetting(tree, info);
                break;
        }

    }

    void LoadObjectSetting(EnvironmentObject obj, EnvironmentObjectInfo info)
    {
        obj.transform.position = new Vector3(info.X - 49f, 0, info.Y - 49f);
        if(info.Z) obj.transform.rotation = Quaternion.Euler(0, -90, 0);
        obj.environmentType = info.type;
        if (info.Z) obj.rotated = true;
        obj.X = info.X;
        obj.Y = info.Y;
        environmentObjects[info.X, info.Y] = obj;
    }


    public void RemoveObject(EnvironmentObject obj)
    {
        environmentObjects[obj.X, obj.Y] = null;
        Destroy(obj.gameObject);
    }
}
