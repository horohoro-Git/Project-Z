using System.Collections;
using System.Collections.Generic;
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
    Inventory

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
}


//아이템 정보
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
}

