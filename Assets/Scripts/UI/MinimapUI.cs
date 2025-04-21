using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapUI : MonoBehaviour
{
    public RectTransform playerRectTransform;
    public RectTransform enemyRectTransform;
    public RectTransform objectRectTransform;
 //   Camera camera;
    public Image player;
    public Image item;
    public Image enemy;

    public Sprite playerSprite;
    public Sprite itemSprite;
    public Sprite enemySprite;
    public Sprite itemBoxSprite;

    [SerializeField]
    MinimapIcon icon;
    
    List<Image> images = new List<Image>();
    Queue<MinimapIcon> deactivatedMinimapIcons = new Queue<MinimapIcon>(50);
    Queue<MinimapIcon> activatedMinimapIcons = new Queue<MinimapIcon>(50);

    List<GameObject> players = new List<GameObject>();
    List<GameObject> gameObjects = new List<GameObject>();
    List<GameObject> lives = new List<GameObject>();
    List<GameObject> itemBoxes = new List<GameObject>();

    Dictionary<int, MinimapIcon> icons = new Dictionary<int, MinimapIcon>();
    Dictionary<int, MinimapIcon> destroyIcons = new Dictionary<int, MinimapIcon>();
    // Start is called before the first frame update
    private void Awake()
    {
        GameInstance.Instance.minimapUI = this;
    }
    void Start()
    {
        NewIcons();
     //   camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        destroyIcons = new Dictionary<int, MinimapIcon>(icons);
       // RemoveIcons();
        if (GameInstance.Instance.GetPlayers.Count > 0)
        {
            int playerId = GameInstance.Instance.GetPlayers[0].GetComponent<IIdentifiable>().ID;
            Transform transforms = GameInstance.Instance.GetPlayers[0].Transforms;

            MinimapIcon playerIcon;
            if (icons.ContainsKey(playerId))
            {
                playerIcon = icons[playerId];
                destroyIcons.Remove(playerId);
            }
            else
            {
                playerIcon = CreateIcon();
                icons[playerId] = playerIcon;
            }
            playerIcon.image.sprite = playerSprite;
            playerIcon.GetRectTransform.SetParent(playerRectTransform);
            playerIcon.GetRectTransform.localPosition = Vector3.zero; //플레이어 위치 중앙 기준
            playerIcon.GetRectTransform.localRotation = Quaternion.Euler(0, 0,  -(transforms.eulerAngles.y - 45) + 180); //플레이어 방향
            playerIcon.GetRectTransform.sizeDelta = new Vector2(20, 20);

            if (GameInstance.Instance.worldGrids != null)
            {
              //  List<GameObject> gameObjects = GameInstance.Instance.worldGrids.ReturnObjects();

                for (int i = 0; i < gameObjects.Count; i++)
                {
                    Vector3 pos = gameObjects[i].transform.position - transforms.position;
                    float rotationAngle = 45f;
                    Quaternion rotation = Quaternion.Euler(0, 0, rotationAngle);
                    Vector3 rotatedPosition = rotation * new Vector3(pos.x * 4, pos.z * 4, 0);
                    if(rotatedPosition.x < 100 && rotatedPosition.y < 100 && rotatedPosition.x > -100 && rotatedPosition.y > -100)
                    {
                        int objectId = gameObjects[i].GetComponent<IIdentifiable>().ID;
                        MinimapIcon itemIcon;
                        if (icons.ContainsKey(objectId))
                        {
                            itemIcon = icons[objectId];
                            destroyIcons.Remove(objectId);
                        }
                        else
                        {
                            itemIcon = CreateIcon();
                            icons[objectId] = itemIcon;
                        }
                        itemIcon.image.sprite = itemSprite;
                        itemIcon.GetRectTransform.SetParent(objectRectTransform);
                        itemIcon.GetRectTransform.localPosition = rotatedPosition;
                        itemIcon.GetRectTransform.localRotation = Quaternion.Euler(Vector3.zero);
                        itemIcon.GetRectTransform.sizeDelta = new Vector2(20, 20);
                    }
                }

               // List<GameObject> lives = GameInstance.Instance.worldGrids.ReturnLives();

                for(int i = 0;i < lives.Count;i++)
                {
                    Transform livesTransform = lives[i].transform;
                    Vector3 pos = livesTransform.position - transforms.position;

                    float rotationAngle = 45f;
                    Quaternion rotation = Quaternion.Euler(0, 0, rotationAngle);
                    Vector3 rotatedPosition = rotation * new Vector3(pos.x * 4, pos.z * 4, 0);
                    if (rotatedPosition.x < 100 && rotatedPosition.y < 100 && rotatedPosition.x > -100 && rotatedPosition.y > -100)
                    {
                        int enemyId = lives[i].GetComponent<IIdentifiable>().ID;
                        MinimapIcon enemyIcon;
                        if (icons.ContainsKey(enemyId))
                        {
                            enemyIcon = icons[enemyId];
                            destroyIcons.Remove(enemyId);
                        }
                        else
                        {
                            enemyIcon = CreateIcon();
                            icons[enemyId] = enemyIcon;
                        }
                        enemyIcon.image.sprite = enemySprite;
                        enemyIcon.GetRectTransform.SetParent(enemyRectTransform);
                        enemyIcon.GetRectTransform.localPosition = rotatedPosition;
                        enemyIcon.GetRectTransform.localRotation = Quaternion.Euler(0, 0, -(livesTransform.eulerAngles.y - 45) + 180);
                        enemyIcon.GetRectTransform.sizeDelta = new Vector2(20, 20);
                    }
                }

                for (int i = 0; i < itemBoxes.Count; i++)
                {
                    Transform itemBoxTransform = itemBoxes[i].transform;
                    Vector3 pos = itemBoxTransform.position - transforms.position;

                    float rotationAngle = 45f;
                    Quaternion rotation = Quaternion.Euler(0, 0, rotationAngle);
                    Vector3 rotatedPosition = rotation * new Vector3(pos.x * 4, pos.z * 4, 0);
                    if (rotatedPosition.x < 100 && rotatedPosition.y < 100 && rotatedPosition.x > -100 && rotatedPosition.y > -100)
                    {
                        int boxId = itemBoxes[i].GetComponent<IIdentifiable>().ID;
                        MinimapIcon boxIcon;
                        if (icons.ContainsKey(boxId))
                        {
                            boxIcon = icons[boxId];
                            destroyIcons.Remove(boxId);
                        }
                        else
                        {
                            boxIcon = CreateIcon();
                            icons[boxId] = boxIcon;
                        }
                        boxIcon.image.sprite = itemBoxSprite;
                        boxIcon.GetRectTransform.SetParent(objectRectTransform);
                        boxIcon.GetRectTransform.localPosition = rotatedPosition;
                        boxIcon.GetRectTransform.sizeDelta = new Vector2(30, 30);
                        boxIcon.GetRectTransform.localRotation = Quaternion.Euler(Vector3.zero);
                    }
                }
            }
        }
        RemoveIcons();
    }


    public void ChangeList(MinimapIconType minimapIconType)
    {
        switch (minimapIconType)
        {
            case MinimapIconType.None:
                break;
            case MinimapIconType.Player:
                players = GameInstance.Instance.worldGrids.ReturnObjects(minimapIconType);
                break;
            case MinimapIconType.Enemy:
                lives = GameInstance.Instance.worldGrids.ReturnObjects(minimapIconType);
                break;
            case MinimapIconType.Object:
                gameObjects = GameInstance.Instance.worldGrids.ReturnObjects(minimapIconType);
                break;
            case MinimapIconType.ItemBox:
                itemBoxes = GameInstance.Instance.worldGrids.ReturnObjects(minimapIconType);
                break;
        }
    }

    //아이콘 풀링
    void NewIcons()
    {
        for (int i = 0; i < 50; i++)
        {
            MinimapIcon minimapIcon = Instantiate(icon);
            minimapIcon.gameObject.SetActive(false);
            deactivatedMinimapIcons.Enqueue(minimapIcon);
        }
    }

    MinimapIcon CreateIcon()
    {
        if (deactivatedMinimapIcons.Count > 0)
        {
            MinimapIcon newIcon = deactivatedMinimapIcons.Dequeue();
            newIcon.gameObject.SetActive(true);
           // activatedMinimapIcons.Enqueue(newIcon);
            return newIcon;
        }
        else
        {
            MinimapIcon newIcon = Instantiate(icon);
            newIcon.gameObject.SetActive(true);
            //activatedMinimapIcons.Enqueue(newIcon);
            return newIcon;
        }
    }

    void RemoveIcons()
    {
        foreach(KeyValuePair<int, MinimapIcon> keyValuePair in destroyIcons)
        {
            MinimapIcon minimapIcon = keyValuePair.Value;
            if (icons.ContainsKey(keyValuePair.Key)) icons.Remove(keyValuePair.Key);

            if (deactivatedMinimapIcons.Count > 50)
            {
                Destroy(minimapIcon);
            }
            else
            {
                minimapIcon.gameObject.SetActive(false);
                deactivatedMinimapIcons.Enqueue(minimapIcon);
            }
        }
       /* while (activatedMinimapIcons.Count > 0)
        {
            MinimapIcon minimapIcon = activatedMinimapIcons.Dequeue();

            if(deactivatedMinimapIcons.Count > 50)
            {
                Destroy(minimapIcon);
            }
            else
            {
                minimapIcon.gameObject.SetActive(false);
                deactivatedMinimapIcons.Enqueue(minimapIcon);
            }
        }*/
    }

    private void OnDestroy()
    {
        while (deactivatedMinimapIcons.Count > 0) deactivatedMinimapIcons.Dequeue();
        while (activatedMinimapIcons.Count > 0) activatedMinimapIcons.Dequeue();
        for(int i = 0; i< gameObjects.Count; i++) gameObjects[i] = null;
        for(int i = 0; i< lives.Count; i++) lives[i] = null;
        for (int i = 0; i < itemBoxes.Count; i++) itemBoxes[i] = null;

        itemBoxes.Clear();
        gameObjects.Clear();
        lives.Clear();
    }
}
