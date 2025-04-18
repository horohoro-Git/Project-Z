using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;
using System.IO;
using System.Linq;
using UMA.CharacterSystem;
using Newtonsoft.Json;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.EventSystems;
public class AssetLoader : MonoBehaviour
{

    [NonSerialized]
    public GameObject root;
    [NonSerialized]
    public AssetBundle bundle;

   /* [NonSerialized]
    public List<string> assetkeys = new List<string> {
    "floor", "wall", "door", "roof",
    "preview_floor", "preview_wall", "preview_door",
     "flower_orange", "flower_yellow",  "flower_pink", "grasses",
      "tree", "human_male", "log", "wooden_axe", "apple", "heal", "levelup","enemy_zombie",
      "male", "UMA_GLIB", "item_box","item_box_preview","max_hp_up"
    };*/

    [NonSerialized]
    public static Dictionary<int, StringStruct> itemAssetkeys = new Dictionary<int, StringStruct>();

    public static List<string> previewAssetKeys = new List<string>
    {
        "","","", "item_box_preview"
    };

    [NonSerialized]
    public static Dictionary<int, StringStruct> spriteAssetkeys = new Dictionary<int, StringStruct>();

    [NonSerialized]
    public static List<string> enemykeys = new List<string> {
    "enemy_zombie"
    };
    [NonSerialized]
    public static List<string> humankeys = new List<string> {
    "male"
    };
   /* [NonSerialized]
    public static Dictionary<int, RecipeStruct> recipeKeys = new Dictionary<int, RecipeStruct>();
*/
    [NonSerialized]
    public Dictionary<int, LevelStruct> levelData;
    //public List<LevelData> levelData;
    [NonSerialized]
    public Dictionary<int, ItemStruct> items;

    // public List<EnemyStruct> enemies;

    //[NonSerialized]
    //public Dictionary<int, EnemyStruct> enemyParams = new Dictionary<int, EnemyStruct>();
    [NonSerialized]
    public static Dictionary<int, NPCCombatStruct> npcCombatStructs = new Dictionary<int, NPCCombatStruct>();
    List<NPCCombatStruct> nPCCombats = new List<NPCCombatStruct>();
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

    [NonSerialized]
    public Dictionary<int, NPCStruct> npcs = new Dictionary<int, NPCStruct>();
    [NonSerialized]
    public Dictionary<int, AnimatorStruct> animatorKeys = new Dictionary<int, AnimatorStruct>();


    public static Dictionary<int, RecipeStruct> recipes = new Dictionary<int, RecipeStruct>();

    public static Dictionary<string, GameObject> loadedAssets = new Dictionary<string, GameObject>();
    public static Dictionary<string, Sprite> loadedSprites = new Dictionary<string, Sprite>();

    public static Dictionary<string, UMAWardrobeRecipe> loadedRecipes = new Dictionary<string, UMAWardrobeRecipe>();

    public static Dictionary<string, RuntimeAnimatorController> animators = new Dictionary<string, RuntimeAnimatorController>();
    List<string> animatorKey = new List<string> { 
        "NPCAnimator", "ZombieAnimator"
    };
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
  
    Shader Standard { get { if (standard == null) standard = Shader.Find("Standard"); return standard; } }

    string[] tables = new string[13] 
    { "level", "item", "weapon", "consumption", "enemy", "armor", "craft", "ability", "achievement", "recipes", "npc", "npc_event", "animators" };
    Dictionary<string, string> tableContents = new Dictionary<string, string>();
   /* //데이터 테이블 문자열
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
    [NonSerialized]
    public string achievementContent;
    [NonSerialized]
    public string recipesContent;
*/
    //설치물 키
    [NonSerialized]
    public static List<int> furnituresKey = new List<int>
    { 4 };

    int unloadNum = 0;
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

    public async UniTask DownloadAssetBundle()
    {
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
        await www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {

            bundle = DownloadHandlerAssetBundle.GetContent(www);
           /* var dependencies = AssetBundle.GetAllLoadedAssetBundles(bundleName);
            await UniTask.WhenAll(dependencies.Select(d =>
                AssetBundle.LoadFromFileAsync(d).ToUniTask()));
            await UniTask.Yield();*/
          // await bundle;
            if (Caching.IsVersionCached(homeUrl, bundleHash))
            {
                Debug.Log("AssetBundle Cached Successfully");
            }
     
            if (!bundle.isStreamedSceneAssetBundle)
            {
                //데이터 테이블 로드
                //    yield return StartCoroutine(DatatableLoad());
                await DatatableLoad();

                await UniTask.RunOnThreadPool(() => {
                    items = SaveLoadSystem.GetDictionaryData<int, ItemStruct>(tableContents["item"]);
                    weapons = SaveLoadSystem.GetDictionaryData<int, WeaponStruct>(tableContents["weapon"]);
                    armors = SaveLoadSystem.GetDictionaryData<int, ArmorStruct>(tableContents["armor"]);
                    consumptions = SaveLoadSystem.GetDictionaryData<int, ConsumptionStruct>(tableContents["consumption"]);
                    crafts = SaveLoadSystem.GetDictionaryData<int, CraftStruct>(tableContents["craft"]);
                    abilities = SaveLoadSystem.GetDictionaryData<int, AbilityStruct>(tableContents["ability"]);
                    levelData = SaveLoadSystem.GetDictionaryData<int, LevelStruct>(tableContents["level"]);
                    npcs = SaveLoadSystem.GetDictionaryData<int, NPCStruct>(tableContents["npc"]);
                    recipes = SaveLoadSystem.GetDictionaryData<int, RecipeStruct>(tableContents["recipes"]);
                    animatorKeys = SaveLoadSystem.GetDictionaryData<int, AnimatorStruct>(tableContents["animators"]);
                });
               
                foreach (KeyValuePair<int, ItemStruct> keyValuePair in items)
                {
                    string spriteName = keyValuePair.Value.asset_name + "_Sprite";
                    spriteAssetkeys[keyValuePair.Key] = new StringStruct(spriteName);
                    itemAssetkeys[keyValuePair.Key] = new StringStruct(keyValuePair.Value.asset_name);
                }
               
                await LoadAsync<GameObject, StringStruct, string>(itemAssetkeys, loadedAssets);
                await LoadAsync<Sprite, StringStruct, string>(spriteAssetkeys, loadedSprites);
                await LoadAsync<UMAWardrobeRecipe, RecipeStruct, int>(recipes, loadedRecipes);
                await LoadAsync<RuntimeAnimatorController, AnimatorStruct, int>(animatorKeys, animators);
            }
        }
        else
        {
            unloadNum++;
            Debug.Log("Error"); 
        }

        if (unloadNum == 0)
        {
            nPCCombats = SaveLoadSystem.GetListData<NPCCombatStruct>(tableContents["enemy"]); 

            for (int i=0; i< nPCCombats.Count; i++)
            {
                string itemData = nPCCombats[i].drop_item;
                itemData = itemData.Replace("item_index", "\"item_index\"");
                itemData = itemData.Replace("item_chance", "\"item_chance\"");
                NPCCombatStruct enemy = nPCCombats[i];
                enemy.dropStruct = JsonConvert.DeserializeObject<List<DropStruct>>(itemData);
                nPCCombats[i] = enemy;

                npcCombatStructs[enemy.id] = enemy;
                Debug.Log(enemy.id);
            }
            List<AchievementStruct> achievementStructs = SaveLoadSystem.GetListData<AchievementStruct>(tableContents["achievement"]);
            for(int i=0; i<achievementStructs.Count;i++)
            {
                string d = achievementStructs[i].reward;
                d = d.Replace("reward_id", "\"reward_id\"");
                d = d.Replace("reward_num", "\"reward_num\"");
                List<AchievementRewardStruct> achievementRewardStructs = JsonConvert.DeserializeObject<List<AchievementRewardStruct>>(d);
                AchievementStruct achievement = achievementStructs[i];
                achievement.rewardStruct = achievementRewardStructs;
                achievementStructs[i] = achievement;
            }
            AchievementHandler.LoadEvent(achievementStructs);
            List<NPCEventStruct> npceventStructs = SaveLoadSystem.GetListData<NPCEventStruct>(tableContents["npc_event"]);
            NPCEventHandler.LoadEvent(npceventStructs);
            ItemData.ItemDatabaseSetup();
            assetLoadSuccessful = true;
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
                    go = Instantiate(loadedAssets[AssetLoader.itemAssetkeys[assetID].Name], root.transform);
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

    async UniTask DatatableLoad()
    {
        for (int i = 0; i < tables.Length; i++)
        {
            AssetBundleRequest request = bundle.LoadAssetAsync<TextAsset>(tables[i]);
            //  yield return request;
            await request;

            if (request != null)
            {
                TextAsset itemTextAsset = (TextAsset)request.asset;
                tableContents.Add(tables[i], itemTextAsset.text);
            }
        }
    }

    async UniTask LoadAsync<T, K, V>(Dictionary<int, K> keyValues, Dictionary<string, T> outputs) where K : struct, ITableID<V>
    {
        foreach (KeyValuePair<int, K> keyValue in keyValues)
        {
            if (bundle.Contains(keyValue.Value.Name))
            {
                AssetBundleRequest assetRequest = bundle.LoadAssetAsync<T>(keyValue.Value.Name);
                await assetRequest;
                if (assetRequest != null)
                {
                    if (assetRequest.asset is T castedAsset)
                    {
                        outputs[keyValue.Value.Name] = castedAsset;
                        Debug.Log(keyValue + " loaded");
                    }
                }
                else
                {
                    Debug.Log(keyValue + " loaded fail");
                    unloadNum++;
                }
            }
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
      
        foreach(KeyValuePair<int, ItemStruct> item in items) item.Value.Clear();
       
        loadedAssets.Clear();


        loadedSprites.Clear();
        loadedSprites = null;
        loadedAssets = null;

        if(assetLoadSuccessful) bundle.Unload(true);
    }

}
