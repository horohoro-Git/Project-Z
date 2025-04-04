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
        items[0] = new ItemStruct();

        foreach (KeyValuePair<int, ItemStruct> pair in items)
        {
            Sprite sprite = GameInstance.Instance.assetLoader.loadedSprites[AssetLoader.spriteAssetkeys[pair.Key]];
            GameObject go = GameInstance.Instance.assetLoader.loadedAssets[AssetLoader.itemAssetkeys[pair.Key]];

            ItemStruct newItem = new ItemStruct(pair.Key, sprite, pair.Value.item_name, pair.Value.asset_name, pair.Value.weight, pair.Value.slot_type, pair.Value.item_type, go);
            items[pair.Key] = newItem;
        }
       // items.Add(new ItemStruct());

      //  Dictionary<uint, >

        /*List<ItemStruct> itemStructs = GameInstance.Instance.assetLoader.items;
        for (int i = 0; i < itemStructs.Count; i++)
        {
            Sprite sprite = GameInstance.Instance.assetLoader.loadedSprites[AssetLoader.spriteAssetkeys[i]];
            GameObject go = null;
            if (GameInstance.Instance.assetLoader.loadedAssets.ContainsKey(AssetLoader.itemAssetkeys[i])) go = GameInstance.Instance.assetLoader.loadedAssets[AssetLoader.itemAssetkeys[i]];
            ItemStruct item = itemStructs[i];
            item.image = sprite;
            item.itemGO = go;
            item.used = true;
            items.Add(item);
            
        }*/
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
