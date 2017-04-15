using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Content : MonoBehaviour {

    public static Content Instance;

    void Awake()
    {
        Instance = this;
    }

    [SerializeField]
    List<ContentPiece> InfoBank = new List<ContentPiece>();

    [SerializeField]
    List<PrimaryAbility> PrimaryAbilities = new List<PrimaryAbility>();

    public List<DevMonsterInfo> Monsters = new List<DevMonsterInfo>();

    public ContentPiece GetInfo(string Key)
    {
        for(int i=0;i<InfoBank.Count;i++)
        {
            if(InfoBank[i].Title == Key)
            {
                return InfoBank[i];
            }
        }

        return null;
    }

    public PrimaryAbility GetPrimaryAbility(string key)
    {
        for(int i=0;i<PrimaryAbilities.Count;i++)
        {
            if(PrimaryAbilities[i].Name == key)
            {
                return PrimaryAbilities[i];
            }
        }

        return null;
    }
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