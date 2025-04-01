using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AchievementHandler
{
    static readonly Dictionary<uint, Action<AchievementStruct>> achievements = new Dictionary<uint, Action<AchievementStruct>>();

    static readonly Dictionary<uint, AchievementStruct> achieveEvents = new Dictionary<uint, AchievementStruct>();

    static readonly Dictionary<uint, AchievementStruct> clearEvents = new Dictionary<uint, AchievementStruct>();
    public static void LoadEvent(List<AchievementStruct> achievements)
    {
        //가져온 테이블의 데이터 할당
        for (int i = 0; i < achievements.Count; i++) achieveEvents[achievements[i].id] = achievements[i];

        //이벤트 구독 시작
        GameInstance.Instance.achievementSystem.CreateEvents();
    }

    //이벤트 구독
    public static void Subscribe(uint flag, Action<AchievementStruct> callback)
    {
        if (achieveEvents.ContainsKey(flag))
        {
            if (!achieveEvents[flag].complete)
            {
                if (!achievements.ContainsKey(flag))
                {
                    achievements[flag] = callback;
                    GameInstance.Instance.achievementUI.AddEventUI(achieveEvents[flag], true);
                }
            }
            else
            {
                if (!clearEvents.ContainsKey(flag)) clearEvents[flag] = achieveEvents[flag];
            }
        }
    }

    public static void Publish(uint achivementFlag)
    {
        //이벤트 실행
        if (achievements.TryGetValue(achivementFlag, out Action<AchievementStruct> callback))
        {
            callback?.Invoke(achieveEvents[achivementFlag]);
        }

    }

    public static void UpdateAchievement(AchievementStruct achievementStruct)
    {
        if (achievementStruct.complete)
        {
            Debug.Log("Already Completed");
            return;
        }

        Debug.Log(achievementStruct.progress + " / " + achievementStruct.target);


        if (achievementStruct.progress == achievementStruct.target)
        {
            achievementStruct.complete = true;
            achievements.Remove(achievementStruct.id);
            Debug.Log("Complete");
            clearEvents[achievementStruct.id] = achievementStruct;
           
        }
        achieveEvents[achievementStruct.id] = achievementStruct;
        GameInstance.Instance.achievementUI.UpdateUI(achievementStruct);
    }
}
