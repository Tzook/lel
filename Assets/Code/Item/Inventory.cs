using UnityEngine;
using System.Collections;
using SimpleJSON;
using System;

[System.Serializable]
public class Inventory
{
    public ItemInfo[] Content;
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
        Content = new ItemInfo[20];
        for (int i = 0; i < inventoryNode.Count; i++)
        {
            if (!string.IsNullOrEmpty(inventoryNode[i]["name"].Value))
            {
                Content[i] = new ItemInfo(inventoryNode[i]);
            }
        }
    }

    public void SwapSlots(int fromIndex, int toIndex)
    {
        ItemInfo draggedInfo = Content[fromIndex];
        Content[fromIndex] = Content[toIndex];
        Content[toIndex] = draggedInfo;
    }

    public void RemoveItem(int index)
    {
        Content[index] = null;
    }

    internal void AddItem(ItemInfo info)
    {
        Content[GetFreeSlot()] = info;
    }

    private int GetFreeSlot()
    {
        for(int i=0;i<Content.Length;i++)
        {
            if (Content[i] == null || string.IsNullOrEmpty(Content[i].Name))
            {
                return i;
            }
        }

        return -1;
    }
}
