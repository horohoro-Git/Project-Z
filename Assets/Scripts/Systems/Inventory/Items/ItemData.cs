using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemData : MonoBehaviour
{
   

    public static List<ItemStruct> items = new List<ItemStruct>();

    public static void ItemDatabaseSetup()
    {
        items.Add(new ItemStruct());
        List<ItemStruct> itemStructs = GameInstance.Instance.assetLoader.items;
        for (int i = 0; i < AssetLoader.itemAssetkeys.Count; i++)
        {
            Sprite sprite = GameInstance.Instance.assetLoader.loadedSprites[AssetLoader.spriteAssetkeys[i]];
            GameObject go = GameInstance.Instance.assetLoader.loadedAssets[AssetLoader.itemAssetkeys[i]];
            ItemStruct item = itemStructs[i];
            item.image = sprite;
            item.itemGO = go;
            item.used = true;
            items.Add(item);
            
        }
    }

    public static ItemStruct GetItem(int index)
    {
        return items[index];
    }
}
