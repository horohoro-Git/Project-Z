using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axe : Weapon
{
    private Collider[] hitColliders;

    private void FixedUpdate()
    {
        if (!attack) return;
        Vector3 boxCenter = weaponColider.transform.position;
        Vector3 boxSize = weaponColider.bounds.size;

        hitColliders = Physics.OverlapBox(boxCenter, boxSize);

        foreach (var collider in hitColliders)
        {
            if (collider.CompareTag("Tree"))
            {
                collider.gameObject.GetComponent<Tree>().ChopDown(transform, equippedPlayer);
            }

            int layer = equippedPlayer.gameObject.layer;

            IDamageable damageable = collider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                if (damageable.Damaged(weaponStruct.attack_damage, layer))
                {
                    attack = false;
                    return;
                }
            }
        }

    }
    /*private void OnTriggerEnter(Collider other)
    {
        if (!attack) return;
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
            attack = false;
            damageable.Damaged(weaponStruct.attack_damage);
        }
    }*/
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
