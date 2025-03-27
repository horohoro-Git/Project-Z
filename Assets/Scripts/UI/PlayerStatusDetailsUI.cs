using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerStatusDetailsUI : MonoBehaviour
{
    public TMP_Text hp;
    public TMP_Text defense;
    public TMP_Text damage;
    public TMP_Text attackSpeed;
    public TMP_Text moveSpeed;
    public TMP_Text weight;
    public TMP_Text energy;
    public TMP_Text exp;

    private void Awake()
    {
        GameInstance.Instance.playerStatusDetailsUI = this;
    }
   
    public void Setup(PlayerStruct player)
    {
        CharacterAbilityManager characterAbility = GameInstance.Instance.characterAbilityManager;
        UpdateHP(player.hp, player.maxHP + characterAbility.maxHp);
        UpdateDefense(player.defense);
        UpdateDamage(player.attackDamage);
        UpdateAttackSpeed(player.attackSpeed);
        UpdateMoveSpeed(player.moveSpeed);
        UpdateWeight(player.weight);
        UpdateEnergy(player.energy, player.maxEnergy);
        UpdateExp(player.exp, player.requireEXP);
    }
    public void UpdateHP(int currentHP, int maxHP)
    {
        this.hp.text = currentHP.ToString() + " / " + maxHP.ToString();
    }
    public void UpdateDefense(int defense)
    {
        this.defense.text = defense.ToString();
    }

    public void UpdateDamage(int damage)
    {
        this.damage.text = damage.ToString();
    }
    public void UpdateAttackSpeed(float attackSpeed)
    {
        this.attackSpeed.text = attackSpeed.ToString("F1");
    }
    public void UpdateMoveSpeed(float moveSpeed)
    {
        this.moveSpeed.text = moveSpeed.ToString("F1");
    }
    public void UpdateWeight(int weight)
    {
        this.weight.text = weight.ToString();
    }
    public void UpdateEnergy(int currentEnergy, int maxEnergy)
    {
        this.energy.text = currentEnergy.ToString() + " / " + maxEnergy.ToString();
    }
    public void UpdateExp(int currentEXP, int requireEXP)
    {
        this.exp.text = currentEXP.ToString() + " / " + requireEXP.ToString();

    }
}
