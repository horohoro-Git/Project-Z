using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatusUI : MonoBehaviour
{
    float exp = 0;



    private void Awake()
    {
        GameInstance.Instance.playerStatusUI = this;
    }



    public void GetEXP(float exp)
    {
        if (this.exp < exp) GettingEXP();
        this.exp = exp;
    }

    void GettingEXP()
    {

    }
}
