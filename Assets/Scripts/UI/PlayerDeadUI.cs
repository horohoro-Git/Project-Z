using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerDeadUI : MonoBehaviour, IUIComponent
{
    public TMP_Text text;

    public void Setup(bool init)
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
    }

    // Update is called once per frame
    void Update()
    {
        float a = text.color.a;
        if(a >= 1)
        {
            
            text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
            GameInstance.Instance.uiManager.SwitchUI(UIType.QuickSlot, true);
        }
        else
        {
            a += Time.deltaTime * 0.1f;
            text.color = new Color(text.color.r, text.color.g, text.color.b, a);
        }
    }
}
