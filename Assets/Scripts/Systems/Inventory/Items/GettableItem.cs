using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GettableItem : Item, IIdentifiable
{
    public int ID { get; set; }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController pc = other.GetComponent<PlayerController>();
            if (pc)
            {
                pc.RegisterAction(GetItem);
                
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
                pc.RemoveAction(GetItem);
                
            }
        }
    }


    void GetItem(PlayerController pc)
    {
        Action<PlayerController> action = (player) => GetItemComplete(player);
        pc.GetItem_Animation(action);
    }


    void GetItemComplete(PlayerController pc)
    {
        if(!GameInstance.Instance.inventorySystem.AddItem(this)) return;
        GameInstance.Instance.worldGrids.RemoveObjects(GetInstanceID(), MinimapIconType.Object);
        pc.RemoveAction(GetItem);
        Destroy(this.gameObject);
    }

    public void Spawned(bool load)
    {
        GameInstance.Instance.worldGrids.AddObjects(this.gameObject, MinimapIconType.Object, load);
        if (GetItemInteractionColider != null)
        {
            GetItemInteractionColider.enabled = true;
            GetItemInteractionColider.isTrigger = true;
            GetItemInteractionColider.excludeLayers &= 0b1111111111110111;
        }
    }
}
