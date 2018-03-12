using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

[System.Serializable]
public class ItemStats
{
    public int RequiresLVL = 0;

    public int JumpBonus = 0;
    public int SpeedBonus = 0;

    public ItemStats Clone()
    {
        ItemStats stats = new ItemStats();

        stats.RequiresLVL = this.RequiresLVL;

        stats.JumpBonus = this.JumpBonus;
        stats.SpeedBonus = this.SpeedBonus;

        return stats;
    }

    public void SetInfo(JSONNode node)
    {
        this.RequiresLVL = node["reqLVL"].AsInt;

        this.JumpBonus = 0;
        this.SpeedBonus = 0;
    }
}