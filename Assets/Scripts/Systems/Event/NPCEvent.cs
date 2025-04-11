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

    }
    //�ʱ� �̺�Ʈ
    public void CreateEvents()
    {
        NPCEventHandler.Subscribe(1000001, (Action<NPCEventStruct, NPCController>)AllEventManager.customEvents[1000001]);

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
}
