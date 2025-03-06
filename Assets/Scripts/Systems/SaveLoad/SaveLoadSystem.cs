using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using UnityEngine;

public class SaveLoadSystem
{

    static string path = Application.persistentDataPath;

    //게임 맵 데이터 저장
    public static void SaveEnviromentData()
    {
        //필드의 환경 오브젝트
        string dir = Path.Combine(path, "Save");
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        string p = Path.Combine(path, "Save/Environment.dat");

        EnvironmentObject[,] environmentObjects = GameInstance.Instance.environmentSpawner.environmentObjects;
        using (MemoryStream ms = new MemoryStream())
        {
            using (BinaryWriter writer = new BinaryWriter(ms))
            {
                for (int i = 0; i < 100; i++)
                {
                    for (int j = 0; j < 100; j++)
                    {
                        if (environmentObjects[i, j] != null)
                        {
                            writer.Write((int)environmentObjects[i, j].environmentType); //환경 오브젝트 타입
                            writer.Write(i);    // x 위치
                            writer.Write(j);    // y 위치
                            writer.Write(environmentObjects[i, j].rotated);
                        }
                    }
                }
            }
            File.WriteAllBytes(p, ms.ToArray());
        }
    }

    //게임 맵 데이터 로드
    public static bool LoadEnvironmentData()
    {
        List<EnvironmentObjectInfo> housingInfos = new List<EnvironmentObjectInfo>();
        byte[] data;
        string p = Path.Combine(path, "Save/Environment.dat");
        if (File.Exists(p))
        {
            using (FileStream fs = new FileStream(p, FileMode.Open))
            {
                data = new byte[fs.Length];
                fs.Read(data, 0, data.Length);
                if (data.Length > 0)
                {
                    using (BinaryReader reader = new BinaryReader(new MemoryStream(data)))
                    {
                        while (reader.BaseStream.Position < reader.BaseStream.Length)
                        {
                            EnvironmentType environmentType = (EnvironmentType)reader.ReadInt32();
                            int posX = reader.ReadInt32();
                            int posY = reader.ReadInt32();
                            bool objectRotated = reader.ReadBoolean();

                            EnvironmentObjectInfo environmentObjectInfo = new EnvironmentObjectInfo(environmentType, posX, posY, objectRotated);
                            GameInstance.Instance.environmentSpawner.LoadObject(environmentObjectInfo);
                        }
                    }
                }
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    //레벨 데이터 테이블 불러오기
    public static List<LevelData> GetLevelData(string content)
    {
      //  string p = Path.Combine(path, "PlayData/Level.dat");
       // string content = File.ReadAllText(p);

        string data = EncryptorDecryptor.Decyptor(content, "AAA");

        return JsonConvert.DeserializeObject<List<LevelData>>(data);
    }

    //아이템 데이터 테이블 불러오기
    public static List<ItemStruct> GetItemData(string content)
    {
        string data = EncryptorDecryptor.Decyptor(content, "AAA");

        return JsonConvert.DeserializeObject<List<ItemStruct>>(data);

    }

    //무기 데이터 불러오기
    public static List<WeaponStruct> GetWeaponData(string content)
    {
        string data = EncryptorDecryptor.Decyptor(content, "AAA");

        return JsonConvert.DeserializeObject<List<WeaponStruct>>(data);
    }

    //소비 아이템 불러오기
    public static List<ConsumptionStruct> GetConsumptionData(string content)
    {
        string data = EncryptorDecryptor.Decyptor(content, "AAA");

        return JsonConvert.DeserializeObject<List<ConsumptionStruct>>(data);
    }


    //플레이어 데이터 저장
    public static void SavePlayerData(Player player)
    {
        string dir = Path.Combine(path, "Save");
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        string p = Path.Combine(path, "Save/Player.dat");
        using (MemoryStream ms = new MemoryStream())
        {
            using (BinaryWriter writer = new BinaryWriter(ms))
            {
                //플레이어 위치
                float x = player.Transforms.position.x;
                float y = player.Transforms.position.y;
                float z = player.Transforms.position.z;
                writer.Write(x);
                writer.Write(y);
                writer.Write(z);

                //플레이어 방향
                float rx = player.Transforms.rotation.x;
                float ry = player.Transforms.rotation.y;
                float rz = player.Transforms.rotation.z;
                float rw = player.Transforms.rotation.w;
                writer.Write(rx);
                writer.Write(ry);
                writer.Write(rz);
                writer.Write(rw);


                PlayerStruct ps = player.playerStruct;
                //플레이어 능력치
                int hp = ps.hp;
                int maxHP = ps.maxHP;
                int energy = ps.energy;
                int maxEnergy = ps.maxEnergy;
                int exp = ps.exp;
                int requireEXP = ps.requireEXP;
                int level = ps.level;
                int attackDamage = ps.attackDamage;
                int skillPoint = ps.skillPoint;
                int hpLevel = ps.hpLevel;
                int energyLevel = ps.energyLevel;
                int weightLevel = ps.weightLevel;

                writer.Write(hp);
                writer.Write(maxHP);
                writer.Write(energy);
                writer.Write(maxEnergy);
                writer.Write(exp);
                writer.Write(requireEXP);
                writer.Write(level);
                writer.Write(attackDamage);
                writer.Write(skillPoint);
                writer.Write(hpLevel);
                writer.Write(energyLevel);
                writer.Write(weightLevel);
            }
            File.WriteAllBytes(p, ms.ToArray());
        }
    }

    //플레이어 데이터 로드
    public static bool LoadPlayerData(PlayerController pc)
    {
        List<HousingInfo> housingInfos = new List<HousingInfo>();
        byte[] data;
        string p = Path.Combine(path, "Save/Player.dat");

        if (File.Exists(p))
        {
            using (FileStream fs = new FileStream(p, FileMode.Open))
            {
                data = new byte[fs.Length];
                fs.Read(data, 0, data.Length);
                if (data.Length > 0)
                {
                    using (BinaryReader reader = new BinaryReader(new MemoryStream(data)))
                    {  
                        //플레이어 위치
                        float x = reader.ReadSingle();
                        float y = reader.ReadSingle();
                        float z = reader.ReadSingle();

                        //플레이어 방향
                        float rx = reader.ReadSingle();
                        float ry = reader.ReadSingle();
                        float rz = reader.ReadSingle();
                        float rw = reader.ReadSingle(); 

                        Quaternion rot = new Quaternion(rx, ry, rz, rw);
                        Vector3 pos = new Vector3(x, y, z);

                        //플레이어 능력치
                        int hp = reader.ReadInt32();
                        int maxHP = reader.ReadInt32();
                        int energy = reader.ReadInt32();
                        int maxEnergy = reader.ReadInt32();
                        int exp = reader.ReadInt32();
                        int requireEXP = reader.ReadInt32();
                        int level = reader.ReadInt32();
                        int attackDamage = reader.ReadInt32();
                        int skillPoint = reader.ReadInt32();
                        int hpLevel = reader.ReadInt32();
                        int energyLevel = reader.ReadInt32();
                        int weightLevel = reader.ReadInt32();


                        PlayerStruct playerStruct = new PlayerStruct(hp, maxHP, energy, maxEnergy, exp, requireEXP, level, attackDamage, skillPoint, hpLevel, energyLevel, weightLevel);
                        pc.SetPlayerData(pos, rot, playerStruct);
                    }
                }
            }
            return true;
        }
        else
        {
            
            return false;
        }
    }


    //인벤토리 데이터 저장
    public static void SaveInventoryData()
    {
        string dir = Path.Combine(path, "Save");
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        string p = Path.Combine(path, "Save/Inventory.dat");

        Slot[,] inventorySlots = GameInstance.Instance.inventorySystem.inventoryArray;
        using (MemoryStream ms = new MemoryStream())
        {
            using(BinaryWriter writer = new BinaryWriter(ms))
            {
                for(int i=0; i<7;i++)
                {
                    for(int j=0; j<10; j++)
                    {
                        Slot slot = inventorySlots[i, j];
                        if (slot != null)
                        {
                            
                            ItemStruct item = slot.item;
                            if (item.item_index == 0) continue; 
                            writer.Write(slot.slotX);
                            writer.Write(slot.slotY);
                            writer.Write(item.item_index);
                            writer.Write(item.item_name);
                            writer.Write((int)item.slot_type);
                            writer.Write((int)item.item_type);
                        }
                    }
                }
            }
            File.WriteAllBytes(p, ms.ToArray());
        }

    }


    //인벤토리 데이터 로드
    public static bool LoadInventoryData()
    {
        List<HousingInfo> housingInfos = new List<HousingInfo>();
        byte[] data;
        string p = Path.Combine(path, "Save/Inventory.dat");

        if (File.Exists(p))
        {
            using (FileStream fs = new FileStream(p, FileMode.Open))
            {
                data = new byte[fs.Length];
                fs.Read(data, 0, data.Length);
                if (data.Length > 0)
                {
                    using (BinaryReader reader = new BinaryReader(new MemoryStream(data)))
                    {
                        while (reader.BaseStream.Position < reader.BaseStream.Length)
                        {
                            int X = reader.ReadInt32();
                            int Y = reader.ReadInt32();
                            int index = reader.ReadInt32();
                            string name = reader.ReadString();
                            int slotType = reader.ReadInt32();
                            int itemType = reader.ReadInt32();

                            ItemStruct item = new ItemStruct(index, null, name, (SlotType)slotType, (ItemType)itemType, null);
                            GameInstance.Instance.inventorySystem.LoadInvetory(X, Y, item);

                        }
                    }
                }
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    //하우징 시스템 저장
    public static void SaveBuildSystem()
    {
        string dir = Path.Combine(path, "Save");
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        string p = Path.Combine(path, "Save/Housing.dat");

        using (MemoryStream ms = new MemoryStream())
        {
            using (BinaryWriter writer = new BinaryWriter(ms))
            {
                BuildSystem_WriteFloor(writer);
                BuildSystem_WriteWall(writer);

            }
            File.WriteAllBytes(p, ms.ToArray());
        }
    }

    //하우징 시스템 로드
    public static List<HousingInfo> LoadBuildSystem()
    {
        List<HousingInfo> housingInfos = new List<HousingInfo>();
        byte[] data;
        string p = Path.Combine(path, "Save/Housing.dat");

        if (File.Exists(p))
        {
            using (FileStream fs = new FileStream(p, FileMode.Open))
            {
                data = new byte[fs.Length];
                fs.Read(data, 0, data.Length);

                if(data.Length > 0)
                {
                    using (BinaryReader reader = new BinaryReader(new MemoryStream(data)))
                    {
                        while(reader.BaseStream.Position < reader.BaseStream.Length)
                        {
                            //재질 타입
                            MaterialsType type = (MaterialsType)reader.ReadInt32();

                            if(type == MaterialsType.Floor)
                            {
                                int x = reader.ReadInt32();
                                int y = reader.ReadInt32();
                                HousingInfo housingInfo = new HousingInfo(x,y,0,type);
                                housingInfos.Add(housingInfo);
                               
                            }
                            else
                            {
                                int x = reader.ReadInt32();
                                int y = reader.ReadInt32();
                                int z = reader.ReadInt32();
                                bool d = reader.ReadBoolean();

                                HousingInfo housingInfo = new HousingInfo(x,y,z, d ? MaterialsType.Door : MaterialsType.Wall);
                                housingInfos.Add(housingInfo);
                            }
                        }
                    }
                }
            }
         
        }
        else
        {

        }
        return housingInfos;
    }

    public static void BuildSystem_WriteFloor(BinaryWriter writer)
    {
        for (int i = 0; i < 100; i++)
        {
            for (int j = 0; j < 100; j++)
            {
                GameObject floor = GameInstance.Instance.housingSystem.GetFloor(i, j);
                if (floor != null)
                {
                    writer.Write((int)MaterialsType.Floor);
                    writer.Write(i);
                    writer.Write(j);
                }
            }
        }
    }
    public static void BuildSystem_WriteWall(BinaryWriter writer)
    {
        for (int i = 0; i < 100; i++)
        {
            for (int j = 0; j < 100; j++)
            {
                for (int k = 0; k < 2; k++)
                {
                    Wall wall = GameInstance.Instance.housingSystem.GetWall(i, j, k);

                    if (wall != null)
                    {
                        writer.Write((int)MaterialsType.Wall);
                        writer.Write(i);
                        writer.Write(j);
                        writer.Write(k);
                        writer.Write(wall.isDoor);
                    }
                }
            }
        }
    }
}
