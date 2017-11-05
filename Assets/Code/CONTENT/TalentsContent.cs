using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalentsContent : MonoBehaviour
{

    public List<TalentGroup> TalentGroups = new List<TalentGroup>();
    public List<Buff> Buffs = new List<Buff>();
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
    public List<ActiveTalentEffect> ActiveEffects = new List<ActiveTalentEffect>();
    public List<PassiveTalentEffect> PassiveEffects = new List<PassiveTalentEffect>();
    public List<string> Buffs = new List<string>();
    public List<string> Dots = new List<string>();
}

[System.Serializable]
public class Buff
{
    public string Key;
    public float Duration;

    public float BaseDmgPercent;
    public float Interval;
    public float Speed;
    public bool Stunned;
    public bool manaLess;
    public float StrBonusPrecent;
    public float MagicBonusPrecent;
    public float DexBonusPrecent;
    public float HPBonusPrecent;
    public float MPBonusPrecent;
}

[System.Serializable]
public class ActiveTalentEffect
{
    public int attacksCount;
    public int mobsAffectedCount;
    public float baseDmgPercent;
    public float threatBonusPercent;
    public float splashDmgPercent;
    public float splashMobsAffectedCount;
}

[System.Serializable]
public class PassiveTalentEffect
{
    public float baseDmgBonusPercent;
    public float chargeStunsChance;
    public float chargeDmgPercentBonus;
    public int hpPerStrBonus;
    public float reflectMeeleBaseDmgPercent;
    public float blockDmgReductionPrecent;
}
