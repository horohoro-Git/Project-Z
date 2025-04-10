using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementUI : MonoBehaviour, IUIComponent
{
    public RectTransform ongoingBorder;
    public RectTransform clearBorder;
    public AchievementData achievementEvent;

    Dictionary<uint, AchievementData> ongoingDatas = new Dictionary<uint, AchievementData>();
    Dictionary<uint, AchievementData> clearDatas = new Dictionary<uint, AchievementData>();

    private void Awake()
    {
        GameInstance.Instance.achievementUI = this;
    }

    public void AddEventUI(AchievementStruct achievementStruct, bool ongoing)
    {
        AchievementData achievementData = Instantiate(achievementEvent);
        if (ongoing)
        {
            ongoingDatas[achievementStruct.id] = achievementData;
            achievementData.GetRectTransform.SetParent(ongoingBorder);
            ongoingBorder.sizeDelta = new Vector2(1400, 100 + ongoingDatas.Count * 100f);
            achievementData.achievementStruct = achievementStruct;
            achievementData.Setup();
        }
    }

    public void UpdateUI(AchievementStruct achievementStruct)
    {
        if (ongoingDatas.ContainsKey(achievementStruct.id))
        {
            ongoingDatas[achievementStruct.id].UpdateData(achievementStruct);
        }
    }

    public void ClearData(AchievementData achievementData)
    {
        bool completeClear = false; // ¿¬¼â ¾÷Àû 

        if (achievementData.achievementStruct.achievement_chain == 0) completeClear = true;

        AchievementStruct newAchievement = achievementData.achievementStruct;
        ongoingDatas.Remove(achievementData.achievementStruct.id);
   
     
        if(!completeClear)
        {
            AchievementHandler.UpgradeNewData(newAchievement);
            Destroy(achievementData.gameObject);
        }
        else
        {
            clearDatas[achievementData.achievementStruct.id] = achievementData;
            ongoingBorder.sizeDelta = new Vector2(ongoingBorder.sizeDelta.x, 100f + ongoingDatas.Count);
            achievementData.GetRectTransform.SetParent(clearBorder);
            clearBorder.sizeDelta = new Vector2(clearBorder.sizeDelta.x, 100f + clearDatas.Count);
        }
    }

    public void Setup(bool init)
    {
        
    }
}
