using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GettableItem : Item
{

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("AAA");
        if (other.CompareTag("Player"))
        {
            Debug.Log("AAA");
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

        pc.GetItem_Animation();
        GameInstance.Instance.inventorySystem.AddItem(this);
       
        pc.RemoveAction(GetItem);
        Destroy(this.gameObject);
    }
}
