using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusUI : MonoBehaviour
{
    public Image hpProgress;
    public Image energyProgress;
    public Image expPrograss;
    public TMP_Text hp_Text;
    public TMP_Text energy_Text;
    public TMP_Text level_Text;

    int hp = 0;
    int maxHp = 0;
    int energy = 0;
    int maxEnergy = 0;
    float exp = 0;
    float maxEXP = 0;
    int level;

    float targetHP = 0;
    float targetEnergy = 0;
    float targetEXP = 0;
    Coroutine expCoroutine;
    Coroutine energyCoroutine;
    Coroutine hpCoroutine;

    private void Awake()
    {
        GameInstance.Instance.playerStatusUI = this;
    }

    public void UpdateUI(PlayerStruct playerStruct)
    {
        CharacterAbilityManager characterAbility = GameInstance.Instance.characterAbilityManager;
        this.hp = playerStruct.hp;
        this.maxHp = playerStruct.maxHP + characterAbility.maxHp;
        hp_Text.text = this.hp.ToString();
        
        hpProgress.fillAmount = (float)this.hp / (this.maxHp + characterAbility.maxHp);

        this.energy = playerStruct.energy;
        this.maxEnergy = playerStruct.maxEnergy;
        energy_Text.text = this.energy.ToString();
        energyProgress.fillAmount = (float)this.energy / this.maxEnergy;

        this.exp = playerStruct.exp;
        this.maxEXP = playerStruct.requireEXP;

        expPrograss.fillAmount = (float)this.exp / this.maxEXP;

        level_Text.text = playerStruct.level.ToString();

    }


    public void ChangeHP(int hp)
    {
        CharacterAbilityManager characterAbility = GameInstance.Instance.characterAbilityManager;
        this.hp = hp;
        hp_Text.text = hp.ToString();
        targetHP = (float)this.hp / (this.maxHp + characterAbility.maxHp);
        //hpProgress.fillAmount = (float)this.hp / this.maxHp;
        if (hpCoroutine != null) StopCoroutine(hpCoroutine);
        hpCoroutine = StartCoroutine(SmoothHP());
    }

    public void ChangeEnergy(int energy)
    {
        this.energy = energy;
        energy_Text.text = energy.ToString();
        targetEnergy = (float)this.energy / this.maxEnergy;
        if(energyCoroutine != null) StopCoroutine(energyCoroutine);
        energyCoroutine = StartCoroutine(SmoothEnergy());
    }



    IEnumerator SmoothEnergy()
    {
        float timer = 0;
        float currentPercent = energyProgress.fillAmount;
        while (timer < 0.2f)
        {
            timer += Time.deltaTime;
            energyProgress.fillAmount = Mathf.Lerp(currentPercent, targetEnergy, timer * 5);
            yield return null;
        }
        energyProgress.fillAmount = targetEnergy;
    }

    IEnumerator SmoothHP()
    {
        float timer = 0;
        float currentPercent = hpProgress.fillAmount;
        while(timer < 0.2f)
        {
            timer+=Time.deltaTime;
            hpProgress.fillAmount = Mathf.Lerp(currentPercent, targetHP, timer * 5);
            yield return null;
        }

        hpProgress.fillAmount = targetHP;
    }

    public void GetEXP(int exp)
    {
        if (this.exp < exp)
        {
            this.exp = exp;
            targetEXP = (float)this.exp / this.maxEXP;

            if (expCoroutine != null) StopCoroutine(expCoroutine); 
            expCoroutine = StartCoroutine(SmoothEXP());
        } 
    }

    public void LevelUp(int level, int exp, int requireExp)
    {
        this.level = level;
        level_Text.text = this.level.ToString();
        this.exp = exp;
        this.maxEXP = requireExp;
        expPrograss.fillAmount = (float)this.exp / this.maxEXP;
    }

    IEnumerator SmoothEXP()
    {
        float timer = 0;
        float currentPercent = expPrograss.fillAmount;
        while (timer < 0.2f)
        {
            timer += Time.deltaTime;
            expPrograss.fillAmount = Mathf.Lerp(currentPercent, targetEXP, timer * 5);
            yield return null;
        }

        expPrograss.fillAmount = targetEXP;
    }

    
}
