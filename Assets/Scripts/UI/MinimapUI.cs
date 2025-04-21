using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

public class MinimapUI : MonoBehaviour
{
    public RectTransform playerRectTransform;
    public RectTransform npcRectTransform;
    public RectTransform enemyRectTransform;
    public RectTransform objectRectTransform;
 //   Camera camera;
    public Image player;
    public Image item;
    public Image enemy;

    public Sprite playerSprite;
    public Sprite itemSprite;
    public Sprite enemySprite;
    public Sprite friendlySprite;
    public Sprite neturalSprite;
    public Sprite itemBoxSprite;

    [SerializeField]
    MinimapIcon icon;
    
    List<Image> images = new List<Image>();
    Queue<MinimapIcon> deactivatedMinimapIcons = new Queue<MinimapIcon>(50);
    Queue<MinimapIcon> activatedMinimapIcons = new Queue<MinimapIcon>(50);

    List<GameObject> players = new List<GameObject>();
    List<GameObject> gameObjects = new List<GameObject>();
    List<GameObject> lives = new List<GameObject>();
    List<GameObject> npcs = new List<GameObject>();
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
            Transform transforms = GameInstance.Instance.GetPlayers[0].transform;
            if (GameInstance.Instance.worldGrids != null)
            {
                MinimapUpdate(transforms, MinimapIconType.Player);
                MinimapUpdate(transforms, MinimapIconType.Object);
                MinimapUpdate(transforms, MinimapIconType.Enemy);
                MinimapUpdate(transforms, MinimapIconType.NPC);
                MinimapUpdate(transforms, MinimapIconType.ItemBox);
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
            case MinimapIconType.NPC:
                npcs = GameInstance.Instance.worldGrids.ReturnObjects(minimapIconType);
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

    //icons : 현재 프레임에 추가 될 아이콘들
    //destoryIcons : 이전 프레임의 icons로 현재 프레임의 icons와 비교하고 제거
    void MinimapUpdate(Transform transforms, MinimapIconType minimapIconType)
    {
        RectTransform rect = objectRectTransform; // 깊이 역할
        Vector2 sizeDelta = new Vector2(20, 20);  // 아이콘 크기 
        List<GameObject> list = new List<GameObject>();
        Sprite sprite = null;
        switch (minimapIconType)
        {
            case MinimapIconType.None:
                break;
            case MinimapIconType.Player:
                list = players;
                sprite = playerSprite;
                rect = playerRectTransform;
                break;
            case MinimapIconType.Enemy:
                list = lives;
                sprite = enemySprite;
                rect = enemyRectTransform;
                break;
            case MinimapIconType.NPC:
                list = npcs;
                rect = npcRectTransform;
                break;
            case MinimapIconType.Object:
                list = gameObjects;
                sprite = itemSprite;
                rect = objectRectTransform;
                break;
            case MinimapIconType.ItemBox:
                list = itemBoxes;
                sprite = itemBoxSprite;
                sizeDelta = new Vector2(30, 30);
                rect = objectRectTransform;
                break;
        }

        for (int i = 0; i < list.Count; i++)
        {
            Transform listTransform = list[i].transform;
            Vector3 pos = listTransform.position - transforms.position;

            float rotationAngle = 45f;
            Quaternion rotation = Quaternion.Euler(0, 0, rotationAngle);
            Vector3 rotatedPosition = rotation * new Vector3(pos.x * 4, pos.z * 4, 0);
            if (rotatedPosition.x < 100 && rotatedPosition.y < 100 && rotatedPosition.x > -100 && rotatedPosition.y > -100)
            {
                int iconId = list[i].GetComponent<IIdentifiable>().ID;  // 오브젝트의 ID를 기준으로 판별
                MinimapIcon minimapIcon;
                if (icons.ContainsKey(iconId))
                {
                    minimapIcon = icons[iconId];
                    destroyIcons.Remove(iconId);
                }
                else
                {
                    minimapIcon = CreateIcon();
                    icons[iconId] = minimapIcon;
                }
                if(minimapIconType == MinimapIconType.NPC)
                {
                    NPCDispositionType dispositionType = list[i].GetComponent<NPCController>().eventStruct.npc_disposition;
                    switch (dispositionType)
                    {
                        case NPCDispositionType.None:
                            break;
                        case NPCDispositionType.Netural:
                            sprite = neturalSprite;
                            break;
                        case NPCDispositionType.Friendly:
                            sprite = friendlySprite;
                            break;
                        case NPCDispositionType.Hostile:
                            sprite = enemySprite;
                            break;
                    }

                    minimapIcon.image.sprite = sprite;
                }
                else minimapIcon.image.sprite = sprite;
                minimapIcon.GetRectTransform.SetParent(rect);
                minimapIcon.GetRectTransform.localPosition = rotatedPosition;
                minimapIcon.GetRectTransform.sizeDelta = sizeDelta;
                if (minimapIconType == MinimapIconType.ItemBox || minimapIconType == MinimapIconType.Object)
                {
                    minimapIcon.GetRectTransform.localRotation = Quaternion.Euler(Vector3.zero);
                }
                else
                {
                    minimapIcon.GetRectTransform.localRotation = Quaternion.Euler(0, 0, -(listTransform.eulerAngles.y - 45) + 180);

                }
            }
        }
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
