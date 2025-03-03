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
        items = GameInstance.Instance.assetLoader.items;
        //  List<string> keys = GameInstance.Instance.assetLoader.spriteAssetkeys;
        //    List<string> goKeys = GameInstance.Instance.assetLoader.itemAssetkeys;
        for (int i = 0; i < AssetLoader.itemAssetkeys.Count; i++)
        {
            Sprite sprite = GameInstance.Instance.assetLoader.loadedSprites[AssetLoader.spriteAssetkeys[i]];
            GameObject go = GameInstance.Instance.assetLoader.loadedAssets[AssetLoader.itemAssetkeys[i]];
            ItemStruct item = items[i];

            item.image = sprite;
            item.itemGO = go;
            items.Add(item);
        }
    }

    public static ItemStruct GetItem(int index)
    {
        return items[index];
    }
}
