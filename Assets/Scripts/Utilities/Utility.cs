using System.Collections;
using System.Collections.Generic;
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
    Backpack,
    JustView

}

//������ Ÿ��
public enum ItemType
{
    None,
    Consumable,
    Equipmentable,
    Installable
}

//UI�� ���� ��
public  enum UIType
{
    None,
    Menu,
    Housing,
    Inventory

}



//���� ������ Ÿ��
public enum StructureState
{
    None,
    Floor,
    Wall,
    Door
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
}


//������ ����
[System.Serializable]
public struct ItemStruct
{
    public Sprite image;
    public string itemName;
    public SlotType slotType;
    public ItemType itemType;
    public bool used;

    public ItemStruct(Sprite image, string itemName, SlotType slotType, ItemType itemType)
    {
        this.image = image;
        this.itemName = itemName;
        this.slotType = slotType;
        this.itemType = itemType;
        used = true;
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

}

//UI �������̽�
public interface IUIComponent
{
    void Setup();
}

//������ �������̽�
public interface IBuildMaterials
{
    StructureState structureState { get; set; }
}

