using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCEventHandler : MonoBehaviour
{
    static readonly Dictionary<uint, NPCEventStruct> npcEventStructs = new Dictionary<uint, NPCEventStruct>();
    static readonly Dictionary<uint, Action<NPCEventStruct, NPCController>> npcEvents = new Dictionary<uint, Action<NPCEventStruct, NPCController>>();
    public static void LoadEvent(List<NPCEventStruct> events)
    {
        for (int i = 0; i < events.Count; i++)
        {
            npcEventStructs[events[i].id] = events[i];
        }

        GameInstance.Instance.npcEvent.CreateEvents();
    }
    public static void Subscribe(uint flag, Action<NPCEventStruct, NPCController> callback)
    {
        if (npcEventStructs.ContainsKey(flag))
        {
            npcEvents[flag] = callback;
        }
    }
    public static void Publish(uint flag, NPCController controller)
    {
        if (npcEvents.TryGetValue(flag, out Action<NPCEventStruct, NPCController> callback))
        {
            callback?.Invoke(npcEventStructs[flag], controller);
        }
    }
}
