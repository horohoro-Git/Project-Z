using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draged : MonoBehaviour
{
    public ItemStruct item;
    RectTransform rectTransform;
    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        rectTransform.position = Input.mousePosition;
    }
    void Update()
    {
        rectTransform.position = Input.mousePosition;
    }

    private void OnDestroy()
    {
        item.Clear();
    }
}
