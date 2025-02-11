using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CreatableUISystem : MonoBehaviour, IUIComponent
{
    public GameObject upgradeGO;
    public GameObject tab;

    public Button floorBtn;
    public Button wallBtn;
    public Button doorBtn;
    public Button backBtn;
    public Button applyBtn;
    public Button restBtn;

    [SerializeField]
    Button editModeButton;
    [SerializeField]
    Image modeImage;

    [SerializeField]
    Sprite plus;
    [SerializeField]    
    Sprite minus;

    UnityAction floorBtnListener;
    UnityAction wallBtnListener;
    UnityAction doorBtnListener;
    UnityAction editBtnListener;
    UnityAction backBtnListener;
    UnityAction applyBtnListener;
    UnityAction resetBtnListener;

    // Start is called before the first frame update

    private void OnEnable()
    {
        floorBtnListener = () => ChangeSelectionType(InputManager.StructureState.Floor);
        wallBtnListener = () => ChangeSelectionType(InputManager.StructureState.Wall);
        doorBtnListener = () => ChangeSelectionType(InputManager.StructureState.Door);
        editBtnListener = () => ChangeMode();
        backBtnListener = () => GameInstance.Instance.housingSystem.Revert();
        applyBtnListener = () => GameInstance.Instance.housingSystem.Apply();
        resetBtnListener = () => GameInstance.Instance.housingSystem.ResetMaterials();

        floorBtn.onClick.AddListener(floorBtnListener);
        wallBtn.onClick.AddListener(wallBtnListener);
        doorBtn.onClick.AddListener(doorBtnListener);
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
        GameInstance.Instance.playerController.Rigid.velocity = Vector3.zero;
    }

    private void OnDisable()
    {
        floorBtn.onClick.RemoveListener(floorBtnListener);
        wallBtn.onClick.RemoveListener(wallBtnListener);
        doorBtn.onClick.RemoveListener(doorBtnListener);
        editModeButton.onClick.RemoveListener(editBtnListener);
        backBtn.onClick.RemoveListener(backBtnListener);
        applyBtn.onClick.RemoveListener(applyBtnListener);
        restBtn.onClick.RemoveListener(resetBtnListener);

        if (GameInstance.Instance.quit) return;
        if (!(GameInstance.Instance.gameManager.loaded && GameInstance.Instance.assetLoader.assetLoadSuccessful)) return;
        GameInstance.Instance.editMode = EditMode.None;
        GameInstance.Instance.drawGrid.Remove();
        GameInstance.Instance.housingSystem.ResetMaterials();
        GameInstance.Instance.housingSystem.CheckRoofInWorld();
        GameInstance.Instance.playerController.AddAction();
        if (GameInstance.Instance.gameManager.gameMode == GameMode.DefaultMode)
        {
            //게임 정보 저장
            SaveLoadSystem.SaveBuildSystem();
        }
        
    }

    void ChangeSelectionType(InputManager.StructureState structureState)
    {
        if (GameInstance.Instance.inputManager.structureState == structureState) GameInstance.Instance.inputManager.structureState = InputManager.StructureState.None;
        else GameInstance.Instance.inputManager.structureState = structureState;
    }
 
    void ChangeMode()
    {
        if(GameInstance.Instance.editMode == EditMode.CreativeMode)
        {
         //   tab.SetActive(false);
            modeImage.sprite = plus;
            GameInstance.Instance.editMode = EditMode.DestroyMode;
        }
        else if(GameInstance.Instance.editMode == EditMode.DestroyMode)
        {
         //   tab.SetActive(true);
            modeImage.sprite = minus;
            GameInstance.Instance.editMode = EditMode.CreativeMode;
        }
       
    }

    public void Setup()
    {
    }

   
}
