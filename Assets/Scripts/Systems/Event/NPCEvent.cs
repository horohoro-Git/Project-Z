using System;
using System.Collections;
using UnityEngine;

public class NPCEvent : MonoBehaviour
{
    private void Awake()
    {
        GameInstance.Instance.npcEvent = this;
    }
    private void Start()
    {
        AllEventManager.customEvents[1000001] = (Action<NPCEventStruct, NPCController>)(Natural);
        AllEventManager.customEvents[1000002] = (Action<NPCEventStruct, NPCController>)(Friendly);
        AllEventManager.customEvents[1000003] = (Action<NPCEventStruct, NPCController>)(Hostile);

    }
    //�ʱ� �̺�Ʈ
    public void CreateEvents()
    {
        NPCEventHandler.Subscribe(1000001, (Action<NPCEventStruct, NPCController>)AllEventManager.customEvents[1000001]);
        NPCEventHandler.Subscribe(1000002, (Action<NPCEventStruct, NPCController>)AllEventManager.customEvents[1000002]);
        NPCEventHandler.Subscribe(1000003, (Action<NPCEventStruct, NPCController>)AllEventManager.customEvents[1000003]);

       // NPCEventHandler.Publish(1000001, null);
    }

    //���Ŀ� ����Ǵ� ���� �߰�
    public void NewEvent(uint id)
    {
        NPCEventHandler.Subscribe(id, (Action<NPCEventStruct, NPCController>)AllEventManager.customEvents[id]);
    }
    void Natural(NPCEventStruct nPCEventStruct, NPCController controller)
    {
        controller.ChangeEvent(nPCEventStruct);
    }

    void Friendly(NPCEventStruct nPCEventStruct, NPCController controller)
    {
        controller.ChangeEvent(nPCEventStruct);
    }

    void Hostile(NPCEventStruct nPCEventStruct, NPCController controller)
    {
        controller.ChangeEvent(nPCEventStruct);
    }
}
