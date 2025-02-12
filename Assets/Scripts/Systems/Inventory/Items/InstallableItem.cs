using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InstallableItem : Item, IUIComponent
{

    public Image image;
    public TMP_Text text;
    public void Setup()
    {
        
        image.sprite = item_Image;
        text.text = item_Name;

    }
}
