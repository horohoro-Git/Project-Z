using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axe : Weapon
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Tree"))
        {
            if (attack)
            {
                other.gameObject.GetComponent<Tree>().ChopDown(transform);


                Debug.Log("Attack");
            }
        }
        
    }

    public void EndAttack()
    {
        Invoke("StopAttack", 2.4f);
    }
}
