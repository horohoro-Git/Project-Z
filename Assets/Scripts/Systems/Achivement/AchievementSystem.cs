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
        //������ ������ �� �߻��� �̺�Ʈ
        achieveEvents[100001] = CutTree;
        achieveEvents[100002] = CutTree;
    }

    //�ʱ� �̺�Ʈ
    public void CreateEvents()
    {
        AchievementHandler.Subscribe(100001, achieveEvents[100001]);
    }

    //���Ŀ� ����Ǵ� ���� �߰�
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
