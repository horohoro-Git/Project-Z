using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemData : MonoBehaviour
{
   

    public static Dictionary<int, ItemStruct> items = new Dictionary<int,ItemStruct>();

    public static void ItemDatabaseSetup()
    {
        foreach (KeyValuePair<int, ItemStruct> pair in GameInstance.Instance.assetLoader.items)
        {
            Sprite sprite = null;
            if (AssetLoader.loadedSprites.ContainsKey(AssetLoader.spriteAssetkeys[pair.Key].Name))
            {
                sprite = AssetLoader.loadedSprites[AssetLoader.spriteAssetkeys[pair.Key].Name];
            }
            GameObject go = null;
            if (AssetLoader.loadedAssets.ContainsKey(AssetLoader.itemAssetkeys[pair.Key].Name))
            {
                go = AssetLoader.loadedAssets[AssetLoader.itemAssetkeys[pair.Key].Name];
             //   Instantiate(go);
            }
            ItemStruct newItem = new ItemStruct(pair.Key, sprite, pair.Value.item_name, pair.Value.asset_name, pair.Value.weight, pair.Value.slot_type, pair.Value.item_type, go);
            items[pair.Key] = newItem;
            
        }
        items[0] = new ItemStruct();
    }

    public static ItemStruct GetItem(int index)
    {
        if (items.ContainsKey(index)) return items[index];
        return items[0];
    }

    public static void Clear()
    { 
        foreach (KeyValuePair<int, ItemStruct> item in items)
        {
            item.Value.Clear();
        }
    }
}
