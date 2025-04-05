using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using UnityEngine;

public class SaveLoadSystem
{

    static string path = Application.persistentDataPath;

     public static Hash128 ComputeHash128(byte[] rawData)
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] hashBytes = sha256Hash.ComputeHash(rawData);

            // SHA256 �ؽð��� Hash128���� ��ȯ (32����Ʈ -> 16����Ʈ�� �߶� Hash128���� �����)
            byte[] hash128Bytes = new byte[16];
            System.Array.Copy(hashBytes, hash128Bytes, 16); // �� 16����Ʈ�� ���

            return new Hash128(hash128Bytes[0], hash128Bytes[1], hash128Bytes[2], hash128Bytes[3]);
        }
    }

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
    public static Dictionary<int, ItemStruct> GetItemData(string content)
    {
        string data = EncryptorDecryptor.Decyptor(content, "AAA");

        List<ItemStruct> items = JsonConvert.DeserializeObject<List<ItemStruct>>(data);

        Dictionary<int, ItemStruct> itemDictionary = new Dictionary<int, ItemStruct>();
        for (int i = 0; i < items.Count; i++)
        {
         //   itemDictionary[items[i].item_index] = items[i];
            itemDictionary.Add(items[i].item_index, items[i]);
        }

        return itemDictionary;
    }

    public static List<T> GetListData<T>(string content)
    {
        string data = EncryptorDecryptor.Decyptor(content, "AAA");

        return JsonConvert.DeserializeObject<List<T>>(data);
    }

    public static Dictionary<K, V> GetDictionaryData<K, V>(string content) where V : struct, ITableID<K>
    {
        string data = EncryptorDecryptor.Decyptor(content, "AAA");
        List<V> vList = JsonConvert.DeserializeObject<List<V>>(data);
        Dictionary<K, V> d = new Dictionary<K, V>(); 
        foreach (var item in vList)
        {
            d.Add(item.ID, item);
        }
        return d;
    }

    //���� ������ �ҷ�����
    public static List<WeaponStruct> GetWeaponData(string content)
    {
        string data = EncryptorDecryptor.Decyptor(content, "AAA");

        return JsonConvert.DeserializeObject<List<WeaponStruct>>(data);
    }

    //�� ������ �ҷ�����
    public static List<ArmorStruct> GetArmorData(string content)
    {
        string data = EncryptorDecryptor.Decyptor(content, "AAA");
        return JsonConvert.DeserializeObject<List<ArmorStruct>>(data);
    }

    //�Һ� ������ �ҷ�����
    public static List<ConsumptionStruct> GetConsumptionData(string content)
    {
        string data = EncryptorDecryptor.Decyptor(content, "AAA");

        return JsonConvert.DeserializeObject<List<ConsumptionStruct>>(data);
    }

    //���� ���� ��������
    public static List<CraftStruct> GetCraftData(string content)
    {
        string data = EncryptorDecryptor.Decyptor(content, "AAA");
        return JsonConvert.DeserializeObject<List<CraftStruct>>(data);
    }

    //Ư�� ���� ��������
    public static List<AbilityStruct> GetAbilityData(string content)
    {
        string data = EncryptorDecryptor.Decyptor(content, "AAA");
        return JsonConvert.DeserializeObject<List<AbilityStruct>>(data);
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

    //���� ���̺�
    public static List<AchievementStruct> LoadAchievement(string content)
    {
        string data = EncryptorDecryptor.Decyptor(content, "AAA");
        List<AchievementStruct> achievements = JsonConvert.DeserializeObject<List<AchievementStruct>>(data);

        for(int i = 0; i < achievements.Count; i++)
        {
            string d = achievements[i].reward;
            d = d.Replace("reward_id", "\"reward_id\"");
            d = d.Replace("reward_num", "\"reward_num\"");
            List<AchievementRewardStruct> rewardStructs = JsonConvert.DeserializeObject<List<AchievementRewardStruct>>(d);
            AchievementStruct achievement = achievements[i];
            achievement.rewardStruct = rewardStructs;
            achievements[i] = achievement;
        }

        return achievements;
    }


    //�÷��̾� ������ ����
    public static void SavePlayerData(Player player)
    {
        Debug.Log("����");
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

                //�÷��̾� ���� ����
                writer.Write(player.dead);

                PlayerStruct ps = player.playerStruct;
                PlayerStruct equip = player.equipmentStats;
                //�÷��̾� �ɷ�ġ
                int hp = ps.hp;
                int maxHP = ps.maxHP;
                int energy = ps.energy;
                int maxEnergy = ps.maxEnergy;
                int defense = ps.defense - equip.defense;
                int exp = ps.exp;
                int requireEXP = ps.requireEXP;
                int level = ps.level;
                int attackDamage = ps.attackDamage - equip.attackDamage;
                float attackSpeed = ps.attackSpeed - equip.attackSpeed;
                float moveSpeed = ps.moveSpeed - equip.moveSpeed;
                float weight = ps.weight;
                int skillPoint = ps.skillPoint;
                int hpLevel = ps.hpLevel;
                int energyLevel = ps.energyLevel;
                int weightLevel = ps.weightLevel;
                int backpackLevel = ps.backpackLevel - equip.backpackLevel;

                writer.Write(hp);
                writer.Write(maxHP);
                writer.Write(energy);
                writer.Write(maxEnergy);
                writer.Write(defense);
                writer.Write(exp);
                writer.Write(requireEXP);
                writer.Write(level);
                writer.Write(attackDamage);
                writer.Write(attackSpeed);
                writer.Write(moveSpeed);
                writer.Write(weight);
                writer.Write(skillPoint);
                writer.Write(hpLevel);
                writer.Write(energyLevel);
                writer.Write(weightLevel);
                writer.Write(backpackLevel);


                //���� ����
                CraftingLearnSystem craftingLearnSystem = GameInstance.Instance.craftingLearnSystem;
                writer.Write(craftingLearnSystem.learnedDatas.Count);
                for (int i = 0; i < craftingLearnSystem.learnedDatas.Count; i++)
                {
                    writer.Write(craftingLearnSystem.learnedDatas[i].item.item_index);
                }

                //�÷��̾� Ư��
                CharacterAbilitySystem characterAbilitySystem = GameInstance.Instance.characterAbilitySystem;
                writer.Write(characterAbilitySystem.learnedAbilities.Count);
                for (int i = 0; i < characterAbilitySystem.learnedAbilities.Count; i++)
                {
                    writer.Write(characterAbilitySystem.learnedAbilities[i].item.item_index);
                }
            }
            File.WriteAllBytes(p, ms.ToArray());

        }
    }

    //�÷��̾� ������ �ε�
    public static bool LoadPlayerData(PlayerController pc)
    {
        Debug.Log("�ε�");
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

                        //���� ����
                        bool dead = reader.ReadBoolean();

                        Quaternion rot = new Quaternion(rx, ry, rz, rw);
                        Vector3 pos = new Vector3(x, y, z);

                        //�÷��̾� �ɷ�ġ
                        int hp = reader.ReadInt32();
                        int maxHP = reader.ReadInt32();
                        int energy = reader.ReadInt32();
                        int maxEnergy = reader.ReadInt32();
                        int defense = reader.ReadInt32();
                        int exp = reader.ReadInt32();
                        int requireEXP = reader.ReadInt32();
                        int level = reader.ReadInt32();
                        int attackDamage = reader.ReadInt32();
                        float attackSpeed = reader.ReadSingle();
                        float moveSpeed = reader.ReadSingle();
                        float weight = reader.ReadSingle();
                        int skillPoint = reader.ReadInt32();
                        int hpLevel = reader.ReadInt32();
                        int energyLevel = reader.ReadInt32();
                        int weightLevel = reader.ReadInt32();
                        int backpackLevel = reader.ReadInt32();

                        if (!dead)
                        {
                            PlayerStruct playerStruct = new PlayerStruct(hp, maxHP, energy, maxEnergy, defense, exp, requireEXP, level, attackDamage, attackSpeed, moveSpeed, weight, skillPoint, hpLevel, energyLevel, weightLevel, backpackLevel);
                            pc.SetPlayerData(pos, rot, playerStruct);
                        }
                        else
                        {
                            exp = (int)(exp * 0.9f);
                            //��Ȱ�� ���� �⺻ ��
                            PlayerStruct playerStruct = new PlayerStruct(maxHP / 2, maxHP, energy, maxEnergy, 0, exp, requireEXP, level, attackDamage, 0, 200, 20, skillPoint, hpLevel, energyLevel, weightLevel, 0);
                            pc.SetPlayerData(Vector3.zero, rot, playerStruct);
                        }
                        List<CraftStruct> craftStructs = new List<CraftStruct>();
                        List<AbilityStruct> abilityStructs = new List<AbilityStruct>();
                        //���� ����
                        int craftNum = reader.ReadInt32();
                        for(int i = 0; i < craftNum; i++)
                        {
                            int index = reader.ReadInt32();
                            CraftStruct craftStruct = new CraftStruct();
                            craftStruct.index = index;
                            craftStructs.Add(craftStruct);
                        }

                        //�÷��̾� Ư��
                        int abilityNum = reader.ReadInt32();
                        for (int i = 0; i < abilityNum; i++)
                        {
                            int index = reader.ReadInt32();
                            AbilityStruct abilityStruct = new AbilityStruct();
                            abilityStruct.index = index;
                            abilityStructs.Add(abilityStruct);
                        }

                        GameInstance.Instance.craftingLearnSystem.LoadLearns(craftStructs);
                    
                        GameInstance.Instance.characterAbilitySystem.LoadLearns(abilityStructs);
                    }
                }
            }
            return true;
        }
        else
        {
            List<CraftStruct> craftStructs = new List<CraftStruct>();
            List<AbilityStruct> abilityStructs = new List<AbilityStruct>();
            GameInstance.Instance.craftingLearnSystem.LoadLearns(craftStructs);

            GameInstance.Instance.characterAbilitySystem.LoadLearns(abilityStructs);
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
                            writer.Write(item.asset_name);
                            writer.Write(item.weight);
                            writer.Write((int)item.slot_type);
                            writer.Write((int)item.item_type);

                            if ((int)item.item_type == 1)
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
                            if ((int)item.item_type == 3)
                            {
                                //�� ������
                                ArmorStruct armorStruct = slot.armor;
                             
                                writer.Write(armorStruct.item_index);
                                writer.Write((int)armorStruct.armor_type);
                                writer.Write(armorStruct.defense);
                                writer.Write(armorStruct.durability);
                                writer.Write(armorStruct.move_speed);
                                writer.Write(armorStruct.attack_damage);
                                writer.Write(armorStruct.carrying_capacity);
                                writer.Write(armorStruct.key_index);
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
                            string assetName = reader.ReadString();
                            float weight = reader.ReadSingle();
                            int slotType = reader.ReadInt32();
                            int itemType = reader.ReadInt32();

                            ItemStruct item = new ItemStruct(index, null, name, assetName, weight, (SlotType)slotType, (ItemType)itemType, null);

                            WeaponStruct weaponStruct = new WeaponStruct();
                            ConsumptionStruct consumptionStruct = new ConsumptionStruct();
                            ArmorStruct armorStruct = new ArmorStruct();
                           
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


                            if((ItemType)itemType == ItemType.Wearable)
                            {
                                int item_index = reader.ReadInt32();
                                SlotType armor_type = (SlotType)reader.ReadInt32();
                                int defense = reader.ReadInt32();
                                int durability = reader.ReadInt32();
                                int move_speed = reader.ReadInt32();
                                int attack_damage = reader.ReadInt32();
                                int carrying_capacity = reader.ReadInt32();
                                int key_index = reader.ReadInt32();
                                armorStruct = new ArmorStruct(item_index, armor_type, defense, durability, carrying_capacity, move_speed, attack_damage, key_index);
                            }
                            GameInstance.Instance.inventorySystem.LoadInvetory(X, Y, item, weaponStruct, consumptionStruct, armorStruct);
                            GameInstance.Instance.boxInventorySystem.LoadInvetory(X,Y,item, weaponStruct, consumptionStruct, armorStruct);
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
                BuildSystem_WriteFurniture(writer);
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

                            switch (type)
                            {
                                case MaterialsType.None:
                                    break;
                                case MaterialsType.Floor:
                                    int x = reader.ReadInt32();
                                    int y = reader.ReadInt32();
                                    HousingInfo housingInfo = new HousingInfo(x, y, 0, type, 0);
                                    housingInfos.Add(housingInfo);
                                    break;
                                case MaterialsType.Wall:
                                case MaterialsType.Door:
                                    int wx = reader.ReadInt32();
                                    int wy = reader.ReadInt32();
                                    int wz = reader.ReadInt32();
                                    bool d = reader.ReadBoolean();

                                    HousingInfo housingWInfo = new HousingInfo(wx, wy, wz, d ? MaterialsType.Door : MaterialsType.Wall, 0);
                                    housingInfos.Add(housingWInfo);
                                    break;
                                case MaterialsType.Furniture:
                                    int fx = reader.ReadInt32();
                                    int fy = reader.ReadInt32();
                                    int fz = reader.ReadInt32();
                                    int id = reader.ReadInt32();
                                    HousingInfo housingFInfo = new HousingInfo(fx, fy, fz, type, id);
                                    housingInfos.Add(housingFInfo);

                                    break;
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

    public static void BuildSystem_WriteFurniture(BinaryWriter writer)
    {
        for (int i = 0; i < 100; i++)
        {
            for (int j = 0; j < 100; j++)
            {
                GameObject furnitureGO = GameInstance.Instance.housingSystem.GetFurniture(i, j);
                if (furnitureGO != null)
                {
                    InstallableObject furniture = furnitureGO.GetComponent<InstallableObject>();
                    if (furniture != null)
                    {
                        writer.Write((int)MaterialsType.Furniture);
                        writer.Write(i);
                        writer.Write(j);
                        writer.Write((int)furniture.buildWallDirection);
                        writer.Write(furniture.assetID);
                    }
                }
            }
        }
    }

    //â�� ����, �ε�
    public static void SaveItemBox()
    {
        string dir = Path.Combine(path, "Save");
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        string p = Path.Combine(path, "Save/ItemBox.dat");

        using (MemoryStream ms = new MemoryStream())
        {
            using (BinaryWriter writer = new BinaryWriter(ms))
            {
                for (int i = 0; i < 100; i++)
                {
                    for(int j = 0;j < 100; j++)
                    {
                        GameObject itemBoxGO = GameInstance.Instance.housingSystem.GetFurniture(i, j);
                        if (itemBoxGO != null)
                        {
                            ItemBox itemBox = GameInstance.Instance.housingSystem.GetFurniture(i, j).GetComponent<ItemBox>();
                          
                            if (itemBox != null)
                            {
                                int x = i;
                                int y = j;
                                writer.Write(x);
                                writer.Write(y);
                                for (int k = 0; k < 7; k++)
                                {
                                    for (int l = 0; l < 10; l++)
                                    {
                                        ItemStruct item = itemBox.itemData[k, l];
                                        if (item.item_index == 0)
                                        {
                                            writer.Write(false);
                                            continue;
                                        }
                                        writer.Write(true);

                                        writer.Write(item.item_index);
                                        writer.Write(item.item_name);
                                        writer.Write(item.asset_name);
                                        writer.Write(item.weight);
                                        writer.Write((int)item.slot_type);
                                        writer.Write((int)item.item_type);

                                        switch (itemBox.itemData[k, l].item_type)
                                        {
                                            case ItemType.None:
                                                break;
                                            case ItemType.Consumable:
                                                //�Һ� ������
                                                ConsumptionStruct consumptionStruct = itemBox.consumptionData[k, l];
                                                writer.Write(consumptionStruct.item_index);
                                                writer.Write((int)consumptionStruct.consumption_type);
                                                writer.Write(consumptionStruct.heal_amount);
                                                writer.Write(consumptionStruct.energy_amount);
                                                writer.Write(consumptionStruct.duration);
                                                break;
                                            case ItemType.Equipmentable:
                                                //���� ������
                                                WeaponStruct weaponStruct = itemBox.weaponData[k, l];
                                                writer.Write(weaponStruct.item_index);
                                                writer.Write((int)weaponStruct.weapon_type);
                                                writer.Write(weaponStruct.attack_damage);
                                                writer.Write(weaponStruct.attack_speed);
                                                writer.Write(weaponStruct.max_ammo);
                                                writer.Write(weaponStruct.durability);
                                                break;
                                            case ItemType.Wearable:
                                                //�� ������
                                                ArmorStruct armorStruct = itemBox.armorData[k, l];
                                                writer.Write(armorStruct.item_index);
                                                writer.Write((int)armorStruct.armor_type);
                                                writer.Write(armorStruct.defense);
                                                writer.Write(armorStruct.durability);
                                                writer.Write(armorStruct.move_speed);
                                                writer.Write(armorStruct.attack_damage);
                                                writer.Write(armorStruct.carrying_capacity);
                                                writer.Write(armorStruct.key_index);
                                                break;
                                        }

                                    }

                                }
                            }
                        }
                    }
                }
            }
            File.WriteAllBytes(p, ms.ToArray());
        }
    }

    public static void LoadItemBox()
    {
        List<HousingInfo> housingInfos = new List<HousingInfo>();
        byte[] data;
        string p = Path.Combine(path, "Save/ItemBox.dat");

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
                            int x = reader.ReadInt32();
                            int y = reader.ReadInt32();
                            ItemStruct[,] itemStructs = new ItemStruct[7, 10];
                            ConsumptionStruct[,] consumptionStructs = new ConsumptionStruct[7, 10];
                            WeaponStruct[,] weaponStructs = new WeaponStruct[7, 10];
                            ArmorStruct[,] armorStructs = new ArmorStruct[7, 10];

                            for(int i = 0;i<7; i++)
                            {
                                for(int j =0; j<10; j++)
                                {
                                    bool used = reader.ReadBoolean();
                                 
                                    ConsumptionStruct consumptionStruct = new ConsumptionStruct();
                                    WeaponStruct weaponStruct = new WeaponStruct();
                                    ArmorStruct armorStruct = new ArmorStruct();
                                    ItemStruct itemStruct = new ItemStruct();
                                    itemStructs[i, j] = itemStruct;
                                    consumptionStructs[i, j] = consumptionStruct;
                                    weaponStructs[i, j] = weaponStruct;
                                    armorStructs[i, j] = armorStruct;
                                    if (!used) continue;
                                    int index = reader.ReadInt32();
                                    string name = reader.ReadString();
                                    string assetName = reader.ReadString();
                                    float weight = reader.ReadSingle();
                                    SlotType slotType = (SlotType)reader.ReadInt32();
                                    ItemType itemType = (ItemType)reader.ReadInt32();

                                    itemStruct = new ItemStruct(index,null,name, assetName, weight, slotType,itemType,null);
                                    itemStructs[i, j] = itemStruct;
                                    switch (itemType)
                                    {
                                        case ItemType.None:
                                            break;
                                        case ItemType.Consumable:
                                            int consumpI_index = reader.ReadInt32();
                                            ConsumptionType consumptionType = (ConsumptionType)reader.ReadInt32(); 
                                            int heal_amount = reader.ReadInt32();
                                            int energy_amount = reader.ReadInt32();
                                            float duration = reader.ReadSingle();
                                            consumptionStruct = new ConsumptionStruct(consumpI_index, consumptionType, heal_amount, energy_amount, duration);
                                            consumptionStructs[i, j] = consumptionStruct;
                                            break;
                                        case ItemType.Equipmentable:
                                            int weapon_index = reader.ReadInt32();
                                            WeaponType weaponType = (WeaponType)reader.ReadInt32();
                                            int attack_damage = reader.ReadInt32();
                                            float attack_speed= reader.ReadSingle();
                                            int max_ammo = reader.ReadInt32();
                                            int durability = reader.ReadInt32();
                                            weaponStruct = new WeaponStruct(weapon_index, weaponType,attack_damage,attack_speed,max_ammo,durability);
                                            weaponStructs[i, j] = weaponStruct;
                                            break;
                                        case ItemType.Wearable:
                                            int armor_index = reader.ReadInt32();
                                            SlotType armorType = (SlotType)reader.ReadInt32();
                                            int defense = reader.ReadInt32();
                                            int durabiliry = reader.ReadInt32();
                                            int move_speed = reader.ReadInt32();
                                            int armor_damage = reader.ReadInt32();
                                            int carrying_capacity = reader.ReadInt32();
                                            int key_index = reader.ReadInt32();
                                            armorStruct = new ArmorStruct(armor_index, armorType, defense, durabiliry, carrying_capacity, move_speed, armor_damage, key_index);
                                            armorStructs[i, j] = armorStruct;

                                            break;
                                    
                                    }


                                }
                            }
                            

                            BoxStruct boxStruct = new BoxStruct(itemStructs, consumptionStructs, weaponStructs, armorStructs);


                            //�ε�
                            ItemBox itemBox = GameInstance.Instance.housingSystem.GetFurniture(x, y).GetComponent<ItemBox>();
                            itemBox.LoadItem(boxStruct);
                        }
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
                            Vector3 position = new Vector3(x, y, z);
                            Quaternion rotation = new Quaternion(rx, ry, rz, rw);
                            if (modelType > 0)
                            {
                                string helmet = reader.ReadString();
                                string chest = reader.ReadString();
                                string arms = reader.ReadString();
                                string legs = reader.ReadString();
                                string feet = reader.ReadString();
                                string cape = reader.ReadString();

                                int count = reader.ReadInt32();

                                List<ItemStruct> itemStructs = new List<ItemStruct>();
                                for (int i = 0; i < count; i++)
                                {
                                    int index = reader.ReadInt32();
                                    string name = reader.ReadString();
                                    string assetName = reader.ReadString();
                                    float weight = reader.ReadSingle();
                                    int slotType = reader.ReadInt32();
                                    int itemType = reader.ReadInt32();
                                    ItemStruct item = new ItemStruct(index, null, name, assetName, weight, (SlotType)slotType, (ItemType)itemType, null);
                                    itemStructs.Add(item);

                                }

                                GameInstance.Instance.enemySpawner.LoadEnemies(position, rotation, type, itemStructs, modelType, helmet, chest, arms, legs, feet, cape);
                            }
                            else
                            {

                                GameInstance.Instance.enemySpawner.LoadEnemies(position, rotation, type, new List<ItemStruct>(), modelType);
                            }

                        }
                        GameInstance.Instance.minimapUI.ChangeList(MinimapIconType.Enemy);
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
                foreach (GameObject enemy in GameInstance.Instance.worldGrids.ReturnObjects(MinimapIconType.Enemy))
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

                    if (modelType > 0)
                    {
                        UMACharacterAvatar avatar = EC.GetComponentInChildren<UMACharacterAvatar>();

                        string helmet = avatar.GetClothes("Helmet");
                        string chest = avatar.GetClothes("Chest");
                        string arms = avatar.GetClothes("Arms");
                        string legs = avatar.GetClothes("Legs");
                        string feet = avatar.GetClothes("Feet");
                        string cape = avatar.GetClothes("Cape");

                        writer.Write(helmet);
                        writer.Write(chest);
                        writer.Write(arms);
                        writer.Write(legs);
                        writer.Write(feet);
                        writer.Write(cape);

                        writer.Write(EC.itemStructs.Count);
                        //���� �κ��丮
                        for (int i = 0; i < EC.itemStructs.Count; i++)
                        {
                            int item_index = EC.itemStructs[i].item_index;
                            string item_name = EC.itemStructs[i].item_name;
                            string asset_name = EC.itemStructs[i].asset_name;
                            float item_weight = EC.itemStructs[i].weight;

                            int slotType = (int)EC.itemStructs[i].slot_type;
                            int itemType = (int)EC.itemStructs[i].item_type;

                            writer.Write(item_index);
                            writer.Write(item_name);
                            writer.Write(asset_name);
                            writer.Write(item_weight);
                            writer.Write(slotType);
                            writer.Write(itemType);
                        }
                    }
                }
            }
            File.WriteAllBytes(p, ms.ToArray());
        }
    }
}
