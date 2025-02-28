using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerStruct playerStruct;

    public List<LevelData> levelData = new List<LevelData>();
    public void GetDamage(int damage)
    {
        playerStruct.hp -= damage;
        GameInstance.Instance.playerStatusUI.ChangeHP(playerStruct.hp);
    }

    public void SpendEnergy(int energy)
    {
        playerStruct.energy -= energy;
        GameInstance.Instance.playerStatusUI.ChangeEnergy(playerStruct.energy);
    }

    public void GetExperience(int experience)
    {
        playerStruct.exp += experience;

        if(playerStruct.exp >= playerStruct.requireEXP)
        {
            playerStruct.exp -= playerStruct.requireEXP;

            //다음 레벨의 경험치표로 변경

            playerStruct.level++;
            playerStruct.skillPoint++;
            playerStruct.requireEXP = levelData[playerStruct.level - 1].exp;

            GameInstance.Instance.playerStatusUI.LevelUp(playerStruct.level, playerStruct.exp, playerStruct.requireEXP);
        }

        GameInstance.Instance.playerStatusUI.GetEXP(playerStruct.exp);
    }

    public void UpdatePlayer()
    {
        levelData = SaveLoadSystem.GetLevelData();
        playerStruct.requireEXP = levelData[playerStruct.level - 1].exp;

        GameInstance.Instance.playerStatusUI.UpdateUI(playerStruct);
    }
}
