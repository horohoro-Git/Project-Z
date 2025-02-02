using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;
public class AssetLoader : MonoBehaviour
{

    [NonSerialized]
    public GameObject root;
    [NonSerialized]
    public AssetBundle bundle;

    public string floor_url = "Assets/Edited/Floor/Floor.prefab";
    public string wall_url = "Assets/Edited/wall/wall.prefab";
    public string door_url = "Assets/Edited/wall/door.prefab";
    public string roof_url = "Assets/Edited/roof/roof.prefab";
    [NonSerialized]
    public GameObject floor;
    [NonSerialized]
    public GameObject door;
    [NonSerialized]
    public GameObject wall;
    [NonSerialized]
    public GameObject roof;


    List<GameObject> floor_List = new List<GameObject>();
    public void Awake()
    {
        GameInstance.Instance.assetLoader = this;
    }
    public void Start()
    {
        root = new GameObject();
        root.name = "root";
        root.transform.position = Vector3.zero;
    }
    public IEnumerator DownloadAssetBundle(string url, bool justShader)
    {

        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();
       
        if (www.result == UnityWebRequest.Result.Success)
        {
            if (justShader)
            {
                AssetBundle.LoadFromMemory(www.downloadHandler.data);
                yield break;
            }
            else bundle = AssetBundle.LoadFromMemory(www.downloadHandler.data);
            LoadAsset();
        }
        else
        {
            Debug.LogError("다운로드 실패: " + www.error);
        }

    }

    public void LoadFloor(int x, int y)
    {
        if (!GameInstance.Instance.housingSystem.CheckFloor(x, y))
        {
            GameObject f = Instantiate(floor, root.transform);
            f.transform.position = new Vector3(x * 2 + 1, 0.1f, y * 2 + 1);
            floor_List.Add(f);
            GameInstance.Instance.housingSystem.BuildFloor(x, y,f);
        }
    }

    public void LoadWall(HousingSystem.BuildWallDirection buildWallDirection, int x, int y, bool justWall = true)
    {
        if (GameInstance.Instance.housingSystem.CheckFloor(x, y)) // 바닥 확인
        {
            if (!GameInstance.Instance.housingSystem.CheckWall(x, y, buildWallDirection))
            {
                GameObject go; 
                if (justWall) go = Instantiate(wall);
                else go = Instantiate(door);
                Wall w = go.GetComponent<Wall>();
                w.x = x - GameInstance.Instance.housingSystem.minx;
                w.y = y - GameInstance.Instance.housingSystem.minx;
                switch (buildWallDirection)
                {
                    case HousingSystem.BuildWallDirection.None:
                         return;
                    case HousingSystem.BuildWallDirection.Left:
                        int leftX = x * 2;
                        int leftY = y * 2 + 1;
                        w.transform.position = new Vector3(leftX, 1.7f, leftY);
                        w.transform.rotation = Quaternion.Euler(-90, -90, 0);
                        break;
                    case HousingSystem.BuildWallDirection.Right:
                        int rightX = x * 2 + 2;
                        int rightY = y * 2 + 1;
                        w.transform.position = new Vector3(rightX, 1.7f, rightY);
                        w.transform.rotation = Quaternion.Euler(-90, -90, 0);
                        w.x += 1;
                        break;
                    case HousingSystem.BuildWallDirection.Top:
                        int topX = x * 2 + 1;
                        int topY = y * 2 + 2;
                        w.transform.position = new Vector3(topX, 1.7f, topY);
                        w.y += 1;
                        break;
                    case HousingSystem.BuildWallDirection.Bottom:
                        int botX = x * 2 + 1;
                        int botY = y * 2;
                        w.transform.position = new Vector3(botX, 1.7f, botY);

                        break;
                }
                GameInstance.Instance.housingSystem.BuildWall(x, y, w, buildWallDirection, justWall);
            }
        }



          /*  float pointX = point.x;
            float pointY = point.z;

            //크기 확인
            float disA = pointX - x * 2;
            float disB = (x * 2 + 2) - pointX;
            float disC = pointY - y * 2;
            float disD = (y * 2 + 2) - pointY;
            float min = 2;
            if(disA < min) min = disA;
            if(disB < min) min = disB;
            if(disC < min) min = disC;
            if(disD < min) min = disD;

            HousingSystem.BuildWallDirection buildWallDirection = HousingSystem.BuildWallDirection.None;

            GameObject w = Instantiate(wall);
            if (min == disA)
            {
                buildWallDirection = HousingSystem.BuildWallDirection.Left;
                if (!GameInstance.Instance.housingSystem.CheckWall(x, y, buildWallDirection))
                {
                    int posX = x * 2;
                    int posY = y * 2 + 1;
                    w.transform.position = new Vector3(posX, 1.7f, posY);
                    w.transform.rotation = Quaternion.Euler(-90, -90, 0);
                    Debug.Log("좌측 벽 생성 " + min);

                    GameInstance.Instance.housingSystem.BuildWall(x, y, w, buildWallDirection);
                }
            }
            if (min == disB)
            {
                buildWallDirection = HousingSystem.BuildWallDirection.Right;
                if (!GameInstance.Instance.housingSystem.CheckWall(x, y, buildWallDirection))
                {
                    int posX = x * 2 + 2;
                    int posY = y * 2 + 1;
                    w.transform.position = new Vector3(posX, 1.7f, posY);
                    w.transform.rotation = Quaternion.Euler(-90, -90, 0);
                    Debug.Log("우측 벽 생성 " + min);

                    GameInstance.Instance.housingSystem.BuildWall(x, y, w, buildWallDirection);
                }
            }
            if (min == disC)
            {
                buildWallDirection = HousingSystem.BuildWallDirection.Bottom;
                if (!GameInstance.Instance.housingSystem.CheckWall(x, y, buildWallDirection))
                {
                    int posX = x * 2 + 1;
                    int posY = y * 2;
                    w.transform.position = new Vector3(posX, 1.7f, posY);
                    Debug.Log("아래 벽 생성 " + min);

                    GameInstance.Instance.housingSystem.BuildWall(x, y, w, buildWallDirection);
                }
            }
            if (min == disD)
            {
                buildWallDirection = HousingSystem.BuildWallDirection.Top;
                if (!GameInstance.Instance.housingSystem.CheckWall(x, y, buildWallDirection))
                {
                    int posX = x * 2 + 1;
                    int posY = y * 2 + 2;
                    w.transform.position = new Vector3(posX, 1.7f, posY);
                    Debug.Log("윗벽 생성 " + min);

                    GameInstance.Instance.housingSystem.BuildWall(x, y, w, buildWallDirection);
                }
            }
*/
        
    }

    public void LoadAsset()
    {
        if (bundle != null)
        {
     /*       foreach (string assetName in bundle.GetAllAssetNames())
            {
                Debug.Log("번들에 포함된 에셋 이름: " + assetName);
            }*/
            // 번들에서 에셋을 로드 (예: GameObject)
            floor = bundle.LoadAsset<GameObject>(floor_url);
            wall = bundle.LoadAsset<GameObject>(wall_url);
            door = bundle.LoadAsset<GameObject>(door_url);
            roof = bundle.LoadAsset<GameObject>(roof_url);
        }
    }

    public void Clear()
    {
        AssetBundle.UnloadAllAssetBundles(bundle);
    }
}
