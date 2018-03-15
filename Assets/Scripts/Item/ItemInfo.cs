using UnityEngine;
using System.Collections;
using SimpleJSON;
using System.Collections.Generic;
using System;

public class ItemInfo {

    public string Key;
    public string Name;
    public string IconKey;
    public string UseSound;
    public string Description;
    public string Type;
    public string SubType;
    public int StackCap;
    public int Stack = 1;
    public bool SawItem = true;

    public ItemStats Stats;

    public List<DevPerkMap> Perks;

    public Dictionary<string, string> Sprites = new Dictionary<string, string>();

    public ItemInfo(DevItemInfo storedItem, int stack = 1)
    {
        try
        {
            if (stack == 0)
            {
                stack = 1;
            }

            this.Stack = stack;

            this.Key = storedItem.Key;
            this.Name = storedItem.Name;
            this.Description = storedItem.Description;
            this.IconKey = storedItem.Icon;
            this.UseSound = storedItem.UseSound;
            this.Type = storedItem.Type;
            this.SubType = storedItem.SubType;
            this.StackCap = storedItem.StackCap;

            this.Stats = storedItem.Stats.Clone();
            this.Perks = storedItem.Perks;

            Sprites.Clear();
            for (int i = 0; i < storedItem.ItemSprites.Count; i++)
            {
                Sprites.Add(storedItem.ItemSprites[i].PartKey, storedItem.ItemSprites[i].Sprite);
            }
        }
        catch
        {
            Debug.LogError("Problematic Item!");

            this.Stack = 1;

            this.Key = "unknown";
            this.Name = "Unknown / Removed Item";
            this.Description = "This item is not existing / causes problems.";
            this.IconKey = "xIcon";
            this.StackCap = 1;

            this.Stats = new ItemStats();
            this.Perks = new List<DevPerkMap>();

            Sprites.Clear();
        }
    }
}
