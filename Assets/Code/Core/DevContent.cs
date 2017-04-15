using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevContent : MonoBehaviour {

    public List<DevMonsterInfo> Monsters = new List<DevMonsterInfo>();

    public List<DevItemInfo> Items = new List<DevItemInfo>();

    //void Start()
    //{
    //    Content.Instance.Monsters.InsertRange(0, Monsters);
    //}
}

[System.Serializable]
public class DevMonsterInfo
{
    [SerializeField]
    public string MonsterKey;

    [SerializeField]
    public string MonsterName;

    [SerializeField]
    public int MonsterHP;

    [SerializeField]
    public int MonsterLevel;

    [SerializeField]
    public int MinDMG;

    [SerializeField]
    public int MaxDMG;

    [SerializeField]
    public int RewardEXP;

    [SerializeField]
    public int MinGoldDrop;

    [SerializeField]
    public int MaxGoldDrop;

    [SerializeField]
    public List<string> PossibleLoot = new List<string>();



}

[System.Serializable]
public class DevItemInfo
{
    public string Name;
    public string Icon;
    public string Type;
    public int DropChance;
    public int GoldValue;

    public List<DevItemSprite> ItemSprites = new List<DevItemSprite>();
}

[SerializeField]
public class DevItemSprite
{
    public string PartKey;
    public string Sprite;
}

