using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public int itemIndex;
    public Sprite item_Image;
    public string item_Name;
    public SlotType item_Slot;
    public ItemType item_Type;
    public GameObject item_GO;
    
    void Start()
    {
      //  Invoke("LateStart", 1f);
    }


    void LateStart()
    {
    //    GameInstance.Instance.inventorySystem.AddItem(this);

    }
}
