using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axe : Weapon
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Tree"))
        {
           // if (attack)
            {
                other.gameObject.GetComponent<Tree>().ChopDown(transform, equippedPlayer);
            }
        }

        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.Damaged(weaponStruct.attack_damage);
        }
      /*  if(other.gameObject.CompareTag("Enemy"))
        {
            other.gameObject.GetComponent<EnemyController>().BeAttacked(weaponStruct.attack_damage);
        }
        */
    }
 /*   private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Tree"))
        {
        //    if (attack)
            {
                other.gameObject.GetComponent<Tree>().ChopDown(transform);
            }
        }
    }*/
}
