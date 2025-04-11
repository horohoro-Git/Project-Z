using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementSystem : MonoBehaviour
{

   // Dictionary<uint, Delegate> customEvents = new Dictionary<uint, Delegate>();
   

    private void Awake()
    {
        GameInstance.Instance.achievementSystem = this;
    }

    public void Start()
    {
        //나무를 베었을 때 발생할 이벤트
        AllEventManager.customEvents[100001] = (Action<AchievementStruct>)(CutTree);
        AllEventManager.customEvents[100002] = (Action<AchievementStruct>)(CutTree);
    }

    //초기 이벤트
    public void CreateEvents()
    {
        AchievementHandler.Subscribe(100001, (Action<AchievementStruct>)AllEventManager.customEvents[100001]);
    }

    //이후에 연계되는 업적 추가
    public void NewEvent(uint id)
    {
        AchievementHandler.Subscribe(id, (Action<AchievementStruct>)AllEventManager.customEvents[id]);

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
