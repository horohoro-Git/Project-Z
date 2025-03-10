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
    public Sprite highLightSlot;
    [NonSerialized]
    public GameObject draggedItem;

    List<SlotArray> slotArrays = new List<SlotArray>();


    [SerializeField]
    SlotArray inventoryList;


    //���� ǥ�� ����

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


    int currentSlotIndex = 0;
    private void Awake()
    {
        //GameInstance.Instance.inventorySystem = this;
       
    }

    private void Start()
    {
        
        //gameObject.SetActive(false);
    }

    //�κ��丮 Ȯ��
    public void InventoryExtends(SlotArray slots)
    {
        slots.slotIndex = slotNum;
        for(int i = 0; i< 10; i++)
        {
            inventoryArray[slotNum, i] = slots.slots[i];
            //Debug.Log(inventoryArray[slotNum, i]);
        }
        slots.slotX = slotNum;
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
                    Debug.Log("�������� ����" + item.itemStruct.item_index);
                   // ItemStruct itemStruct = new ItemStruct(item.itemIndex, item.item_Image, item.item_Name, item.item_Slot, item.item_Type, item.item_GO);

                    inventoryArray[i, j].AddItem(item.itemStruct);
                    GameInstance.Instance.boxInventorySystem.UpdateSlot(item.itemStruct, i, j);
                    if(i == 0)
                    {
                        GameInstance.Instance.quickSlotUI.UpdateSlot(item.itemStruct, j);
                    }

                    SaveLoadSystem.SaveInventoryData();
                    return;
                }
            }
        }
        Debug.Log("�κ��丮 ������ ����");
    }

    public void UseItem(PlayerController pc ,int index)
    {
        inventoryArray[0, currentSlotIndex].image.sprite = defaultSlot;
        GameInstance.Instance.quickSlotUI.slots[currentSlotIndex].image.sprite = defaultSlot;

        currentSlotIndex = index;
        ItemStruct item = inventoryArray[0, index].GetItem();
        pc.Equipment(item, index);
        inventoryArray[0, index].image.sprite = highLightSlot;
        GameInstance.Instance.quickSlotUI.slots[index].image.sprite = highLightSlot;
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
        slotArrays.Add(slots);
        for (int i = 0; i < 5; i++)
        {
            SlotArray newSlots = Instantiate(inventoryList);
            newSlots.GetComponent<RectTransform>().SetParent(list);
            InventoryExtends(newSlots);
            slotArrays.Add(newSlots);
        }
        head.Setup();
        chest.Setup();
        arm.Setup();
        leg.Setup();
        backpack.Setup();
    }

    public void UpdateSlot(ItemStruct itemStruct, int x, int y)
    {
        inventoryArray[x, y].AddItem(itemStruct);
    }

    public void RemoveSlot(int x, int y)
    {
        inventoryArray[x, y].RemoveItem();
    }

    public void LoadInvetory(int x, int y, ItemStruct itemStruct)
    {
        ItemStruct item = ItemData.GetItem(itemStruct.item_index);

        itemStruct.itemGO = item.itemGO;
        itemStruct.image = item.image;
        itemStruct.used = true;
        if(x == 0)
        {
            GameInstance.Instance.quickSlotUI.UpdateSlot(itemStruct, y);
        }
        inventoryArray[x, y].AddItem(itemStruct);
    }

    public void ResetInventory()
    {
        slotNum = 1;
        for (int i = slotArrays.Count -1; i > 0; i--)
        {
            for(int j=0; j< 10; j++)
            {
                inventoryArray[i, j] = null;
            }
            Destroy(slotArrays[i].gameObject);
        }

        for(int i = 0; i< 10; i++)
        {
            RemoveSlot(0, i);
        }
        GameInstance.Instance.boxInventorySystem.ResetInventory();
    }
}
