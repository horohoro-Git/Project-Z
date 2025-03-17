using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightHand : MonoBehaviour
{
    TrailRenderer trailRenderer;
   // WeaponTrail weaponTrail;
    public TrailRenderer GetTrailRenderer {  get { if (trailRenderer == null) trailRenderer = GetComponent<TrailRenderer>(); return trailRenderer; } }
    public BoxCollider boxCollider;

    bool attacking = false;
    int damage;
    public void Attack(int damage)
    {
        boxCollider.enabled = true;
        attacking = true;
        this.damage = damage;
    }

    public void Trail(bool on)
    {
        GetTrailRenderer.enabled = on;
    }

    public void StopAttack()
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
