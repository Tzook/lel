using 
    UnityEngine;
using System.Collections;
using SimpleJSON;
using System.Collections.Generic;

public class ActorInfo
{
    public string ID;
    public string Name;
    public Gender Gender;
    public ActorInstance Instance;
    public string CurrentRoom;
    public Vector3 LastPosition;
    public int SkinColor;
    public string Hair = "hair_0";
    public string Eyes = "eyes_0a";
    public string Nose = "nose_0";
    public string Mouth= "mouth_0";

    public int LVL;

    public int EXP;
    public int NextLevelXP
    {
        get
        {
            // see http://tibia.wikia.com/wiki/Experience_Formula
            return Mathf.FloorToInt(((50f / 3f) * ((LVL+1) * (LVL + 1) * (LVL + 1) - 6f * (LVL + 1) * (LVL + 1) + 17f * (LVL + 1) - 12f)) 
                                    - ((50f / 3f) * (LVL * LVL * LVL - 6f * LVL * LVL + 17f * LVL - 12f)));
        }
        private set { }
    }



    public int STR;
    public int MAG;
    public int DEX;

    public int MaxHealth;
    public int CurrentHealth;

    public int MaxMana;
    public int CurrentMana;

    public List<string> PrimaryAbilities = new List<string>();
    public string CurrentPrimaryAbility;

    public Inventory Inventory;

    public Equipment Equipment;

    public int Gold;

    public ActorInfo()
    {
        Equipment = new Equipment(new JSONClass());
    }

    public ActorInfo(JSONNode node)
    {
        this.ID = node["_id"].Value;
        this.Name = node["name"].Value;
        this.CurrentRoom = node["room"].Value;
        this.LastPosition = new Vector3(node["position"]["x"].AsFloat, node["position"]["y"].AsFloat, node["position"]["z"].AsFloat);
        this.Gold = node["gold"].AsInt;
        

        if (node["looks"]["g"].AsBool)
        {
            this.Gender = Gender.Male;
        }
        else
        {
            this.Gender = Gender.Female;
        }

        Hair = node["looks"]["hair"].Value;
        Eyes = node["looks"]["eyes"].Value;
        Nose = node["looks"]["nose"].Value;
        Mouth = node["looks"]["mouth"].Value;
        SkinColor = node["looks"]["skin"].AsInt;

        Inventory = new Inventory(node["items"]);

        Equipment = new Equipment(node["equips"]);

        if (node["stats"] != null)
        {
            SetStats(node["stats"]);
        }
    }

    public void ChangeGold(int amount)
    {
        Gold += amount;
        InGameMainMenuUI.Instance.RefreshInventory();

        if (amount > 0)
        {
            InGameMainMenuUI.Instance.MinilogMessage("Gained " + amount + " gold");
        }
        else
        {
            InGameMainMenuUI.Instance.MinilogMessage("Lost " + (-amount) + " gold");
        }
    }

    public void SetStats(JSONNode node)
    {
        this.LVL = node["lvl"].AsInt;

        this.EXP = node["exp"].AsInt;

        this.STR = node["str"].AsInt;
        this.MAG = node["mag"].AsInt;
        this.DEX = node["dex"].AsInt;

        this.MaxHealth = node["hp"]["total"].AsInt;
        this.CurrentHealth = node["hp"]["now"].AsInt;

        this.MaxMana = node["mp"]["total"].AsInt;
        this.CurrentMana = node["mp"]["now"].AsInt;

        this.PrimaryAbilities.Clear();
        for(int i=0;i<node["abilities"].Count;i++)
        {
            this.PrimaryAbilities.Add(node["abilities"][i].Value);
        }

        this.CurrentPrimaryAbility = node["primaryAbility"].Value;
    }
}

public enum Gender
{
    Male, Female
}
