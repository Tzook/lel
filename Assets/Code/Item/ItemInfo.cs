﻿using UnityEngine;
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
    public int Stack = 1;

    public ItemStats Stats;

    public Dictionary<string, string> Sprites = new Dictionary<string, string>();

    public ItemInfo(DevItemInfo storedItem, int stack = 1)
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

        this.Stats = storedItem.Stats.Clone();

        Sprites.Clear();
        for (int i = 0; i < storedItem.ItemSprites.Count; i++)
        {
            Sprites.Add(storedItem.ItemSprites[i].PartKey, storedItem.ItemSprites[i].Sprite);
        }
    }
}
