using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CreatableUISystem : MonoBehaviour, IUIComponent
{
    public GameObject upgradeGO;
    public GameObject tab;
    public GameObject creatableTab;

    public Button floorBtn;
    public Button wallBtn;
    public Button doorBtn;
    public Button furnitureBtn;
    public Button backBtn;
    public Button applyBtn;
    public Button restBtn;

    const string floorStr = "Floor";
    const string wallStr = "Wall";
    const string doorStr = "Door";
    const string furnitureStr = "Furnitures";

    [SerializeField]
    Button editModeButton;
    [SerializeField]
    Image modeImage;

    [SerializeField]
    Sprite plus;
    [SerializeField]    
    Sprite minus;
    [SerializeField]
    TMP_Text labelText;
    [SerializeField]
    RectTransform detailsTab;

    UnityAction floorBtnListener;
    UnityAction wallBtnListener;
    UnityAction doorBtnListener;
    UnityAction furnitureBtnListener;
    UnityAction editBtnListener;
    UnityAction backBtnListener;
    UnityAction applyBtnListener;
    UnityAction resetBtnListener;

    List<InstallableItem> items = new List<InstallableItem>();

    public List<ItemStruct> floors = new List<ItemStruct>();
    public List<ItemStruct> walls = new List<ItemStruct>();
    public List<ItemStruct> doors = new List<ItemStruct>();
    public List<ItemStruct> furnitures = new List<ItemStruct>();

    [SerializeField]
    InstallableItem installableItem;
    // Start is called before the first frame update

    private void Awake()
    {
        GameInstance.Instance.creatableUISystem = this;
    }

    private void OnEnable()
    {
        floorBtnListener = () => ChangeSelectionType(StructureState.Floor);
        wallBtnListener = () => ChangeSelectionType(StructureState.Wall);
        doorBtnListener = () => ChangeSelectionType(StructureState.Door);
        furnitureBtnListener = () => ChangeSelectionType(StructureState.Furniture);
        editBtnListener = () => ChangeMode();
        backBtnListener = () => GameInstance.Instance.housingSystem.Revert();
        applyBtnListener = () => GameInstance.Instance.housingSystem.Apply();
        resetBtnListener = () => GameInstance.Instance.housingSystem.ResetMaterials();

        floorBtn.onClick.AddListener(floorBtnListener);
        wallBtn.onClick.AddListener(wallBtnListener);
        doorBtn.onClick.AddListener(doorBtnListener);
        furnitureBtn.onClick.AddListener(furnitureBtnListener);
        editModeButton.onClick.AddListener(editBtnListener);
        backBtn.onClick.AddListener(backBtnListener);
        applyBtn.onClick.AddListener(applyBtnListener);
        restBtn.onClick.AddListener(resetBtnListener);

        if (GameInstance.Instance.quit) return;

        if (!(GameInstance.Instance.gameManager.loaded && GameInstance.Instance.assetLoader.assetLoadSuccessful))
        {
            GameInstance.Instance.uiManager.SwitchUI(UIType.Housing);
            return;
        }

        GameInstance.Instance.editMode = EditMode.CreativeMode;
        GameInstance.Instance.drawGrid.Draw();
        GameInstance.Instance.housingSystem.RemoveRoofInWorld();
        GameInstance.Instance.housingSystem.TempStorage();
        GameInstance.Instance.playerController.RemoveAction();
      //  GameInstance.Instance.playerController.AddCameraAction();
        GameInstance.Instance.playerController.Rigid.velocity = Vector3.zero;
    }

    private void OnDisable()
    {
        floorBtn.onClick.RemoveListener(floorBtnListener);
        wallBtn.onClick.RemoveListener(wallBtnListener);
        doorBtn.onClick.RemoveListener(doorBtnListener);
        furnitureBtn.onClick.RemoveListener(furnitureBtnListener);
        editModeButton.onClick.RemoveListener(editBtnListener);
        backBtn.onClick.RemoveListener(backBtnListener);
        applyBtn.onClick.RemoveListener(applyBtnListener);
        restBtn.onClick.RemoveListener(resetBtnListener);
        tab.SetActive(false);
        labelText.text = "";
   
        if (!(GameInstance.Instance.gameManager.loaded && GameInstance.Instance.assetLoader.assetLoadSuccessful)) return;
        GameInstance.Instance.inputManager.structureState = StructureState.None;
        GameInstance.Instance.editMode = EditMode.None;
        GameInstance.Instance.drawGrid.Remove();
        GameInstance.Instance.assetLoader.RemovePreview();
        GameInstance.Instance.housingSystem.ResetMaterials();
        GameInstance.Instance.housingSystem.CheckRoofInWorld();
        GameInstance.Instance.playerController.AddAction();
    //    GameInstance.Instance.playerController.RemoveCameraAction();
    }

    void ChangeSelectionType(StructureState structureState)
    {
        if (GameInstance.Instance.inputManager.structureState == structureState)
        {
            GameInstance.Instance.inputManager.structureState = StructureState.None;
            tab.SetActive(true);
            return;
        }
        else if (GameInstance.Instance.inputManager.structureState != StructureState.None)
        {

            GameInstance.Instance.inputManager.structureState = StructureState.None;
        }

        switch (structureState)
        {
            case StructureState.None:
                break;
            case StructureState.Floor:
                if(labelText.text == floorStr)
                {
                    tab.SetActive(false);
                    labelText.text = "";
                    return;
                }
                labelText.text = floorStr;
                tab.SetActive(true);
                break;
            case StructureState.Wall:
                if (labelText.text == wallStr)
                {
                    tab.SetActive(false);
                    labelText.text = "";
                    return;
                }
                labelText.text = wallStr;
                tab.SetActive(true);
                break;
            case StructureState.Door:
                if (labelText.text == doorStr)
                {
                    tab.SetActive(false);
                    labelText.text = "";
                    return;
                }
                labelText.text = doorStr;
                tab.SetActive(true);
                break;
            case StructureState.Furniture:
                if (labelText.text == furnitureStr)
                {
                    tab.SetActive(false);
                    labelText.text = "";
                    return ;
                }
                labelText.text= furnitureStr;
                tab.SetActive(true);
                break;
        }

        LoadInstallableMaterials(structureState);
    }
 
    void ChangeMode()
    {
        if(GameInstance.Instance.editMode == EditMode.CreativeMode)
        {
            creatableTab.SetActive(false);
            tab.SetActive(false);
            modeImage.sprite = plus;
            GameInstance.Instance.editMode = EditMode.DestroyMode;
            GameInstance.Instance.inputManager.structureState = StructureState.None;
        }
        else if(GameInstance.Instance.editMode == EditMode.DestroyMode)
        {
            creatableTab.SetActive(true);
            modeImage.sprite = minus;
            GameInstance.Instance.editMode = EditMode.CreativeMode;
        }
       
    }

    void LoadInstallableMaterials(StructureState structureState)
    {
        for (int i = items.Count - 1; i >= 0; i--)
        {
            //DestroyImmediate(items[i].gameObject,true);
            Destroy(items[i].gameObject);
            items.RemoveAt(i);
        }

        switch(structureState)
        {
            case StructureState.None:
                break;
            case StructureState.Floor:
                for (int i = 0; i < floors.Count; i++)
                {
                    InstallableItem spawnItem = Instantiate(installableItem);
                    spawnItem.SetItemStruct(floors[i], StructureState.Floor);
                    spawnItem.Setup(true);
                    spawnItem.GetComponent<RectTransform>().SetParent(detailsTab);
                    items.Add(spawnItem);
                }
                break;
            case StructureState.Wall:
                for (int i = 0; i < walls.Count; i++)
                {
                    InstallableItem spawnItem = Instantiate(installableItem);
                    spawnItem.SetItemStruct(walls[i], StructureState.Wall);
                    spawnItem.Setup(true);
                    spawnItem.GetComponent<RectTransform>().SetParent(detailsTab);
                    items.Add(spawnItem);
                }
                break;
            case StructureState.Door:
                for (int i = 0; i < doors.Count; i++)
                {
                    InstallableItem spawnItem = Instantiate(installableItem);
                    spawnItem.SetItemStruct(doors[i], StructureState.Door);
                    spawnItem.Setup(true);
                    spawnItem.GetComponent<RectTransform>().SetParent(detailsTab);
                    items.Add(spawnItem);
                }
                break;
            case StructureState.Furniture:
                List<CraftingData> craftingDatas = GameInstance.Instance.craftingLearnSystem.learnedDatas;
                int index = AssetLoader.furnituresKey.Count;
                int c = craftingDatas.Count;    
                for (int i = 0; i< c; i++)
                {
                    InstallableItem spawnItem = Instantiate(installableItem);
                    spawnItem.SetItemStruct(craftingDatas[i].item, StructureState.Furniture);
                    spawnItem.Setup(true);
                    spawnItem.GetComponent<RectTransform>().SetParent(detailsTab);
                    items.Add(spawnItem);
                }
                break;
        }
      
    }

    public void Setup(bool init)
    {
    }

    private void OnDestroy()
    {
        for(int i = 0;i < items.Count;i++) items[i].itemStruct.Clear();
        for(int i =0; i < floors.Count; i++) floors[i].Clear();
        for(int i =0; i < walls.Count; i++) walls[i].Clear();
        for(int i =0; i < doors.Count; i++) doors[i].Clear();
    }
}
