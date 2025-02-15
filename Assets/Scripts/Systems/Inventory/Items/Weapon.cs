using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Item
{
    public WeaponType type;
    public bool attack;


    public void StartAttack()
    {
        attack = true;
    }

    public void StopAttack()
    {
        attack = false;
    }
}
