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
    private void Awake()
    {
        GameInstance.Instance.craftingLearnSystem = this;
    }

    public void LoadLearns(List<CraftStruct> craftStructs)
    {
        foreach(KeyValuePair<int, CraftStruct> keyValuePair in GameInstance.Instance.assetLoader.crafts)
        {
            int id = keyValuePair.Value.index;
            ItemStruct itemStruct = ItemData.GetItem(id);
            CraftingData craftingLearn = Instantiate(craftingData);
            craftingLearn.learnSystem = this;
            craftingLearn.GetComponent<RectTransform>().SetParent(rightBorder.transform);
            craftingLearn.Setup(itemStruct);
            notLearnedDatas.Add(craftingLearn);
        }
    }

    public void Learn(CraftingData craftingData)
    {
        notLearnedDatas.Remove(craftingData);
        learnedDatas.Add(craftingData);
        craftingData.GetComponent<RectTransform>().SetParent(leftBorder.transform);
    }
}
