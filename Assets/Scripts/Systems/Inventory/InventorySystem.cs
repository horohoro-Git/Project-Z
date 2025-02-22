using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySystem : MonoBehaviour, IUIComponent
{
    [NonSerialized]
    public Slot[,] inventoryArray = new Slot[7, 10];
    [NonSerialized]
    public int slotNum = 0;
    [NonSerialized]
    public int inventorySize = 0;
    public SlotInfo info;
    public RectTransform border;
    public RectTransform list;
    public GameObject draggingItem;
    public Sprite defaultSlot;
    [NonSerialized]
    public GameObject draggedItem;


    [SerializeField]
    SlotArray inventoryList;


    //장착 표시 슬롯

    [SerializeField]
    Slot head;
    [SerializeField]
    Slot chest;
    [SerializeField]
    Slot arm;
    [SerializeField]
    Slot leg;
    [SerializeField]
    Slot backpack;


    [SerializeField]
    Image playerView;
    [SerializeField]
    Sprite trashcan;


    int currentSlotIndex = -1;
    private void Awake()
    {
       // GameInstance.Instance.inventorySystem = this;
       
    }

    private void Start()
    {
        
        //gameObject.SetActive(false);
    }

    //인벤토리 확장
    public void InventoryExtends(SlotArray slots)
    {
        slots.slotIndex = slotNum;
        for(int i = 0; i< 10; i++)
        {
            inventoryArray[slotNum, i] = slots.slots[i];
            //Debug.Log(inventoryArray[slotNum, i]);
        }
        slots.Setup();
        slotNum++;
    }

    public void AddItem(Item item)
    {
        for(int i =0; i< slotNum; i++)
        {
            for(int j =0; j < 10; j++)
            {
                if(!inventoryArray[i, j].GetItem().used)
                {
                    Debug.Log("아이템을 얻음" + item.itemIndex);
                    ItemStruct itemStruct = new ItemStruct(item.itemIndex, item.item_Image, item.item_Name, item.item_Slot, item.item_Type, item.item_GO);

                    inventoryArray[i, j].AddItem(itemStruct);

                    if(i == 0)
                    {
                        GameInstance.Instance.quickSlotUI.UpdateSlot(itemStruct, j);
                    }

                    SaveLoadSystem.SaveInventoryData();
                    return;
                }
            }
        }

        Debug.Log("인벤토리 공간이 없음");

    }

    public void UseItem(PlayerController pc ,int index)
    {
        currentSlotIndex = index;
        ItemStruct item = inventoryArray[0, index].GetItem();
        pc.Equipment(item, index);
        /*  if (currentSlotIndex == index)
          {
              currentSlotIndex = -1;
          }
          else
          {
              currentSlotIndex = index;
              ItemStruct item = inventoryArray[0, index].GetItem();
              if (item.used)
              {
                  switch (item.itemType)
                  {
                      case ItemType.Equipmentable:
                          pc.Equipment(item, index);
                          break;
                  }

              }
          }*/
    }


    public void UpdateEquipSlot(SlotType slotType, ItemStruct item)
    {
        switch (slotType)
        {
            case SlotType.None:
                break;
            case SlotType.Head:
                head.AddItem(item);
                break;
            case SlotType.Chest:
                chest.AddItem(item);
                break;
            case SlotType.Arm:
                arm.AddItem(item);
                break;
            case SlotType.Leg:
                leg.AddItem(item);
                break;
            case SlotType.Backpack:
                backpack.AddItem(item); 
                break;
        }
    }

    public void SetPlayerView(bool on)
    {
        if(on) playerView.sprite = null;
        else playerView.sprite = trashcan;
    }
    public void Setup()
    {
        GameInstance.Instance.inventorySystem = this;
        SlotArray slots = GetComponentInChildren<SlotArray>();
        InventoryExtends(slots);

        for (int i = 0; i < 5; i++)
        {
            SlotArray newSlots = Instantiate(inventoryList);
            newSlots.GetComponent<RectTransform>().SetParent(list);
            InventoryExtends(newSlots);
        }
        head.Setup();
        chest.Setup();
        arm.Setup();
        leg.Setup();
        backpack.Setup();
    }

    public void LoadInvetory(int x, int y, int index)
    {
        Debug.Log(x + " " + y + " " + index);
        inventoryArray[x, y].AddItem(ItemData.GetItem(index));


    }
}
