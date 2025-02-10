using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour, IUIComponent
{
    public RectTransform rectTransform;
    [SerializeField]
    Button menuBtn;
    [SerializeField]
    Button createBtn;
    [SerializeField]
    Button inventoryBtn;
    [SerializeField]
    Button a3;
    Coroutine menuCoroutine;
    [SerializeField]
    Sprite defaultSprite;
    [SerializeField]
    Sprite selectedSprite;

    bool isSpread = false;

    float scrollY;

    float scrollHeight;
  
    // Start is called before the first frame update
    void Start()
    {
        scrollY = rectTransform.rect.height;
        scrollHeight = rectTransform.localPosition.y;

        menuBtn.onClick.AddListener(() =>
        {
            if (isSpread) Fold();
            else Spread();
        });

        createBtn.onClick.AddListener(() =>
        {
            GameInstance.Instance.uiManager.SwitchUI(UIType.Housing);
        });
        inventoryBtn.onClick.AddListener(() =>
        {
            GameInstance.Instance.uiManager.SwitchUI(UIType.Inventory);
        });
        a3.onClick.AddListener(() =>
        {
            Debug.Log("Click A3");

        });

        SetupButtonEvent(menuBtn);
        SetupButtonEvent(createBtn);
        SetupButtonEvent(inventoryBtn);

    }

    void SetupButtonEvent(Button btn)
    {
        EventTrigger eventTrigger = btn.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entryHover = new EventTrigger.Entry();
        entryHover.eventID = EventTriggerType.PointerEnter;
        entryHover.callback.AddListener((eventData) => { OnHoverEnter(btn.GetComponent<Image>()); });
        eventTrigger.triggers.Add(entryHover);

        //호버 종료
        EventTrigger.Entry entryExit = new EventTrigger.Entry();
        entryExit.eventID = EventTriggerType.PointerExit;
        entryExit.callback.AddListener((eventData) => { OnHoverExit(btn.GetComponent<Image>()); });
        eventTrigger.triggers.Add(entryExit);
    }

    void OnHoverEnter(Image image)
    {
        image.sprite = selectedSprite;
    }
    void OnHoverExit(Image image)
    {
        image.sprite = defaultSprite;
    }
    void Spread()
    {
        isSpread = true;

        if(menuCoroutine != null) StopCoroutine(menuCoroutine);
        menuCoroutine = StartCoroutine(SpreadMenu());
    }

    void Fold()
    {
        isSpread = false;
        if (menuCoroutine != null) StopCoroutine(menuCoroutine);
        menuCoroutine = StartCoroutine(FoldMenu());
    }

    IEnumerator SpreadMenu()
    {
        Vector2 currentPos2 = rectTransform.sizeDelta;
        Vector3 currentPos3 = rectTransform.localPosition;
        float elapsedTimer = 0;
        while(elapsedTimer / 0.3f <= 1)
        {
            elapsedTimer += Time.unscaledDeltaTime;
            float cal = Mathf.Lerp(currentPos3.y, 0, elapsedTimer / 0.3f);

            Vector3 v = new Vector3(currentPos3.x, cal, currentPos3.z);
            rectTransform.localPosition = v;

            yield return null;
        }

    }
    IEnumerator FoldMenu()
    {
        Vector2 currentPos2 = rectTransform.sizeDelta;
        Vector3 currentPos3 = rectTransform.localPosition;
        float elapsedTimer = 0;
        while (elapsedTimer / 0.3f <= 1)
        {
            elapsedTimer += Time.unscaledDeltaTime;
            float cal = Mathf.Lerp(currentPos3.y, scrollHeight, elapsedTimer / 0.3f);

            Vector3 v = new Vector3(currentPos3.x, cal, currentPos3.z);
            rectTransform.localPosition = v;

            yield return null;
        }

    }

    public void Setup()
    {

    }
}
