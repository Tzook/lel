using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

[System.Serializable]
public class ItemStats
{
    public int RequiresSTR = 0;
    public int RequiresMAG = 0;
    public int RequiresDEX = 0;
    public int RequiresLVL = 0;

    public int BonusSTR = 0;
    public int BonusMAG = 0;
    public int BonusDEX = 0;
    public int BonusHP = 0;
    public int BonusMP = 0;

    public int JumpBonus = 0;
    public int SpeedBonus = 0;

    public ItemStats Clone()
    {
        ItemStats stats = new ItemStats();

        stats.RequiresSTR = this.RequiresSTR;
        stats.RequiresMAG = this.RequiresMAG;
        stats.RequiresDEX = this.RequiresDEX;
        stats.RequiresLVL = this.RequiresLVL;

        stats.BonusSTR = this.BonusSTR;
        stats.BonusMAG = this.BonusMAG;
        stats.BonusDEX = this.BonusDEX;
        stats.BonusHP = this.BonusHP;
        stats.BonusMP = this.BonusMP;

        stats.JumpBonus = this.JumpBonus;
        stats.SpeedBonus = this.SpeedBonus;

        return stats;
    }

    public void SetInfo(JSONNode node)
    {
        this.RequiresSTR = node["reqSTR"].AsInt;
        this.RequiresMAG = node["reqMAG"].AsInt;
        this.RequiresDEX = node["reqDEX"].AsInt;
        this.RequiresLVL = node["reqLVL"].AsInt;

        this.BonusSTR = node["bonusSTR"].AsInt;
        this.BonusMAG = node["bonusMAG"].AsInt;
        this.BonusDEX = node["bonusDEX"].AsInt;
        this.BonusHP = node["bonusHP"].AsInt;
        this.BonusMP = node["bonusMP"].AsInt;
        this.JumpBonus = 0;
        this.SpeedBonus = 0;
    }
}