using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

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
    int[] findX = new int[9] {0, 1, 1, 0,-1,-1,-1,0,1 };
    int[] findY = new int[9] {0, 0, -1, -1,-1, 0, 1, 1, 1 };

    Dictionary<string, GameObject> objects = new Dictionary<string, GameObject>();   
    Dictionary<string, GameObject> lives = new Dictionary<string, GameObject>();
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
    public void UpdatePlayerInGrid(PlayerController pc, ref int refX, ref int refY)
    {
        //10f �������� �迭�� ��ġ
        Vector3 playerLoc = pc.Transforms.position; 
        int x = Mathf.FloorToInt(playerLoc.x / 10) - indexingMinX; 
        int y = Mathf.FloorToInt(playerLoc.y / 10) - indexingMinY;

        if (x == refX && y == refY) return; // �ε����� ��ġ�� �޶����� ������ �迭�� ������ ����

        players[refX, refY].Remove(pc); //���� ��ġ�� �ִ� �÷��̾� ���� ����

        refX = x;
        refY = y;
        players[x,y].Add(pc);   //���ο� ��ġ�� �÷��̾� ����
    }

    //���� ��ġ�� ������� �׸��� Ž��
    public List<PlayerController> FindPlayersInGrid(Transform transforms)
    {
        Vector3 currentPosition = transforms.position;   
        int x = Mathf.FloorToInt(currentPosition.x /10)- indexingMinX;
        int y = Mathf.FloorToInt(currentPosition.y /10)- indexingMinY;

        List<PlayerController> returnPlayers = new List<PlayerController>();

        for (int i = 0; i < 9; i++) // ���� ��ġ���� 8���� Ž�� 
        {
            int posX = x + findX[i];
            int posY = y + findY[i];

            if (ValidCheck(posX, posY)) //�ε��� ��ȿ�� üũ
            {
                List<PlayerController> controllers = players[posX, posY];
                for (int j = 0; j < controllers.Count; j++)
                {
                    PlayerController controller = controllers[j];
                    Vector3 dir = controller.Transforms.position - currentPosition;
                    float distance = dir.magnitude;
                    dir = Vector3.Normalize(dir);
                    if (Physics.Raycast(currentPosition,dir,distance,1 << 3)) returnPlayers.Add(controller); // �÷��̾�� �� ���̿� ��ֹ��� ���� ���� ������
                }
            }
        }
        return returnPlayers;
    }

    bool ValidCheck(int x, int y)
    {
     
        return x >= 0 && x < sizeX && y >=0 && y < sizeY;

    }
    string ConvertBinaryToString(byte[] binary)
    {
        return BitConverter.ToString(binary).Replace("-", "").ToLower();
    }
    public void AddObjects(GameObject ob)
    {
        while (true)
        {
            byte[] random = Guid.NewGuid().ToByteArray();
            string key = ConvertBinaryToString(random);

            if(!objects.ContainsKey(key))
            {
                IIdentifiable identifiable = ob.GetComponent<IIdentifiable>();
                identifiable.ID = key;
                objects[key] = ob;
                break;
            }
        }
    }

    public void RemoveObjects(IIdentifiable ob)
    {
      //  ob.ID = null;
        objects.Remove(ob.ID);
    }

    public List<GameObject> ReturnObjects()
    {
        List<GameObject> returnObjects = new List<GameObject>();
        foreach (var obj in objects)
        {
            returnObjects.Add(obj.Value);
        }

        return returnObjects;
    }


    public void AddLives(GameObject livingObject)
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
    }


    public void RemoveLives(string ID)
    {
        lives.Remove(ID);
    }


    public List<GameObject> ReturnLives()
    {
        List<GameObject> returnObjects = new List<GameObject>();
        foreach (var livingObject in lives)
        {
            returnObjects.Add(livingObject.Value);
        }

        return returnObjects;
    }
}
