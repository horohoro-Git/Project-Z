using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using UnityEngine;

public class SaveLoadSystem
{

    static string path = Application.persistentDataPath;

    //���� �� ������ ����
    public static void SaveEnviromentData()
    {
        //�ʵ��� ȯ�� ������Ʈ
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
                            writer.Write((int)environmentObjects[i, j].environmentType); //ȯ�� ������Ʈ Ÿ��
                            writer.Write(i);    // x ��ġ
                            writer.Write(j);    // y ��ġ
                            writer.Write(environmentObjects[i, j].rotated);
                        }
                    }
                }
            }
            File.WriteAllBytes(p, ms.ToArray());
        }
    }

    //���� �� ������ �ε�
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

    //���� ������ ���̺� �ҷ�����
    public static List<LevelData> GetLevelData(string content)
    {
        string data = EncryptorDecryptor.Decyptor(content, "AAA");

        return JsonConvert.DeserializeObject<List<LevelData>>(data);
    }

    //������ ������ ���̺� �ҷ�����
    public static List<ItemStruct> GetItemData(string content)
    {
        string data = EncryptorDecryptor.Decyptor(content, "AAA");

        return JsonConvert.DeserializeObject<List<ItemStruct>>(data);

    }

    //���� ������ �ҷ�����
    public static List<WeaponStruct> GetWeaponData(string content)
    {
        string data = EncryptorDecryptor.Decyptor(content, "AAA");

        return JsonConvert.DeserializeObject<List<WeaponStruct>>(data);
    }

    //�Һ� ������ �ҷ�����
    public static List<ConsumptionStruct> GetConsumptionData(string content)
    {
        string data = EncryptorDecryptor.Decyptor(content, "AAA");

        return JsonConvert.DeserializeObject<List<ConsumptionStruct>>(data);
    }

    //���� �ҷ�����
    public static string LoadServerURL()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "servercommunication.bytes");
        
        string content = File.ReadAllText(path);
        string decryptedData = EncryptorDecryptor.Decyptor(content, "AAA");
        Dictionary<string, object> data = JsonConvert.DeserializeObject<Dictionary<string, object>>(decryptedData);

        string returnURL = data["server"].ToString();

        return returnURL;

    }

    //���� ���� ������ ���̺�
    public static List<EnemyStruct> LoadEnemyData(string content)
    {
        //�޾ƿ� ������ ��ȣȭ
        string data = EncryptorDecryptor.Decyptor(content, "AAA");

        //�� ���̺� ������ȭ
        List<EnemyStruct> enemyStruct = JsonConvert.DeserializeObject<List<EnemyStruct>>(data);

        for(int i = 0; i < enemyStruct.Count; i++)
        {
            string itemData = enemyStruct[i].drop_item;

            //�ӽ� ������ ���� �� ����
            EnemyStruct enemy = enemyStruct[i];
            enemy.dropStruct = LoadDropData(itemData);
            enemyStruct[i] = enemy;
        }
       
        return enemyStruct;
    }


    //��� ���̺�
    public static List<DropStruct> LoadDropData(string content)
    { 
        List<DropStruct> dropStructs = new List<DropStruct>();
        string data = content;

        //json �������� ����
        string item_index = "\"item_index\"";
        string item_chance = "\"item_chance\"";

        data = data.Replace("item_index", item_index);
        data = data.Replace("item_chance", item_chance);

        //������ �������� ������ȭ
        dropStructs = JsonConvert.DeserializeObject<List<DropStruct>>(data);

        return dropStructs;
    }


    //�÷��̾� ������ ����
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
                //�÷��̾� ��ġ
                float x = player.Transforms.position.x;
                float y = player.Transforms.position.y;
                float z = player.Transforms.position.z;
                writer.Write(x);
                writer.Write(y);
                writer.Write(z);

                //�÷��̾� ����
                float rx = player.Transforms.rotation.x;
                float ry = player.Transforms.rotation.y;
                float rz = player.Transforms.rotation.z;
                float rw = player.Transforms.rotation.w;
                writer.Write(rx);
                writer.Write(ry);
                writer.Write(rz);
                writer.Write(rw);


                PlayerStruct ps = player.playerStruct;
                //�÷��̾� �ɷ�ġ
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

    //�÷��̾� ������ �ε�
    public static bool LoadPlayerData(PlayerController pc)
    {
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
                        //�÷��̾� ��ġ
                        float x = reader.ReadSingle();
                        float y = reader.ReadSingle();
                        float z = reader.ReadSingle();

                        //�÷��̾� ����
                        float rx = reader.ReadSingle();
                        float ry = reader.ReadSingle();
                        float rz = reader.ReadSingle();
                        float rw = reader.ReadSingle(); 

                        Quaternion rot = new Quaternion(rx, ry, rz, rw);
                        Vector3 pos = new Vector3(x, y, z);

                        //�÷��̾� �ɷ�ġ
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


    //�κ��丮 ������ ����
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

                           
                            if((int)item.item_type == 1)
                            {
                                //�Һ� ������
                                ConsumptionStruct consumptionStruct = slot.consumption;
                                writer.Write(consumptionStruct.item_index);
                                writer.Write((int)consumptionStruct.consumption_type);
                                writer.Write(consumptionStruct.heal_amount);
                                writer.Write(consumptionStruct.energy_amount);
                                writer.Write(consumptionStruct.duration);
                            }
                            if ((int)item.item_type == 2)
                            {
                                //���� ������
                                WeaponStruct weaponStruct = slot.weapon;
                                writer.Write(weaponStruct.item_index);
                                writer.Write((int)weaponStruct.weapon_type);
                                writer.Write(weaponStruct.attack_damage);
                                writer.Write(weaponStruct.attack_speed);
                                writer.Write(weaponStruct.max_ammo);
                                writer.Write(weaponStruct.durability);
                            }
                        }
                    }
                }
            }
            File.WriteAllBytes(p, ms.ToArray());
        }

    }


    //�κ��丮 ������ �ε�
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

                            WeaponStruct weaponStruct = new WeaponStruct();
                            ConsumptionStruct consumptionStruct = new ConsumptionStruct();

                           
                            if ((ItemType)itemType == ItemType.Consumable)
                            {
                                int consumption_index = reader.ReadInt32();
                                ConsumptionType consumption_type = (ConsumptionType)reader.ReadInt32();
                                int heal_amount = reader.ReadInt32();
                                int energy_amount = reader.ReadInt32();
                                float duration = reader.ReadSingle();
                                consumptionStruct = new ConsumptionStruct(consumption_index, consumption_type, heal_amount, energy_amount, duration);
                            }
                            if ((ItemType)itemType == ItemType.Equipmentable)
                            {
                                int weapon_index = reader.ReadInt32();
                                WeaponType weapon_type = (WeaponType)reader.ReadInt32();
                                int attack_damage = reader.ReadInt32();
                                float attack_spped = reader.ReadSingle();
                                int max_ammo = reader.ReadInt32();
                                int durability = reader.ReadInt32();

                                weaponStruct = new WeaponStruct(weapon_index, weapon_type, attack_damage, attack_spped, max_ammo, durability);
                            }
                            GameInstance.Instance.inventorySystem.LoadInvetory(X, Y, item, weaponStruct, consumptionStruct);
                            GameInstance.Instance.boxInventorySystem.LoadInvetory(X,Y,item, weaponStruct, consumptionStruct);
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

    //�Ͽ�¡ �ý��� ����
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

    //�Ͽ�¡ �ý��� �ε�
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
                            //���� Ÿ��
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




    //�� ���� �ε�
    public static bool LoadEnemyInfo()
    {
        byte[] data;
        string p = Path.Combine(path, "Save/Enemies.dat");

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
                            float x = reader.ReadSingle();
                            float y = reader.ReadSingle();
                            float z = reader.ReadSingle();

                            float rx = reader.ReadSingle();
                            float ry = reader.ReadSingle();
                            float rz = reader.ReadSingle();
                            float rw = reader.ReadSingle();


                            int type = reader.ReadInt32();
                            int modelType = reader.ReadInt32();
                            int count = reader.ReadInt32();

                            List<ItemStruct> itemStructs = new List<ItemStruct>();
                            for(int i = 0; i< count; i++)
                            {
                                int index = reader.ReadInt32();
                                string name = reader.ReadString();
                                int slotType = reader.ReadInt32();
                                int itemType = reader.ReadInt32();
                                ItemStruct item = new ItemStruct(index, null, name, (SlotType)slotType, (ItemType)itemType, null);
                                itemStructs.Add(item);
                            }

                            Vector3 position = new Vector3(x, y, z);
                            Quaternion rotation = new Quaternion(rx, ry, rz, rw);

                            GameInstance.Instance.enemySpawner.LoadEnemies(position, rotation, type, itemStructs, modelType);
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
    //�� ���� ����
    public static void SaveEnemyInfo()
    {
        string dir = Path.Combine(path, "Save");
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        string p = Path.Combine(path, "Save/Enemies.dat");
        using (MemoryStream ms = new MemoryStream())
        {
            using (BinaryWriter writer = new BinaryWriter(ms))
            {
                foreach (GameObject enemy in GameInstance.Instance.worldGrids.ReturnLives())
                {
                    
                    EnemyController EC = enemy.GetComponent<EnemyController>();
                    //��ġ
                    Vector3 position = EC.Transforms.position;

                    //����
                    Quaternion rotation = EC.Transforms.rotation;

                    //���� Ÿ��
                    int type = EC.enemyStruct.id;
                    int modelType = EC.playerType;

                    writer.Write(EC.Transforms.position.x);
                    writer.Write(EC.Transforms.position.y);
                    writer.Write(EC.Transforms.position.z);

                    writer.Write(EC.Transforms.rotation.x);
                    writer.Write(EC.Transforms.rotation.y);
                    writer.Write(EC.Transforms.rotation.z);
                    writer.Write(EC.Transforms.rotation.w);
                    writer.Write(type);
                    writer.Write(modelType);

                    writer.Write(EC.itemStructs.Count);
                    //���� �κ��丮
                    for (int i = 0; i < EC.itemStructs.Count; i++)
                    {
                        int item_index = EC.itemStructs[i].item_index;
                        string item_name = EC.itemStructs[i].item_name;
                     
                        int slotType = (int)EC.itemStructs[i].slot_type;
                        int itemType = (int)EC.itemStructs[i].item_type;

                        writer.Write(item_index);
                        writer.Write(item_name);
                        writer.Write(slotType);
                        writer.Write(itemType);
                    }

                }
            }
            File.WriteAllBytes(p, ms.ToArray());
        }
    }
}
