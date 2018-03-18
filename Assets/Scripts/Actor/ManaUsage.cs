using UnityEngine;

public class ManaUsage
{
    private static ManaUsage _instance; 
    public static ManaUsage Instance
    { get { return _instance == null ? _instance = new ManaUsage() : _instance; } }

    public bool UseMana(int mana)
    {
        bool canUse = CanUseMana(mana);
        if (canUse)
        {
            LocalUserInfo.Me.ClientCharacter.CurrentMana -= GetManaCost(mana);

            InGameMainMenuUI.Instance.RefreshMP();
        }
        else
        {
            WarnAboutMana();
        }
        return canUse;
    }

    public bool CanUseMana(int mana)
    {
        return GetManaCost(mana) <= LocalUserInfo.Me.ClientCharacter.CurrentMana;
    }

    protected int GetManaCost(int mana)
    {
        return (int)(mana * LocalUserInfo.Me.ClientCharacter.ManaCost);
    }

    public void WarnAboutMana()
    {
        InGameMainMenuUI.Instance.ShockMessageTop.CallMessage("Not enough mana.", Color.red, true);
    }
}