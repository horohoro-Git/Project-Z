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
        AchievementHandler.Subscribe(AchievementType.ItemCollected, data =>
        {

        });
    }
}
