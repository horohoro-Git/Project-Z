using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapUI : MonoBehaviour
{
    public RectTransform rectTransform;
    Camera camera;
    public Image player;
    public Image item;
    public Image enemy;
    List<Image> images = new List<Image>();
    // Start is called before the first frame update
    private void Awake()
    {
        GameInstance.Instance.minimapUI = this;
    }
    void Start()
    {
        camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        RemoveIcons();
        if (GameInstance.Instance.GetPlayers.Count > 0)
        {
            Transform transforms = GameInstance.Instance.GetPlayers[0].Transforms;
            Image playerIcon = Instantiate(player);
            playerIcon.rectTransform.SetParent(rectTransform);
            playerIcon.rectTransform.localPosition = Vector3.zero; //플레이어 위치 중앙 기준
            playerIcon.rectTransform.localRotation = Quaternion.Euler(0, 0,  -(transforms.eulerAngles.y - 45) + 180); //플레이어 방향
            images.Add(playerIcon);
            
            if (GameInstance.Instance.worldGrids != null)
            {
                List<GameObject> gameObjects = GameInstance.Instance.worldGrids.ReturnObjects();

                for (int i = 0; i < gameObjects.Count; i++)
                {
                    Vector3 pos = gameObjects[i].transform.position - transforms.position;


                    float rotationAngle = 45f;
                    Quaternion rotation = Quaternion.Euler(0, 0, rotationAngle);
                    Vector3 rotatedPosition = rotation * new Vector3(pos.x * 4, pos.z * 4, 0);

                    if(rotatedPosition.x < 100 && rotatedPosition.y < 100 && rotatedPosition.x > -100 && rotatedPosition.y > -100)
                    {
                        Image itemIcon = Instantiate(item);
                        itemIcon.rectTransform.SetParent(rectTransform);
                        itemIcon.rectTransform.localPosition = rotatedPosition;
                        images.Add(itemIcon);
                    }
                }



            }
        }
    }

    void RemoveIcons()
    {
        int c = images.Count -1;
        for(int i = c; i >= 0; i--)
        {
            Image image = images[i];
            images.RemoveAt(i);
            Destroy(image.gameObject);
        }
    }
}
