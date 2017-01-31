using UnityEngine;
using System.Collections;
using SimpleJSON;
using System.Collections.Generic;

[System.Serializable]
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
            return Mathf.FloorToInt(
                (50f * Mathf.Pow(this.LVL, 3) 
                - 150f * Mathf.Pow(this.LVL, 2) 
                + 400f * this.LVL) 
                / 3);
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


    public Inventory Inventory;

    public Equipment Equipment;

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

        if(node["stats"]!=null)
        {
            this.LVL = node["stats"]["lvl"].AsInt;

            this.EXP = node["stats"]["exp"].AsInt;

            this.STR = node["stats"]["str"].AsInt;
            this.MAG = node["stats"]["mag"].AsInt;
            this.DEX = node["stats"]["dex"].AsInt;

            this.MaxHealth = node["stats"]["hp"]["total"].AsInt;
            this.CurrentHealth = node["stats"]["hp"]["now"].AsInt;

            this.MaxMana = node["stats"]["mp"]["total"].AsInt;
            this.CurrentMana = node["stats"]["mp"]["now"].AsInt;
        }

    }
}

public enum Gender
{
    Male, Female
}
