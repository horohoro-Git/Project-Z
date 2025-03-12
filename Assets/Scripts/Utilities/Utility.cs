using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


//벽을 짓는 위치와 설치되는 바닥의 위치 저장
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

//문의 좌표
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

//벽의 방향
public enum BuildWallDirection
{
    None,
    Left,
    Right,
    Top,
    Bottom
}

//게임 모드
public enum GameMode
{
    DefaultMode,
    NoSaveMode,
    TestMode
}

//건축 모드
public enum EditMode
{
    None,
    CreativeMode,
    DestroyMode

}

//플레이어 상태
public enum PlayerState
{ 
    Default,
    Combat,
    Dead
}

//슬롯 타입
public enum SlotType
{
    None,
    Head,
    Chest,
    Arm,
    Leg,
    Backpack,
    JustView

}

//환경 오브젝트 타입
public enum EnvironmentType
{
    None,
    Grasses,
    Flower_Pink,
    Flower_Yellow,
    Flower_Orange,
    Tree
}

//환경 오브젝트 정보
public struct EnvironmentObjectInfo
{
    public EnvironmentType type;
    public int X;
    public int Y;
    public bool Z;  //각도

    public EnvironmentObjectInfo(EnvironmentType type, int X, int Y, bool Z)
    {
        this.type = type;
        this.X = X; 
        this.Y = Y;
        this.Z = Z;
    }
}

//제거할 환경 오브젝트 모음
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

//아이템 타입
public enum ItemType
{
    None,
    Consumable,
    Equipmentable,
    Installable
}

//UI의 상태 값
public  enum UIType
{
    None,
    Menu,
    Housing,
    Inventory,
    AbilityMenu,
    BoxInventory,
    QuickSlot

}

//미니맵에 사용할 개체의 타입
public enum MinimapIconType
{
    None,
    Player,
    Enemy,
    Object
}

//장착 무기 타입
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

//소비 타입
public enum ConsumptionType
{
    None,
    Heal,
    EnergyRegain,
    HealAndEnergy
}



//건축 자재의 타입
public enum StructureState
{
    None,
    Floor,
    Wall,
    Door
}


//하우징에 사용되는 모드
public enum ChangeInfo
{
    Addition,
    Subtraction
}


//하우징 시스템의 변화 정보
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


//아이템 정보
[System.Serializable]
public struct ItemStruct
{
    public int item_index;
    public Sprite image;
    public string item_name;
    public SlotType slot_type;
    public ItemType item_type;
    public bool used;
    public GameObject itemGO;

    public ItemStruct(int itemIndex, Sprite image, string itemName, SlotType slotType, ItemType itemType, GameObject itemGO)
    {
        this.item_index = itemIndex;
        this.image = image;
        this.item_name = itemName;
        this.slot_type = slotType;
        this.item_type = itemType;
        used = true;
        this.itemGO = itemGO;
    }

    public void Clear()
    {
        image = null;
        itemGO = null;
    }
}

//소비 아이템의 정보
[System.Serializable]
public struct ConsumptionStruct
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
}

//무기 아이템 정보
public struct WeaponStruct
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
}

//버프 정보
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


//플레이어의 정보
public struct PlayerStruct
{
    public int hp;
    public int maxHP;
    public int energy;
    public int maxEnergy;
    public int exp;
    public int requireEXP;
    public int level;
    public int attackDamage;
    public int skillPoint;
    public int hpLevel;
    public int energyLevel;
    public int weightLevel;

    public PlayerStruct(int hp, int maxHP, int energy, int maxEnergy, int exp, int requireEXP, int level, int attackDamage, int skillPoint, int hpLevel, int energyLevel, int weightLevel)
    {
        this.hp = hp;
        this.maxHP = maxHP;
        this.energy = energy;
        this.maxEnergy = maxEnergy;
        this.exp = exp;
        this.requireEXP = requireEXP;
        this.level = level;
        this.attackDamage = attackDamage;
        this.skillPoint = skillPoint;
        this.hpLevel = hpLevel;
        this.energyLevel = energyLevel;
        this.weightLevel = weightLevel;
    }

}

//적의 정보
public struct EnemyStruct
{
    public int id;
    public string enemy_name;
    public int health;
    public int attack;
    public string drop_item;
    
    public List<DropStruct> dropStruct;
    public EnemyStruct(int id, string enemy_name, int health, int attack, string drop_item, List<DropStruct> dropStructs)
    {
        this.id = id;
        this.enemy_name = enemy_name;
        this.health = health;
        this.attack = attack;
        this.drop_item = drop_item;
        this.dropStruct = dropStructs;
    }

   
}

//적의 드랍 정보
public struct DropStruct
{
    public int item_index;
    public int item_chance;
}


public class Utility
{

    //벽을 짓는 위치와 설치되는 바닥의 위치
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
}

//UI 인터페이스
public interface IUIComponent
{
    void Setup();
}

//구조물 인터페이스
public interface IBuildMaterials
{
    StructureState structureState { get; set; }
    public Renderer renderer { get; }

    public void Clear();
}

//객체 인터페이스
public interface IIdentifiable
{
    public string ID { get; set; }
   
}

public interface IDamageable
{
    public void Damaged(int damage);
}

