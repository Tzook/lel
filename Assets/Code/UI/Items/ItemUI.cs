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
    public ItemSlotsContainerUI ParentContainer;

    public void SetData(ItemInfo info = null, ItemSlotsContainerUI Container = null)
    {
        if (info != null)
        {
            CurrentItem = info;
        }

        ParentContainer = Container;
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

        InGameMainMenuUI.Instance.BeginDrag(this);
    }

    public void Hover()
    {
        InGameMainMenuUI.Instance.HoveredSlot = this;

        outline.enabled = true;
    }

    public void UnHover()
    {
        if (InGameMainMenuUI.Instance.HoveredSlot == this)
        {
            InGameMainMenuUI.Instance.HoveredSlot = null;
        }

        outline.enabled = false;
    }

    internal void UnDrag()
    {
        IconImage.color = new Color(IconImage.color.r, IconImage.color.g, IconImage.color.b, 1f);
    }

}
