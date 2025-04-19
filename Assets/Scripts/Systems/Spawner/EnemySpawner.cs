using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.XR;
using UnityEngine.UIElements;

public class EnemySpawner : MonoBehaviour
{
    public int num;
    bool[,] objectChecks = new bool[100, 100];
    int[] sizeCheckX = new int[8] { 0, 1, -1, 0, 1, 1, -1, 1 };
    int[] sizeCheckY = new int[8] { 1, 0, 0, -1, 1, -1, -1, 1 };

    [NonSerialized]
    public bool loaded = false;
    private void Awake()
    {
        GameInstance.Instance.enemySpawner = this;
    }

    public void Setup()
    {
        loaded = true;
    }

    public void SpawnEnemies()
    {
        for (int i = 0; i < num; i++)
        {
            int x = UnityEngine.Random.Range(0, 100);
            int y = UnityEngine.Random.Range(0, 100);

            int check = 0;
            for (int k = 0; k < 8; k++)
            {
                if (CheckSize(x + sizeCheckX[k], y + sizeCheckY[k]))
                {
                    check++;
                }
            }

            if (check == 8)
            {
                for (int k = 0; k < 8; k++) objectChecks[x + sizeCheckX[k], y + sizeCheckY[k]] = true;
                objectChecks[x, y] = true;
                int type = UnityEngine.Random.Range(1, 2);
                Vector3 pos = new Vector3(x - 49, 0, y - 49);
                SpawnEnemy(pos, type);
            }
        }

        GameInstance.Instance.minimapUI.ChangeList(MinimapIconType.Enemy);
    }
    
    public void LoadEnemies(Vector3 position, Quaternion rotation, NPCCombatStruct npcCombatStruct, List<ItemPackageStruct> itemStructs, string helmet = "", string chest = "", string arms = "", string legs = "", string feet = "", string cape = "")
    {
        if(npcCombatStruct.infected_player)
        {
            //감염된 플레이어 

            GameObject model = Instantiate(AssetLoader.loadedAssets[AssetLoader.humankeys[0]]);
            model.AddComponent<Rigidbody>();
            model.AddComponent<NavMeshAgent>();
            CapsuleCollider capsuleCollider = model.AddComponent<CapsuleCollider>();
            capsuleCollider.center = new Vector3(0, 0.9f, 0);
            capsuleCollider.radius = 0.3f;
            capsuleCollider.height = 1.8f;

            NPCController enemy = model.AddComponent<NPCController>();
            enemy.Setup(npcCombatStruct, false);

            //위치 조정
            enemy.GetTransform.position = position;
            enemy.GetTransform.rotation = rotation;

            //적의 타입으로 충돌 변경
            model.tag = "Enemy";
            model.layer = 0b1010;
            enemy.GetRigidbody.excludeLayers = 0;
            Utility.ChangeTagLayer(enemy.GetTransform, "Enemy", 0b1010);
            enemy.GetComponent<CapsuleCollider>().excludeLayers = 0;
            enemy.GetLeftHand.boxCollider.excludeLayers = 0b10000000000;
            enemy.GetRightHand.boxCollider.excludeLayers = 0b10000000000;

            enemy.itemStructs = itemStructs;
            enemy.Setup(npcCombatStruct, false);
            enemy.Invoke("UpdateController", 0.3f);
            GameInstance.Instance.worldGrids.AddObjects(enemy.gameObject, MinimapIconType.Enemy, true);
            NPCEventHandler.Publish(1000004, enemy);
            if (enemy.GetCharacterAvatar == null) return;
            if (helmet.Length > 0) enemy.GetCharacterAvatar.AddCloth(helmet);
            if (chest.Length > 0) enemy.GetCharacterAvatar.AddCloth(chest);
            if (arms.Length > 0) enemy.GetCharacterAvatar.AddCloth(arms);
            if (legs.Length > 0) enemy.GetCharacterAvatar.AddCloth(legs);
            if (feet.Length > 0) enemy.GetCharacterAvatar.AddCloth(feet);
            if (cape.Length > 0) enemy.GetCharacterAvatar.AddCloth(cape);
        }
        else
        {
            
            NPCController enemy = Instantiate(AssetLoader.loadedAssets[npcCombatStruct.asset_name]).GetComponent<NPCController>();
            enemy.transform.position = position;
            enemy.transform.rotation = rotation;

            enemy.Setup(npcCombatStruct, false);
            GameInstance.Instance.worldGrids.AddObjects(enemy.gameObject, MinimapIconType.Enemy, true);

            NPCEventHandler.Publish(1000004, enemy);
            //enemyController.Setup();
            if (enemy.GetCharacterAvatar == null) return;
            if (helmet.Length > 0) enemy.GetCharacterAvatar.AddCloth(helmet);
            if (chest.Length > 0) enemy.GetCharacterAvatar.AddCloth(chest);
            if (arms.Length > 0) enemy.GetCharacterAvatar.AddCloth(arms);
            if (legs.Length > 0) enemy.GetCharacterAvatar.AddCloth(legs);
            if (feet.Length > 0) enemy.GetCharacterAvatar.AddCloth(feet);
            if (cape.Length > 0) enemy.GetCharacterAvatar.AddCloth(cape);
        }
    
    }


    void SpawnEnemy(Vector3 pos, int type)
    {
        NPCCombatStruct npcStruct = AssetLoader.npcCombatStructs[type];
        NPCController enemy = Instantiate(AssetLoader.loadedAssets[npcStruct.asset_name]).GetComponent<NPCController>();
        enemy.GetTransform.position = pos;
        GameInstance.Instance.worldGrids.AddObjects(enemy.gameObject, MinimapIconType.Enemy, true);
        enemy.Setup(npcStruct, true);
        
        NPCEventHandler.Publish(1000004, enemy);
    }

    void SpawnNPC(Vector3 pos, int type)
    {
        NPCCombatStruct npcStruct = AssetLoader.npcCombatStructs[type];
        NPCController enemy = Instantiate(AssetLoader.loadedAssets[npcStruct.asset_name]).GetComponent<NPCController>();
        GameInstance.Instance.worldGrids.AddObjects(enemy.gameObject, MinimapIconType.Enemy, true);
        enemy.Setup(npcStruct, true);
    }
    bool CheckSize(int a, int b)
    {
        return a >= 0 && a < 100 && b >= 0 && b < 100;
    }
}
