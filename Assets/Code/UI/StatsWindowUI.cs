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



    internal void Show(ActorInfo info)
    {
        this.gameObject.SetActive(true);

        this.EXPText.text = info.EXP.ToString("N0");
        this.MaxHealthText.text = info.MaxHealth.ToString("N0");
        this.MaxManaText.text = info.MaxMana.ToString("N0");
        this.STRText.text = info.STR.ToString("N0");
        this.MAGText.text = info.MAG.ToString("N0");
        this.DEXText.text = info.DEX.ToString("N0");
    }

    internal void Hide()
    {
        this.gameObject.SetActive(false);
    }
}
