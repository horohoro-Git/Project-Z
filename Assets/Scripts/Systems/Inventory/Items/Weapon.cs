using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Item
{

  //  public Player equippedPlayer;
    public WeaponType type;
    public bool attack;

    public Collider weaponColider;
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
