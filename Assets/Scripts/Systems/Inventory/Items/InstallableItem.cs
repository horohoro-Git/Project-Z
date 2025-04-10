using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InstallableItem : Item, IUIComponent
{
    public Button btn;
    public Image image;
    public TMP_Text text;
    StructureState type;
    UnityAction clickAction;
    
    void OnEnable()
    {
        clickAction = () => BuildStart();
        btn.onClick.AddListener(clickAction);
    }
    private void OnDisable()
    {
        btn.onClick.RemoveListener(clickAction);
    }
    public void Setup(bool init)
    {
        
        image.sprite = itemStruct.image;
        text.text = itemStruct.item_name;

    }
    public void SetItemStruct(ItemStruct itemStruct, StructureState structureType)
    {
        this.itemStruct = itemStruct;
    /*    item_Image = itemStruct.image;
        item_Name = itemStruct.item_name;
        item_Slot = itemStruct.slot_type;
        item_Type = itemStruct.item_type;*/
        type = structureType;
    }


    void BuildStart()
    {
        GameInstance.Instance.creatableUISystem.tab.SetActive(false);
        GameInstance.Instance.inputManager.selectedInstallableItem = itemStruct;
        GameInstance.Instance.inputManager.structureState = type;
    }
}
