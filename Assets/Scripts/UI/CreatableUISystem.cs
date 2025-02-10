using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CreatableUISystem : MonoBehaviour, IUIComponent
{
    public GameObject upgradeGO;

    public Button floorBtn;
    public Button wallBtn;
    public Button doorBtn;

    [SerializeField]
    Button editModeButton;
    [SerializeField]
    Image modeImage;

    [SerializeField]
    Sprite plus;
    [SerializeField]    
    Sprite minus;

    // Start is called before the first frame update

    private void OnEnable()
    {
        if (!(GameInstance.Instance.gameManager.loaded && GameInstance.Instance.assetLoader.assetLoadSuccessful))
        {
            GameInstance.Instance.uiManager.SwitchUI(UIType.Housing);
            return;
        }
    
        floorBtn.onClick.AddListener(() =>
        {
            ChangeSelectionType(InputManager.StructureState.Floor);
        });

        wallBtn.onClick.AddListener(() =>
        {
            ChangeSelectionType(InputManager.StructureState.Wall);
        });
        doorBtn.onClick.AddListener(() =>
        {
            ChangeSelectionType(InputManager.StructureState.Door);
        });

        editModeButton.onClick.AddListener(() =>
        {
            ChangeMode();
        });
        GameInstance.Instance.editMode = EditMode.CreativeMode;
        GameInstance.Instance.drawGrid.Draw();
        GameInstance.Instance.housingSystem.RemoveRoofInWorld();
        GameInstance.Instance.playerController.RemoveAction();
        GameInstance.Instance.playerController.Rigid.velocity = Vector3.zero;

    }

    private void OnDisable()
    {
        if (!(GameInstance.Instance.gameManager.loaded && GameInstance.Instance.assetLoader.assetLoadSuccessful)) return;
    
        floorBtn.onClick.RemoveListener(() =>
        {
            ChangeSelectionType(InputManager.StructureState.Floor);
        });
        wallBtn.onClick.RemoveListener(() =>
        {
            ChangeSelectionType(InputManager.StructureState.Wall);
        });
        doorBtn.onClick.RemoveListener(() =>
        {
            ChangeSelectionType(InputManager.StructureState.Door);
        });
        editModeButton.onClick.RemoveListener(() =>
        {
            ChangeMode();
        });
        

       
        GameInstance.Instance.editMode = EditMode.None;
        GameInstance.Instance.drawGrid.Remove();
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
            modeImage.sprite = plus;
            GameInstance.Instance.editMode = EditMode.DestroyMode;
        }
        else if(GameInstance.Instance.editMode == EditMode.DestroyMode)
        {
            modeImage.sprite = minus;
            GameInstance.Instance.editMode = EditMode.CreativeMode;
        }
       
    }

    public void Setup()
    {
    }
}
