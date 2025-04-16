using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    /*public int itemIndex;
    public Sprite item_Image;
    public string item_Name;
    public SlotType item_Slot;
    public ItemType item_Type;
    public GameObject item_GO;*/

    public ItemStruct itemStruct;
   // public WeaponStruct weaponStruct;
   // public ConsumptionStruct consumptionStruct;

    public Player equippedPlayer;

    Collider itemInteractionColider;

    public Collider GetItemInteractionColider {   get { if (itemInteractionColider == null) itemInteractionColider = GetComponent<CapsuleCollider>(); return itemInteractionColider; } }


    private void OnDestroy()
    {
        itemStruct.Clear();
    }
}
