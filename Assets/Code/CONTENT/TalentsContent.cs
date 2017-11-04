using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalentsContent : MonoBehaviour {

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
    int Rank;
    int MPCost;
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

    float BaseDmgPercent;
    float Interval;
    float Speed;
    bool Stunned;
    bool manaLess;
    float StrBonusPrecent;
    float MagicBonusPrecent;
    float DexBonusPrecent;
    float HPBonusPrecent;
    float MPBonusPrecent;
}

[System.Serializable]
public class ActiveTalentEffect
{
    int attacksCount;
    int mobsAffectedCount;
    float baseDmgPercent;
    float threatBonusPercent;
    float splashDmgPercent;
    float splashMobsAffectedCount;
}

[System.Serializable]
public class PassiveTalentEffect
{
    float baseDmgBonusPercent;
	float chargeStunsChance;
    float chargeDmgPercentBonus;
    int hpPerStrBonus;
    float reflectMeeleBaseDmgPercent;
    float blockDmgReductionPrecent;
}
