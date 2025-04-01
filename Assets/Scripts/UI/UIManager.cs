using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GraphicRaycaster graphicRaycaster;

    public Canvas canvas;
    [SerializeField]
    GameObject menuUI;
    [SerializeField]
    GameObject housingSystemUI;
    [SerializeField]
    GameObject inventoryUI;
    [SerializeField]
    GameObject quickSlotUI;
    [SerializeField]
    GameObject abilityMenuUI;
    [SerializeField]
    GameObject boxInventoryUI;
    [SerializeField]
    GameObject achievementUI;
    public GameObject acceptanceUI;

    UIType uiType = UIType.None;

    Dictionary<UIType, GameObject> uiDictionary = new Dictionary<UIType, GameObject>(); 
    private void Awake()
    {
        GameInstance.Instance.uiManager = this;
    }
    void Start()
    {
        RectTransform rectTransform = canvas.GetComponent<RectTransform>();
        CreateUI(housingSystemUI, rectTransform, UIType.Housing);
        CreateUI(inventoryUI, rectTransform, UIType.Inventory);
        CreateUI(abilityMenuUI, rectTransform, UIType.AbilityMenu);
        CreateUI(quickSlotUI, rectTransform, UIType.QuickSlot);
        CreateUI(boxInventoryUI, rectTransform, UIType.BoxInventory);
        CreateUI(achievementUI, rectTransform, UIType.Achievement);
        CreateUI(menuUI, rectTransform, UIType.Menu);
        SwitchUI(UIType.QuickSlot, true);
    }

    void CreateUI(GameObject ui, RectTransform target, UIType type)
    {
        GameObject uiGO = Instantiate(ui);
        RectTransform rect = uiGO.GetComponent<RectTransform>();
        rect.SetParent(target);
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = new Vector2(target.rect.width, target.rect.height);
        uiDictionary.Add(type, uiGO);

        uiGO.GetComponent<IUIComponent>().Setup();
    }

    public bool SwitchUI(UIType type, bool forcedUpdate = false)
    {
        bool returnValue = false;
        if (!(GameInstance.Instance.gameManager.loaded && GameInstance.Instance.assetLoader.assetLoadSuccessful) && !forcedUpdate) return false;
        if (uiType == type && type != UIType.Menu)
        {
            SwitchUI(UIType.QuickSlot);
            return false;
        }
        uiType = type;
        
        foreach (KeyValuePair<UIType, GameObject> ui in uiDictionary)
        {
            if (ui.Key == UIType.Menu) continue;
            if (ui.Key == type)
            {
                ui.Value.SetActive(true);
                returnValue = true;  
            }
            else ui.Value.SetActive(false);
        }

        return returnValue;
    }


}
