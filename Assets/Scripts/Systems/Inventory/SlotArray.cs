using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotArray : MonoBehaviour, IUIComponent
{
    public List<Slot> slots;

 //   [NonSerialized] 
    public int slotIndex = 0;

    public void Setup()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].Setup();
        }
    }

    private void Awake()
    {
       
    }
    void Start()
    {
     /*   Debug.Log(transform.position);
        SlotArray[] arr = GameObject.FindObjectsByType<SlotArray>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        int i = 0;
        foreach (SlotArray array in arr)
        {
            if (array == this)
            {
                slotIndex = i;
            }
            i++;
        }*/
        //�κ��丮�� ���� �߰�
        //GameInstance.Instance.inventorySystem.InventoryExtends(this);
    }
}
