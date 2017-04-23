using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Content : MonoBehaviour {

    public static Content Instance;

    void Awake()
    {
        Instance = this;
    }

    public List<DevMonsterInfo> Monsters = new List<DevMonsterInfo>();

    public List<DevItemInfo> Items = new List<DevItemInfo>();

    public List<ContentPiece> InfoBank = new List<ContentPiece>();

    public List<PrimaryAbility> PrimaryAbilities = new List<PrimaryAbility>();

    public ContentPiece GetInfo(string Key)
    {
        for (int i = 0; i < InfoBank.Count; i++)
        {
            if (InfoBank[i].Title == Key)
            {
                return InfoBank[i];
            }
        }

        return null;
    }

    public PrimaryAbility GetPrimaryAbility(string key)
    {
        for (int i = 0; i < PrimaryAbilities.Count; i++)
        {
            if (PrimaryAbilities[i].Name == key)
            {
                return PrimaryAbilities[i];
            }
        }

        return null;
    }

    public DevItemInfo GetItem(string itemKey)
    {
        for(int i=0;i<Items.Count;i++)
        {
            if(Items[i].Key == itemKey)
            {
                return Items[i];
            }
        }

        return null;
    }

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
    public string Key;
    public string Name;

    //Will send "Icon" only if "IconPlaceable" is null.
    public Sprite IconPlaceable;
    public string Icon;

    public string UseSound;

    public string Type;
    public float DropChance;
    public int GoldValue;

    public List<DevItemSprite> ItemSprites = new List<DevItemSprite>();
}

[System.Serializable]
public class DevItemSprite
{
    public string PartKey;

    //Will send "Sprite" only if "SpritePlaceable" is null.
    public Sprite SpritePlaceable = null;

    public string Sprite;
}

[System.Serializable]
public class ContentPiece
{
    public string Title;

    [TextArea(16, 9)]
    public string Description;

    public Sprite Icon;
}

[System.Serializable]
public class PrimaryAbility
{
    public string Name;

    public Sprite Icon;
}

