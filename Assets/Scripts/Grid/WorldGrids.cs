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

    Dictionary<int, GameObject> objects = new Dictionary<int, GameObject>(); //아이템
    Dictionary<int, GameObject> lives = new Dictionary<int, GameObject>(); //적대적 생명체
    Dictionary<int, GameObject> npcs = new Dictionary<int, GameObject>(); //npc
    Dictionary<int, GameObject> itemBoxes = new Dictionary<int, GameObject>(); //아이템 슬롯
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
  

    //플레이어의 위치를 기록
    public void UpdatePlayerInGrid(PlayerController pc, ref int refX, ref int refY, bool init)
    {
        //10f 간격으로 배열에 배치
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
            if (x == refX && y == refY) return; // 인덱스상 위치가 달라졌을 때에만 배열의 내용을 변경

            players[refX, refY].Remove(pc); //기존 위치에 있던 플레이어 참조 제거

            refX = x;
            refY = y;
            players[x, y].Add(pc);   //새로운 위치에 플레이어 참조
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

    //적의 위치를 기반으로 그리드 탐색
    public List<PlayerController> FindPlayersInGrid(Transform transforms, ref List<PlayerController> lists)
    {
        lists.Clear();
        Vector3 currentPosition = transforms.position;   
        int x = Mathf.FloorToInt(currentPosition.x /10)- indexingMinX;
        int y = Mathf.FloorToInt(currentPosition.z /10)- indexingMinY;

        for (int i = 0; i < 9; i++) // 현재 위치부터 8방향 탐색 
        {
            int posX = x + findX[i];
            int posY = y + findY[i];

            if (ValidCheck(posX, posY)) //인덱스 유효성 체크
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
                   // if (!Physics.Raycast(currentPosition,dir,distance,layerMask)) lists.RemoveAt(j); // 플레이어와 적 사이에 장애물이 있지 않을 때에만
                }
            }
        }
        return lists;
    }

    //적을 탐색
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
                //아이디 갖고 있지 않음
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
                //아이디를 갖고 있고 사용 가능
                dic[identifiable.ID] = ob;
            }
            else //갖고있던 아이디 뺏김
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
                //아이디 갖고 있지 않음
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
                //아이디를 갖고 있고 사용 가능
                lives[identifiable.ID] = livingObject;
            }
            else //갖고있던 아이디 뺏김
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
