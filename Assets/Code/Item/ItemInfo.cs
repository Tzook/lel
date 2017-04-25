using UnityEngine;
using System.Collections;
using SimpleJSON;
using System.Collections.Generic;
using System;

public class ItemInfo {

    public string Name;
    public string IconKey;
    public string UseSound;
    public string Description;
    public string Type;
    public int Stack = 1;

    public Dictionary<string, string> Sprites = new Dictionary<string, string>();

    public ItemInfo(JSONNode itemNode, int stack = 1)
    {
        if(stack == 0)
        {
            stack = 1;
        }

        this.Stack = stack;

        this.Name = itemNode["name"].Value;
        this.IconKey = itemNode["icon"].Value;
        this.Type = itemNode["type"].Value;

        Sprites.Clear();
        for (int i=0;i<itemNode["sprites"].Count;i++)
        {
            Sprites.Add(((JSONClass)itemNode["sprites"]).GetKey(i), itemNode["sprites"][i].Value);
        }
    }

    public ItemInfo(DevItemInfo storedItem, int stack = 1)
    {
        if (stack == 0)
        {
            stack = 1;
        }

        this.Stack = stack;

        this.Name = storedItem.Name;
        this.IconKey = storedItem.Icon;
        this.UseSound = storedItem.UseSound;
        this.Type = storedItem.Type;

        Sprites.Clear();
        for (int i = 0; i < storedItem.ItemSprites.Count; i++)
        {
            Sprites.Add(storedItem.ItemSprites[i].PartKey, storedItem.ItemSprites[i].Sprite);
        }
    }
}
