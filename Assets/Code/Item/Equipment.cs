using UnityEngine;
using System.Collections;
using SimpleJSON;

[System.Serializable]
public class Equipment {

    public ItemInfo Head;
    public ItemInfo Chest;
    public ItemInfo Gloves;
    public ItemInfo Legs;
    public ItemInfo Shoes;
    public ItemInfo Weapon;

    ActorInfo Parent;

    public Equipment(JSONNode node, ActorInfo parent)
    {
        this.Parent = parent;

        string partKey = "";

        for (int i = 0; i < node.Count; i++)
        {
            if (!string.IsNullOrEmpty(node[i]["key"].Value))
            {
                partKey = ((JSONClass)node).GetKey(i);
                SetItem(partKey, new ItemInfo(Content.Instance.GetItem(node[partKey]["key"].Value)));
            }
        }
    }

    public void SetItem(string slot, ItemInfo item)
    {
        switch (slot)
        {
            case "head":
                {
                    Head = item;
                    break;
                }
            case "chest":
                {
                    Chest = item;
                    break;
                }
            case "gloves":
                {
                    Gloves = item;
                    break;
                }
            case "legs":
                {
                    Legs = item;
                    break;
                }
            case "shoes":
                {
                    Shoes = item;
                    break;
                }
            case "weapon":
                {
                    Weapon = item;
                    break;
                }
        }

        Parent.RefreshBonuses(this);
    }
    public ItemInfo GetItem(string slot)
    {
        switch (slot)
        {
            case "head":
                {
                    return Head;
                }
            case "chest":
                {
                    return Chest;
                }
            case "gloves":
                {
                    return Gloves;
                }
            case "legs":
                {
                    return Legs;
                }
            case "shoes":
                {
                    return Shoes;
                }
            case "weapon":
                {
                    return Weapon;
                }
        }

        return null;
    }

    public bool CanEquip(string type, string slot)
    {
        return (type == slot);
    }
}
