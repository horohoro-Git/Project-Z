using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LifeObjectHoverInfoUI : MonoBehaviour
{
    RectTransform rectTransform;
    public RectTransform GetRectTransform { get { if (rectTransform == null) rectTransform = GetComponent<RectTransform>(); return rectTransform; } }

    public TMP_Text nameText;
    public Image hpBar;
    public void Setup(string objectName, int objectMaxHP, int objectHP)
    {
        nameText.text = objectName;

        hpBar.fillAmount = (float)objectHP / objectMaxHP;
    }

}
