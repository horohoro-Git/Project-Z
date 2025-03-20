using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class BoxInventorySystem : MonoBehaviour, IUIComponent
{
    [NonSerialized]
    public Slot[,] inventoryArray = new Slot[7, 10];

    [NonSerialized]
    public Slot[,] boxInventoryArray = new Slot[7, 10];
    [NonSerialized]
    public int slotNum = 0;
    public int boxSlotNum = 0;

    [SerializeField]
    GameObject myInventory;
    [SerializeField]
    GameObject opponentInventory;
    [SerializeField]
    SlotArray inventoryList;
    public RectTransform list;
    public RectTransform list2;

    public RectTransform border1;
    public RectTransform border2;

    List<SlotArray> slotArrays = new List<SlotArray>();
    public void Setup()
    {
        GameInstance.Instance.boxInventorySystem = this;
        SlotArray slots = list.gameObject.GetComponentInChildren<SlotArray>();
        InventoryExtends(slots, inventoryArray, ref slotNum);
        slotArrays.Add(slots);
        for (int i = 0; i < 6; i++)
        {
            SlotArray newSlots = Instantiate(inventoryList);
            newSlots.GetComponent<RectTransform>().SetParent(list);
            InventoryExtends(newSlots, inventoryArray, ref slotNum);
            slotArrays.Add(newSlots);
        }

        for (int i = 0; i < 7; i++)
        {
            SlotArray newSlots = Instantiate(inventoryList);
            newSlots.GetComponent<RectTransform>().SetParent(list2);
            InventoryExtends(newSlots, boxInventoryArray, ref boxSlotNum);
        }
    }

    public void InventoryExtends(SlotArray slots, Slot[,] inventory, ref int num)
    {
        slots.slotIndex = num;
        for (int i = 0; i < 10; i++)
        {
            inventory[num, i] = slots.slots[i];
            //Debug.Log(inventoryArray[slotNum, i]);
        }
        slots.slotX = num;
        slots.Setup();
        num++;
    }
    /*public void AddItem(ItemStruct )
    {
        for (int i = 0; i < slotNum; i++)
        {
            for (int j = 0; j < 10; j++)
            {

            }
        }
    }*/

    public void UpdateSlot(ItemStruct itemStruct, WeaponStruct weaponStruct, ConsumptionStruct consumptionStruct, ArmorStruct armorStruct, int x, int y)
    {
        inventoryArray[x, y].AddItem(itemStruct, weaponStruct, consumptionStruct, armorStruct, true);
    }

    public void LoadInvetory(int x, int y, ItemStruct itemStruct, WeaponStruct weaponStruct, ConsumptionStruct consumptionStruct, ArmorStruct armorStruct)
    {
        ItemStruct item = ItemData.GetItem(itemStruct.item_index);

        itemStruct.itemGO = item.itemGO;
        itemStruct.image = item.image;
        itemStruct.used = true;
        if (x == 0)
        {
            GameInstance.Instance.quickSlotUI.UpdateSlot(itemStruct, weaponStruct, consumptionStruct, armorStruct, y, true);
        }
        inventoryArray[x, y].AddItem(itemStruct, weaponStruct, consumptionStruct, armorStruct, true);
    }

    public void GetOpponentInventory(List<ItemStruct> itemStructs)
    {
        int index = 0;
        for(int i = 0; i < itemStructs.Count; i++)
        {
            boxInventoryArray[index / 10, index % 10].AddItem(itemStructs[i], new WeaponStruct(), new ConsumptionStruct(), new ArmorStruct(), true);
            index++;
        }
    }

    public void ResetInventory()
    {
        slotNum = 1;
        for (int i = slotArrays.Count - 1; i > 0; i--)
        {
            for (int j = 0; j < 10; j++)
            {
                inventoryArray[i, j] = null;
            }

            SlotArray slotArray = slotArrays[i];
            slotArrays.RemoveAt(i);
            Destroy(slotArray.gameObject);
        }

        for (int i = 0; i < 10; i++)
        {
            inventoryArray[0, i].RemoveItem();
        }
    }
    private void OnDestroy()
    {
      
     //   Destroy(slotArrays[0].gameObject);
        //ResetInventory();
        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                inventoryArray[i, j] = null;
            }
        }

        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                boxInventoryArray[i, j] = null;
            }
        }

       
    }
}
