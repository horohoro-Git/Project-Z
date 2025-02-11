using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySystem : MonoBehaviour, IUIComponent
{

    Slot[,] inventoryArray = new Slot[7, 10];
    int slotNum = 0;
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
                    Debug.Log("아이템을 얻음");
                    ItemStruct itemStruct = new ItemStruct(item.item_Image, item.item_Name, item.item_Type);

                    inventoryArray[i, j].AddItem(itemStruct);
                    return;
                }
            }
        }
        Debug.Log("인벤토리 공간이 없음");

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
}
