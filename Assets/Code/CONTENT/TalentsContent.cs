using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalentsContent : MonoBehaviour
{
    public static TalentsContent Instance;

    public List<TalentGroup> TalentGroups = new List<TalentGroup>();

    private void Awake()
    {
        Instance = this;
    }

    public TalentContent GetTalent(string key)
    {
        for(int a=0;a<TalentGroups.Count;a++)
        {
            for(int b=0;b<TalentGroups[a].Talents.Count;b++)
            {
                if(TalentGroups[a].Talents[b].Key == key)
                {
                    return TalentGroups[a].Talents[b];
                }
            }
        }

        return null;
    }
}

[System.Serializable]
public class TalentGroup
{
    public string jobKey;
    public List<TalentContent> Talents = new List<TalentContent>();
}

[System.Serializable]
public class TalentContent
{
    public string Key;

    [TextArea(16, 9)]
    public string Description;

    public int PointCap;
    public List<TalentRequirement> Requirements = new List<TalentRequirement>();
    public List<TalentRank> Ranks = new List<TalentRank>();
}

[System.Serializable]
public class TalentRequirement
{
    public string TalentKey;
    public int PointAmount;
}

[System.Serializable]
public class TalentRank
{
    public int Rank;
    public int MPCost;
    public List<TalentEffect> ActiveEffects = new List<TalentEffect>();
    public List<TalentEffect> PassiveEffects = new List<TalentEffect>();
}

[System.Serializable]
public class TalentEffect
{
    public string Key;
    public string Value;
}