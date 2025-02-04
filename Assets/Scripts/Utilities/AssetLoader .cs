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
    public string preFloor_url = "Assets/Edited/Floor/PreloadFloor.prefab";
    public string preWall_url = "Assets/Edited/wall/PreloadWall.prefab";
    public string preDoor_url = "Assets/Edited/wall/PreloadDoor.prefab";
    public string possibleMat_url = "Assets/Edited/PreLoadMaterial_Possible.mat";
    public string impossibleMat_url = "Assets/Edited/PreLoadMaterial_Impossible.mat";
    [NonSerialized]
    public GameObject floor;
    [NonSerialized]
    public GameObject door;
    [NonSerialized]
    public GameObject wall;
    [NonSerialized]
    public GameObject roof;
    [NonSerialized]
    public GameObject preFloor;
    [NonSerialized]
    public GameObject preWall;
    [NonSerialized]
    public GameObject preDoor;
    [NonSerialized]
    public Material possibleMat;
    [NonSerialized]
    public Material impossibleMat;
    [NonSerialized]
    public GameObject preLoadedObject;


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

    public void PreLoadFloor(int x, int y)
    {
        if (preLoadedObject != null) Destroy(preLoadedObject);
        if (!GameInstance.Instance.housingSystem.CheckFloor(x, y))
        {
            preLoadedObject = Instantiate(preFloor, root.transform);
            preLoadedObject.transform.position = new Vector3(x * 2 + 1, 0.1f, y * 2 + 1);
        }
        else
        {
            preLoadedObject = Instantiate(preFloor, root.transform);
            preLoadedObject.GetComponent<MeshRenderer>().material.color = Color.red;
            preLoadedObject.transform.position = new Vector3(x * 2 + 1, 0.1f, y * 2 + 1);
        }
    }
    public void PreLoadWall(int x, int y, bool wall)
    {
        if (preLoadedObject != null) Destroy(preLoadedObject);
        preLoadedObject = Instantiate(floor, root.transform);
        preLoadedObject.transform.position = new Vector3(x * 2 + 1, 0.1f, y * 2 + 1);
    }

    public void LoadFloor(int x, int y, bool forcedBuild = false)
    {
        if (!GameInstance.Instance.housingSystem.CheckFloor(x, y) || forcedBuild)
        {
            if (preLoadedObject != null) Destroy(preLoadedObject);
            GameObject f = Instantiate(floor, root.transform);
            f.transform.position = new Vector3(x * 2 + 1, 0.1f, y * 2 + 1);
            floor_List.Add(f);
            GameInstance.Instance.housingSystem.BuildFloor(x, y,f);
        }
    }

    public void LoadWall(HousingSystem.BuildWallDirection buildWallDirection, int x, int y, bool justWall = true, bool forecedBuild = false)
    {
        if (GameInstance.Instance.housingSystem.CheckFloor(x, y) || forecedBuild) // 바닥 확인
        {
            if (!GameInstance.Instance.housingSystem.CheckWall(x, y, buildWallDirection) || forecedBuild)
            {
                if (preLoadedObject != null) Destroy(preLoadedObject);
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
            preFloor = bundle.LoadAsset<GameObject>(preFloor_url);
            preWall = bundle.LoadAsset<GameObject>(preWall_url);
            preDoor = bundle.LoadAsset<GameObject>(preDoor_url);
            possibleMat = bundle.LoadAsset<Material>(possibleMat_url);
            impossibleMat = bundle.LoadAsset<Material>(impossibleMat_url);

        }
    }

    public void Clear()
    {
        AssetBundle.UnloadAllAssetBundles(bundle);
    }
}
