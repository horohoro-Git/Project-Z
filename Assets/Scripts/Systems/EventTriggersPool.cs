using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventTriggersPool : MonoBehaviour
{
    static Queue<EventTrigger> eventTriggers = new Queue<EventTrigger>();
    // Start is called before the first frame update

    private void Awake()
    {
        for (int i = 0; i < 200; i++)
        {
            GameObject a = new GameObject();

            EventTrigger eventTrigger = a.AddComponent<EventTrigger>();
            eventTrigger.enabled = false;
            a.SetActive(false);
            eventTriggers.Enqueue(eventTrigger);
        }
    }

    public static EventTrigger GetEventTrigger()
    {
        EventTrigger e = eventTriggers.Dequeue();
        e.enabled = true;
        e.gameObject.SetActive(true);
        return e; 
    }
    public static void ReturnTrigger(EventTrigger eventTrigger)
    {
        eventTrigger.enabled = false;
        eventTrigger.gameObject.SetActive(false);
        eventTriggers.Enqueue(eventTrigger);
    }
}
