﻿using UnityEngine;
using System.Collections;
using SimpleJSON;
using System;
using System.Collections.Generic;

[System.Serializable]
public class Inventory
{
    public ItemInfo[] ContentArray;
    public bool isFull
    {
        get { return (GetFreeSlot() == -1); }
        set { }
    }

    public Inventory(JSONNode inventoryNode = null)
    {
        if(inventoryNode != null)
        {
            SetInventory(inventoryNode);
        }
    }

    public void SetInventory(JSONNode inventoryNode)
    {
        ContentArray = new ItemInfo[20];
        for (int i = 0; i < inventoryNode.Count; i++)
        {
            if (!string.IsNullOrEmpty(inventoryNode[i]["key"].Value))
            {
                ContentArray[i] = new ItemInfo(Content.Instance.GetItem(inventoryNode[i]["key"].Value), PerkListGetter.Instance.Get(inventoryNode[i]), inventoryNode[i]["stack"].AsInt);
            }
        }
    }

    public Dictionary<string, int> GetInventoryCounts()
    {
        Dictionary<string, int> dick = new Dictionary<string, int>();
        for (int i = 0; i < ContentArray.Length; i++)
        {
            ItemInfo item = ContentArray[i];
            if (item != null) {
                if (!dick.ContainsKey(item.Key)) 
                {
                    dick[item.Key] = 0;
                }
                dick[item.Key] += item.Stack;
            }
        }
        return dick;   
    }
    
    public string canPickItem(ItemInstance item)
    {
        string cannotPickErrorMessage = "Inventory is full!";
        if (!canPickItemByOwner(item)) 
        {
            // item has an owner and it is not you
            cannotPickErrorMessage = "Item belongs to someone else!";
        }
        else if (item.Info.Key == "gold") 
        {
            // gold can always be picked up
            cannotPickErrorMessage = "";
        } 
        else
        {
            int stackNeeded = item.Info.Stack;
            for (int i = 0; i < ContentArray.Length; i++)
            {
                if (ContentArray[i] == null)
                {
                    // empty slot - pick it up
                    cannotPickErrorMessage = "";
                    break;
                }
                else if (item.Info.Key == ContentArray[i].Key // same key
                    && item.Info.StackCap > 1 // stackable
                    && ContentArray[i].Stack < ContentArray[i].StackCap) // with empty stack slots
                {
                    // sum the stack added so far. if it reached the item's stack amount, it's pickable
                    stackNeeded -= (ContentArray[i].StackCap - ContentArray[i].Stack);
                    if (stackNeeded <= 0) {
                        cannotPickErrorMessage = "";
                        break;
                    }
                }
            }
        }
        return cannotPickErrorMessage;
    }

    protected bool canPickItemByOwner(ItemInstance item)
    {
        return String.IsNullOrEmpty(item.Owner) || item.Owner == LocalUserInfo.Me.ClientCharacter.Name || (LocalUserInfo.Me.CurrentParty != null && LocalUserInfo.Me.CurrentParty.Members.Contains(item.Owner));
    }

    public void SwapSlots(int fromIndex, int toIndex)
    {
        ItemInfo draggedInfo = ContentArray[fromIndex];
        ContentArray[fromIndex] = ContentArray[toIndex];
        ContentArray[toIndex] = draggedInfo;
    }

    public void RemoveItem(int index)
    {
        LocalUserInfo.Me.ClientCharacter.UpdateProgress(ContentArray[index].Key, -ContentArray[index].Stack);
        ContentArray[index] = null;
    }

    internal void AddItemAt(int index, ItemInfo info)
    {
        ContentArray[index] = info;
        LocalUserInfo.Me.ClientCharacter.UpdateProgress(info.Key, info.Stack);
    }

    internal ItemInfo GetItemAt(int index)
    {
        return ContentArray[index];
    }

    internal void ChangeItemStack(int index, int stack)
    {
        int oldStack = ContentArray[index].Stack;
        ContentArray[index].Stack = stack;
        LocalUserInfo.Me.ClientCharacter.UpdateProgress(ContentArray[index].Key, stack - oldStack);
    }

    private int GetFreeSlot()
    {
        for(int i=0;i<ContentArray.Length;i++)
        {
            if (ContentArray[i] == null || string.IsNullOrEmpty(ContentArray[i].Name))
            {
                return i;
            }
        }

        return -1;
    }

    public ItemInfo GetItem(string key)
    {
        for(int i=0;i < ContentArray.Length;i++)
        {
            if(ContentArray[i] != null && ContentArray[i].Key == key)
            {
                return ContentArray[i];
            }
        }

        return null;
    }
}
