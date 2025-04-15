using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class CharacterProfileUI : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    [NonSerialized]
    public UMACharacterAvatar avatar;

    public RenderTexture renderTexture;

    public RawImage characterImage;

  /*  EventTrigger eventTrigger;
    EventTrigger.Entry dragStart;
    EventTrigger.Entry dragEnd;
    UnityAction<BaseEventData> dragEnter;
    UnityAction<BaseEventData> dragExit;*/
    bool dragModel;
    Vector3 dragPosition = Vector3.zero;
    Vector3 positionDelta;

    private void Awake()
    {
        GameInstance.Instance.characterProfileUI = this;
    }

    private void OnEnable()
    {
    /*    if (eventTrigger == null)
        {
            eventTrigger = gameObject.AddComponent<EventTrigger>();
            dragStart = new EventTrigger.Entry();
            dragStart.eventID = EventTriggerType.PointerDown;
            dragEnter = (eventData) => OnDragEnter();

            dragEnd = new EventTrigger.Entry();
            dragEnd.eventID = EventTriggerType.PointerUp;
            dragExit = (eventData) => OnDragExit();
        }
        //드래그 시작
        dragStart.callback.AddListener(dragEnter);
        eventTrigger.triggers.Add(dragStart);

        //드래그 종료
        dragEnd.callback.AddListener(dragExit);
        eventTrigger.triggers.Add(dragEnd);*/
    }

    private void OnDisable()
    {
   /*     if(eventTrigger != null)
        {
            dragStart.callback.RemoveListener(dragEnter);
            eventTrigger.triggers.Remove(dragStart);

            dragEnd.callback.RemoveListener(dragExit);
            eventTrigger.triggers.Remove(dragEnd);

        }
   */
        if(avatar != null)
        {
            avatar.transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }

    private void Update()
    {
        if(dragModel)
        {
            dragPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            float x = positionDelta.x - dragPosition.x;

            avatar.transform.Rotate(0, x * 100, 0);

            positionDelta = dragPosition;
        }
    }

    void OnDragEnter()
    {
        dragModel = true;
        positionDelta = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Debug.Log("Enter");

    }

    void OnDragExit()
    {
        dragModel= false;
        Debug.Log("Exit");
    }

    public void CreateCharacter(bool load, GameObject characterGO)
    {
        if (!load) Destroy(avatar.gameObject);

        if(load)
        {
            GameObject newCamera = new GameObject();

            Camera camera = newCamera.AddComponent<Camera>();
            camera.targetDisplay = 2;
            camera.targetTexture = renderTexture;   
            newCamera.transform.position = new Vector3(1000, 2.12f, 998.4f);
            newCamera.transform.rotation = Quaternion.Euler(30, 0, 0);

            characterImage.texture = camera.targetTexture;
        }

        avatar = Instantiate(characterGO).GetComponent<UMACharacterAvatar>();

        avatar.transform.position = new Vector3(1000, 0, 1000);
        avatar.transform.rotation = Quaternion.Euler(0, 180, 0);
        
    
    }
    public void AddCloth(int recipeIndex)
    {
        avatar.AddCloth(recipeIndex);
    }
    public void RemoveCloth(string slotName)
    {
        avatar.RemoveCloth(slotName);
    }

    public void EquipItem(GameObject itemObject)
    {
        avatar.AddItem(itemObject);
    }

    public void UnEquipItem()
    {
        avatar.RemoveItem();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OnDragExit();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDragEnter();
    }
}
