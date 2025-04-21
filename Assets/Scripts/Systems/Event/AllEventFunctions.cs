// 1 : 생명체 호버기능 동작
// 2 : 호버 기능 중지
// 3 : 장비 착용
// 4 : 사망(플레이어)
// 4-1 : 좀비화 (플레이어)
// 4-2 : 부활
// 5 : 사망(NPC)


using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.Windows;

public class AllEventFunctions : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AllEventManager.customEvents[1] = (Action<string, int, int>)(Hover);
        AllEventManager.customEvents[2] = (Action)(Hide);
        AllEventManager.customEvents[3] = (Action<PlayerController, Slot, int, bool>)(Equipment);
        AllEventManager.customEvents[4] = (Action<PlayerController, int>)(Dead_Player);
        AllEventManager.customEvents[5] = (Action<NPCController>)(Dead_NPC);
    }

    //1
    void Hover(string objectName, int maxHP, int hp)
    {
        LifeObjectHoverInfoUI hoverUI = GameInstance.Instance.lifeObjectHoverUI.hoverUI;
        hoverUI.gameObject.SetActive(true);
        hoverUI.Setup(objectName, maxHP, hp);
    }

    //2
    void Hide()
    {
        LifeObjectHoverInfoUI hoverUI = GameInstance.Instance.lifeObjectHoverUI.hoverUI;
        hoverUI.gameObject.SetActive(false);
    }

    //3
    void Equipment(PlayerController pc, Slot equipItem, int index, bool forcedEquip = false)
    {
        bool equip = true;
        //무기
        if (equipItem.item.item_type == ItemType.Equipmentable)
        {
            if (pc.equipItem != null)
            {
                Destroy(pc.equipItem.gameObject);
                pc.equipItem = null;
            }
            pc.equipItem = Instantiate(AssetLoader.loadedAssets[AssetLoader.itemAssetkeys[equipItem.item.item_index].Name]).GetComponent<Item>();  //equipItem.itemGO).GetComponent<Item>();
            Destroy(pc.equipItem.GetComponent<Rigidbody>());
            pc.equipItem.equippedPlayer = pc.GetPlayer;
            AttachItem attachItem = pc.GetComponentInChildren<AttachItem>();
            pc.equipItem.transform.SetParent(attachItem.transform);
            pc.equipItem.itemStruct = equipItem.item;
            pc.equipItem.GetComponent<Weapon>().weaponStruct = equipItem.weapon;
            pc.equipItem.GetItemInteractionColider.enabled = false;
            pc.equipItem.transform.localPosition = Vector3.zero;
            pc.equipItem.transform.localRotation = Quaternion.Euler(-90, -90, 0);
            pc.modelAnimator.SetFloat("equip", 1);

            GameInstance.Instance.characterProfileUI.UnEquipItem();
            GameInstance.Instance.characterProfileUI.EquipItem(AssetLoader.loadedAssets[AssetLoader.itemAssetkeys[equipItem.item.item_index].Name]);
        }
        else if (equipItem.item.item_type == ItemType.Consumable)     //음식을 손에 듬
        {
            if (pc.equipItem != null)
            {
                Destroy(pc.equipItem.gameObject);
                pc.equipItem = null;
            }
            pc.equipItem = Instantiate(AssetLoader.loadedAssets[AssetLoader.itemAssetkeys[equipItem.item.item_index].Name]).GetComponent<Item>();
            pc.equipItem.equippedPlayer = pc.GetPlayer;
            AttachItem attachItem = pc.GetComponentInChildren<AttachItem>();
            pc.equipItem.transform.SetParent(attachItem.transform);
            pc.equipItem.itemStruct = equipItem.item;
            pc.equipItem.GetComponent<ConsumptionItem>().consumtionStruct = equipItem.consumption;
            pc.equipItem.GetItemInteractionColider.enabled = false;
            pc.equipItem.transform.localPosition = Vector3.zero;
            pc.equipItem.transform.localRotation = Quaternion.Euler(-90, -90, 0);
            pc.modelAnimator.SetFloat("equip", 0);

            GameInstance.Instance.characterProfileUI.UnEquipItem();
            GameInstance.Instance.characterProfileUI.EquipItem(AssetLoader.loadedAssets[AssetLoader.itemAssetkeys[equipItem.item.item_index].Name]);
        }
        else
        {
            if (pc.equipItem != null)
            {
                Destroy(pc.equipItem.gameObject);
                pc.equipItem = null;
            }
            pc.modelAnimator.SetFloat("equip", 0);
            equip = false;
            GameInstance.Instance.characterProfileUI.UnEquipItem();
        }

        GameInstance.Instance.inventorySystem.EquipItem(equipItem, equip);
    }

    //4
    void Dead_Player(PlayerController controller, int opponentLayer)
    {
       
        controller.GetPlayer.dead = true;
        controller.state = PlayerState.Dead;
        controller.playerCamera.lookAround = false;
        controller.lookAround = false;
        GameInstance.Instance.worldGrids.RemovePlayer(controller.gameObject, ref controller.lastGridX, ref controller.lastGridY);
        controller.Rigid.excludeLayers = 0;
        controller.GetComponent<CapsuleCollider>().excludeLayers = 0;
        controller.Rigid.interpolation = RigidbodyInterpolation.None;
        if (opponentLayer == 0b1010)
        {
            Zombification(controller);
           
        }
        else
        {
            //인벤토리 초기화
            GameInstance.Instance.inventorySystem.ResetInventory();

            //인벤토리 저장
            SaveLoadSystem.SaveInventoryData();

         
          //  controller.Inputs.actions["OpenInventory"].performed -= controller.OpenInventory;
            controller.Rigid.velocity = Vector3.zero;
            controller.Rigid.constraints = RigidbodyConstraints.FreezeAll;
            StartCoroutine(RespawnPlayer(controller));
         
        }
        GameInstance.Instance.uiManager.SwitchUI(UIType.Dead);

    }

    //4-1
    void Zombification(PlayerController controller)
    {
        controller.gameObject.tag = "Enemy";
        controller.gameObject.layer = 0b1010;
        Utility.ChangeTagLayer(controller.Transforms, "Enemy", 0b1010);
        controller.GetLeftHand.boxCollider.excludeLayers = 0b10000000000;
        controller.GetRightHand.boxCollider.excludeLayers = 0b10000000000;

        //죽은 시체에 인벤토리의 아이템 적용
        controller.gameObject.AddComponent<NavMeshAgent>();
        NPCController enemyController = controller.gameObject.AddComponent<NPCController>();
        enemyController.bDead = true;
        Player player = controller.GetPlayer;
     
        enemyController.npcStruct = new NPCCombatStruct(0, "male", "male", 100, 100, 50, true, 1.5f, 1.2f, "", true, null);
        GameInstance.Instance.worldGrids.AddObjects(enemyController.gameObject, MinimapIconType.Enemy, false);
        for (int i = 0; i < GameInstance.Instance.inventorySystem.slotNum; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                Slot slot = GameInstance.Instance.inventorySystem.inventoryArray[i, j];
                ItemStruct itemStruct = slot.item;
                WeaponStruct weaponStruct = slot.weapon;
                ConsumptionStruct consumption = slot.consumption;
                ArmorStruct armor = slot.armor;
                if (itemStruct.item_index == 0) continue;
                enemyController.itemStructs.Add(new ItemPackageStruct(itemStruct,weaponStruct, consumption, armor));
            }
        }
        SaveLoadSystem.SavePlayerData(controller.GetPlayer);

        //인벤토리 초기화
        GameInstance.Instance.inventorySystem.ResetInventory();

        //인벤토리와 적 데이터 저장
        SaveLoadSystem.SaveEnemyInfo();
        SaveLoadSystem.SaveInventoryData();

        controller.Invoke("Infected", 2f);
    }

    //4-2
    IEnumerator RespawnPlayer(PlayerController controller)
    {
        float timer = 0;
        while (timer < 10)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        Destroy(controller.GetComponent<PlayerInput>());

        controller.RemoveAction();
        controller.Inputs.actions["OpenInventory"].performed -= controller.OpenInventory;
        GameInstance.Instance.gameManager.PlayerSettings(false);

        if (controller.playerCamera != null) controller.playerCamera.ResetPlayer();
        Destroy(controller);
    }

    //5
    void Dead_NPC(NPCController controller)
    {
        GameInstance.Instance.worldGrids.RemoveObjects(controller.ID, controller.npcStruct.infected ? MinimapIconType.Enemy : MinimapIconType.NPC);

        CapsuleCollider capsule = controller.GetComponent<CapsuleCollider>();
        if (controller.itemStructs.Count > 0)
        {
            capsule.isTrigger = true;
        }
        else
        {
            capsule.enabled = false;
        }
        Rigidbody r = controller.GetComponent<Rigidbody>();
        r.useGravity = false;
        r.excludeLayers = 0b111111111111111;
    }
}
