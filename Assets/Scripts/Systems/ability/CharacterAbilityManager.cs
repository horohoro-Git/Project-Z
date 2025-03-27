using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAbilityManager : MonoBehaviour
{

    List<IAbility> abilities = new List<IAbility>();
    public int maxHp = 0;

    private void Awake()
    {
        GameInstance.Instance.characterAbilityManager = this;
    }
    public void ClearAbility()
    {
        for (int i = abilities.Count - 1; i >= 0; i--)
        {
            IAbility ability = abilities[i];
            ability.DestroyAbility();
            abilities.RemoveAt(i);
        }
    }

    public void UpdateAbility(List<AbilityData> abilityDatas)
    {
        ClearAbility();
        for (int i = 0; i < abilityDatas.Count; i++)
        {
            IAbility ability = Instantiate(abilityDatas[i].item.itemGO, transform).GetComponent<IAbility>();
            ability.Work();
            abilities.Add(ability);
        }

        GameInstance.Instance.GetPlayers[0].GetPlayer.Renewal();
    }
}
