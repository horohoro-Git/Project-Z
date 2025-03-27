using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingLearnSystem : MonoBehaviour
{
    public CraftingData craftingData;
    public RectTransform leftBorder;
    public RectTransform rightBorder;
    [NonSerialized]
    public List<CraftingData> learnedDatas = new List<CraftingData>();
    [NonSerialized]
    public List<CraftingData> notLearnedDatas = new List<CraftingData>();

   
    List<CraftingData> tempLearnedDatas = new List<CraftingData>();
    List<CraftingData> tempNotLearnedDatas = new List<CraftingData>();
    private void Awake()
    {
        GameInstance.Instance.craftingLearnSystem = this;
    }
    private void OnDisable()
    {
        Revert();
    }

    public void LoadLearns(List<CraftStruct> craftStructs)
    {
        foreach(KeyValuePair<int, CraftStruct> keyValuePair in GameInstance.Instance.assetLoader.crafts)
        {
            int id = keyValuePair.Value.index;
            bool check = false;
            for (int i = 0; i < craftStructs.Count; i++)
            {
                if (id == craftStructs[i].index)
                {
                    check = true;
                    break;
                }
            }

            ItemStruct itemStruct = ItemData.GetItem(id);
            CraftingData craftingLearn = Instantiate(craftingData);
            craftingLearn.learnSystem = this;
            craftingLearn.Setup(itemStruct, check);
            if (!check)
            {
                craftingLearn.GetComponent<RectTransform>().SetParent(rightBorder.transform);  
                notLearnedDatas.Add(craftingLearn);
            }
            else
            {
                craftingLearn.GetComponent<RectTransform>().SetParent(leftBorder.transform);
                learnedDatas.Add(craftingLearn);
            }
        }
    }

    public void AddLearn(CraftingData craftingData)
    {
        craftingData.learned = true;
        tempNotLearnedDatas.Remove(craftingData);
        tempLearnedDatas.Add(craftingData);
        craftingData.GetRectTransform.SetParent(leftBorder.transform);
    }
    public void RemoveLearn(CraftingData craftingData)
    {
        craftingData.learned = false;
        tempLearnedDatas.Remove(craftingData);
        tempNotLearnedDatas.Add(craftingData);
        craftingData.GetRectTransform.SetParent(rightBorder.transform);
    }
    public void Apply()
    {
        if (tempLearnedDatas.Count > 0 || tempNotLearnedDatas.Count > 0)
        {
            for (int i = 0; i < tempLearnedDatas.Count; i++)
            {
                CraftingData craftData = tempLearnedDatas[i];
                notLearnedDatas.Remove(craftData);
                learnedDatas.Add(craftData);
                craftData.learned = true;
                craftData.GetRectTransform.SetParent(leftBorder);
            }
            for (int i = 0; i < tempNotLearnedDatas.Count; i++)
            {
                CraftingData craftData = tempNotLearnedDatas[i];
                learnedDatas.Remove(craftData);
                notLearnedDatas.Add(craftData);
                craftData.learned = false;
                craftData.GetRectTransform.SetParent(rightBorder);
            }
        }
    }

    public void Revert()
    {
        tempNotLearnedDatas.Clear();
        tempLearnedDatas.Clear();

        for (int i = 0; i < learnedDatas.Count; i++)
        {
            CraftingData craftData = learnedDatas[i];
            craftData.learned = true;
            craftData.GetRectTransform.SetParent(leftBorder);
        }
        for (int i = 0; i < notLearnedDatas.Count; i++)
        {
            CraftingData craftData = notLearnedDatas[i];
            craftData.learned = false;
            craftData.GetRectTransform.SetParent(rightBorder);
        }
    }
}
