using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MaxHPUP : MonoBehaviour, IAbility
{
    public ItemStruct itemStruct;

    public void DestroyAbility()
    {
        Release();
        Destroy(gameObject);
    }

    public void Release()
    {

        GameInstance.Instance.characterAbilityManager.maxHp -= 10;
        if (GameInstance.Instance.GetPlayers.Count > 0)
        {
            int hp = GameInstance.Instance.GetPlayers[0].GetPlayer.playerStruct.hp;
            if (hp > GameInstance.Instance.GetPlayers[0].GetPlayer.playerStruct.maxHP)
            {
                GameInstance.Instance.GetPlayers[0].GetPlayer.playerStruct.hp = GameInstance.Instance.GetPlayers[0].GetPlayer.playerStruct.maxHP;
            }
        }
    }

    public void Setup(RectTransform rectTransform)
    {
        GetComponent<RectTransform>().SetParent(rectTransform);
    }

    public void Work()
    {
        GameInstance.Instance.characterAbilityManager.maxHp += 10;
        if (GameInstance.Instance.GetPlayers.Count > 0) GameInstance.Instance.GetPlayers[0].GetPlayer.playerStruct.hp += 10;
    }

  
}
