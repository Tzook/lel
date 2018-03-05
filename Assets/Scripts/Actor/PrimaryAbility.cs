using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Ability
{
    public string Key;
    public int Points = 0;
    public int Exp = 0;
    public int LVL = 1;

    public List<PAPerk> Perks = new List<PAPerk>();
    public List<string> PerkPool = new List<string>();

    public Ability(string key) 
    {
        Key = key;
    }

    public int NextLevelXP
    {
        get
        {
            // see http://tibia.wikia.com/wiki/Experience_Formula
            return Mathf.FloorToInt(((50f / 3f) * ((LVL + 1) * (LVL + 1) * (LVL + 1) - 6f * (LVL + 1) * (LVL + 1) + 17f * (LVL + 1) - 12f))
                                    - ((50f / 3f) * (LVL * LVL * LVL - 6f * LVL * LVL + 17f * LVL - 12f)));
        }
        private set { }
    }

    public void GainPerk(string perkKey)
    {
        PAPerk tempPerk = GetPerk(perkKey);
        DevPAPerk perkRef = Content.Instance.GetPerk(perkKey);

        if (tempPerk != null)
        {
            tempPerk.Points++;
        }
        else
        {
            tempPerk = new PAPerk();
            
            tempPerk.Key = perkRef.Key;
            tempPerk.Points = 1;

            Perks.Add(tempPerk);
        }
    }

    public PAPerk GetPerk(string perkKey)
    {
        Debug.Log(perkKey);
        for(int i=0; i < Perks.Count; i++)
        {
            Debug.Log("-" + Perks[i].Key);
            if (Perks[i].Key == perkKey)
            {
                return Perks[i];
            }
        }

        return null;
    }

}
