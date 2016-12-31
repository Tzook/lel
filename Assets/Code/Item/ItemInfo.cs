using UnityEngine;
using System.Collections;
using SimpleJSON;
using System.Collections.Generic;
using System;

public class ItemInfo {

    public string Name;
    public string IconKey;
    public string Description;
    public string Type;

    public Dictionary<string, string> Sprites = new Dictionary<string, string>();

    public ItemInfo(JSONNode itemNode)
    {
        this.Name = itemNode["name"].Value;
        this.IconKey = itemNode["icon"].Value;
        this.Type = itemNode["type"].Value;

        Sprites.Clear();
        for (int i=0;i<itemNode["sprites"].Count;i++)
        {
            Sprites.Add(((JSONClass)itemNode["sprites"]).GetKey(i), itemNode["sprites"][i].Value);
        }
    }
}
