using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AbilityData : MonoBehaviour
{
    RectTransform rectTransform;
    public RectTransform GetRectTransform { get {  if(rectTransform == null) rectTransform = GetComponent<RectTransform>(); return rectTransform; } }


    public Button btn;
    public Image icon;
    public TMP_Text text;
    public ItemStruct item;

    UnityAction btnAction;
    public bool learn;

    private void OnEnable()
    {
        if (btnAction == null) btnAction = Ability;
        btn.onClick.AddListener(btnAction);
    }

    private void OnDisable()
    {
        btn.onClick.RemoveListener(btnAction);
    }

    void Ability()
    {
        if (learn)
        {
            GameInstance.Instance.characterAbilitySystem.RemoveAbility(this);
        }
        else
        {
            GameInstance.Instance.characterAbilitySystem.AddAbility(this);
        }
    }

    public void Setup(ItemStruct item, bool learn)
    {
        this.item = item;
        icon.sprite = item.image;
        text.text = item.item_name;
        this.learn = learn;
    }
}
