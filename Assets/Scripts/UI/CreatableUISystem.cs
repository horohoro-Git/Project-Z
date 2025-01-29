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
    }

    void ChangeSelectionType(InputManager.StructureState structureState)
    {
        if (GameInstance.Instance.inputManager.structureState == structureState) GameInstance.Instance.inputManager.structureState = InputManager.StructureState.None;
        else GameInstance.Instance.inputManager.structureState = structureState;
    }
    void ShowGridWithStructure()
    {
        if (upgradeGO.gameObject.activeSelf)
        {
            GameInstance.Instance.creativeMode = false;
            upgradeGO.gameObject.SetActive(false);
            GameInstance.Instance.drawGrid.Remove();
            GameInstance.Instance.playerController.AddAction();
        }
        else
        {
            GameInstance.Instance.creativeMode = true;
            upgradeGO.gameObject.SetActive(true);
            GameInstance.Instance.drawGrid.Draw();
            GameInstance.Instance.playerController.RemoveAction();
            GameInstance.Instance.playerController.Rigid.velocity = Vector3.zero;
          
        }
    }
}
