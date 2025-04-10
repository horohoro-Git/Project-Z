using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
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
    Button upgradeBtn;
    [SerializeField]
    Button achievementBtn;
    Coroutine menuCoroutine;
    [SerializeField]
    Sprite defaultSprite;
    [SerializeField]
    Sprite selectedSprite;

    bool isSpread = false;

    float scrollY;

    float scrollHeight;

    UnityAction menuClick;
    UnityAction createClick;
    UnityAction inventoryClick;
    UnityAction upgradeClick;
    UnityAction achievementClick;

    EventTrigger menuTrigger;
    EventTrigger createTrigger;
    EventTrigger inventoryTrigger;
    EventTrigger upgradeTrigger;
    EventTrigger achievementTrigger;

    EventTrigger.Entry menuEnterEntry;
    EventTrigger.Entry menuExitEntry;
    EventTrigger.Entry createEnterEntry;
    EventTrigger.Entry createExitEntry;
    EventTrigger.Entry inventoryEnterEntry;
    EventTrigger.Entry inventoryExitEntry;
    EventTrigger.Entry upgradeEnterEntry;
    EventTrigger.Entry upgradeExitEntry;
    EventTrigger.Entry achievementEnterEntry;
    EventTrigger.Entry achievementExitEntry;

    // Start is called before the first frame update
    void Start()
    {
        scrollY = rectTransform.rect.height;
        scrollHeight = rectTransform.localPosition.y;
    }

    private void OnEnable()
    {
        //메뉴의 각 버튼 클릭 이벤트
        menuClick = () => MenuWork();
        createClick = () => GameInstance.Instance.uiManager.SwitchUI(UIType.Housing);
        inventoryClick = () => GameInstance.Instance.uiManager.SwitchUI(UIType.Inventory);
        upgradeClick = () => GameInstance.Instance.uiManager.SwitchUI(UIType.AbilityMenu);
        achievementClick = () => GameInstance.Instance.uiManager.SwitchUI(UIType.Achievement);

        menuBtn.onClick.AddListener(menuClick);
        createBtn.onClick.AddListener(createClick);
        inventoryBtn.onClick.AddListener(inventoryClick);
        upgradeBtn.onClick.AddListener(upgradeClick);
        achievementBtn.onClick.AddListener(achievementClick);

        //메뉴 버튼의 호버 기능
        SetupButtonHoverEvent(menuBtn, ref menuTrigger, ref menuEnterEntry, ref menuExitEntry);
        SetupButtonHoverEvent(createBtn, ref createTrigger, ref createEnterEntry, ref createExitEntry);
        SetupButtonHoverEvent(inventoryBtn, ref inventoryTrigger, ref inventoryEnterEntry, ref inventoryExitEntry);
        SetupButtonHoverEvent(upgradeBtn, ref upgradeTrigger, ref upgradeEnterEntry, ref upgradeExitEntry);
        SetupButtonHoverEvent(achievementBtn, ref achievementTrigger, ref achievementEnterEntry, ref achievementExitEntry);
    }

    private void OnDisable()
    {
        //버튼의 이벤트 제거
        menuBtn.onClick.RemoveListener(menuClick);
        createBtn.onClick.RemoveListener(createClick);
        inventoryBtn.onClick.RemoveListener(inventoryClick);
        upgradeBtn.onClick.RemoveListener(upgradeClick);
        achievementBtn.onClick.RemoveListener(achievementClick);

        RemoveButtonHoverEvent(menuBtn, menuTrigger, menuEnterEntry, menuExitEntry);
        RemoveButtonHoverEvent(createBtn, createTrigger, createEnterEntry, createExitEntry);
        RemoveButtonHoverEvent(inventoryBtn, inventoryTrigger, inventoryEnterEntry, inventoryExitEntry);
        RemoveButtonHoverEvent(upgradeBtn, upgradeTrigger, upgradeEnterEntry, upgradeExitEntry);
        RemoveButtonHoverEvent(achievementBtn, achievementTrigger, achievementEnterEntry, achievementExitEntry);
    }

    void SetupButtonHoverEvent(Button btn, ref EventTrigger eventTrigger, ref EventTrigger.Entry enterEntry, ref EventTrigger.Entry exitEntry)
    {
        if(eventTrigger == null) eventTrigger = btn.gameObject.AddComponent<EventTrigger>();
        //호버 시작
        if (enterEntry == null) enterEntry = new EventTrigger.Entry();
        enterEntry.eventID = EventTriggerType.PointerEnter;
        UnityAction<BaseEventData> enterEvent = (data) => OnHoverEnter(btn.GetComponent<Image>());
        enterEntry.callback.AddListener(enterEvent);
        eventTrigger.triggers.Add(enterEntry);

        //호버 종료
        if(exitEntry == null) exitEntry = new EventTrigger.Entry();
        exitEntry.eventID = EventTriggerType.PointerExit;
        UnityAction<BaseEventData> exitEvent = (data) => OnHoverExit(btn.GetComponent<Image>());
        exitEntry.callback.AddListener(exitEvent);
        eventTrigger.triggers.Add(exitEntry);
    }

   
    void RemoveButtonHoverEvent(Button btn, EventTrigger eventTrigger, EventTrigger.Entry enterEntry, EventTrigger.Entry exitEntry)
    {
        EventTrigger trigger = btn.gameObject.GetComponent<EventTrigger>();
        if(trigger == null) return;

        //이벤트 트리거의 호버 기능 제거
        enterEntry.callback.RemoveAllListeners();
        eventTrigger.triggers.Remove(enterEntry);

        exitEntry.callback.RemoveAllListeners();
        eventTrigger.triggers.Remove(exitEntry);
    }

    void MenuWork()
    {
        if (isSpread) Fold();
        else Spread();
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

    public void Setup(bool init)
    {

    }
}
