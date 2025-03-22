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
    [NonSerialized]
    public Image itemImage;

    public SlotType slotType;
    public ItemStruct item = new ItemStruct();
    public WeaponStruct weapon = new WeaponStruct();
    public ConsumptionStruct consumption = new ConsumptionStruct();
    public ArmorStruct armor = new ArmorStruct();

    ArmorStruct lastArmor = new ArmorStruct();

    //SerializeField]
    public int slotX;
    //[SerializeField]
    public int slotY;
    public bool justView;

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

            if (!justView)
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

        if (justView) return;

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

            if (justView) return;


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
                    if (s.justView) break;
                    int targetNum = s.armor.carrying_capacity;
                    int currentSlotNum = armor.carrying_capacity;
                   
                    if(armor.armor_type == SlotType.Backpack)
                    {
                        if (!ValidCheckBackpackChange(this, targetNum)) return;
                    }

                    if(s.armor.armor_type == SlotType.Backpack)
                    {
                        if (!ValidCheckBackpackChange(this, targetNum)) return;
                    }
              /*      if (armor.armor_type == SlotType.Backpack && slotX == 0 && slotY == 9)
                    {
                        int slotNum = GameInstance.Instance.inventorySystem.slotNum;

                        int targetNum = s.armor.carrying_capacity;

                        if (slotNum > targetNum)
                        {
                            if(!GameInstance.Instance.inventorySystem.CheckItem(targetNum, slotNum))
                            {
                                Debug.Log("Fail");
                                return;
                            }
                        }
                    }*/



                    ItemStruct tempStruct = s.item;
                    s.item = item;
                    item = tempStruct;
                    tempStruct.Clear();
                    WeaponStruct weaponTemp = s.weapon;
                    s.weapon = weapon;
                    weapon = weaponTemp;
                    ConsumptionStruct consumptionTemp = s.consumption;
                    s.consumption = consumption;
                    consumption = consumptionTemp;
                    ArmorStruct armorTemp = s.armor;
                    s.armor = armor;
                    armor = armorTemp;
                    UpdateSlot(true);
                    s.UpdateSlot(true);


                   

                    PlayerController pc = GameInstance.Instance.GetPlayers[0];
                 

                    if(GetComponentInParent<InventorySystem>())
                    {
                        GameInstance.Instance.boxInventorySystem.UpdateSlot(item, weapon, consumption, armor, slotX, slotY);
                        GameInstance.Instance.boxInventorySystem.UpdateSlot(s.item, s.weapon, s.consumption, s.armor, s.slotX, s.slotY);
                        if (slotX == 0)
                        {
                            GameInstance.Instance.quickSlotUI.UpdateSlot(item, weapon, consumption, armor, slotY,false);
                            if (slotY == pc.equipSlotIndex)
                            {
                                pc.Unequipment();
                            //    GameInstance.Instance.inventorySystem.UseItem(pc, pc.equipSlotIndex);
                            }
                           
                        }

                        if (s.slotX == 0)
                        {
                            GameInstance.Instance.quickSlotUI.UpdateSlot(s.item, s.weapon, s.consumption, s.armor, s.slotY, false);
                            if (s.slotY == pc.equipSlotIndex)
                            {
                                pc.Unequipment();
                              //  GameInstance.Instance.inventorySystem.UseItem(pc, pc.equipSlotIndex);
                            }
                        }
                    }

                    if(GetComponentInParent<BoxInventorySystem>())
                    {
                        if(s.transform.parent.parent == GameInstance.Instance.boxInventorySystem.list)
                        {

                            if (s.slotX == 0)
                            {
                                GameInstance.Instance.quickSlotUI.UpdateSlot(s.item, s.weapon, s.consumption, s.armor, s.slotY, false);
                                if (s.slotY == pc.equipSlotIndex)
                                {
                                    pc.Unequipment();
                                }
                            }
                        }
                        if (transform.parent.parent == GameInstance.Instance.boxInventorySystem.list)
                        {
                            if (slotX == 0)
                            {
                                GameInstance.Instance.quickSlotUI.UpdateSlot(item, weapon, consumption, armor, slotY, false);
                                if (slotY == pc.equipSlotIndex)
                                {
                                    pc.Unequipment();
                                }
                            }
                        }

                        if (transform.parent.parent == GameInstance.Instance.boxInventorySystem.list && s.transform.parent.parent == GameInstance.Instance.boxInventorySystem.list2)
                        {
                            GameInstance.Instance.inventorySystem.RemoveSlot(slotX, slotY);
                        }
                        else if(transform.parent.parent == GameInstance.Instance.boxInventorySystem.list2 && s.transform.parent.parent == GameInstance.Instance.boxInventorySystem.list)
                        {
                         //   GameInstance.Instance.inventorySystem.UpdateSlot(item, weapon, consumption, slotX, slotY);
                            GameInstance.Instance.inventorySystem.UpdateSlot(s.item, s.weapon, s.consumption, s.armor, s.slotX, s.slotY);

                        }
                        else
                        {

                            GameInstance.Instance.inventorySystem.UpdateSlot(item, weapon, consumption, armor, slotX, slotY);
                            GameInstance.Instance.inventorySystem.UpdateSlot(s.item, s.weapon, s.consumption, s.armor, s.slotX, s.slotY);
                        }
                    }
                 

                }

                Trashcan t = r.gameObject.GetComponent<Trashcan>();
                if(t != null) // 아이템 버리기
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


    //가방 교체 가능 확인
    bool ValidCheckBackpackChange(Slot slot, int targetNum)
    {
        ArmorStruct armorStruct = slot.armor;
        if (armorStruct.armor_type == SlotType.Backpack && slotX == 0 && slotY == 9)
        {
            int slotNum = GameInstance.Instance.inventorySystem.slotNum;

            if (slotNum > targetNum)
            {
                if (!GameInstance.Instance.inventorySystem.CheckItem(targetNum, slotNum))
                {
                    Debug.Log("Fail");
                    return false;
                }
            }
        }
        return true;
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

        info.UpdateSlotInfo(item, consumption, weapon, armor);
        SaveLoadSystem.SaveInventoryData();
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

    public void AddItem(ItemStruct item, WeaponStruct weaponStruct, ConsumptionStruct consumptionStruct, ArmorStruct armorStruct, bool justUpdate)
    {
        this.item = item;
        this.weapon = weaponStruct;
        this.consumption = consumptionStruct;
        lastArmor = this.armor;
        this.armor = armorStruct;
        UpdateSlot(justUpdate);
    }

    public void RemoveItem()
    {
        ItemStruct itemS = new ItemStruct();
        item.Clear();
        item = itemS;
        weapon = new WeaponStruct();
        consumption = new ConsumptionStruct();
        armor = new ArmorStruct();
        GameInstance.Instance.boxInventorySystem.UpdateSlot(itemS, weapon, consumption, armor, slotX, slotY);
        if (slotX == 0)
        {
            GameInstance.Instance.quickSlotUI.UpdateSlot(itemS, weapon, consumption, armor, slotY, false);
           
            PlayerController pc = GameInstance.Instance.GetPlayers[0];
            if (pc != null)
            {
                if(pc.equipSlotIndex == slotY)
                {
                    pc.Unequipment();
                }
            }
        }
        UpdateSlot(true);
    }

    public void UpdateSlot(bool justUpdate)
    {
        //   GameInstance.Instance.assetLoader.loadedSprites[GameInstance.Instance.assetLoader.spriteAssetkeys[item.itemIndex]];
        if (item.used)
        {
   //         itemImage.gameObject.SetActive(true);
            itemImage.sprite = GameInstance.Instance.assetLoader.loadedSprites[AssetLoader.spriteAssetkeys[item.item_index - 1]];
        }
        else
        {
  //          itemImage.gameObject.SetActive(false);
            //image.sprite = originImage;
            itemImage.sprite = originImage;
        }

        if (!justUpdate)
        {
            switch (slotType)
            {
                case SlotType.None:
                    break;
                default:
                    if(slotType == armor.armor_type) GameInstance.Instance.inventorySystem.UpdateEquipSlot(slotType, item, weapon, consumption, armor);
                    else GameInstance.Instance.inventorySystem.UpdateEquipSlot(slotType, new ItemStruct(), new WeaponStruct(), new ConsumptionStruct(), new ArmorStruct());
                    break;
            }
            UMACharacterAvatar character = GameInstance.Instance.GetPlayers[0].GetAvatar;
            Player player = GameInstance.Instance.GetPlayers[0].GetPlayer;
            CharacterProfileUI characterProfileUI = GameInstance.Instance.characterProfileUI;
            switch (slotType)
            {
                case SlotType.None:
                    return;
                case SlotType.Head:
                    character.RemoveCloth("Helmet");
                    characterProfileUI.RemoveCloth("Helmet");

                    //    player.
                    break;
                case SlotType.Chest:
                    character.RemoveCloth("Chest");
                    characterProfileUI.RemoveCloth("Chest");
                    break;
                case SlotType.Arm:
                    character.RemoveCloth("Hands");
                    characterProfileUI.RemoveCloth("Hands");
                    break;
                case SlotType.Leg:
                    character.RemoveCloth("Legs");
                    characterProfileUI.RemoveCloth("Legs");
                    break;
                case SlotType.Foot:
                    character.RemoveCloth("Feet");
                    characterProfileUI.RemoveCloth("Feet");
                    break;
                case SlotType.Backpack:
                    character.RemoveCloth("Cape");
                    characterProfileUI.RemoveCloth("Cape");
                    //GameInstance.Instance.inventorySystem.ClearSlots();
                    player.WearingArmor(armor, true);
                    break;
            }
            //if (slotType != SlotType.Backpack)
            Debug.Log(lastArmor.armor_type);
            Debug.Log(armor.armor_type);
            if (slotType != SlotType.Backpack) player.PutonArmor(lastArmor);
            if (armor.armor_type == slotType)
            {
                if (slotType != SlotType.Backpack)
                {
                    player.WearingArmor(armor, false);
                }
                character.AddCloth(armor.key_index);
                characterProfileUI.AddCloth(armor.key_index);
            }
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
       // itemImage.gameObject.SetActive(false);
       // itemImage.sprite = originImage;
    }
    private void OnDestroy()
    {
        item.Clear();
    }
}
