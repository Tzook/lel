using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellBoxUI : MonoBehaviour {

    [SerializeField]
    Text m_BindText;

    [SerializeField]
    Image m_Icon;

    DevSpell CurrentSpell;

    public void Set(DevSpell spell)
    {
        CurrentSpell = spell;
        Refresh();
    }

    public void Refresh()
    {
        m_Icon.sprite = CurrentSpell.Icon;

        string BindingName = InputMap.Map["Spell" + (Content.Instance.GetSpellIndex(CurrentSpell) + 1)].ToString();

        if(BindingName.Substring(0,5) == "Alpha")
        {
            BindingName = BindingName.Substring(BindingName.Length - 1, 1);
        }

        m_BindText.text = "(" + BindingName + ")";

        RefreshMana();
    }

    public void RefreshMana()
    {
        if(CurrentSpell.Mana > LocalUserInfo.Me.ClientCharacter.CurrentMana)
        {
            m_Icon.color = new Color(Color.white.r, Color.white.g, Color.white.b, 0.3f);
        }
        else
        {
            m_Icon.color = Color.white;
        }
    }

    public void Show()
    {
        m_BindText.enabled = true;
        m_Icon.enabled = true;
    }
    
    public void Hide()
    {
        m_BindText.enabled = false;
        m_Icon.enabled = false;
    }

}
