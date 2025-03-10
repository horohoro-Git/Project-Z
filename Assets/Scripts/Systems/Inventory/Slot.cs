using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class Slot : MonoBehaviour, IUIComponent
{
    Button slotBtn;
    RectTransform self;
    [NonSerialized]
    public Image image;
    SlotInfo info;
    [SerializeField]
    Sprite originImage;
    Image itemImage;

    public SlotType slotType;
    public ItemStruct item = new ItemStruct();
    public WeaponStruct weapon = new WeaponStruct();
    public ConsumptionStruct consumption = new ConsumptionStruct();

    //SerializeField]
    public int slotX;
    //[SerializeField]
    public int slotY;

    EventTrigger eventTrigger;
    EventTrigger.Entry entryHover;
    EventTrigger.Entry entryExit;
    EventTrigger.Entry dragStart;
    EventTrigger.Entry dragEnd;
    UnityAction<BaseEventData> hoverEnter;
    UnityAction<BaseEventData> hoverExit;
    UnityAction<BaseEventData> dragEnter;
    UnityAction<BaseEventData> dragExit;

    private void OnEnable()
    {
        if (eventTrigger == null)
        {
            eventTrigger = gameObject.AddComponent<EventTrigger>();

            entryHover = new EventTrigger.Entry();
            entryHover.eventID = EventTriggerType.PointerEnter;
            hoverEnter = (eventData) => OnHoverEnter();

            entryExit = new EventTrigger.Entry();
            entryExit.eventID = EventTriggerType.PointerExit;
            hoverExit = (eventData) => OnHoverExit();

            if (slotType != SlotType.JustView)
            {
                dragStart = new EventTrigger.Entry();
                dragStart.eventID = EventTriggerType.PointerDown;
                dragEnter = (eventData) => OnDragEnter();

                dragEnd = new EventTrigger.Entry();
                dragEnd.eventID = EventTriggerType.PointerUp;
                dragExit = (eventData) => OnDragExit();
            }
        }
        //호버
        entryHover.callback.AddListener(hoverEnter);
        eventTrigger.triggers.Add(entryHover);

        //호버 종료
      
        entryExit.callback.AddListener(hoverExit);
        eventTrigger.triggers.Add(entryExit);

        if (slotType == SlotType.JustView) return;

        //상호작용 가능 슬롯

        //드래그 시작
      
        dragStart.callback.AddListener(dragEnter);
        eventTrigger.triggers.Add(dragStart);

        //드래그 종료
   
        dragEnd.callback.AddListener(dragExit);
        eventTrigger.triggers.Add(dragEnd);
    }

    private void OnDisable()
    {
        if (eventTrigger != null)
        {
            entryHover.callback.RemoveListener(hoverEnter);
            eventTrigger.triggers.Remove(entryHover);

            entryExit.callback.RemoveListener(hoverExit);
            eventTrigger.triggers.Remove(entryExit);

            if (slotType == SlotType.JustView) return;


            dragStart.callback.RemoveListener(dragEnter);
            eventTrigger.triggers.Remove(dragStart);

            dragEnd.callback.RemoveListener(dragExit);
            eventTrigger.triggers.Remove(dragEnd);

            if (info != null) Destroy(info.gameObject);
        }
    }


    void OnDragEnter()
    {
        if (!item.used) return;
        GameInstance.Instance.inventorySystem.draggedItem = Instantiate(GameInstance.Instance.inventorySystem.draggingItem);
        GameInstance.Instance.inventorySystem.draggedItem.GetComponent<Image>().sprite = itemImage.sprite;

        if(GetComponentInParent<InventorySystem>())
        {
            GameInstance.Instance.inventorySystem.draggedItem.GetComponent<RectTransform>().SetParent(GameInstance.Instance.inventorySystem.border);
        }
        else if(GetComponentInParent<BoxInventorySystem>())
        {
            GameInstance.Instance.inventorySystem.draggedItem.GetComponent<RectTransform>().SetParent(GameInstance.Instance.boxInventorySystem.border2);
        }
        //image.sprite = originImage;
        itemImage.sprite = GameInstance.Instance.inventorySystem.defaultSlot;

        GameInstance.Instance.inventorySystem.SetPlayerView(false);
    }

    void OnDragExit()
    {
        if (!item.used || GameInstance.Instance.inventorySystem.draggedItem == null) return;

        itemImage.sprite = GameInstance.Instance.inventorySystem.draggedItem.GetComponent<Image>().sprite;
        Destroy(GameInstance.Instance.inventorySystem.draggedItem.gameObject);

        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = Input.mousePosition;

        List<RaycastResult> raycastResult = new List<RaycastResult>();

        GameInstance.Instance.uiManager.graphicRaycaster.Raycast(pointerData, raycastResult);

        if (raycastResult.Count > 0)
        {
            foreach (RaycastResult r in raycastResult)
            {
                Slot s = r.gameObject.GetComponent<Slot>();
                if (s != null)  //다른 슬롯으로 아이템 이동
                {
                    if (s.slotType == SlotType.JustView) break;
                    ItemStruct tempStruct = s.item;
                    s.item = item;
                    item = tempStruct;
                    WeaponStruct weaponTemp = s.weapon;
                    s.weapon = weapon;
                    weapon = weaponTemp;
                    ConsumptionStruct consumptionTemp = s.consumption;
                    s.consumption = consumption;
                    s.consumption = consumptionTemp;

                    UpdateSlot();
                    s.UpdateSlot();

                    PlayerController pc = GameInstance.Instance.GetPlayers[0];
                    if (slotX == 0 )
                    {
                        GameInstance.Instance.quickSlotUI.UpdateSlot(item, weapon, consumption, slotY);
                        if(slotY == pc.equipSlotIndex)
                        {
                            pc.Unequipment();
                        }
                    }

                    if (s.slotX == 0)
                    {
                        GameInstance.Instance.quickSlotUI.UpdateSlot(s.item, s.weapon, s.consumption, s.slotY);
                        if (slotY == pc.equipSlotIndex)
                        {
                            pc.Unequipment();
                        }
                    }

                    if(GetComponentInParent<InventorySystem>())
                    {
                        GameInstance.Instance.boxInventorySystem.UpdateSlot(item, weapon, consumption, slotX, slotY);
                        GameInstance.Instance.boxInventorySystem.UpdateSlot(s.item, s.weapon, s.consumption, s.slotX, s.slotY);
                    }

                    if(GetComponentInParent<BoxInventorySystem>())
                    {
                        if (transform.parent.parent == GameInstance.Instance.boxInventorySystem.list && s.transform.parent.parent == GameInstance.Instance.boxInventorySystem.list2)
                        {
                            GameInstance.Instance.inventorySystem.RemoveSlot(slotX, slotY);
                        }
                        else
                        {
                            GameInstance.Instance.inventorySystem.UpdateSlot(item, weapon, consumption, slotX, slotY);
                            GameInstance.Instance.inventorySystem.UpdateSlot(s.item, s.weapon, s.consumption, s.slotX, s.slotY);

                        }
                    }
                 

                }
                else // 아이템 버리기
                {
                    AcceptanceUI acceptanceUI = Instantiate(GameInstance.Instance.uiManager.acceptanceUI).GetComponent<AcceptanceUI>();
                    RectTransform rect = GameInstance.Instance.uiManager.canvas.GetComponent<RectTransform>();  
                    RectTransform uiRect = acceptanceUI.GetComponent<RectTransform>();
                    uiRect.SetParent(rect);
                    uiRect.anchorMin = new Vector2(0.5f, 0.5f);
                    uiRect.anchorMax = new Vector2(0.5f, 0.5f);
                    uiRect.anchoredPosition = Vector2.zero;
                    uiRect.sizeDelta = new Vector2(rect.rect.width, rect.rect.height);
                    UnityAction action = () => RemoveItem();
                    acceptanceUI.GetAction(action);
                    //RemoveItem();


                }
                break;
            }

        }
        GameInstance.Instance.inventorySystem.SetPlayerView(true);
    }

    void OnHoverEnter()
    {
        if (!item.used) return;

        info = Instantiate(GameInstance.Instance.inventorySystem.info);
        if(GetComponentInParent<InventorySystem>())
        {
            info.GetComponent<RectTransform>().SetParent(GameInstance.Instance.inventorySystem.border);

            if (self.position.y - 300 > 200)
            {
                if (self.position.x + 150 > 1600) info.GetComponent<RectTransform>().position = new Vector3(self.position.x - 150, self.position.y - 300, 0);
                else info.GetComponent<RectTransform>().position = new Vector3(self.position.x + 150, self.position.y - 300, 0);
            }
            else
            {
                if (self.position.x + 150 > 1600) info.GetComponent<RectTransform>().position = new Vector3(self.position.x - 150, self.position.y + 300, 0);
                else info.GetComponent<RectTransform>().position = new Vector3(self.position.x + 150, self.position.y + 300, 0);
            }
        }
        if (GetComponentInParent<BoxInventorySystem>())
        {
            if (transform.parent.parent == GameInstance.Instance.boxInventorySystem.list)
            {
                info.GetComponent<RectTransform>().SetParent(GameInstance.Instance.boxInventorySystem.border1);
                if (self.position.y - 300 > 200)
                {
                    if (self.position.x + 150 > 700) info.GetComponent<RectTransform>().position = new Vector3(self.position.x - 150, self.position.y - 300, 0);
                    else info.GetComponent<RectTransform>().position = new Vector3(self.position.x + 150, self.position.y - 300, 0);
                }
                else
                {
                    if (self.position.x + 150 > 700) info.GetComponent<RectTransform>().position = new Vector3(self.position.x - 150, self.position.y + 300, 0);
                    else info.GetComponent<RectTransform>().position = new Vector3(self.position.x + 150, self.position.y + 300, 0);
                }
            }
            else if(transform.parent.parent == GameInstance.Instance.boxInventorySystem.list2)
            {
                info.GetComponent<RectTransform>().SetParent(GameInstance.Instance.boxInventorySystem.border2);
                if (self.position.y - 300 > 200)
                {
                    if (self.position.x + 150 > 1600) info.GetComponent<RectTransform>().position = new Vector3(self.position.x - 150, self.position.y - 300, 0);
                    else info.GetComponent<RectTransform>().position = new Vector3(self.position.x + 150, self.position.y - 300, 0);
                }
                else
                {
                    if (self.position.x + 150 > 1600) info.GetComponent<RectTransform>().position = new Vector3(self.position.x - 150, self.position.y + 300, 0);
                    else info.GetComponent<RectTransform>().position = new Vector3(self.position.x + 150, self.position.y + 300, 0);
                }
            }
        }

        info.UpdateSlotInfo(item);
        SaveLoadSystem.SaveInventoryData();
        //     info

    }

    void OnHoverExit()
    {
        if(info != null) Destroy(info.gameObject);
     //   Debug.Log("호버 종료");
    }

    public ItemStruct GetItem()
    {
        return item;
    }

    public void AddItem(ItemStruct item, WeaponStruct weaponStruct, ConsumptionStruct consumptionStruct)
    {
        this.item = item;
        this.weapon = weaponStruct;
        this.consumption = consumptionStruct;
        UpdateSlot();
    }

    public void RemoveItem()
    {
        ItemStruct itemS = new ItemStruct();
        item = itemS;
        weapon = new WeaponStruct();
        consumption = new ConsumptionStruct();
        GameInstance.Instance.boxInventorySystem.UpdateSlot(itemS, weapon, consumption, slotX, slotY);
        if (slotX == 0)
        {
            GameInstance.Instance.quickSlotUI.UpdateSlot(itemS, weapon, consumption, slotY);
           
            PlayerController pc = GameInstance.Instance.GetPlayers[0];
            if (pc != null)
            {
                if(pc.equipSlotIndex == slotY)
                {
                    pc.Unequipment();
                }
            }
        }
        UpdateSlot();
    }

    public void UpdateSlot()
    {
     //   GameInstance.Instance.assetLoader.loadedSprites[GameInstance.Instance.assetLoader.spriteAssetkeys[item.itemIndex]];
        if(item.used)
        itemImage.sprite = GameInstance.Instance.assetLoader.loadedSprites[AssetLoader.spriteAssetkeys[item.item_index - 1]];
        else
        {
            itemImage.sprite = originImage;
        }

        switch (slotType)
        {
            case SlotType.None:
                break;
            default:
                GameInstance.Instance.inventorySystem.UpdateEquipSlot(slotType, item, weapon, consumption);
                break;
        }

    }

    public void Setup()
    {
        image = GetComponent<Image>();

        slotBtn = GetComponent<Button>();
        self = GetComponent<RectTransform>();
        Image[] images = GetComponentsInChildren<Image>(true);
        foreach (Image i in images)
        {
            if (image != i) itemImage = i;
        }

        itemImage.sprite = originImage;
    }
}
