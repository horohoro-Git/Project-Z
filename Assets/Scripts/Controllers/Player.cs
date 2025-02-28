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
        GameInstance.Instance.playerStatusUI.ChangeHP(playerStruct.hp);
    }

    public void GetExperience(int experience)
    {
        playerStruct.exp += experience;

        if(playerStruct.exp >= playerStruct.requireEXP)
        {
            playerStruct.exp -= playerStruct.requireEXP;

            //다음 레벨의 경험치표로 변경
            //playerStruct.requireEXP = 

            playerStruct.level++;
            playerStruct.skillPoint++;

            GameInstance.Instance.playerStatusUI.LevelUp();


        }

        GameInstance.Instance.playerStatusUI.GetEXP(playerStruct.exp);
    }

    public void UpdatePlayer()
    {
        GameInstance.Instance.playerStatusUI.UpdateUI(playerStruct);
    }
}
