using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    public PlayerStruct playerStruct;

    Transform transforms;
    public Transform Transforms
    {
        get
        {
            if (transforms == null) transforms = transform;
            return transforms;
        }
    }

    [NonSerialized]
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

            //���� ������ ����ġǥ�� ����

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
      //  playerStruct.skillPoint += 10;
        GameInstance.Instance.playerStatusUI.UpdateUI(playerStruct);
        GameInstance.Instance.abilityMenuUI.GetPoint(playerStruct.skillPoint);
        GameInstance.Instance.abilityMenuUI.ShowChanges(this);
    }
}
