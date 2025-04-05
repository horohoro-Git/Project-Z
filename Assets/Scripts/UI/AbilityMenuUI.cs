using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AbilityMenuUI : MonoBehaviour, IUIComponent
{

    public TMP_Text pointText;
    public TMP_Text healthUpgradeBefore;
    public TMP_Text healthUpgradeAfter;
    public TMP_Text energyUpgradeBefore;
    public TMP_Text energyUpgradeAfter;
    public TMP_Text weightUpgradeBefore;
    public TMP_Text weightUpgradeAfter;

    public TMP_Text healthChangeBefore;
    public TMP_Text healthChangeAfter;
    public TMP_Text energyChangeBefore;
    public TMP_Text energyChangeAfter;
    public TMP_Text weightChangeBefore;
    public TMP_Text weightChangeAfter;

    public Button healthPlusBtn;
    public Button healthMinusBtn;
    public Button energyPlusBtn;
    public Button energyMinusBtn;
    public Button weightPlusBtn;
    public Button weightMinusBtn;
    public Button applyBtn;

    PlayerStruct currentStruct;
    PlayerStruct playerStruct;

    Dictionary<int, LevelStruct> levelData = new Dictionary<int, LevelStruct>();
    Player player;
    public void Setup()
    {
        GameInstance.Instance.abilityMenuUI = this;   
    }

    private void OnEnable()
    {
        healthPlusBtn.onClick.AddListener(() => { UpgradeHealth(true); });
        healthMinusBtn.onClick.AddListener(() => { UpgradeHealth(false); });
        energyPlusBtn.onClick.AddListener(() => { UpgradeEnergy(true); });
        energyMinusBtn.onClick.AddListener(() => { UpgradeEnergy(false); });
        weightPlusBtn.onClick.AddListener(() => { UpgradeWeight(true); });
        weightMinusBtn.onClick.AddListener(() => { UpgradeWeight(false); });
        applyBtn.onClick.AddListener(() => { ApplyData(); });
    }

    private void OnDisable()
    {
        healthPlusBtn.onClick.RemoveAllListeners();
        healthMinusBtn.onClick.RemoveAllListeners();
        energyPlusBtn.onClick.RemoveAllListeners();
        energyMinusBtn.onClick.RemoveAllListeners();
        weightPlusBtn.onClick.RemoveAllListeners();
        weightMinusBtn.onClick.RemoveAllListeners();
        applyBtn.onClick.RemoveAllListeners();
        Revert();
    }

    public void GetPoint(int point)
    {
        pointText.text = point.ToString();
    }

    public void ShowChanges(Player player)
    {
        this.player = player;
        playerStruct = player.playerStruct;
        if(levelData.Count ==0) levelData = GameInstance.Instance.assetLoader.levelData;

        currentStruct = playerStruct;


        ChangeBefore();
        ChangeAfter();

    }

    void ChangeBefore()
    {
        //레벨 변화 정보
        healthUpgradeBefore.text = playerStruct.hpLevel.ToString();
        energyUpgradeBefore.text = playerStruct.energyLevel.ToString();
        weightUpgradeBefore.text = playerStruct.weightLevel.ToString();

        //플레이어 스텟 변화 정보
        healthChangeBefore.text = levelData[playerStruct.hpLevel].heal.ToString() + "%";
        energyChangeBefore.text = levelData[playerStruct.energyLevel].energy_regain.ToString() + "%";
        weightChangeBefore.text = levelData[playerStruct.weightLevel].weight.ToString();
    }

    void ChangeAfter()
    {
        //레벨 변화 정보
        if (currentStruct.hpLevel <= 20) healthUpgradeAfter.text = currentStruct.hpLevel.ToString();
     //   else healthUpgradeAfter.text = currentStruct.hpLevel.ToString();

        if (currentStruct.energyLevel <= 20) energyUpgradeAfter.text = currentStruct.energyLevel.ToString();
        //   else energyUpgradeAfter.text = currentStruct.energyLevel.ToString();

        if (currentStruct.weightLevel <= 20) weightUpgradeAfter.text = currentStruct.weightLevel.ToString();
        //  else weightUpgradeAfter.text = currentStruct.weightLevel.ToString();

        //플레이어 스텟 변화 정보
        if (currentStruct.hpLevel <= 20) healthChangeAfter.text = levelData[currentStruct.hpLevel].heal.ToString() + "%";
        //  else healthChangeAfter.text = levelData[currentStruct.hpLevel - 1].heal.ToString();

        if (currentStruct.energyLevel <= 20) energyChangeAfter.text = levelData[currentStruct.energyLevel].energy_regain.ToString() + "%";
        //     else energyChangeAfter.text = levelData[currentStruct.energyLevel - 1].energy_regain.ToString();

        if (currentStruct.weightLevel <= 20) weightChangeAfter.text = levelData[currentStruct.weightLevel].weight.ToString();
        //  else weightChangeAfter.text = levelData[currentStruct.weightLevel - 1].weight.ToString();
    }

    void UpgradeHealth(bool upgrade)
    {
        if (upgrade)
        {
            if (currentStruct.hpLevel < 20 && currentStruct.skillPoint > 0)
            {
                currentStruct.hpLevel++;
                currentStruct.skillPoint--;

            }
        }
        else
        {
            if (playerStruct.hpLevel < currentStruct.hpLevel)
            {
                currentStruct.hpLevel--;
                currentStruct.skillPoint++;

            }
        }
        ChangeAfter();
        GetPoint(currentStruct.skillPoint);
    }

    void UpgradeEnergy(bool upgrade)
    {
        if (upgrade)
        {
            if (currentStruct.energyLevel < 20 && currentStruct.skillPoint > 0)
            {
                currentStruct.energyLevel++;
                currentStruct.skillPoint--;

            }
        }
        else
        {
            if (playerStruct.energyLevel < currentStruct.energyLevel)
            {
                currentStruct.energyLevel--;
                currentStruct.skillPoint++;

            }
        }
        ChangeAfter();
        GetPoint(currentStruct.skillPoint);
    }
    void UpgradeWeight(bool upgrade)
    {
        if (upgrade)
        {
            if (currentStruct.weightLevel < 20 && currentStruct.skillPoint > 0)
            {
                currentStruct.weightLevel++;
                currentStruct.skillPoint--;

            }
        }
        else
        {
            if (playerStruct.weightLevel < currentStruct.weightLevel)
            {
                currentStruct.weightLevel--;
                currentStruct.skillPoint++;

            }
        }
        ChangeAfter();
        GetPoint(currentStruct.skillPoint);
    }

    void Revert()
    {
        if (GameInstance.Instance.GetPlayers.Count > 0)
        {
            ShowChanges(GameInstance.Instance.GetPlayers[0].GetPlayer);
        }
    }


    void ApplyData()
    {
        playerStruct = currentStruct;

        player.playerStruct = playerStruct;
        ChangeBefore();
        GameInstance.Instance.characterAbilitySystem.Apply();
        GameInstance.Instance.craftingLearnSystem.Apply();
        SaveLoadSystem.SavePlayerData(player);
    }
}
