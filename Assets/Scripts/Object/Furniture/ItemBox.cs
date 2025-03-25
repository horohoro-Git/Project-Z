using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBox : MonoBehaviour
{
    ItemStruct[,] itemData = new ItemStruct[7, 10];
    WeaponStruct[,] weaponData = new WeaponStruct[7, 10];
    ConsumptionStruct[,] consumptionData = new ConsumptionStruct[7, 10];
    ArmorStruct[,] armorData = new ArmorStruct[7, 10];
    bool open;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController pc = other.GetComponent<PlayerController>();
            if (pc)
            {
                pc.RegisterAction(OpenBox);

            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController pc = other.GetComponent<PlayerController>();
            if (pc)
            {
                pc.RemoveAction(OpenBox);

            }
        }
    }

    void OpenBox(PlayerController pc)
    {
     
        if (!open)
        {
            open = true;
            GameInstance.Instance.uiManager.SwitchUI(UIType.BoxInventory, false);

            GameInstance.Instance.boxInventorySystem.currentItemBox = this;
            GameInstance.Instance.boxInventorySystem.NewBox(itemData, weaponData, consumptionData, armorData);
        }
        else
        { 
            if(GameInstance.Instance.uiManager.SwitchUI(UIType.BoxInventory, false))
            {
                GameInstance.Instance.boxInventorySystem.currentItemBox = this;
                GameInstance.Instance.boxInventorySystem.NewBox(itemData, weaponData, consumptionData, armorData);
            }
            else
            {
                GameInstance.Instance.boxInventorySystem.currentItemBox = null;

            }
              
            //GameInstance.Instance.boxInventorySystem.NewBox();
        }
    }
    private void OnDisable()
    {
        open = false;
    }

    private void OnDestroy()
    {
        for (int i = 0; i < 7; i++)
        {
            for(int j = 0; j < 10; j++)
            {
                itemData[i,j].Clear();
            }
        }
    }

    public void AddItem(ItemStruct item, WeaponStruct weapon, ConsumptionStruct consumption, ArmorStruct armor, int slotX, int slotY)
    {
        itemData[slotX,slotY] = item;
        weaponData[slotX,slotY] = weapon;
        armorData[slotX,slotY] = armor;
        consumptionData[slotX,slotY] = consumption;
    }
}
