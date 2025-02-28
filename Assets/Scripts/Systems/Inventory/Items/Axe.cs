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
