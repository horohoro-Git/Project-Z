using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;
using UnityEngine.ResourceManagement.ResourceLocations;
using System.Net;
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
    public List<string> assetkeys = new List<string> {
    "floor", "wall", "door", "roof",
    "preview_floor", "preview_wall", "preview_door",
     "flower_orange", "flower_yellow",  "flower_pink", "greasses",
      "tree", "human_male", "log", "wooden Axe", "apple", "heal", "levelup"
    };

    [NonSerialized]
    public static List<string> itemAssetkeys = new List<string> {
    "log", "wooden Axe", "apple"
    };

    [NonSerialized]
    public static List<string> spriteAssetkeys = new List<string> {
    "log_Sprite", "axe_Sprite", "apple_Sprite"
    };

    [NonSerialized]
    public List<LevelData> levelData;
    [NonSerialized]
    public List<ItemStruct> items;

    [NonSerialized]
    public Dictionary<int, WeaponStruct> weapons = new Dictionary<int, WeaponStruct>(); //List<WeaponStruct> weapons;

    [NonSerialized]
    public Dictionary<int, ConsumptionStruct> consumptions = new Dictionary<int, ConsumptionStruct>(); //List<ConsumptionStruct> consumptions;

    public Dictionary<string, GameObject> loadedAssets = new Dictionary<string, GameObject>();
    public Dictionary<string, Sprite> loadedSprites = new Dictionary<string, Sprite>();

    [SerializeField]
    Shader holoShader;
  
    int preLoadX = -100;
    int preLoadY = -100;
    BuildWallDirection buildDirection = BuildWallDirection.None;
    bool isWall = false;

    List<GameObject> floor_List = new List<GameObject>();
    [NonSerialized]
    public GameObject preLoadedObject;

    [NonSerialized]
    public bool assetLoadSuccessful;

    public GameObject lastSelectedMaterial;
    Color lastSelectedColor;
    Shader standard;
    AsyncOperationHandle<IList<GameObject>> handle;
    AsyncOperationHandle<IList<Sprite>> spritehandle;

  
    Shader Standard { get { if (standard == null) standard = Shader.Find("Standard"); return standard; } }

    [NonSerialized]
    public string levelContent;
    [NonSerialized] 
    public string itemContent;
    [NonSerialized]
    public string weaponContent;
    [NonSerialized]
    public string consumptionContent;

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
        //데이터 테이블
        AsyncOperationHandle<TextAsset> lvlHandle = Addressables.LoadAssetAsync<TextAsset>("level");
        yield return lvlHandle;
        if(lvlHandle.Status == AsyncOperationStatus.Succeeded)
        {
            TextAsset textAsset = lvlHandle.Result;
            levelContent = textAsset.text;
        }
        AsyncOperationHandle<TextAsset> itemHandle = Addressables.LoadAssetAsync<TextAsset>("item");
        yield return itemHandle;
        if (itemHandle.Status == AsyncOperationStatus.Succeeded)
        {
            TextAsset textAsset = itemHandle.Result;
            itemContent = textAsset.text;
        }
        AsyncOperationHandle<TextAsset> weaponHandle = Addressables.LoadAssetAsync<TextAsset>("weapon");
        yield return weaponHandle;
        if (weaponHandle.Status == AsyncOperationStatus.Succeeded)
        {
            TextAsset textAsset = weaponHandle.Result;
            weaponContent = textAsset.text;
        }
        AsyncOperationHandle<TextAsset> consumptionHandle = Addressables.LoadAssetAsync<TextAsset>("consumption");
        yield return consumptionHandle;
        if (consumptionHandle.Status == AsyncOperationStatus.Succeeded)
        {
            TextAsset textAsset = consumptionHandle.Result;
            consumptionContent = textAsset.text;
        }






        int loadNum = 0;
        Debug.Log("Loading");
        //호스팅 서버에 Remote Load Path로 연결 후

        handle = Addressables.LoadAssetsAsync<GameObject>(
             assetkeys, 
             asset => { Debug.Log("Loaded: " + asset.name); },
             Addressables.MergeMode.Union,
             false); // false: 모든 에셋이 로드될 때까지 기다림

        yield return handle;
     
        if(handle.Status == AsyncOperationStatus.Succeeded)
        {
            foreach (var asset in handle.Result)
            {
                Debug.Log("Loaded " + asset.name);
                loadedAssets[asset.name] = asset;   //에셋을 사용할 때에는 해당 에셋의 이름으로 호출
            }
            loadNum++;
        }
        else
        {
            Debug.Log("error");
        }


        spritehandle = Addressables.LoadAssetsAsync<Sprite>(spriteAssetkeys,
            spriteAsset => { Debug.Log("Loaded: " + spriteAsset.name); },
            Addressables.MergeMode.Union,
            false);

        yield return spritehandle;

        if (spritehandle.Status == AsyncOperationStatus.Succeeded)
        {
            foreach (var asset in spritehandle.Result)
            {
                Debug.Log(asset.name);
                loadedSprites[asset.name] = asset;
            }
            loadNum++;
        }
        else
        {
            Debug.Log("error");
        }

        if (loadNum == 2)
        {
            assetLoadSuccessful = true;
        }
        levelData = SaveLoadSystem.GetLevelData(levelContent);
        items = SaveLoadSystem.GetItemData(itemContent);
        List<WeaponStruct> weaponTable = SaveLoadSystem.GetWeaponData(weaponContent);
        List<ConsumptionStruct> consumptionTable = SaveLoadSystem.GetConsumptionData(consumptionContent);

        Debug.Log(weaponTable[0].attack_damage);
        for(int i = 0; i < weaponTable.Count; i++) weapons[weaponTable[i].item_index] = weaponTable[i];
        for(int i = 0; i < consumptionTable.Count; i++) consumptions[consumptionTable[i].item_index] = consumptionTable[i];
    

        ItemData.ItemDatabaseSetup();

    }

    public void ClearAsset()
    {
        Addressables.Release(handle);
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
    public void RemovePreview()
    {
        if (preLoadedObject != null) Destroy(preLoadedObject);
        preLoadedObject = null;
    }
    public void PreLoadFloor(int x, int y)
    {
        if (CheckAssetLoaded())
        {
            if (x != preLoadX || y != preLoadY)
            {
                if (preLoadedObject != null) Destroy(preLoadedObject);
                preLoadedObject = Instantiate(loadedAssets[LoadURL.PreviewFloor], root.transform);
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

    public void PreLoadWall(Vector3 hit, int x, int y, bool wall)
    {
        if (CheckAssetLoaded())
        {
            BuildWallDirection buildWallDirection = GameInstance.Instance.housingSystem.GetWallDirection(hit, x, y);
            if (x != preLoadX || y != preLoadY || buildDirection != buildWallDirection || isWall != wall)
            {
                if (preLoadedObject != null) Destroy(preLoadedObject);
                if (isWall) preLoadedObject = Instantiate(loadedAssets[LoadURL.PreviewWall], root.transform);
                else preLoadedObject = Instantiate(loadedAssets[LoadURL.PreviewDoor], root.transform);
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

    public void PreviewDestoryObject(BuildWallDirection buildWallDirection, RaycastHit hits, int x, int y)
    {
        if (CheckAssetLoaded())
        {
          //  if (preLoadX != x && preLoadY != y)
            {
                //  preLoadX = x;
                // preLoadY = y;

               
                if (x != preLoadX || y != preLoadY || buildDirection != buildWallDirection)
                {
                    if (lastSelectedMaterial != null)
                    {
                        IBuildMaterials buildMaterials = lastSelectedMaterial.GetComponent<IBuildMaterials>();

                        Renderer render = buildMaterials.renderer;
                        render.material.shader = Standard;
                        render.material.color = lastSelectedColor;

                    }


                    preLoadX = x;
                    preLoadY = y;
                    buildDirection = buildWallDirection;

                    int xx = x + 25;
                    int yy = y + 25;
                    int zz = 0;
                    switch (buildWallDirection)
                    { 
                        case BuildWallDirection.Left:
                            break;
                        case BuildWallDirection.Right:
                            xx++;
                            break;
                        case BuildWallDirection.Top:
                            yy++;
                            zz++;
                            break;
                        case BuildWallDirection.Bottom:
                            zz++;
                            break;
                    }

                    if (xx < 100 && yy < 100 && GameInstance.Instance.housingSystem.GetWall(xx,yy,zz) != null)
                    {
                        IBuildMaterials buildMaterials = GameInstance.Instance.housingSystem.GetWall(xx, yy, zz).GetComponent<IBuildMaterials>();

                        lastSelectedMaterial = GameInstance.Instance.housingSystem.GetWall(xx, yy, zz).gameObject;
                        Renderer render = buildMaterials.renderer;
                        lastSelectedColor = render.material.color;
                        render.material.color = Color.yellow;
                        render.material.shader = holoShader;
                        return;
                    }
                    xx = x + 25;
                    yy = y + 25;
                    if (GameInstance.Instance.housingSystem.GetWall(xx, yy, 0) != null && xx - 1 >= 0 && GameInstance.Instance.housingSystem.GetFloor(xx - 1, yy) == null) return;
                    else if (GameInstance.Instance.housingSystem.GetWall(xx, yy, 1) != null && yy - 1 >= 0 && GameInstance.Instance.housingSystem.GetFloor(xx, yy - 1) == null) return;
                    else if (xx + 1 < 100 && GameInstance.Instance.housingSystem.GetWall(xx + 1, yy, 0) != null && GameInstance.Instance.housingSystem.GetFloor(xx + 1, yy) == null) return;
                    else if (yy + 1 < 100 && GameInstance.Instance.housingSystem.GetWall(xx, yy + 1, 1) != null && GameInstance.Instance.housingSystem.GetFloor(xx, yy + 1) == null) return;

                    GameObject floor = GameInstance.Instance.housingSystem.GetFloor(xx, yy);
                    if(floor == null) return;
                    Renderer renderer = floor.GetComponent<IBuildMaterials>().renderer;
                    lastSelectedColor = renderer.material.color;
                    lastSelectedMaterial = floor;
                    renderer.material.color = Color.yellow;
                    renderer.material.shader = holoShader;
                }


                   
            }

          /*  Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                IBuildMaterials buildMaterials = hit.collider.GetComponent<IBuildMaterials>();  
                if (buildMaterials != null)
                {
                    if(lastSelectedMaterial != hit.collider.gameObject)
                    {
                        if (lastSelectedMaterial != null)
                        {
                            Renderer render = lastSelectedMaterial.GetComponent<Renderer>();
                            render.material.shader = Standard;
                            render.material.color = lastSelectedColor;
                        }

                        switch (buildMaterials.structureState)
                        {

                            case StructureState.None:
                                break;
                            case StructureState.Floor:
                                GameObject floor = GameInstance.Instance.housingSystem.GetFloor(x + 25, y + 25).gameObject;
                                if (floor)
                                {
                                    Renderer renderFloor = floor.GetComponent<Renderer>();
                                    lastSelectedMaterial = floor;
                                    lastSelectedColor = renderFloor.material.color;
                                    renderFloor.material.color = Color.yellow;
                                    renderFloor.material.shader = holoShader;
                                }
                                break;
                        *//*    case StructureState.Wall:
                                GameObject wall = GameInstance.Instance.housingSystem.GetWall(x + 25, y + 25, (int)buildWallDirection).gameObject;
                                if (wall)
                                {
                                    Renderer renderWall = wall.GetComponent<Renderer>();
                                    lastSelectedMaterial = wall;
                                    lastSelectedColor = renderWall.material.color;
                                    renderWall.material.color = Color.yellow;
                                    renderWall.material.shader = holoShader;
                                }
                                break;*//*
                        }
                    }
                }
                else
                {
                    if (lastSelectedMaterial != null)
                    {
                        Renderer render = lastSelectedMaterial.GetComponent<Renderer>();
                        render.material.shader = Standard;
                        render.material.color = lastSelectedColor;
                        lastSelectedMaterial = null;

                    }
                }
            }
            else
            {
                if(lastSelectedColor != null)
                {
                    Renderer render = lastSelectedMaterial.GetComponent<Renderer>();
                    render.material.shader = Standard;
                    render.material.color = lastSelectedColor;
                    lastSelectedMaterial = null;

                }
            }*/
          /*  if(R)
            Renderer renderer = hit.collider.gameObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.shader = holoShader;
            }*/
        }
    }

    public void LoadFloor(int x, int y, bool forcedBuild = false)
    {
        if (CheckAssetLoaded()) 
        {
            if (!GameInstance.Instance.housingSystem.CheckFloor(x, y) || forcedBuild)
            {
                if (preLoadedObject != null) Destroy(preLoadedObject);
                GameObject f = Instantiate(loadedAssets[LoadURL.Floor], root.transform);
                f.transform.position = new Vector3(x * 2 + 1, 0.1f, y * 2 + 1);
                floor_List.Add(f);
                GameInstance.Instance.housingSystem.BuildFloor(x, y, f, forcedBuild);
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
                    if (justWall) go = Instantiate(loadedAssets[LoadURL.Wall], root.transform);
                    else go = Instantiate(loadedAssets[LoadURL.Door], root.transform);

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


                    GameInstance.Instance.housingSystem.BuildWall(x, y, w, buildWallDirection, justWall, forecedBuild);
                }
            }
        }
        
    }

    public void LoadAsset()
    {
        if (bundle != null)
        {
       //     floor = bundle.LoadAsset<GameObject>(floor_url);
       //     wall = bundle.LoadAsset<GameObject>(wall_url);
         //   door = bundle.LoadAsset<GameObject>(door_url);
         //   roof = bundle.LoadAsset<GameObject>(roof_url);
      //      preFloor = bundle.LoadAsset<GameObject>(preFloor_url);
        //    preWall = bundle.LoadAsset<GameObject>(preWall_url);
       //     preDoor = bundle.LoadAsset<GameObject>(preDoor_url);
          //  possibleMat = bundle.LoadAsset<Material>(possibleMat_url);
         //   impossibleMat = bundle.LoadAsset<Material>(impossibleMat_url);
         //   flowerOrange = bundle.LoadAsset<GameObject>(flowerOrange_url);
        //    flowerYellow = bundle.LoadAsset<GameObject>(flowerYellow_url);
           // flowerPink = bundle.LoadAsset<GameObject>(flowerPink_url);
      //      grasses = bundle.LoadAsset<GameObject>(grasses_url);
       //     tree03 = bundle.LoadAsset<GameObject>(tree03_url);
     //       human = bundle.LoadAsset<GameObject>(human_url);
        }
    }

    public void Clear()
    {
        AssetBundle.UnloadAllAssetBundles(bundle);
    }
}
