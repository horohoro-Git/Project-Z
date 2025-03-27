using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAbilitySystem : MonoBehaviour
{
    public RectTransform leftBorder;
    public RectTransform rightBorder;

    public AbilityData abilityData;

    public List<AbilityData> learnedAbilities = new List<AbilityData>();
    List<AbilityData> notLearnedAbilities = new List<AbilityData>();


    List<AbilityData> tempLearnedAbilities = new List<AbilityData>();
    List<AbilityData> tempNotLearnedAbilities = new List<AbilityData>();


    private void Awake()
    {
        GameInstance.Instance.characterAbilitySystem = this;
    }

    private void OnDisable()
    {
        Revert();
    }

    public void LoadLearns(List<AbilityStruct> abilityStructs)
    {
        foreach (KeyValuePair<int, AbilityStruct> keyValuePair in GameInstance.Instance.assetLoader.abilities)
        {
            int id = keyValuePair.Value.index;

            bool check = false;
            for (int i = 0; i< abilityStructs.Count; i++)
            {
                if(id == abilityStructs[i].index)
                {
                    check = true;
                    break;
                }
            }

            ItemStruct itemStruct = ItemData.GetItem(id);
            AbilityData ability = Instantiate(abilityData);
            ability.Setup(itemStruct, check);

            if (!check)
            {
                ability.GetRectTransform.SetParent(rightBorder.transform);
                notLearnedAbilities.Add(ability);
            }
            else
            {
                ability.GetRectTransform.SetParent(leftBorder.transform);
                learnedAbilities.Add(ability);
            }
        }

        Apply(true);
    }

    public void AddAbility(AbilityData abilityData)
    {
        abilityData.learn = true;
        tempNotLearnedAbilities.Remove(abilityData);
        tempLearnedAbilities.Add(abilityData);
        abilityData.GetRectTransform.SetParent(leftBorder);
    }

    public void RemoveAbility(AbilityData abilityData)
    {
        abilityData.learn = false;
        tempLearnedAbilities.Remove(abilityData);
        tempNotLearnedAbilities.Add(abilityData);
        abilityData.GetRectTransform.SetParent(rightBorder);
    }


    public void Apply(bool init = false)
    {
        if (tempLearnedAbilities.Count == 0 && tempNotLearnedAbilities.Count == 0 && !init) return;
        for (int i = 0; i < tempLearnedAbilities.Count; i++)
        {
            AbilityData abilityData = tempLearnedAbilities[i];
            notLearnedAbilities.Remove(abilityData);
            learnedAbilities.Add(abilityData);
            abilityData.learn = true;
            abilityData.GetRectTransform.SetParent(leftBorder);
        }
        for (int i = 0; i < tempNotLearnedAbilities.Count; i++)
        {
            AbilityData abilityData = tempNotLearnedAbilities[i];
            learnedAbilities.Remove(abilityData);
            notLearnedAbilities.Add(abilityData);
            abilityData.learn = false;
            abilityData.GetRectTransform.SetParent(rightBorder);
        }
        tempLearnedAbilities.Clear();
        tempNotLearnedAbilities.Clear();

        GameInstance.Instance.characterAbilityManager.UpdateAbility(learnedAbilities);
    }

    public void Revert()
    {
        tempLearnedAbilities.Clear();
        tempNotLearnedAbilities.Clear();
        for (int i = 0; i < learnedAbilities.Count; i++)
        {
            AbilityData abilityData = learnedAbilities[i];
            abilityData.learn = true;
            abilityData.GetRectTransform.SetParent(leftBorder);
        }
        for (int i = 0; i < notLearnedAbilities.Count; i++)
        {
            AbilityData abilityData = notLearnedAbilities[i];
            abilityData.learn = false;
            abilityData.GetRectTransform.SetParent(rightBorder);
        }
    }
}
