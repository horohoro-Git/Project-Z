using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    public PlayerStruct playerStruct;

    Transform transforms;
    public Transform Transforms
    {
        get
        {
            if (transforms == null) transforms = transform;
            return transforms;
        }
    }

  
    GameObject healBuff;
    GameObject levelupBuff;
    Coroutine healCoroutine;
    Coroutine levelupCoroutine;

    BuffStruct buffStruct = new BuffStruct(true);
    float healBuffTimer;

    [NonSerialized]
    public bool dead;

    private void Update()
    {
        if (healBuffTimer < Time.time)
        {
            if(playerStruct.hp > playerStruct.maxHP)
            {
                playerStruct.hp = playerStruct.maxHP;
            }
        }
    }
    public void GetDamage(int damage)
    {
        playerStruct.hp -= damage;
        GameInstance.Instance.playerStatusUI.ChangeHP(playerStruct.hp);
    }

    public void SpendEnergy(int energy)
    {
        playerStruct.energy -= energy;
        GameInstance.Instance.playerStatusUI.ChangeEnergy(playerStruct.energy);
    }

    public void GetExperience(int experience)
    {
        playerStruct.exp += experience;

        if(playerStruct.exp >= playerStruct.requireEXP)
        {
            playerStruct.exp -= playerStruct.requireEXP;

            //다음 레벨의 경험치표로 변경

            playerStruct.level++;
            playerStruct.skillPoint++;
            playerStruct.requireEXP = GameInstance.Instance.assetLoader.levelData[playerStruct.level - 1].exp;

            GameInstance.Instance.playerStatusUI.LevelUp(playerStruct.level, playerStruct.exp, playerStruct.requireEXP);
            buffStruct.levelupNums++;
            if (levelupCoroutine == null) levelupCoroutine = StartCoroutine(LevelupEffect());
        }

        GameInstance.Instance.playerStatusUI.GetEXP(playerStruct.exp);
    }

    public void UpdatePlayer()
    {

        playerStruct.requireEXP = GameInstance.Instance.assetLoader.levelData[playerStruct.level - 1].exp;

        //힐 버프 이펙트
        CreateBuff(ref healBuff, LoadURL.Heal);

        //레벨 업 이펙트
        CreateBuff(ref levelupBuff, LoadURL.LevelUp);
       
        //  playerStruct.skillPoint += 10;
        GameInstance.Instance.playerStatusUI.UpdateUI(playerStruct);
        GameInstance.Instance.abilityMenuUI.GetPoint(playerStruct.skillPoint);
        GameInstance.Instance.abilityMenuUI.ShowChanges(this);
    }


    void CreateBuff(ref GameObject go, string name)
    {
        go = Instantiate(GameInstance.Instance.assetLoader.loadedAssets[name]);
        go.transform.SetParent(Transforms);
        go.transform.localPosition = Vector3.zero;
        go.SetActive(false);
    }

    public void GetBuff(ConsumptionItem item)
    {
        Debug.Log(item.consumtionType);
        switch(item.consumtionType)
        {
            case ConsumptionType.None:
                break;
            case ConsumptionType.Heal:
                buffStruct.healBuff.Enqueue(item.consumtionStruct);
                if(healCoroutine == null) healCoroutine = StartCoroutine(HPRecovery(item.consumtionStruct.duration, item.consumtionStruct.heal_amount));
                break;

            case ConsumptionType.EnergyRegain:
                break;

        }
       
    }

    IEnumerator HPRecovery(float duration, int recoveryAmount)
    {
        healBuff.SetActive(true);
        float currentTimer = 0;
        float timer = 0;
        while (currentTimer / duration < 1f)
        {
            while (timer < 1)
            {
                timer += Time.deltaTime;
                yield return null;
            }
            currentTimer += timer;
            timer = 0;
         
            playerStruct.hp += (int)(recoveryAmount / duration);
            if(playerStruct.hp > playerStruct.maxHP)
            {
                playerStruct.hp = playerStruct.maxHP;
                healBuffTimer = Time.time + 10f;
            }
            GameInstance.Instance.playerStatusUI.ChangeHP(playerStruct.hp);
        }

        buffStruct.healBuff.Dequeue();

        if(buffStruct.healBuff.Count > 0)
        {
            ConsumptionStruct consumtionStruct = buffStruct.healBuff.Peek();
            healCoroutine = StartCoroutine(HPRecovery(consumtionStruct.duration, consumtionStruct.heal_amount));
        }

        healBuff.SetActive(false);
        healCoroutine = null;
    }

    IEnumerator LevelupEffect()
    {
        levelupBuff.SetActive(true);
        float timer = 0;
        while (timer < 2)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        buffStruct.levelupNums--;

        if(buffStruct.levelupNums > 0)
        {
            levelupCoroutine = StartCoroutine(LevelupEffect());
        }
        levelupBuff.SetActive(false);
        levelupBuff = null;
    }
}
