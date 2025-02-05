using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CreatableUISystem : MonoBehaviour
{
    public Button upgradeBtn;

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
        upgradeBtn.onClick.AddListener(() =>
        {
            ShowGridWithStructure();
        });

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
    }

    private void OnDisable()
    {
        upgradeBtn.onClick.RemoveListener(() =>
        {
            ShowGridWithStructure();
        });
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
    }

    void ChangeSelectionType(InputManager.StructureState structureState)
    {
        if (GameInstance.Instance.inputManager.structureState == structureState) GameInstance.Instance.inputManager.structureState = InputManager.StructureState.None;
        else GameInstance.Instance.inputManager.structureState = structureState;
    }
    void ShowGridWithStructure()
    {
        if (GameInstance.Instance.gameManager.loaded)
        {
            if (upgradeGO.gameObject.activeSelf)
            {
                GameInstance.Instance.editMode = EditMode.None;
                upgradeGO.gameObject.SetActive(false);
                GameInstance.Instance.drawGrid.Remove();
                GameInstance.Instance.housingSystem.CheckRoofInWorld();
                GameInstance.Instance.playerController.AddAction();
                if (GameInstance.Instance.gameManager.gameMode == GameMode.DefaultMode)
                {
                    //게임 정보 저장
                    SaveLoadSystem.SaveBuildSystem();
                }
            }
            else
            {
                GameInstance.Instance.editMode = EditMode.CreativeMode;
                upgradeGO.gameObject.SetActive(true);
                GameInstance.Instance.drawGrid.Draw();
                GameInstance.Instance.housingSystem.RemoveRoofInWorld();
                GameInstance.Instance.playerController.RemoveAction();
                GameInstance.Instance.playerController.Rigid.velocity = Vector3.zero;

            }
        }
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
}
