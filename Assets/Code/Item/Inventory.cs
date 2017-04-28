using UnityEngine;
using System.Collections;
using SimpleJSON;
using System;

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
                ContentArray[i] = new ItemInfo(Content.Instance.GetItem(inventoryNode[i]["key"].Value), inventoryNode[i]["stack"].AsInt);
            }
        }
    }

    public void SwapSlots(int fromIndex, int toIndex)
    {
        ItemInfo draggedInfo = ContentArray[fromIndex];
        ContentArray[fromIndex] = ContentArray[toIndex];
        ContentArray[toIndex] = draggedInfo;
    }

    public void RemoveItem(int index)
    {
        ContentArray[index] = null;
    }

    internal void AddItem(ItemInfo info)
    {
        ContentArray[GetFreeSlot()] = info;
    }

    internal void AddItemAt(int index, ItemInfo info)
    {
        ContentArray[index] = info;
    }

    internal ItemInfo GetItemAt(int index)
    {
        return ContentArray[index];
    }

    internal void ChangeItemStack(int index, int stack)
    {
        ContentArray[index].Stack = stack;
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
}
