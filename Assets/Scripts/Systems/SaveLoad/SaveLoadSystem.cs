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


    //플레이어 데이터 저장
    public static void SavePlayerData()
    {
     
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
                            if (item.itemIndex == 0) continue; 
                            writer.Write(slot.slotX);
                            writer.Write(slot.slotY);
                            writer.Write(item.itemIndex);
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

                            GameInstance.Instance.inventorySystem.LoadInvetory(X, Y, index);

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
