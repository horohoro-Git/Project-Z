using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AcceptanceUI : MonoBehaviour, IUIComponent
{
    [SerializeField]
    Button okBtn;
    [SerializeField]
    Button cancelBtn;
    public void Setup()
    {
    }


    public void GetAction(UnityAction action)
    {
        okBtn.onClick.AddListener(action);
        okBtn.onClick.AddListener(() => DestoryUI());
      
        cancelBtn.onClick.AddListener(() => DestoryUI());
    
    }
    
    void DestoryUI()
    {
        okBtn.onClick.RemoveAllListeners();
        cancelBtn.onClick.RemoveAllListeners();
        Destroy(this.gameObject);
    }
}
