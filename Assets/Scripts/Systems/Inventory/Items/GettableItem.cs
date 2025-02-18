using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GettableItem : Item
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController pc = other.GetComponent<PlayerController>();
            pc.RegisterAction(GetItem);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController pc = other.GetComponent<PlayerController>();
            pc.RemoveAction(GetItem);
        }
    }


    void GetItem(PlayerController pc)
    {
        Action<PlayerController> action = (player) => GetItemComplete(player);
        pc.GetItem_Animation(action);
    }


    void GetItemComplete(PlayerController pc)
    {
        GameInstance.Instance.inventorySystem.AddItem(this);

        pc.RemoveAction(GetItem);
        Destroy(this.gameObject);
    }
}
