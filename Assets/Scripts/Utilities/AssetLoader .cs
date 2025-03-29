using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;
using System.IO;
using System.Linq;
using UMA.CharacterSystem;
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
     "flower_orange", "flower_yellow",  "flower_pink", "grasses",
      "tree", "human_male", "log", "wooden Axe", "apple", "heal", "levelup","enemy_zombie",
      "male", "UMA_GLIB", "cardboardbox","cardboardbox_preview","MaxHPUp"
    };

    [NonSerialized]
    public static List<string> itemAssetkeys = new List<string> {
    "log", "wooden Axe", "apple", "cardboardbox", "","","","","","","MaxHPUp"
    };

    public static List<string> previewAssetKeys = new List<string>
    {
        "","","", "cardboardbox_preview"
    };

    [NonSerialized]
    public static List<string> spriteAssetkeys = new List<string> {
        "log_Sprite", "axe_Sprite", "apple_Sprite", "cardboardbox_sprite", "HatLeatherM_Sprite", "GambesonM_Sprite",
        "GlovesLinenM_Sprite","KnickerbockerBlackM_Sprite", "SchoesLaceUpBlackM_Sprite",  "SamplePack01_M_Sprite", "MaxHPUp_Sprite"
        
    };
    [NonSerialized]
    public static List<string> enemykeys = new List<string> {
    "enemy_zombie"
    };
    [NonSerialized]
    public static List<string> humankeys = new List<string> {
    "male"
    };
    [NonSerialized]
    public static List<string> recipeKeys = new List<string> {
         "HatLeatherM_Recipe", "GambesonM_Recipe", "GlovesLinenM_Recipe", "KnickerbockerBlackM_Recipe", "SchoesLaceUpBlackM_Recipe",
         "SamplePack01_M_Recipe"
    };
    

    [NonSerialized]
    public List<LevelData> levelData;
    [NonSerialized]
    public List<ItemStruct> items;
    
    public List<EnemyStruct> enemies;

    [NonSerialized]
    public Dictionary<int, WeaponStruct> weapons = new Dictionary<int, WeaponStruct>(); //List<WeaponStruct> weapons;

    [NonSerialized]
    public Dictionary<int, ConsumptionStruct> consumptions = new Dictionary<int, ConsumptionStruct>(); //List<ConsumptionStruct> consumptions;

    [NonSerialized]
    public Dictionary<int, ArmorStruct> armors = new Dictionary<int, ArmorStruct>();

    [NonSerialized]
    public Dictionary<int, CraftStruct> crafts = new Dictionary<int, CraftStruct>();
    [NonSerialized]
    public Dictionary<int, AbilityStruct> abilities = new Dictionary<int, AbilityStruct>();

    public Dictionary<string, GameObject> loadedAssets = new Dictionary<string, GameObject>();
    public Dictionary<string, Sprite> loadedSprites = new Dictionary<string, Sprite>();

    public Dictionary<string, UMAWardrobeRecipe> loadedRecipes = new Dictionary<string, UMAWardrobeRecipe>();

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

    Vector3 hitPoint = new Vector3(-100,-100,-100);
//    AsyncOperationHandle<IList<GameObject>> handle;
//    AsyncOperationHandle<IList<Sprite>> spritehandle;

  
    Shader Standard { get { if (standard == null) standard = Shader.Find("Standard"); return standard; } }

    //데이터 테이블 문자열
    [NonSerialized]
    public string levelContent;
    [NonSerialized] 
    public string itemContent;
    [NonSerialized]
    public string weaponContent;
    [NonSerialized]
    public string consumptionContent;
    [NonSerialized]
    public string enemyContent;
    [NonSerialized]
    public string armorContent;
    [NonSerialized]
    public string craftContent;
    [NonSerialized]
    public string abilityContent;

    //설치물 키
    [NonSerialized]
    public static List<int> furnituresKey = new List<int>
    { 4 };

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
      //  Caching.ClearCache();
        int unloadNum = 0;

        string homeUrl = Path.Combine(SaveLoadSystem.LoadServerURL(), "home");
        Hash128 bundleHash = SaveLoadSystem.ComputeHash128(System.Text.Encoding.UTF8.GetBytes(homeUrl));

        if (Caching.IsVersionCached(homeUrl, bundleHash))
        {
            Debug.Log("Asset Found");
        }
        else
        {
            Debug.Log("Asset Not Found");
          
        }
        UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(homeUrl, bundleHash, 0);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {

            bundle = DownloadHandlerAssetBundle.GetContent(www);
            yield return bundle;

            if (Caching.IsVersionCached(homeUrl, bundleHash))
            {
                Debug.Log("AssetBundle Cached Successfully");
            }


            if (!bundle.isStreamedSceneAssetBundle)
            {
                //데이터 테이블 로드
                AssetBundleRequest levelrequest = bundle.LoadAssetAsync<TextAsset>("level");
                yield return levelrequest;

                if (levelrequest != null)
                {
                    TextAsset levelTextAsset = (TextAsset)levelrequest.asset;
                    levelContent = levelTextAsset.text;
                }

                AssetBundleRequest itemrequest = bundle.LoadAssetAsync<TextAsset>("item");
                yield return itemrequest;

                if (itemrequest != null)
                {
                    TextAsset itemTextAsset = (TextAsset)itemrequest.asset;
                    itemContent = itemTextAsset.text;
                }
                AssetBundleRequest weaponrequest = bundle.LoadAssetAsync<TextAsset>("weapon");

                yield return weaponrequest;

                if (weaponrequest != null)
                {
                    TextAsset weaponTextAsset = (TextAsset)weaponrequest.asset;
                    weaponContent = weaponTextAsset.text;
                }
                AssetBundleRequest consumptionrequest = bundle.LoadAssetAsync<TextAsset>("consumption");

                yield return consumptionrequest;

                if (consumptionrequest != null)
                {
                    TextAsset consumptionTextAsset = (TextAsset)consumptionrequest.asset;
                    consumptionContent = consumptionTextAsset.text;
                }

                AssetBundleRequest enemyrequest = bundle.LoadAssetAsync<TextAsset>("enemy");
                yield return enemyrequest;
                if (enemyrequest != null)
                {
                    TextAsset enemyTextAsset = (TextAsset)enemyrequest.asset;
                    enemyContent = enemyTextAsset.text;
                }

                AssetBundleRequest armorrequest = bundle.LoadAssetAsync<TextAsset>("armor");
                yield return armorrequest;
                if (armorrequest != null)
                {
                    TextAsset armorTextAsset = (TextAsset)armorrequest.asset;
                    armorContent = armorTextAsset.text;
                }

                AssetBundleRequest craftrequest = bundle.LoadAssetAsync<TextAsset>("craft");
                yield return craftrequest;

                if (craftrequest != null)
                {
                    TextAsset craftTextAsset = (TextAsset)craftrequest.asset;
                    craftContent = craftTextAsset.text;
                }

                AssetBundleRequest abilityrequest = bundle.LoadAssetAsync<TextAsset>("ability");
                yield return abilityrequest;

                if (abilityrequest != null)
                {
                    TextAsset abilityTextAsset = (TextAsset)abilityrequest.asset;
                    abilityContent = abilityTextAsset.text;
                }

                //프리팹 에셋 로드
                foreach (string key in assetkeys)
                {
                    GameObject asset = bundle.LoadAsset<GameObject>(key);
                    if(asset != null)
                    {
                        loadedAssets[key] = asset;
                        Debug.Log(key + " loaded");
                    }
                    else
                    {
                        Debug.Log(key + " loaded fail");
                        unloadNum++;
                    }
                }

                //이미지 에셋 로드
                foreach (string key in spriteAssetkeys)
                {
                    Sprite asset = bundle.LoadAsset<Sprite>(key);
                    if (asset != null)
                    {
                        loadedSprites[key] = asset;
                        Debug.Log(key + " loaded");
                    }
                    else
                    {
                        Debug.Log(key + " loaded fail");
                        unloadNum++;
                    }
                }

                //레시피 로드
                foreach(string key in recipeKeys)
                {
                    UMAWardrobeRecipe wardrobeRecipe = bundle.LoadAsset<UMAWardrobeRecipe>(key);
                    if (wardrobeRecipe != null)
                    {
                        loadedRecipes[key] = wardrobeRecipe;
                        Debug.Log(key + " loaded");
                    }
                    else
                    {
                        Debug.Log(key + " loaded fail");
                        unloadNum++;
                    }
                }
            }

           
        }
        else
        {
            unloadNum++;
            Debug.Log("Error"); 
        }

        /*    Debug.Log(levelContent);

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
            }*/

        if (unloadNum == 0)
        {

            levelData = SaveLoadSystem.GetLevelData(levelContent);
            items = SaveLoadSystem.GetItemData(itemContent);
            List<WeaponStruct> weaponTable = SaveLoadSystem.GetWeaponData(weaponContent);
            List<ConsumptionStruct> consumptionTable = SaveLoadSystem.GetConsumptionData(consumptionContent);
            enemies = SaveLoadSystem.LoadEnemyData(enemyContent);
            List<ArmorStruct> armorTable = SaveLoadSystem.GetArmorData(armorContent);
            List<CraftStruct> craftStructs = SaveLoadSystem.GetCraftData(craftContent);
            List<AbilityStruct> abilityStructs = SaveLoadSystem.GetAbilityData(abilityContent);

            for (int i = 0; i < weaponTable.Count; i++) weapons[weaponTable[i].item_index] = weaponTable[i];
            for (int i = 0; i < consumptionTable.Count; i++) consumptions[consumptionTable[i].item_index] = consumptionTable[i];
            for (int i = 0; i < armorTable.Count; i++) armors[armorTable[i].item_index] = armorTable[i];
            for(int i = 0; i< craftStructs.Count; i++) crafts[craftStructs[i].index] = craftStructs[i];
            for(int i=0; i< abilityStructs.Count; i++) abilities[abilityStructs[i].index] = abilityStructs[i];
            for(int i=0; i<items.Count; i++)
            {
                Debug.Log(items[i].item_type + " " + items[i].item_index);
            }
            ItemData.ItemDatabaseSetup();
            assetLoadSuccessful = true;
           // GameInstance.Instance.creatableUISystem
        }
    }

    public void ClearAsset()
    {
        //AssetBundle.UnloadAllAssetBundles(true);
        //Addressables.Release(handle);
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
    public void PreLoadFloor(Vector3 point, int x, int y)
    {
        if (CheckAssetLoaded())
        {
            if (x != preLoadX || y != preLoadY || point != hitPoint)
            {
                if (preLoadedObject != null) Destroy(preLoadedObject);
                preLoadedObject = Instantiate(loadedAssets[LoadURL.PreviewFloor], root.transform);
                preLoadX = x; preLoadY = y; hitPoint = point;
            }
            if (preLoadedObject)
            {
                preLoadedObject.transform.position = new Vector3(x * 2 + 1, 0.11f, y * 2 + 1);


                if (GameInstance.Instance.editMode == EditMode.DestroyMode)
                {
                    if (GameInstance.Instance.housingSystem.CheckFloor(x, y) && !GameInstance.Instance.housingSystem.CheckFurniture(x,y)) preLoadedObject.GetComponent<MeshRenderer>().material.color = Color.yellow;
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
            if (x != preLoadX || y != preLoadY || buildDirection != buildWallDirection || isWall != wall || hitPoint != hit)
            {
                if (preLoadedObject != null) Destroy(preLoadedObject);
                if (isWall) preLoadedObject = Instantiate(loadedAssets[LoadURL.PreviewWall], root.transform);
                else preLoadedObject = Instantiate(loadedAssets[LoadURL.PreviewDoor], root.transform);
                preLoadX = x; preLoadY = y; buildDirection = buildWallDirection; isWall = wall;
                hitPoint = hit;
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

    public void PreviewFurniture(Vector3 hit, int x, int y, string assetName)
    {
        if (CheckAssetLoaded())
        {
            BuildWallDirection buildWallDirection = GameInstance.Instance.housingSystem.GetWallDirection(hit, x, y);
            if (x != preLoadX || y != preLoadY || buildDirection != buildWallDirection || hit != hitPoint)
            {
                if (preLoadedObject != null) Destroy(preLoadedObject);
                preLoadedObject = Instantiate(loadedAssets[assetName], root.transform);

                preLoadX = x; preLoadY = y; buildDirection = buildWallDirection;
                hitPoint = hit; 
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
                float xx = 0;
                float yy = 0;
                if (offsetX == -1) xx = 0.5f;
                if (offsetX == 1) xx = -0.5f;
                if (offsetY == -1) yy = 0.5f;
                if (offsetY == 1) yy = -0.5f;
                preLoadedObject.transform.position = new Vector3(locX + xx, 0.75f, locY + yy);
                if (offsetZ == 0) preLoadedObject.transform.rotation = Quaternion.Euler(-90, -90, 0);
                else preLoadedObject.transform.rotation = Quaternion.Euler(-90, 0, 0);

                if (GameInstance.Instance.editMode == EditMode.DestroyMode)
                {
                    preLoadedObject.GetComponent<MeshRenderer>().material.color = Color.red;

                    if (GameInstance.Instance.housingSystem.CheckFurniture(x, y)) preLoadedObject.GetComponent<MeshRenderer>().material.color = Color.yellow;
                }
                else
                {
                    if (!GameInstance.Instance.housingSystem.CheckFloor(x, y) || GameInstance.Instance.housingSystem.CheckFurniture(x,y)) preLoadedObject.GetComponent<MeshRenderer>().material.color = Color.red;
                }

            }
        }
    }

    public void PreviewDestoryObject(RaycastHit hits, int x, int y)
    {
        if (CheckAssetLoaded())
        {
            if (x != preLoadX || y != preLoadY || hits.point != hitPoint)
            {
                if (lastSelectedMaterial != null)
                {
                    IBuildMaterials buildMaterials = lastSelectedMaterial.GetComponent<IBuildMaterials>();

                    Renderer render = buildMaterials.renderer;
                    render.material.shader = Standard;
                    render.material.color = lastSelectedColor;
                }

                hitPoint = hits.point;
                preLoadX = x;
                preLoadY = y;
                buildDirection = BuildWallDirection.None;

                int xx = x + 25;
                int yy = y + 25;

                GameObject furniture = GameInstance.Instance.housingSystem.GetFurniture(xx, yy);
                if (furniture != null)
                {
                    Renderer otherRender = furniture.GetComponent<Renderer>();
                    lastSelectedColor = otherRender.material.color;
                    lastSelectedMaterial = furniture;
                    otherRender.material.color = Color.yellow;
                    otherRender.material.shader = holoShader;
                  
                    return;
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
                   //     Renderer render = lastSelectedMaterial.GetComponent<Renderer>();
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

                    if(GameInstance.Instance.housingSystem.CheckFurniture(x,y))
                    {
                        renderer.material.color = Color.red;
                    }
                    else
                    {
                        renderer.material.color = Color.yellow;
                    }
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
    public void LoadFurniture(BuildWallDirection buildWallDirection, int x, int y, int assetID, bool forecedBuild = false)
    {
        if (CheckAssetLoaded())
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

            HousingSystem housing = GameInstance.Instance.housingSystem;

            if (housing.CheckFloor(x, y) || forecedBuild)
            {
                if (!housing.CheckFurniture(x, y) || forecedBuild)
                {
                    if (preLoadedObject != null) Destroy(preLoadedObject);
                    GameObject go;
                    go = Instantiate(loadedAssets[AssetLoader.itemAssetkeys[assetID]], root.transform);
                    go.GetComponent<InstallableObject>().assetID = assetID;
                    float xx = 0;
                    float yy = 0;
                    if (offsetX == -1) xx = 0.5f;
                    else if (offsetX == 1) xx = -0.5f;
                    if (offsetY== -1) yy = 0.5f;
                    else if (offsetY == 1) yy = -0.5f;
                    go.transform.position = new Vector3(locX + xx, 0.75f, locY + yy);
                    if (offsetZ == 0)
                    {
                        go.transform.rotation = Quaternion.Euler(-90, -90, 0);
                    }
                    else
                    {
                        go.transform.rotation = Quaternion.Euler(-90, 0, 0);
                    }
                    housing.BuilFurniture(x, y, go, buildWallDirection, forecedBuild);
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
        if (GameInstance.Instance.gameManager.glib != null) Destroy(GameInstance.Instance.gameManager.glib);

        GameInstance instance = GameInstance.Instance;
        instance.characterAbilityManager.ClearAbility();
        if (instance.enemySpawner.loaded) SaveLoadSystem.SaveEnemyInfo();
        if (instance.GetPlayers.Count > 0 && instance.GetPlayers[0].loaded) SaveLoadSystem.SavePlayerData(instance.GetPlayers[0].GetPlayer); 
        ItemData.Clear();
        GameInstance.Instance.environmentSpawner.Clear();
        GameInstance.Instance.worldGrids.Clear();

        var keys = loadedAssets.Keys;
        for(int i = 0; i< keys.Count; i++)
        {
            loadedAssets[keys.ElementAt(i)] = null;
        }

        var keyss = loadedSprites.Keys;
        for (int i = 0; i < keyss.Count; i++)
        {
            loadedSprites[keyss.ElementAt(i)] = null;
        }
     
        loadedAssets.Clear();


        loadedSprites.Clear();
        loadedSprites = null;
        loadedAssets = null;

        if(assetLoadSuccessful) bundle.Unload(true);
    }

}
