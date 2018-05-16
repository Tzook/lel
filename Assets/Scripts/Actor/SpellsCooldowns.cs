using System.Collections.Generic;
using UnityEngine;

public class SpellsCooldowns
{
    private Dictionary<string, float> Cooldowns = new Dictionary<string, float>();

    public bool AttemptUseSpell(DevSpell spell)
    {
        bool canUseSpell = CanUseSpell(spell);
        if (!canUseSpell)
        {
            WarnAboutCooldown();
        }
        return canUseSpell;
    }

    public bool CanUseSpell(DevSpell spell)
    {
        return GetCurrentSpellCooldown(spell.Key) <= 0;
    }

    public float GetCurrentSpellCooldown(string spellKey)
    {
        return Cooldowns.ContainsKey(spellKey) ? Cooldowns[spellKey] : 0;
    }
    
    public void TickSpellsCooldowns()
    {
        if (Cooldowns.Count > 0)
        {
            float time = Time.deltaTime;
            List<KeyValuePair<string, float>> CooldownsUpdates = new List<KeyValuePair<string, float>>();
            
            // you cannot modify the dic while iterating it, so we basically clone it
            foreach (var spellCooldownKeyValue in Cooldowns)
            {
                CooldownsUpdates.Add(spellCooldownKeyValue);
            }

            foreach (var spellCooldownKeyValue in CooldownsUpdates)
            {
                SetSpellInCooldown(spellCooldownKeyValue.Key, spellCooldownKeyValue.Value - time);
            }
        }
    }

    public void UseSpell(DevSpell spell)
    {
        float cooldown = spell.Cooldown * LocalUserInfo.Me.ClientCharacter.ClientPerks.CooldownModifier;
        SetSpellInCooldown(spell.Key, cooldown);
    }

    public void SetSpellInCooldown(string key, float cooldown)
    {
        if (cooldown <= 0) 
        {
            Cooldowns.Remove(key);
        }
        else
        {
            Cooldowns[key] = cooldown;
        }
    }

    public void WarnAboutCooldown()
    {
        InGameMainMenuUI.Instance.ShockMessageTop.CallMessage("Spell is in cooldown.", Color.red, false);
    }
}