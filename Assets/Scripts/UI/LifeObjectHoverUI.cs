using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeObjectHoverUI : MonoBehaviour
{
    RectTransform rectTransform;
    RectTransform GetRectTransform {  get { if (rectTransform == null) rectTransform = GetComponent<RectTransform>(); return rectTransform; } }


    public LifeObjectHoverInfoUI hoverUI;

    void Awake()
    {
        GameInstance.Instance.lifeObjectHoverUI = this;
    }
    public void Start()
    {
       
        
    }
/*    public void Hover(string objectName, int maxHP, int hp)
    {
        hoverUI.gameObject.SetActive(true);
        hoverUI.Setup(objectName, maxHP, hp);
    }

    public void Hide()
    {
        hoverUI.gameObject.SetActive(false);
    }*/
}
