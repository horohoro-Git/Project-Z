using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftHand : MonoBehaviour
{
    TrailRenderer trailRenderer;
    // WeaponTrail weaponTrail;
    public TrailRenderer GetTrailRenderer { get { if (trailRenderer == null) trailRenderer = GetComponent<TrailRenderer>(); return trailRenderer; } }
    public BoxCollider boxCollider;
    private Collider[] hitColliders;

    bool attacking = false;
    int damage;

    private void Start()
    {
        GetTrailRenderer.time = 0.5f;
        GetTrailRenderer.startWidth = 0.2f;
        GetTrailRenderer.endWidth = 0.2f;
    }
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

    private void FixedUpdate()
    {
        if (!attacking) return;
        Vector3 boxCenter = boxCollider.transform.position;
        Vector3 boxSize = boxCollider.bounds.size;

        hitColliders = Physics.OverlapBox(boxCenter, boxSize);

        foreach (var collider in hitColliders)
        {
            int layer = gameObject.layer;

            IDamageable damageable = collider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                if (damageable.Damaged(damage, layer))
                {
                    attacking = false;
                    return;
                }
            }
        }
    }

    void TurnOff()
    {
        attacking = false;
        boxCollider.enabled = false;
    }

 /*   private void OnTriggerEnter(Collider other)
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
    }*/
}
