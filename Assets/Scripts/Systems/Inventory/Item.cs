using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public Sprite item_Image;
    public string item_Name;
    public SlotType item_Type;


    
    void Start()
    {
        Invoke("LateStart", 0.5f);
    }


    void LateStart()
    {
      //  GameInstance.Instance.inventorySystem.AddItem(this);

    }
}
