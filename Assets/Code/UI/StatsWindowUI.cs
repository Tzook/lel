using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class StatsWindowUI : MonoBehaviour
{
    [SerializeField]
    Text EXPText;

    [SerializeField]
    Text MaxHealthText;

    [SerializeField]
    Text MaxManaText;

    [SerializeField]
    Text STRText;

    [SerializeField]
    Text MAGText;

    [SerializeField]
    Text DEXText;



    public void Show(ActorInfo info)
    {
        this.gameObject.SetActive(true);
        Refresh(info);
    }

    public void Refresh(ActorInfo info)
    {
        this.EXPText.text = info.EXP.ToString("N0");
        this.MaxHealthText.text = info.MaxHealth.ToString("N0");
        this.MaxManaText.text = info.MaxMana.ToString("N0");
        this.STRText.text = info.TotalSTR.ToString("N0");
        this.MAGText.text = info.TotalMAG.ToString("N0");
        this.DEXText.text = info.TotalDEX.ToString("N0");

        //this.EXPText.text = info.EXP.ToString("N0");
        //this.MaxHealthText.text = (info.STR * 5).ToString("N0") + " <color=green> + " + (info.BonusSTR * 5).ToString("N0") + "</color>";
        //this.MaxManaText.text = (info.MAG * 5).ToString("N0") + " <color=green> + " + (info.BonusMAG * 5).ToString("N0") + "</color>";
        //this.STRText.text = info.STR.ToString("N0") + " <color=green> + " + info.BonusSTR.ToString("N0") + "</color>";
        //this.MAGText.text = info.MAG.ToString("N0") + " <color=green> + " + info.BonusMAG.ToString("N0") + "</color>";
        //this.DEXText.text = info.DEX.ToString("N0") + " <color=green> + " + info.BonusDEX.ToString("N0") + "</color>";
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
}
