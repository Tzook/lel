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

        this.EXPText.text = info.EXP.ToString();
        this.MaxHealthText.text = info.MaxHealth.ToString();
        this.MaxManaText.text = info.MaxMana.ToString();
        this.STRText.text = info.STR.ToString();
        this.MAGText.text = info.MAG.ToString();
        this.DEXText.text = info.DEX.ToString();
    }

    internal void Hide()
    {
        this.gameObject.SetActive(false);
    }
}
