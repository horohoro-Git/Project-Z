using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    Button slotBtn;
    RectTransform self;
    Image image;
    GameObject info;
    Sprite originImage;
    private void Awake()
    {
     
        image = GetComponent<Image>();
        originImage = image.sprite;
        slotBtn = GetComponent<Button>();
        self = GetComponent<RectTransform>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
        EventTrigger eventTrigger = gameObject.AddComponent<EventTrigger>();

        //ȣ��
        EventTrigger.Entry entryHover = new EventTrigger.Entry();
        entryHover.eventID = EventTriggerType.PointerEnter;
        entryHover.callback.AddListener((eventData) => { OnHoverEnter(); });
        eventTrigger.triggers.Add(entryHover);

        //ȣ�� ����
        EventTrigger.Entry entryExit = new EventTrigger.Entry();
        entryExit.eventID = EventTriggerType.PointerExit;
        entryExit.callback.AddListener((eventData) => { OnHoverExit(); });
        eventTrigger.triggers.Add(entryExit);

        //�巡�� ����
        EventTrigger.Entry dragStart = new EventTrigger.Entry();
        dragStart.eventID = EventTriggerType.PointerDown;
        dragStart.callback.AddListener((eventData) => { OnDragEnter(); });
        eventTrigger.triggers.Add(dragStart);

        //�巡�� ����
        EventTrigger.Entry dragEnd = new EventTrigger.Entry();
        dragEnd.eventID = EventTriggerType.PointerUp;
        dragEnd.callback.AddListener((eventData) => { OnDragExit(); });
        eventTrigger.triggers.Add(dragEnd);
    }

    void OnDragEnter()
    {
        Debug.Log("A");
        GameInstance.Instance.inventorySystem.draggedItem = Instantiate(GameInstance.Instance.inventorySystem.draggingItem);
        GameInstance.Instance.inventorySystem.draggedItem.GetComponent<Image>().sprite = image.sprite;
        GameInstance.Instance.inventorySystem.draggedItem.GetComponent<RectTransform>().SetParent(GameInstance.Instance.inventorySystem.border);
        image.sprite = originImage;

    }

    void OnDragExit()
    {
        if(GameInstance.Instance.inventorySystem.draggedItem != null)
        {
            image.sprite = GameInstance.Instance.inventorySystem.draggedItem.GetComponent<Image>().sprite;
            Destroy(GameInstance.Instance.inventorySystem.draggedItem.gameObject);
           
         
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = Input.mousePosition;

            // Raycast ����� ������ ����Ʈ ����
            List<RaycastResult> raycastResult = new List<RaycastResult>();

            // Raycast ����
            GameInstance.Instance.inventorySystem.graphicRaycaster.Raycast(pointerData, raycastResult);

            // ����ĳ��Ʈ ��Ʈ�� ��ü�� �ִ��� Ȯ��
            if (raycastResult.Count > 0)
            {
                foreach (RaycastResult r in raycastResult)
                {
                    if(r.gameObject.name == "item")
                    {
                        Debug.Log("��Ʈ�� UI ��ü: " + r.gameObject.name);
                    }
                }
         
            }
            else
            {
                Debug.Log("UI ��ü�� ��Ʈ���� ����.");
            }
        }
    }

    void OnHoverEnter()
    {
        info = Instantiate(GameInstance.Instance.inventorySystem.info);
        info.GetComponent<RectTransform>().SetParent(GameInstance.Instance.inventorySystem.border);

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
     //   Debug.Log("ȣ�� ����");
    }
}
