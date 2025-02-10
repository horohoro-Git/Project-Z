using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;
using UnityEngine.InputSystem.HID;
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
    public string flowerOrange_url = "Assets/Edited/flower/flower_orange.prefab";
    public string flowerYellow_url = "Assets/Edited/flower/flower_yellow.prefab";
    public string flowerPink_url = "Assets/Edited/flower/flower_pink.prefab";
    public string grasses_url = "Assets/Edited/grass/grasses.prefab";
    public string tree03_url = "Assets/Edited/Tree/Tree_03.prefab";
    public string human_url = "Assets/Edited/Player/Model/human_male.prefab";


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
    [NonSerialized]
    public GameObject flowerOrange;
    [NonSerialized]
    public GameObject flowerYellow;
    [NonSerialized]
    public GameObject flowerPink;
    [NonSerialized]
    public GameObject grasses;
    [NonSerialized]
    public GameObject tree03; 
    [NonSerialized]
    public GameObject human;
  
    int preLoadX = -100;
    int preLoadY = -100;
    BuildWallDirection buildDirection = BuildWallDirection.None;
    bool isWall = false;

    List<GameObject> floor_List = new List<GameObject>();


    [NonSerialized]
    public bool assetLoadSuccessful;
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
            assetLoadSuccessful = true;
        }
        else
        {
            Debug.LogError("다운로드 실패: " + www.error);
        }

    }
    public bool CheckAssetLoaded()
    {
        if (!assetLoadSuccessful)
        {
            Debug.Log("에셋이 로드되지 않음");
            return false;
        }
    
        return true;
    }
    public void PreLoadFloor(int x, int y)
    {
        if (CheckAssetLoaded())
        {
            if (x != preLoadX || y != preLoadY)
            {
                if (preLoadedObject != null) Destroy(preLoadedObject);
                preLoadedObject = Instantiate(preFloor, root.transform);
                preLoadX = x; preLoadY = y; 
            }
            if (preLoadedObject)
            {
                preLoadedObject.transform.position = new Vector3(x * 2 + 1, 0.11f, y * 2 + 1);


                if (GameInstance.Instance.editMode == EditMode.DestroyMode)
                {
                    if (GameInstance.Instance.housingSystem.CheckFloor(x, y)) preLoadedObject.GetComponent<MeshRenderer>().material.color = Color.yellow;
                    else preLoadedObject.GetComponent<MeshRenderer>().material.color = Color.red;
                }
                else
                {
                    if (GameInstance.Instance.housingSystem.CheckFloor(x, y)) preLoadedObject.GetComponent<MeshRenderer>().material.color = Color.red;
                }
            }
         
        }
    }
    public void RemovePreloadObject()
    {
        if (preLoadedObject != null) Destroy(preLoadedObject);
    }
    public void PreLoadWall(Vector3 hit, int x, int y, bool wall)
    {
        if (CheckAssetLoaded())
        {
            BuildWallDirection buildWallDirection = GameInstance.Instance.housingSystem.GetWallDirection(hit, x, y);
            if (x != preLoadX || y != preLoadY || buildDirection != buildWallDirection || isWall != wall)
            {
                if (preLoadedObject != null) Destroy(preLoadedObject);
                if (isWall) preLoadedObject = Instantiate(preWall, root.transform);
                else preLoadedObject = Instantiate(preDoor, root.transform);
                preLoadX = x; preLoadY = y; buildDirection = buildWallDirection; isWall = wall;
            }
            if (preLoadedObject)
            {
                int offsetX = 0;
                int offsetY = 0;
                int offsetZ = 0;
                int locX = 0;
                int locY = 0;
                BuildDirectionWithOffset buildDirectionWithOffset = Utility.GetWallDirectionWithOffset(buildWallDirection, x, y);

                offsetX = buildDirectionWithOffset.offsetX;
                offsetY = buildDirectionWithOffset.offsetY;
                offsetZ = buildDirectionWithOffset.offsetZ;
                locX = buildDirectionWithOffset.locX;
                locY = buildDirectionWithOffset.locY;

                preLoadedObject.transform.position = new Vector3(locX, 1.7f, locY);
                if(offsetZ == 0) preLoadedObject.transform.rotation = Quaternion.Euler(-90, -90, 0);
                else preLoadedObject.transform.rotation = Quaternion.Euler(-90, 0, 0);

                if (GameInstance.Instance.editMode == EditMode.DestroyMode)
                {
                    preLoadedObject.GetComponent<MeshRenderer>().material.color = Color.red;

                    if (GameInstance.Instance.housingSystem.CheckWall(x, y, buildWallDirection)) preLoadedObject.GetComponent<MeshRenderer>().material.color = Color.yellow;

                    if (!(GameInstance.Instance.housingSystem.CheckFloor(x, y) || GameInstance.Instance.housingSystem.CheckFloor(x + offsetX, y + offsetY))) preLoadedObject.GetComponent<MeshRenderer>().material.color = Color.yellow;
                }
                else
                {
                    if (GameInstance.Instance.housingSystem.CheckWall(x, y, buildWallDirection)) preLoadedObject.GetComponent<MeshRenderer>().material.color = Color.red;

                    if (!(GameInstance.Instance.housingSystem.CheckFloor(x, y) || GameInstance.Instance.housingSystem.CheckFloor(x + offsetX, y + offsetY))) preLoadedObject.GetComponent<MeshRenderer>().material.color = Color.red;
                }
             
            }
        }
    }

    public void LoadFloor(int x, int y, bool forcedBuild = false)
    {
        if (CheckAssetLoaded()) 
        {
            if (!GameInstance.Instance.housingSystem.CheckFloor(x, y) || forcedBuild)
            {
                if (preLoadedObject != null) Destroy(preLoadedObject);
                GameObject f = Instantiate(floor, root.transform);
                f.transform.position = new Vector3(x * 2 + 1, 0.1f, y * 2 + 1);
                floor_List.Add(f);
                GameInstance.Instance.housingSystem.BuildFloor(x, y, f);
            }
        }
    }

    public void LoadWall(BuildWallDirection buildWallDirection, int x, int y, bool justWall = true, bool forecedBuild = false)
    {
        if (CheckAssetLoaded())
        {
            int offsetX = 0;
            int offsetY = 0;
            int offsetZ = 0;
            int locX = 0;
            int locY = 0;
            BuildDirectionWithOffset buildDirectionWithOffset = Utility.GetWallDirectionWithOffset(buildWallDirection, x,y);
            
            offsetX = buildDirectionWithOffset.offsetX;
            offsetY = buildDirectionWithOffset.offsetY;
            offsetZ = buildDirectionWithOffset.offsetZ;
            locX = buildDirectionWithOffset.locX;
            locY = buildDirectionWithOffset.locY;

            if(GameInstance.Instance.housingSystem.CheckFloor(x, y) || GameInstance.Instance.housingSystem.CheckFloor(x + offsetX, y + offsetY) || forecedBuild)
            {
                if (!GameInstance.Instance.housingSystem.CheckWall(x, y, buildWallDirection) || forecedBuild)
                {
                    if (preLoadedObject != null) Destroy(preLoadedObject);
                    GameObject go;
                    if (justWall) go = Instantiate(wall, root.transform);
                    else go = Instantiate(door, root.transform);

                    Wall w = go.GetComponent<Wall>();

                    w.transform.position = new Vector3(locX, 1.7f, locY);
                    if (offsetZ == 0)
                    {
                        if(w.isDoor) w.GetComponentInChildren<Door>().isHorizontal = true;
                    
                        w.transform.rotation = Quaternion.Euler(-90, -90, 0);
                    }
                    else
                    {
                        if (w.isDoor) w.GetComponentInChildren<Door>().isHorizontal = false;
                        w.transform.rotation = Quaternion.Euler(-90, 0, 0);
                    }


                    GameInstance.Instance.housingSystem.BuildWall(x, y, w, buildWallDirection, justWall);
                }
            }
        }
        
    }

    public void LoadAsset()
    {
        if (bundle != null)
        {
            floor = bundle.LoadAsset<GameObject>(floor_url);
            wall = bundle.LoadAsset<GameObject>(wall_url);
            door = bundle.LoadAsset<GameObject>(door_url);
            roof = bundle.LoadAsset<GameObject>(roof_url);
            preFloor = bundle.LoadAsset<GameObject>(preFloor_url);
            preWall = bundle.LoadAsset<GameObject>(preWall_url);
            preDoor = bundle.LoadAsset<GameObject>(preDoor_url);
            possibleMat = bundle.LoadAsset<Material>(possibleMat_url);
            impossibleMat = bundle.LoadAsset<Material>(impossibleMat_url);
            flowerOrange = bundle.LoadAsset<GameObject>(flowerOrange_url);
            flowerYellow = bundle.LoadAsset<GameObject>(flowerYellow_url);
            flowerPink = bundle.LoadAsset<GameObject>(flowerPink_url);
            grasses = bundle.LoadAsset<GameObject>(grasses_url);
            tree03 = bundle.LoadAsset<GameObject>(tree03_url);
            human = bundle.LoadAsset<GameObject>(human_url);
        }
    }

    public void Clear()
    {
        AssetBundle.UnloadAllAssetBundles(bundle);
    }
}
