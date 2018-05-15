using System.Collections.Generic;
using UnityEngine;

public class SpellsCooldowns
{
    public Dictionary<string, float> Cooldowns = new Dictionary<string, float>();

    public bool UseSpell(DevSpell spell)
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

    protected float GetCurrentSpellCooldown(string spellKey)
    {
        return Cooldowns.ContainsKey(spellKey) ? Cooldowns[spellKey] : 0;
    }
    
    public void TickSpellsCooldowns()
    {
        if (Cooldowns.Count > 0)
        {
            float time = Time.deltaTime;
            List<string> CooldownsToRemove = new List<string>();
            
            foreach (var spellCooldownKeyValue in Cooldowns)
            {
                SetSpellInCooldown(spellCooldownKeyValue.Key, spellCooldownKeyValue.Value - time);
                if (Cooldowns[spellCooldownKeyValue.Key] <= 0) 
                {
                    CooldownsToRemove.Add(spellCooldownKeyValue.Key);
                }
            }

            foreach (var spellKey in CooldownsToRemove)
            {
                Cooldowns.Remove(spellKey);
            }
        }
    }

    public void SetSpellInCooldown(string key, float cooldown)
    {
        Cooldowns[key] = cooldown;
    }

    public void WarnAboutCooldown()
    {
        InGameMainMenuUI.Instance.ShockMessageTop.CallMessage("Spell is in cooldown.", Color.red, false);
    }
}