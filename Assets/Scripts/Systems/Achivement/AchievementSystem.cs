using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementSystem : MonoBehaviour
{

    Dictionary<uint, Action<AchievementStruct>> achieveEvents = new Dictionary<uint, Action<AchievementStruct>>();

    private void Awake()
    {
        GameInstance.Instance.achievementSystem = this;
    }

    public void Start()
    {
        //나무를 베었을 때 발생할 이벤트
        achieveEvents[100001] = CutTree;
        achieveEvents[100002] = CutTree;
    }

    //초기 이벤트
    public void CreateEvents()
    {
        AchievementHandler.Subscribe(100001, achieveEvents[100001]);
    }

    //이후에 연계되는 업적 추가
    public void NewEvent(uint id)
    {
        AchievementHandler.Subscribe(id, achieveEvents[id]);

    }

    void CutTree(AchievementStruct achievementStruct)
    {
        if (achievementStruct.target > achievementStruct.progress)
        {
            Debug.Log(achievementStruct.id + " worked");
            achievementStruct.progress += 1;
            AchievementHandler.UpdateAchievement(achievementStruct);
           
        }
    }
}
