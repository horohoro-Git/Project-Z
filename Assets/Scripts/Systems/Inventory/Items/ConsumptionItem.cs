using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumptionItem : Item
{
    public ConsumptionType consumtionType;
    public ConsumptionStruct consumtionStruct;
    public void Consumption()
    {
        if (equippedPlayer.playerStruct.hp == 0) return;
        equippedPlayer.GetBuff(this);

        PlayerController pc = equippedPlayer.GetComponent<PlayerController>();
        GameInstance.Instance.inventorySystem.inventoryArray[0, pc.equipSlotIndex].RemoveItem();

        SaveLoadSystem.SaveInventoryData();
        Destroy(this.gameObject);
    }
}
