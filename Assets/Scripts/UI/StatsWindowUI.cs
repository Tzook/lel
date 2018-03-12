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
        this.EXPText.text = info.EXP.ToString("N0");
        this.MaxHealthText.text = info.MaxHealth.ToString("N0");
        this.MaxManaText.text = info.MaxMana.ToString("N0");
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
}
