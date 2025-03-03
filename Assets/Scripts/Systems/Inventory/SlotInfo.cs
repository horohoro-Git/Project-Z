using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotInfo : MonoBehaviour
{

    public Image image;
    public TMP_Text text;

    public void UpdateSlotInfo(ItemStruct itemStruct)
    {
        text.text = itemStruct.itemName;
        image.sprite = GameInstance.Instance.assetLoader.loadedSprites[AssetLoader.spriteAssetkeys[itemStruct.itemIndex]];
    }
}
