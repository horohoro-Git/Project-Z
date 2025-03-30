using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AchievementHandler
{
    static readonly Dictionary<AchievementType, Action<object>> ahievements = new Dictionary<AchievementType, Action<object>>();
    public static void Subscribe(AchievementType type, Action<object> callback)
    {
        if (!ahievements.ContainsKey(type))
        {
            ahievements[type] = callback;
        }
        else
        {
            ahievements[type] += callback;
        }
    }

    public static void Publish(AchievementType achivementType, object data = null)
    {
        if (ahievements.TryGetValue(achivementType, out Action<object> callback))
        {
            callback?.Invoke(data);
        }
    }
}
