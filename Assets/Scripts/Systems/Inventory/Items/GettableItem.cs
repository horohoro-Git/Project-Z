using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GettableItem : Item, IIdentifiable
{
    public string ID { get; set; }

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
        GameInstance.Instance.worldGrids.RemoveObjects(this);
        pc.RemoveAction(GetItem);
        Destroy(this.gameObject);
    }

    public void Start()
    {
        Invoke("AA", 0.5f);
    }

    void AA()
    {
        GameInstance.Instance.worldGrids.AddObjects(this.gameObject);
    }
}
