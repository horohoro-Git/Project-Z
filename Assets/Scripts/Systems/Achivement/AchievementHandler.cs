using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AchievementHandler
{
    static readonly Dictionary<uint, Action<AchievementStruct>> achievements = new Dictionary<uint, Action<AchievementStruct>>();

    static readonly Dictionary<uint, AchievementStruct> achieveEvents = new Dictionary<uint, AchievementStruct>();

    public static void LoadEvent(List<AchievementStruct> achievements)
    {
        //������ ���̺��� ������ �Ҵ�
        for (int i = 0; i < achievements.Count; i++) achieveEvents[achievements[i].id] = achievements[i];

        //�̺�Ʈ ���� ����
        GameInstance.Instance.achievementSystem.CreateEvents();
    }

    //�̺�Ʈ ����
    public static void Subscribe(uint flag, Action<AchievementStruct> callback)
    {
        if (!achievements.ContainsKey(flag)) achievements[flag] = callback;
    
    }

    public static void Publish(uint achivementFlag)
    {
        //�̺�Ʈ ����
        if (achievements.TryGetValue(achivementFlag, out Action<AchievementStruct> callback))
        {
            callback?.Invoke(achieveEvents[achivementFlag]);
        }

    }

    public static void UpdateAchievement(AchievementStruct achievementStruct)
    {
        Debug.Log(achievementStruct.progress + " / " + achievementStruct.target);
        if (achievementStruct.progress == achievementStruct.target) Debug.Log("Complete");
        achieveEvents[achievementStruct.id] = achievementStruct;
        achievements.Remove(achievementStruct.id);
    }
}
