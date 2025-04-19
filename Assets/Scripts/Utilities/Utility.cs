using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;


//���� ���� ��ġ�� ��ġ�Ǵ� �ٴ��� ��ġ ����
public struct BuildDirectionWithOffset
{
    public int offsetX;
    public int offsetY;
    public int offsetZ;
    public int locX;
    public int locY;
    public BuildDirectionWithOffset(int offsetX, int offsetY, int offsetZ, int locX, int locY)
    {
        this.offsetX = offsetX;
        this.offsetY = offsetY;
        this.offsetZ = offsetZ;
        this.locX = locX;
        this.locY = locY;
    }
}

//���� ��ǥ
struct DoorSturct
{
    public int indexX;
    public int indexY;
    public int indexZ;
    public DoorSturct(int indexX, int indexY, int indexZ)
    {
        this.indexX = indexX;
        this.indexY = indexY;
        this.indexZ = indexZ;
    }
}

//���� ����
public enum BuildWallDirection
{
    None,
    Left,
    Right,
    Top,
    Bottom
}

//���� ���
public enum GameMode
{
    DefaultMode,
    NoSaveMode,
    TestMode
}

//���� ���
public enum EditMode
{
    None,
    CreativeMode,
    DestroyMode

}

//�÷��̾� ����
public enum PlayerState
{ 
    Default,
    Combat,
    Dead
}

//���� Ÿ��
public enum SlotType
{
    None,
    Head,
    Chest,
    Arm,
    Leg,
    Foot,
    Backpack,
    Other
}

//ȯ�� ������Ʈ Ÿ��
public enum EnvironmentType
{
    None,
    Grasses,
    Flower_Pink,
    Flower_Yellow,
    Flower_Orange,
    Tree
}

//NPC ���� Ÿ��
public enum NPCDispositionType
{
    None,
    Netural,
    Friendly,
    Hostile,
    Infected
}

//ȯ�� ������Ʈ ����
public struct EnvironmentObjectInfo
{
    public EnvironmentType type;
    public int X;
    public int Y;
    public bool Z;  //����

    public EnvironmentObjectInfo(EnvironmentType type, int X, int Y, bool Z)
    {
        this.type = type;
        this.X = X; 
        this.Y = Y;
        this.Z = Z;
    }
}

//������ ȯ�� ������Ʈ ����
public struct RemoveEnvironmentList
{
    public List<EnvironmentObject> environmentObjects;

    public RemoveEnvironmentList(List<EnvironmentObject> environmentObjects)
    {
        this.environmentObjects = environmentObjects;
    }

    public void Clear()
    {
        foreach (EnvironmentObject obj in environmentObjects)
        {
            obj.Clear();
        }

        environmentObjects.Clear();
    }

}

//������ Ÿ��
public enum ItemType
{
    None,
    Consumable,
    Equipmentable,
    Wearable,
    Installable_Floor,
    Installable_Wall,
    Installable_Door,
    Installable_Furniture
}

//��� �� �ִ� ���� Ÿ��
public enum LearnType
{
    None,
    Installable
    
}


//UI�� ���� ��
public enum UIType
{
    None,
    Menu,
    Housing,
    Inventory,
    AbilityMenu,
    BoxInventory,
    QuickSlot,
    Achievement,
    Dead

}

//�̴ϸʿ� ����� ��ü�� Ÿ��
public enum MinimapIconType
{
    None,
    Player,
    Enemy,
    NPC,
    Object,
    ItemBox
}

//���� ���� Ÿ��
public enum WeaponType
{
    None,
    Axe,
    Spear,
    OneHand,
    TwoHand,
    Pistol,
    Rifle

}

//�Һ� Ÿ��
public enum ConsumptionType
{
    None,
    Heal,
    EnergyRegain,
    HealAndEnergy
}



//���� ������ Ÿ��
public enum StructureState
{
    None,
    Floor,
    Wall,
    Door,
    Furniture
}

//���� Ÿ��
public enum FurnitureType
{
    None,
    ItemBox
}

//���� Ÿ��
public enum AchievementType
{
    None,
    ItemCollected,
    EnemyKilled

}


//�Ͽ�¡�� ���Ǵ� ���
public enum ChangeInfo
{
    Addition,
    Subtraction
}


//�Ͽ�¡ �ý����� ��ȭ ����
public struct HousingChangeInfo
{
    public int indexX;
    public int indexY;
    public int indexZ;
    public GameObject buildMat;
    public bool used;
    public ChangeInfo change;
    public StructureState type;
    public HousingChangeInfo(GameObject buildMat, int indexX, int indexY, int indexZ, ChangeInfo change, StructureState type)
    {
        this.buildMat = buildMat;
        this.indexX = indexX;
        this.indexY = indexY;
        this.indexZ = indexZ;
        this.change = change;
        this.type = type;
        this.used = true;
    }

    public void Clear()
    {
        buildMat = null;
    }
}

// ������ ����
public struct LevelStruct : ITableID<int>
{
    public int id;
    public int level;
    public int exp;
    public int heal;
    public int energy_regain;
    public int weight;

    public readonly int ID => id;

    public readonly string Name => throw new System.NotImplementedException();
}


public struct ItemPackageStruct
{
    public ItemStruct itemStruct;
    public WeaponStruct weaponStruct;
    public ConsumptionStruct consumptionStruct;
    public ArmorStruct armorStruct;
    public ItemPackageStruct(ItemStruct itemStruct, WeaponStruct weaponStruct, ConsumptionStruct consumptionStruct, ArmorStruct armorStruct)
    {
        this.itemStruct = itemStruct;
        this.weaponStruct = weaponStruct;
        this.consumptionStruct = consumptionStruct;
        this.armorStruct = armorStruct;
    }
    public void ChangeWeaponStruct(WeaponStruct weaponStruct)
    {
        this.weaponStruct = weaponStruct;
    }
    public void ChangeArmorStruct(ArmorStruct armorStruct)
    {
        this.armorStruct = armorStruct;
    }
    public void ChangeConsumptionStruct(ConsumptionStruct consumptionStruct)
    {
        this.consumptionStruct = consumptionStruct;
    }


}

//������ ����
[System.Serializable]
public struct ItemStruct : ITableID<int>
{
    public int item_index;
    public Sprite image;
    public string item_name;
    public string asset_name;
    public SlotType slot_type;  //�� ��
    public ItemType item_type;
    public bool used;
    public float weight;
    public GameObject itemGO;

    public ItemStruct(int itemIndex, Sprite image, string itemName, string asset_name, float weight, SlotType slotType, ItemType itemType, GameObject itemGO)
    {
        this.item_index = itemIndex;
        this.image = image;
        this.item_name = itemName;
        this.asset_name = asset_name;
        this.weight = weight;
        this.slot_type = slotType;
        this.item_type = itemType;
        used = true;
        this.itemGO = itemGO;
    }

    public readonly int ID => item_index;

    public readonly string Name => throw new System.NotImplementedException();

    public void Clear()
    {
        image = null;
        itemGO = null;
    }
}

//�Һ� �������� ����
[System.Serializable]
public struct ConsumptionStruct : ITableID<int>
{
    public int item_index;
    public ConsumptionType consumption_type;
    public int heal_amount;
    public int energy_amount;
    public float duration;

    public ConsumptionStruct(int item_index, ConsumptionType consumption_type, int heal_amount, int energy_amount, float duration)
    {
        this.item_index = item_index;
        this.consumption_type = consumption_type;
        this.heal_amount = heal_amount;
        this.energy_amount = energy_amount;
        this.duration = duration;
    }

    public readonly int ID => item_index;

    public readonly string Name => throw new System.NotImplementedException();
}

//���� ������ ����
public struct WeaponStruct : ITableID<int>
{
    public int item_index;
    public WeaponType weapon_type;
    public int attack_damage;
    public float attack_speed;
    public int max_ammo;
    public int durability;

    public WeaponStruct(int item_index, WeaponType weapon_type, int attack_damage, float attack_speed, int max_ammo, int durability)
    {
        this.item_index = item_index;
        this.weapon_type = weapon_type;
        this.attack_damage = attack_damage;
        this.attack_speed = attack_speed;
        this.max_ammo = max_ammo;
        this.durability = durability;
    }

    public readonly int ID => item_index;

    public readonly string Name => throw new System.NotImplementedException();
}

//�� ������ ����
public struct ArmorStruct : ITableID<int>
{
    public int item_index;
    public SlotType armor_type;
    public int defense;
    public int durability;
    public int carrying_capacity;
    public int move_speed;
    public int attack_damage;
    public int key_index;

    public ArmorStruct(int item_index, SlotType armor_type, int defense, int durability, int carrying_capacity, int move_speed, int attack_damage, int key_index)
    {
        this.item_index = item_index;
        this.armor_type = armor_type;
        this.defense = defense;
        this.durability = durability;
        this.carrying_capacity = carrying_capacity;
        this.move_speed = move_speed;
        this.attack_damage = attack_damage;
        this.key_index = key_index;
    }

    public readonly int ID => item_index;

    public readonly string Name => throw new System.NotImplementedException();
}


//���� ����
public struct BuffStruct
{
    public Queue<ConsumptionStruct> healBuff;
    public Queue<ConsumptionStruct> energyBuff;
    public int levelupNums;

    public BuffStruct(bool renew)
    {
        healBuff = new Queue<ConsumptionStruct>();
        energyBuff = new Queue<ConsumptionStruct>();
        levelupNums = 0;
    }
}


//�÷��̾��� ����
public struct PlayerStruct
{
    public int hp;
    public int maxHP;
    public int energy;
    public int maxEnergy;
    public int defense;
    public int exp;
    public int requireEXP;
    public int level;
    public int attackDamage;
    public float attackSpeed;
    public float moveSpeed;
    public float weight;
    public int skillPoint;
    public int hpLevel;
    public int energyLevel;
    public int weightLevel;
    public int backpackLevel;

    public PlayerStruct(int hp, int maxHP, int energy, int maxEnergy, int defense, int exp, int requireEXP, int level, int attackDamage, float attackSpeed, float moveSpeed, float weight, int skillPoint, int hpLevel, int energyLevel, int weightLevel, int backpackLevel)
    {
        this.hp = hp;
        this.maxHP = maxHP;
        this.energy = energy;
        this.maxEnergy = maxEnergy;
        this.defense = defense;
        this.exp = exp;
        this.requireEXP = requireEXP;
        this.level = level;
        this.attackDamage = attackDamage;
        this.attackSpeed = attackSpeed;
        this.moveSpeed = moveSpeed;
        this.weight = weight;
        this.skillPoint = skillPoint;
        this.hpLevel = hpLevel;
        this.energyLevel = energyLevel;
        this.weightLevel = weightLevel;
        this.backpackLevel = backpackLevel;
    }

}

public struct NPCCombatStruct
{
    public int id;
    public string npc_name;
    public string asset_name;
    public int health;
    public int max_health;
    public int attack;
    public bool infected;
    public float speed;
    public float attack_range;
    public string drop_item;
    public bool infected_player;
    public List<DropStruct> dropStruct;
    public NPCCombatStruct(int id, string npc_name, string asset_name, int health, int max_health, int attack, bool infected, float speed, float attack_range, string drop_item, bool infected_player, List<DropStruct> dropStructs)
    {
        this.id = id;
        this.npc_name = npc_name;
        this.asset_name = asset_name;
        this.health = health;
        this.max_health = max_health;
        this.attack = attack;
        this.infected = infected;
        this.speed = speed;
        this.attack_range = attack_range;
        this.drop_item = drop_item;
        this.infected_player = infected_player;
        this.dropStruct = dropStructs;
    }

   
}

//���� ��� ����
public struct DropStruct
{
    public int item_index;
    public int item_chance;
}

//������ �ڽ� ������
public struct BoxStruct
{
    public ItemStruct[,] itemStructs;
    public ConsumptionStruct[,] consumptionStructs;
    public WeaponStruct[,] weaponStructs;
    public ArmorStruct[,] armorStructs;

    public BoxStruct(ItemStruct[,] items, ConsumptionStruct[,] consumptions, WeaponStruct[,] weapons, ArmorStruct[,] armors)
    {
        itemStructs = items;
        consumptionStructs = consumptions;
        weaponStructs = weapons;
        armorStructs = armors;
    }
}

//���� ����
public struct CraftStruct : ITableID<int>
{
    public int index;

    public readonly int ID => index;

    public readonly string Name => throw new System.NotImplementedException();
}
//Ư�� ����
public struct AbilityStruct : ITableID<int>
{
    public int index;

    public readonly int ID => index;

    public readonly string Name => throw new System.NotImplementedException();
}

//���� ����
public struct AchievementStruct
{
    public uint id;
    public string quest_name;
    public int progress;
    public int target;
    public bool complete;
    public string reward;
    public bool reward_complete;
    public uint achievement_chain;
    public int level;
    public List<AchievementRewardStruct> rewardStruct;
   
    public AchievementStruct(uint id, string quest_name, int progress, int target, bool complete, string reward, bool reward_complete, uint achievement_chain, int level, List<AchievementRewardStruct> rewardStruct)
    {
        this.id = id;
        this.quest_name = quest_name;
        this.progress = progress;
        this.target = target;
        this.complete = complete;
        this.reward = reward;
        this.reward_complete = reward_complete;
        this.achievement_chain = achievement_chain;
        this.level = level;
        this.rewardStruct = rewardStruct;
    }
}

//���� ���� ����
public struct AchievementRewardStruct
{
    public int reward_item;
    public int reward_num;

    public AchievementRewardStruct(int reward_item, int reward_num)
    {
        this.reward_item = reward_item;
        this.reward_num = reward_num;
    }
}

//UMA ������ ����
public struct RecipeStruct : ITableID<int>
{
    public int id;
    public int index;
    public string recipe_name;

    public RecipeStruct(int id, int index, string recipe_name)
    {
        this .id = id;
        this .index = index;
        this.recipe_name = recipe_name;
    }

    public readonly int ID => id;

    public readonly string Name => recipe_name;
}

//NPC ����
public struct NPCStruct : ITableID<int>
{
    public int id;
    public int npc_index;   
    public string npc_name;
    public string npc_asset;
    public int npc_hair;
    public int npc_helmet;
    public int npc_armor;
    public int npc_arm;
    public int npc_leg;
    public int npc_boots;
    public int npc_param_index;
    public readonly int ID => id;

    public readonly string Name => npc_asset;
}

public struct NPCEventStruct
{
    public uint id;
    public string event_name;
    public NPCDispositionType npc_disposition;
}
public struct StringStruct : ITableID<string>
{
    public string str;

    public readonly string Name => str;

  
    public readonly string ID => throw new System.NotImplementedException();

    public StringStruct(string str)
    {
        this.str = str;
    }
}

public struct AnimatorStruct : ITableID<int>
{
    public int id;
    public string animator_name;

    public readonly int ID => id;

    public readonly string Name => animator_name;
}

public struct CustomEvent<T>
{
    public T cEvent;

    public CustomEvent(T cEvent)
    {
        this.cEvent = cEvent;
    }

}

public class Utility
{

    //���� ���� ��ġ�� ��ġ�Ǵ� �ٴ��� ��ġ
    public static BuildDirectionWithOffset GetWallDirectionWithOffset(BuildWallDirection buildWallDirection, int x, int y)
    {
        BuildDirectionWithOffset buildDirectionWithOffset;
        switch (buildWallDirection)
        {
            case BuildWallDirection.None:
                buildDirectionWithOffset = new BuildDirectionWithOffset();
                break;
            case BuildWallDirection.Left:
                buildDirectionWithOffset = new BuildDirectionWithOffset(-1, 0, 0, x * 2, y * 2 + 1);
                break;
            case BuildWallDirection.Right:
                buildDirectionWithOffset = new BuildDirectionWithOffset(1, 0, 0, x * 2 + 2, y * 2 + 1);
                break;
            case BuildWallDirection.Top:
                buildDirectionWithOffset = new BuildDirectionWithOffset(0, 1, 1, x * 2 + 1, y * 2 + 2);
                break;
            case BuildWallDirection.Bottom:
                buildDirectionWithOffset = new BuildDirectionWithOffset(0, -1, 1, x * 2 + 1, y * 2);
                break;
            default:
                buildDirectionWithOffset = new BuildDirectionWithOffset();
                break;
        }

        return buildDirectionWithOffset;
    }

    public static byte[] StringToByteArray(string hex)
    {
        int length = hex.Length;
        byte[] bytes = new byte[length / 2];
        for (int i = 0; i < length; i += 2)
        {
            bytes[i / 2] = System.Convert.ToByte(hex.Substring(i, 2), 16);
        }
        return bytes;
    }

    public static bool TryGetComponentInParent<T>(Component component, out T t) where T : Component
    {
        t = component.GetComponentInParent<T>();
        return t != null;
    }


    public static void ChangeTagLayer(Transform parent, string newTag, int layerName)
    {
        if (parent != null)
        {
            foreach (Transform child in parent)
            {
                if (child != null)
                {
                    child.gameObject.tag = newTag;
                    child.gameObject.layer = layerName;
                    ChangeTagLayer(child, newTag, layerName);
                }
            }
        }
    }
}

//UI �������̽�
public interface IUIComponent
{
    void Setup(bool init);
}

//������ �������̽�
public interface IBuildMaterials
{
    StructureState structureState { get; set; }
    public Renderer renderer { get; }

    public void Clear();
}

//��ü �������̽�
public interface IIdentifiable
{
    public int ID { get; set; }
   
}

public interface IDamageable
{
    
    public bool Damaged(int damage, int layer);

    public float DamagedTimer { get; set; }
   // public void Dead();
}

public interface IAbility
{
    public void Setup(RectTransform rectTransform);
    public void Work();

    public void Release();
    public void DestroyAbility();

}

public interface ITableID<K>
{
     K ID { get; }
     public string Name { get; }
}