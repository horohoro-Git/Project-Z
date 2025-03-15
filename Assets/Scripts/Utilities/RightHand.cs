using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightHand : MonoBehaviour
{
    WeaponTrail weaponTrail;
    public WeaponTrail GetWeaponTrail {  get { if (weaponTrail == null) weaponTrail = GetComponent<WeaponTrail>(); return weaponTrail; } }
    public BoxCollider boxCollider;

    bool attacking = false;
    int damage;
    public void Attack(float timer, int damage)
    {
        boxCollider.enabled = true;
        attacking = true;
        this.damage = damage;
        Invoke("TurnOff", timer);
    }

    void TurnOff()
    {
        attacking = false;
        boxCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (attacking)
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                attacking = false;
                damageable.Damaged(damage);
            }
        }
    }
}
