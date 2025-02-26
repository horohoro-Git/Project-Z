using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerStruct playerStruct;


    public void GetDamage(int damage)
    {
        playerStruct.hp -= damage;
    }

    public void GetExperience(int experience)
    {
        playerStruct.exp += experience;

        if(playerStruct.exp >= playerStruct.requireEXP)
        {
            playerStruct.level++;
            playerStruct.skillPoint++;
        }
    }

}
