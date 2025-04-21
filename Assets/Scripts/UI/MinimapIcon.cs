using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapIcon : MonoBehaviour
{
    [NonSerialized]
    public int ID;
    public Image image;

    RectTransform rectTransforms;
    public RectTransform GetRectTransform { get { if (rectTransforms == null) rectTransforms = GetComponent<RectTransform>(); return rectTransforms; } }

    private void OnDestroy()
    {
        image = null;
    }
}
