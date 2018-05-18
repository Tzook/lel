using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellBoxUI : MonoBehaviour {

    [SerializeField]
    Text m_BindText;

    [SerializeField]
    Image m_Icon;

    [SerializeField]
    Image m_MarkerIcon;

    [SerializeField]
    Image m_CooldownImage;

    [SerializeField]
    Text m_txtCooldown;

    public DevSpell CurrentSpell;

    public void Set(DevSpell spell)
    {
        CurrentSpell = spell;
        Refresh();
    }

    public void Refresh()
    {
        m_Icon.sprite = CurrentSpell.Icon;
        m_MarkerIcon.sprite = m_Icon.sprite;

        string BindingName = InputMap.Map["Spell" + (Content.Instance.GetSpellIndex(CurrentSpell) + 1)].ToString();

        if(BindingName.Length > 5 && BindingName.Substring(0,5) == "Alpha")
        {
            BindingName = BindingName.Substring(BindingName.Length - 1, 1);
        }

        m_BindText.text = "(" + BindingName + ")";

        UpdateCooldownUI();
        RefreshMana();
    }

    public void RefreshMana()
    {
        if(ManaUsage.Instance.CanUseMana(CurrentSpell.Mana))
        {
            m_Icon.color = Color.white;
        }
        else
        {
            m_Icon.color = new Color(Color.white.r, Color.white.g, Color.white.b, 0.3f);
        }
    }

    public void Show()
    {
        m_BindText.enabled = true;
        m_Icon.enabled = true;
        m_txtCooldown.enabled = true;
        m_CooldownImage.enabled = true;
    }
    
    public void Hide()
    {
        m_BindText.enabled = false;
        m_Icon.enabled = false;
        m_txtCooldown.enabled = false;
        m_CooldownImage.enabled = false;
    }

    public void Update()
    {
        UpdateCooldownUI();
    }

    private void UpdateCooldownUI()
    {
        float cd = LocalUserInfo.Me.ClientCharacter != null ? LocalUserInfo.Me.ClientCharacter.SpellsCooldowns.GetCurrentSpellCooldown(CurrentSpell.Key) : 0;

        if (cd > 0f)
        {
            m_CooldownImage.fillAmount = cd / CurrentSpell.Cooldown;
            m_txtCooldown.text = Mathf.CeilToInt(cd).ToString();
        }
        else
        {
            m_CooldownImage.fillAmount = 0f;
            m_txtCooldown.text = "";
        }
    }

    public void Activated()
    {
        StopAllCoroutines();

        StartCoroutine(ActivatedRoutine());
    }

    IEnumerator ActivatedRoutine()
    {
        float t = 0f;
        while(t < 1f)
        {
            t += 6f * Time.deltaTime;
            m_MarkerIcon.color = Color.Lerp(m_MarkerIcon.color, new Color(Color.white.r, Color.white.g, Color.white.b, 0.6f), t);
            yield return 0;
        }

        t = 0f;
        while (t < 1f)
        {
            t += 6f * Time.deltaTime;
            m_MarkerIcon.color = Color.Lerp(m_MarkerIcon.color, new Color(Color.white.r, Color.white.g, Color.white.b, 0f), t);
            yield return 0;
        }
    }

    IEnumerator CooldownRoutine(float cd)
    {
        float t = cd;

        while(t>0f)
        {
            t -= Time.deltaTime;

            

            yield return 0;
        }

        m_txtCooldown.text = "";
        m_CooldownImage.fillAmount = 0f;
    }



}
