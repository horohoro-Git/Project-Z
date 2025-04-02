using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AchievementData : MonoBehaviour
{
    RectTransform rectTransform;
    public RectTransform GetRectTransform {  get { if (rectTransform == null) rectTransform = GetComponent<RectTransform>(); return rectTransform; } }

    public AchievementStruct achievementStruct;
    public TMP_Text description;
    public Image icon;
    public Button button;
    UnityAction clearAtion;

    public List<GameObject> stars = new List<GameObject>();
    public void Setup()
    {
        button.enabled = false;
        string mainText = achievementStruct.quest_name + " " + achievementStruct.progress + "/" + achievementStruct.target;
        description.text = mainText;

        for(int i = 0; i<achievementStruct.level; i++)
        {
            stars[i].SetActive(true);
        }
    }

    private void OnEnable()
    {
        if (clearAtion == null) clearAtion = GetReward;

        button.onClick.AddListener(clearAtion);

    }

    private void OnDisable()
    {
        button.onClick.RemoveListener(clearAtion);
    }

    void GetReward()
    {
        if (GameInstance.Instance.GetPlayers.Count > 0)
        {
            Player player = GameInstance.Instance.GetPlayers[0].GetPlayer;

            GameInstance.Instance.inventorySystem.LoadInvetory(0, 5, ItemData.GetItem(1), new WeaponStruct(), new ConsumptionStruct(), new ArmorStruct());
            GameInstance.Instance.boxInventorySystem.LoadInvetory(0, 5, ItemData.GetItem(1), new WeaponStruct(), new ConsumptionStruct(), new ArmorStruct());

            achievementStruct.reward_complete = true;
            button.enabled = false;

            GameInstance.Instance.achievementUI.ClearData(this);
        }
    }
    public void UpdateData(AchievementStruct achievement)
    {
        achievementStruct = achievement;
        string mainText = achievementStruct.quest_name + " " + achievementStruct.progress + "/" + achievementStruct.target;
        description.text = mainText;
        if (achievement.complete) button.enabled = true;
    }
}
