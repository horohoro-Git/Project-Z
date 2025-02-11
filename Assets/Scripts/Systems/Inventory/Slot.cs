using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IUIComponent
{
    Button slotBtn;
    RectTransform self;
    Image image;
    SlotInfo info;
    [SerializeField]
    Sprite originImage;
    Image itemImage;

    public SlotType slotType;
    ItemStruct item;

    [SerializeField]
    int slotX;
    [SerializeField]
    int slotY;

    EventTrigger eventTrigger;
    EventTrigger.Entry entryHover;
    EventTrigger.Entry entryExit;
    EventTrigger.Entry dragStart;
    EventTrigger.Entry dragEnd;
    UnityAction<BaseEventData> hoverEnter;
    UnityAction<BaseEventData> hoverExit;
    UnityAction<BaseEventData> dragEnter;
    UnityAction<BaseEventData> dragExit;
    private void Awake()
    {
     
       
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

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
        }
    }


    void OnDragEnter()
    {
        if (!item.used) return;
        GameInstance.Instance.inventorySystem.draggedItem = Instantiate(GameInstance.Instance.inventorySystem.draggingItem);
        GameInstance.Instance.inventorySystem.draggedItem.GetComponent<Image>().sprite = itemImage.sprite;
        GameInstance.Instance.inventorySystem.draggedItem.GetComponent<RectTransform>().SetParent(GameInstance.Instance.inventorySystem.border);
        //image.sprite = originImage;
        itemImage.sprite = GameInstance.Instance.inventorySystem.defaultSlot; 
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
                if (s.slotType == SlotType.JustView) break;
                ItemStruct tempStruct = s.item;
                s.item = item;
                item = tempStruct;

                UpdateSlot();
                s.UpdateSlot();

                break;
            }

        }

    }

    void OnHoverEnter()
    {
        if (!item.used) return;
        info = Instantiate(GameInstance.Instance.inventorySystem.info);
        info.GetComponent<RectTransform>().SetParent(GameInstance.Instance.inventorySystem.border);
        info.UpdateSlotInfo(item);
   //     info
        if(self.position.y - 300 > 200)
        {
            if(self.position.x + 150 > 1600) info.GetComponent<RectTransform>().position = new Vector3(self.position.x - 150, self.position.y - 300, 0);
            else info.GetComponent<RectTransform>().position = new Vector3(self.position.x + 150, self.position.y - 300, 0);
        }
        else
        {
            if (self.position.x + 150 > 1600) info.GetComponent<RectTransform>().position = new Vector3(self.position.x - 150, self.position.y + 300, 0);
            else info.GetComponent<RectTransform>().position = new Vector3(self.position.x + 150, self.position.y + 300, 0);
        }
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

    public void AddItem(ItemStruct item)
    {
        this.item = item;

        UpdateSlot();
    //    if (!item.used) itemImage.sprite = originImage;
     //   else itemImage.sprite = item.image;
    }

    public void RemoveItem()
    {

    }

    public void UpdateSlot()
    {

        if(item.used)
        itemImage.sprite = item.image;
        else
        {
            itemImage.sprite = originImage;
        }

        switch (slotType)
        {
            case SlotType.None:
                break;
            default:
                GameInstance.Instance.inventorySystem.UpdateEquipSlot(slotType, item);
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
