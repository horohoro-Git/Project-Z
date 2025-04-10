using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickSlotUI : MonoBehaviour, IUIComponent
{
    public List<Slot> slots = new List<Slot>();
    public void Setup(bool init)
    {
        GameInstance.Instance.quickSlotUI = this;

        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].slotY = i;
            slots[i].Setup(init);
        }
    }

    public void UpdateSlot(ItemStruct item, WeaponStruct weaponStruct, ConsumptionStruct consumptionStruct, ArmorStruct armorStruct, int index, bool justUpdate)
    {
        slots[index].AddItem(item, weaponStruct, consumptionStruct, armorStruct, justUpdate);
    }
}
