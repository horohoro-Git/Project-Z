using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementSystem : MonoBehaviour
{
    private void Awake()
    {
        GameInstance.Instance.achievementSystem = this;
    }

    public void Start()
    {
       // AchievementHandler.Subscribe(100001, acti => CutTree());
    }
    public void CreateEvents()
    {
        AchievementHandler.Subscribe(100001, (acti) => { CutTree(acti); });
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
