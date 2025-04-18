using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.LowLevel;
using UnityEngine.UIElements;

public class WorldGrids : MonoBehaviour
{
    
    public float minX;
    public float minY;
    public float maxX;
    public float maxY;
    private int indexingMinX;
    private int indexingMaxX;
    private int indexingMinY;
    private int indexingMaxY;
    int sizeX;
    int sizeY;
    List<PlayerController>[,] players;
    Dictionary<int, PlayerController> playerControllersDic = new Dictionary<int, PlayerController>();
  //  int layerMask = 1 << 3; 
    int[] findX = new int[9] {0, 1, 1, 0,-1,-1,-1,0,1 };
    int[] findY = new int[9] {0, 0, -1, -1,-1, 0, 1, 1, 1 };

    Dictionary<int, GameObject> objects = new Dictionary<int, GameObject>(); //������
    Dictionary<int, GameObject> lives = new Dictionary<int, GameObject>(); //������ ����ü
    Dictionary<int, GameObject> npcs = new Dictionary<int, GameObject>(); //npc
    Dictionary<int, GameObject> itemBoxes = new Dictionary<int, GameObject>(); //������ ����
    //NativeHashMap<string>

    private void Awake()
    {
        GameInstance.Instance.worldGrids = this;
        indexingMinX = Mathf.FloorToInt(minX / 10f);
        indexingMinY = Mathf.FloorToInt(minY / 10f);
        indexingMaxX = Mathf.FloorToInt(maxX / 10f);
        indexingMaxY = Mathf.FloorToInt(maxY / 10f);

        sizeX = indexingMaxX - indexingMinX + 1;
        sizeY = indexingMaxY - indexingMinY + 1;
        players = new List<PlayerController>[sizeX, sizeY];
        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                players[i, j] = new List<PlayerController>();
            }
        }
        //    players = new List<PlayerController>[index]
    }
  

    //�÷��̾��� ��ġ�� ���
    public void UpdatePlayerInGrid(PlayerController pc, ref int refX, ref int refY, bool init)
    {
        //10f �������� �迭�� ��ġ
        Vector3 playerLoc = pc.Transforms.position;
        int x = Mathf.FloorToInt(playerLoc.x / 10) - indexingMinX;
        int y = Mathf.FloorToInt(playerLoc.z / 10) - indexingMinY;
        if (init)
        {
            playerControllersDic[pc.GetInstanceID()] = pc;

            refX = x;
            refY = y;
            players[x, y].Add(pc);
        }
        else
        {
            if (x == refX && y == refY) return; // �ε����� ��ġ�� �޶����� ������ �迭�� ������ ����

            players[refX, refY].Remove(pc); //���� ��ġ�� �ִ� �÷��̾� ���� ����

            refX = x;
            refY = y;
            players[x, y].Add(pc);   //���ο� ��ġ�� �÷��̾� ����
        }
    }

    public void RemovePlayer(PlayerController pc, ref int refX, ref int refY)
    {
        Vector3 playerLoc = pc.Transforms.position;
        int x = Mathf.FloorToInt(playerLoc.x / 10) - indexingMinX;
        int y = Mathf.FloorToInt(playerLoc.z / 10) - indexingMinY;

        playerControllersDic.Remove(pc.GetInstanceID());
        players[refX, refY].Remove(pc);
    }

    //���� ��ġ�� ������� �׸��� Ž��
    public List<PlayerController> FindPlayersInGrid(Transform transforms, ref List<PlayerController> lists)
    {
        lists.Clear();
        Vector3 currentPosition = transforms.position;   
        int x = Mathf.FloorToInt(currentPosition.x /10)- indexingMinX;
        int y = Mathf.FloorToInt(currentPosition.z /10)- indexingMinY;

        for (int i = 0; i < 9; i++) // ���� ��ġ���� 8���� Ž�� 
        {
            int posX = x + findX[i];
            int posY = y + findY[i];

            if (ValidCheck(posX, posY)) //�ε��� ��ȿ�� üũ
            {
                List<PlayerController> controllers = players[posX, posY];
                int preCount = lists.Count;
                lists.AddRange(controllers);

                for (int j = lists.Count - 1; j >= preCount; j--)
                {
                    PlayerController controller = controllers[j];
                    Vector3 dir = controller.Transforms.position - currentPosition;
                    float distance = dir.magnitude;
                    dir = Vector3.Normalize(dir);
                   // if (!Physics.Raycast(currentPosition,dir,distance,layerMask)) lists.RemoveAt(j); // �÷��̾�� �� ���̿� ��ֹ��� ���� ���� ������
                }
            }
        }
        return lists;
    }

    //���� Ž��
    public Dictionary<int, GameObject> FindEnemiesInGrid()
    {

        return lives;
    }

    public Dictionary<int, PlayerController> FindPlayerDictinary()
    {

        return playerControllersDic;
    }

    bool ValidCheck(int x, int y)
    {
     
        return x >= 0 && x < sizeX && y >=0 && y < sizeY;

    }
    string ConvertBinaryToString(byte[] binary)
    {
        return BitConverter.ToString(binary).Replace("-", "").ToLower();
    }
    public void AddObjects(GameObject ob, MinimapIconType type, bool load)
    {
        Dictionary<int, GameObject> dic = new Dictionary<int, GameObject>();
        switch(type)
        {
            case MinimapIconType.None:
                break;
            case MinimapIconType.Object:
                dic = objects;
                break;
            case MinimapIconType.Enemy:
                dic = lives;
                break;
            case MinimapIconType.NPC:
                dic = npcs;
                break;
            case MinimapIconType.ItemBox:
                dic = itemBoxes;
                break;
        }
        int id = ob.GetInstanceID();
        ob.GetComponent<IIdentifiable>().ID = id;
        dic[id] = ob;
      //  IIdentifiable identifiable = ob.GetComponent<IIdentifiable>();
      //  identifiable.ID = 
      /*  if (identifiable.ID == null)
        {
            while (true)
            {
                //���̵� ���� ���� ����
                byte[] random = Guid.NewGuid().ToByteArray();
                string key = ConvertBinaryToString(random);
                if (!dic.ContainsKey(key))
                {
                    identifiable.ID = key;
                    dic[key] = ob;
                    break;
                }
            }
        }
        else
        {
            if (!dic.ContainsKey(identifiable.ID))
            {
                //���̵� ���� �ְ� ��� ����
                dic[identifiable.ID] = ob;
            }
            else //�����ִ� ���̵� ����
            {
                while (true)
                {
                    byte[] random = Guid.NewGuid().ToByteArray();
                    string key = ConvertBinaryToString(random);
                    if (!dic.ContainsKey(key))
                    {
                        identifiable.ID = key;
                        dic[key] = ob;
                        break;
                    }
                }
            }
        }*/
        if (!load) GameInstance.Instance.minimapUI.ChangeList(type);
    }

    public void RemoveObjects(int id, MinimapIconType type)
    {
      
        switch (type)
        {
            case MinimapIconType.None:
                break;
            case MinimapIconType.Object:
                objects.Remove(id);
                GameInstance.Instance.minimapUI.ChangeList(MinimapIconType.Object);
                break;
            case MinimapIconType.Enemy:
                lives.Remove(id);
                GameInstance.Instance.minimapUI.ChangeList(MinimapIconType.Enemy);
                break;
            case MinimapIconType.NPC:
                npcs.Remove(id);
                GameInstance.Instance.minimapUI.ChangeList(MinimapIconType.NPC);
                break;
            case MinimapIconType.ItemBox:
                itemBoxes.Remove(id);
                GameInstance.Instance.minimapUI.ChangeList(MinimapIconType.ItemBox);
                break;
        }
        //  ob.ID = null;
        
    }

    public List<GameObject> ReturnObjects(MinimapIconType type)
    {
        Dictionary<int, GameObject> dic = new Dictionary<int, GameObject>();
        switch (type)
        {
            case MinimapIconType.None:
                break;
            case MinimapIconType.Object:
                dic = objects;
                break;
            case MinimapIconType.Enemy:
                dic = lives;
                break;
            case MinimapIconType.ItemBox:
                dic = itemBoxes;
                break;
        }
        List<GameObject> returnObjects = new List<GameObject>();
        foreach (var obj in dic)
        {
            returnObjects.Add(obj.Value);
        }
        return returnObjects;
    }


  /*  public void AddLives(GameObject livingObject, bool load)
    {
        IIdentifiable identifiable = livingObject.GetComponent<IIdentifiable>();
        if(identifiable.ID == null)
        {
            while (true)
            {
                //���̵� ���� ���� ����
                byte[] random = Guid.NewGuid().ToByteArray();
                string key = ConvertBinaryToString(random);
                if (!lives.ContainsKey(key))
                {
                    identifiable.ID = key;
                    lives[key] = livingObject;
                    break;
                }
            }
        }
        else
        {
            if (!lives.ContainsKey(identifiable.ID))
            {
                //���̵� ���� �ְ� ��� ����
                lives[identifiable.ID] = livingObject;
            }
            else //�����ִ� ���̵� ����
            {
                while (true)
                {
                    byte[] random = Guid.NewGuid().ToByteArray();
                    string key = ConvertBinaryToString(random);
                    if (!lives.ContainsKey(key))
                    {
                        identifiable.ID = key;
                        lives[key] = livingObject;
                        break;
                    }
                }
            }
        }

        if(!load) GameInstance.Instance.minimapUI.ChangeList(MinimapIconType.Enemy);
      
    }
*/

  /*  public void RemoveLives(string ID)
    {
        lives.Remove(ID);
        GameInstance.Instance.minimapUI.ChangeList(MinimapIconType.Enemy);
    }*/


  /*  public List<GameObject> ReturnLives()
    {
        List<GameObject> returnObjects = new List<GameObject>();
     
        foreach (var livingObject in lives)
        {
            returnObjects.Add(livingObject.Value);
        }

        return returnObjects;
    }*/

    public void Clear()
    {
       /* foreach(var livingObject in lives) Destroy(livingObject.Value);
        foreach(var objectItem in objects) Destroy(objectItem.Value);
        foreach(var boxes in itemBoxes) Destroy(boxes.Value);*/
        lives.Clear();
        objects.Clear();
        itemBoxes.Clear();
    }
}
