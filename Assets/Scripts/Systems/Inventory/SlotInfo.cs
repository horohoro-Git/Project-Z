using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotInfo : MonoBehaviour
{

    public Image image;
    public TMP_Text text;

    [SerializeField]
    GameObject weapon_info;
    [SerializeField]
    GameObject consumption_info;
    [SerializeField]
    GameObject armor_info;

    //무기 정보
    [SerializeField]
    TMP_Text weaponDamage_Text;
    [SerializeField]
    TMP_Text weaponSpeed_Text;
    [SerializeField]
    TMP_Text weaponDurability_Text;

    //소비 정보
    [SerializeField]
    GameObject consumption_heal;
    [SerializeField]
    GameObject consumption_energy;
    [SerializeField]
    TMP_Text healAmount_Text;
    [SerializeField]
    TMP_Text energyRegain_Text;
    [SerializeField]
    TMP_Text duration_Text;

    //방어구 정보
    [SerializeField]
    GameObject armor_Defense;
    [SerializeField]
    GameObject armor_durability;
    [SerializeField]
    GameObject armor_moveSpeed;
    [SerializeField]
    GameObject armor_attackDamage;
    [SerializeField]
    TMP_Text armorDefense_Text;
    [SerializeField]
    TMP_Text armorDurability_Text;
    [SerializeField]
    TMP_Text armorMoveSpeed_Text;
    [SerializeField]
    TMP_Text armorAttackDamage_Text;

    [SerializeField]
    TMP_Text ammo;

    [SerializeField]
    TMP_Text carrySize;

    public void UpdateSlotInfo(ItemStruct itemStruct, ConsumptionStruct consumptionStruct, WeaponStruct weaponStruct, ArmorStruct armorStruct)
    {
        text.text = itemStruct.item_name;
        image.sprite = GameInstance.Instance.assetLoader.loadedSprites[AssetLoader.spriteAssetkeys[itemStruct.item_index - 1]];

        switch (itemStruct.item_type)
        {
            case ItemType.None:
                weapon_info.SetActive(false);
                consumption_info.SetActive(false);
                armor_info.SetActive(false);
                break;
            case ItemType.Consumable:
                ObjectActive(consumption_info, weapon_info, armor_info);
                ConsumptionInfo(consumptionStruct);
                break;
            case ItemType.Equipmentable:
                ObjectActive(weapon_info, armor_info, consumption_info);
                WeaponInfo(weaponStruct);
                break;
            case ItemType.Wearable:
                ObjectActive(armor_info, consumption_info, weapon_info);
                ArmorInfo(armorStruct);
                break;
        }
    }

    void ObjectActive(GameObject activeObject, GameObject deactiveObject1, GameObject deactiveObject2)
    {
        activeObject.SetActive(true);
        deactiveObject1.SetActive(false);
        deactiveObject2.SetActive(false);
    }

    void WeaponInfo(WeaponStruct weaponStruct)
    {
      //  weapon_info.SetActive(true);

        weaponDamage_Text.text = weaponStruct.attack_damage.ToString();
        weaponSpeed_Text.text = weaponStruct.attack_speed.ToString();
        weaponDurability_Text.text = weaponStruct.durability.ToString();
    }

    void ConsumptionInfo(ConsumptionStruct consumptionStruct)
    {
        healAmount_Text.text = consumptionStruct.heal_amount.ToString();
        energyRegain_Text.text = consumptionStruct.energy_amount.ToString();
        duration_Text.text = consumptionStruct.duration.ToString();

        switch (consumptionStruct.consumption_type)
        {
            case ConsumptionType.None:
                break;
            case ConsumptionType.Heal:
                consumption_heal.SetActive(true);    
                break;
            case ConsumptionType.EnergyRegain:
                consumption_energy.SetActive(true);
                break;
            case ConsumptionType.HealAndEnergy:
                consumption_heal.SetActive(true);
                consumption_energy.SetActive(true);
                break;
        }

      
    }

    void ArmorInfo(ArmorStruct armorStruct)
    {
        switch (armorStruct.armor_type)
        {
            case SlotType.None:
                break;
            case SlotType.Head: case SlotType.Chest: case SlotType.Arm: case SlotType.Leg: case SlotType.Foot:
                armor_Defense.SetActive(true);
                armor_durability.SetActive(true);
                armorDefense_Text.text = armorStruct.defense.ToString();
                armorDurability_Text.text = armorStruct.durability.ToString();

                if(armorStruct.armor_type == SlotType.Arm)
                {
                    armor_attackDamage.SetActive(true);
                    armorAttackDamage_Text.text = armorStruct.attack_damage.ToString();
                }

                if(armorStruct.armor_type == SlotType.Foot)
                {
                    armor_moveSpeed.SetActive(true);
                    armorMoveSpeed_Text.text = armorStruct.move_speed.ToString();
                }
                break;
            case SlotType.Backpack:
                carrySize.gameObject.SetActive(true);
                carrySize.text = armorStruct.carrying_capacity.ToString();
                break;
            case SlotType.Other:
                break;
        }
    }
}
