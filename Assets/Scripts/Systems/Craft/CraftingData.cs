using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CraftingData : MonoBehaviour
{
    public LearnType type;

    public Image icon;
    public TMP_Text icon_name;
    public ItemStruct item;
    public Button btn;
    UnityAction btnAction;
    public bool learned;

    public CraftingLearnSystem learnSystem;

    public void Setup(ItemStruct itemStruct)
    {
        item = itemStruct;
        icon.sprite = itemStruct.image;
        icon_name.text = itemStruct.item_name;
    }
    private void OnEnable()
    {
        if(btnAction == null) btnAction = Learn;
        btn.onClick.AddListener(btnAction);
    }
    private void OnDisable()
    {
        btn.onClick.RemoveListener(btnAction);
    }

    void Learn()
    {
        if (!learned)
        {
            learned = true;
            learnSystem.Learn(this);
        }
    }
}
