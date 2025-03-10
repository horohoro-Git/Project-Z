using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Item
{

  //  public Player equippedPlayer;

    public WeaponStruct weaponStruct;
    public bool attack;

    public Collider weaponColider;

    private void Start()
    {
    //    Invoke("ItemLoad", 0.2f);
    }

 /*   void ItemLoad()
    {
        if(!GameInstance.Instance.assetLoader.assetLoadSuccessful)
        {
            Invoke("ItemLoad", 0.2f);

            return;
        }

        weaponStruct = GameInstance.Instance.assetLoader.weapons[itemStruct.item_index];
    }*/

    public void Attack(float start, float end)
    {
        Invoke("StartAttack", start);
        Invoke("StopAttack", end);
    }
    public void StartAttack()
    {
        weaponColider.enabled = true;
    /*    Debug.Log("Start");
        attack = true;*/
    }

    public void StopAttack()
    {
        weaponColider.enabled = false;
      /*  Debug.Log("Stop");
        attack = false;*/
    }
}
