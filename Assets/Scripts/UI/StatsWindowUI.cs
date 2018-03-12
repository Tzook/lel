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

    public void Show(ActorInfo info)
    {
        this.gameObject.SetActive(true);
        Refresh(info);
    }

    public void Show()
    {
        Show(LocalUserInfo.Me.ClientCharacter);
    }

    public void Refresh(ActorInfo info)
    {
        this.EXPText.text = FormatStat(info.EXP, info.NextLevelXP);
        this.MaxHealthText.text = FormatStat(info.CurrentHealth, info.MaxHealth);
        this.MaxManaText.text = FormatStat(info.CurrentMana, info.MaxMana);
    }

    protected string FormatStat(int Current, int Max)
    {
        return Current.ToString("N0") + " / " + Max.ToString("N0");
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
}
