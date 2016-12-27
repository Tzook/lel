using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class ItemUI : MonoBehaviour {

    [SerializeField]
    Image IconImage;

    [SerializeField]
    Outline outline;

    public ItemInfo CurrentItem;

    [System.NonSerialized]
    public InventoryUI ParentInventory;

    public void SetData(ItemInfo info = null, InventoryUI inventory = null)
    {
        if (info != null)
        {
            CurrentItem = info;
        }

        ParentInventory = inventory;
        RefreshUI();
    }

    private void RefreshUI()
    {
        if(CurrentItem == null)
        {
            IconImage.sprite = ResourcesLoader.Instance.GetSprite("transparentPixel");
            return;
        }

        if (!string.IsNullOrEmpty(CurrentItem.IconKey))
        {
            IconImage.sprite = ResourcesLoader.Instance.GetSprite(CurrentItem.IconKey);
        }
        else
        {
            IconImage.sprite = ResourcesLoader.Instance.GetSprite("xIcon");
        }
    }

    public void Clear()
    {
        CurrentItem = null;
        RefreshUI();
    }
    
    public void BeginDragItem()
    {
        if(CurrentItem==null)
        {
            return;
        }

        IconImage.color = new Color(IconImage.color.r, IconImage.color.g, IconImage.color.b, 0.5f);

        ParentInventory.BeginDrag(this);
    }

    public void Hover()
    {
        ParentInventory.Hover(this);
        outline.enabled = true;
    }

    public void UnHover()
    {
        ParentInventory.UnHover(this);
        outline.enabled = false;
    }

    internal void UnDrag()
    {
        IconImage.color = new Color(IconImage.color.r, IconImage.color.g, IconImage.color.b, 0.5f);
    }

}
