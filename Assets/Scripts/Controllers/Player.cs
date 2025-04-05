using System;
using System.Collections;
using System.Collections.Generic;
using UMA;
using UnityEngine;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    public PlayerStruct playerStruct; //플레이어의 능력치
    public PlayerStruct equipmentStats; //장비로 오른 능력치
    float weight;
    Transform transforms;
    public Transform Transforms
    {
        get
        {
            if (transforms == null) transforms = transform;
            return transforms;
        }
    }

  
    GameObject healBuff;
    GameObject levelupBuff;
    Coroutine healCoroutine;
    Coroutine levelupCoroutine;

    BuffStruct buffStruct = new BuffStruct(true);
    float healBuffTimer;

    [NonSerialized]
    public bool dead;

    private void Update()
    {
        if (healBuffTimer < Time.time)
        {
            CharacterAbilityManager characterAbilityManager = GameInstance.Instance.characterAbilityManager;
            if (playerStruct.hp > playerStruct.maxHP + characterAbilityManager.maxHp)
            {
              
                playerStruct.hp = playerStruct.maxHP + characterAbilityManager.maxHp;
            }
        }
    }

    public void GetDamage(int damage)
    {
        CharacterAbilityManager characterAbilityManager = GameInstance.Instance.characterAbilityManager;
        int defense = playerStruct.defense;
        if (defense > damage)
        {
            damage = damage / 2;
        }
        float damagePercent = (float)((float)100.0f / (float)(defense + 100));
        Debug.Log(damagePercent);
        damage = (int)(damagePercent * damage);
        if (damage == 0) damage = 1;
        playerStruct.hp -= damage;
        GameInstance.Instance.playerStatusUI.ChangeHP(playerStruct.hp);
        GameInstance.Instance.playerStatusDetailsUI.UpdateHP(playerStruct.hp, playerStruct.maxHP + characterAbilityManager.maxHp);
    }

    public void SpendEnergy(int energy)
    {
        playerStruct.energy -= energy;
        GameInstance.Instance.playerStatusUI.ChangeEnergy(playerStruct.energy);
        GameInstance.Instance.playerStatusDetailsUI.UpdateEnergy(playerStruct.energy, playerStruct.maxEnergy);
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
            playerStruct.requireEXP = GameInstance.Instance.assetLoader.levelData[playerStruct.level - 1].exp;

            GameInstance.Instance.playerStatusUI.LevelUp(playerStruct.level, playerStruct.exp, playerStruct.requireEXP);
            buffStruct.levelupNums++;
            if (levelupCoroutine == null) levelupCoroutine = StartCoroutine(LevelupEffect());
        }

        GameInstance.Instance.playerStatusUI.GetEXP(playerStruct.exp);
        GameInstance.Instance.playerStatusDetailsUI.UpdateExp(playerStruct.exp, playerStruct.requireEXP);
    }

    public void UpdatePlayer()
    {
        equipmentStats = new PlayerStruct();
        playerStruct.requireEXP = GameInstance.Instance.assetLoader.levelData[playerStruct.level].exp;

        //힐 버프 이펙트
        CreateBuff(ref healBuff, LoadURL.Heal);

        //레벨 업 이펙트
        CreateBuff(ref levelupBuff, LoadURL.LevelUp);

        //  playerStruct.skillPoint += 10;
 
        GameInstance.Instance.playerStatusUI.UpdateUI(playerStruct);
        GameInstance.Instance.abilityMenuUI.GetPoint(playerStruct.skillPoint);
        GameInstance.Instance.abilityMenuUI.ShowChanges(this);
        GameInstance.Instance.playerStatusDetailsUI.Setup(playerStruct);
    }

    public void Renewal()
    {
        GameInstance.Instance.playerStatusUI.UpdateUI(playerStruct);
        GameInstance.Instance.playerStatusDetailsUI.Setup(playerStruct);
    }


    void CreateBuff(ref GameObject go, string name)
    {
        go = Instantiate(AssetLoader.loadedAssets[name]);
        go.transform.SetParent(Transforms);
        go.transform.localPosition = Vector3.zero;
        go.SetActive(false);
    }

    public void GetBuff(ConsumptionItem item)
    {
        Debug.Log(item.consumtionType);
        switch(item.consumtionType)
        {
            case ConsumptionType.None:
                break;
            case ConsumptionType.Heal:
                buffStruct.healBuff.Enqueue(item.consumtionStruct);
                if(healCoroutine == null) healCoroutine = StartCoroutine(HPRecovery(item.consumtionStruct.duration, item.consumtionStruct.heal_amount));
                break;

            case ConsumptionType.EnergyRegain:
                break;

        }
       
    }

    IEnumerator HPRecovery(float duration, int recoveryAmount)
    {
        CharacterAbilityManager characterAbilityManager = GameInstance.Instance.characterAbilityManager;
        healBuff.SetActive(true);
        float currentTimer = 0;
        float timer = 0;
        while (currentTimer / duration < 1f)
        {
            while (timer < 1)
            {
                timer += Time.deltaTime;
                yield return null;
            }
            currentTimer += timer;
            timer = 0;
         
            playerStruct.hp += (int)(recoveryAmount / duration);
            if(playerStruct.hp > playerStruct.maxHP + characterAbilityManager.maxHp)
            {
                playerStruct.hp = playerStruct.maxHP + characterAbilityManager.maxHp;
                healBuffTimer = Time.time + 10f;
            }
            GameInstance.Instance.playerStatusUI.ChangeHP(playerStruct.hp);
            GameInstance.Instance.playerStatusDetailsUI.UpdateHP(playerStruct.hp, playerStruct.maxHP + characterAbilityManager.maxHp);
        }

        buffStruct.healBuff.Dequeue();

        if(buffStruct.healBuff.Count > 0)
        {
            ConsumptionStruct consumtionStruct = buffStruct.healBuff.Peek();
            healCoroutine = StartCoroutine(HPRecovery(consumtionStruct.duration, consumtionStruct.heal_amount));
        }

        healBuff.SetActive(false);
        healCoroutine = null;
    }

    IEnumerator LevelupEffect()
    {
        levelupBuff.SetActive(true);
        float timer = 0;
        while (timer < 2)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        buffStruct.levelupNums--;

        if(buffStruct.levelupNums > 0)
        {
            levelupCoroutine = StartCoroutine(LevelupEffect());
        }
        levelupBuff.SetActive(false);
        levelupBuff = null;
    }

    public void WearingArmor(ArmorStruct armorStruct, bool backpack)
    {
        if (!backpack)
        {
            switch (armorStruct.armor_type)
            {
                case SlotType.None:
                    break;
                case SlotType.Head:
                case SlotType.Chest:
                case SlotType.Arm:
                case SlotType.Leg:
                case SlotType.Foot:
                    playerStruct.defense += armorStruct.defense;
                    equipmentStats.defense += armorStruct.defense;
                    GameInstance.Instance.playerStatusDetailsUI.UpdateDefense(playerStruct.defense);

                    if (armorStruct.armor_type == SlotType.Arm)
                    {
                        playerStruct.attackDamage += armorStruct.attack_damage;
                        equipmentStats.attackDamage += armorStruct.attack_damage;
                        GameInstance.Instance.playerStatusDetailsUI.UpdateDamage(playerStruct.attackDamage);
                    }
                    if (armorStruct.armor_type == SlotType.Foot)
                    {
                        playerStruct.moveSpeed += armorStruct.move_speed;
                        equipmentStats.moveSpeed += armorStruct.move_speed;
                        GameInstance.Instance.playerStatusDetailsUI.UpdateMoveSpeed(playerStruct.moveSpeed);

                    }
                    break;
            }
        }
        else
        {
            GameInstance.Instance.inventorySystem.ExpandSlot(armorStruct.carrying_capacity);
            GameInstance.Instance.boxInventorySystem.Extends(armorStruct.carrying_capacity);

        }
    }

    public void PutonArmor(ArmorStruct armorStruct)
    {
        switch (armorStruct.armor_type)
        {
            case SlotType.None:
                break;
            case SlotType.Head: case SlotType.Chest: case SlotType.Arm: case SlotType.Leg: case SlotType.Foot:
                playerStruct.defense -= armorStruct.defense;
                equipmentStats.defense -= armorStruct.defense;
                GameInstance.Instance.playerStatusDetailsUI.UpdateDefense(playerStruct.defense);
                if(armorStruct.armor_type == SlotType.Arm)
                {
                    playerStruct.attackDamage -= armorStruct.attack_damage;
                    equipmentStats.attackDamage -= armorStruct.attack_damage;
                    GameInstance.Instance.playerStatusDetailsUI.UpdateDamage(playerStruct.attackDamage);
                }
                if (armorStruct.armor_type == SlotType.Foot)
                {
                    playerStruct.moveSpeed -= armorStruct.move_speed;
                    equipmentStats.moveSpeed -= armorStruct.move_speed;
                    GameInstance.Instance.playerStatusDetailsUI.UpdateMoveSpeed(playerStruct.moveSpeed);
                }
                break;
        }
    }
}
