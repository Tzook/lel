using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class ItemUI : MonoBehaviour {

    [SerializeField]
    Image IconImage;

    [SerializeField]
    Text CountText;

    [SerializeField]
    Outline outline;

    [SerializeField]
    public string slotKey;

    public ItemInfo CurrentItem;

    [System.NonSerialized]
    public ItemSlotsContainerUI ParentContainer;

    public void SetData(ItemInfo info = null, ItemSlotsContainerUI Container = null)
    {
        CurrentItem = info;

        ParentContainer = Container;
        RefreshUI();
    }

    private void RefreshUI()
    {
        if (CurrentItem == null)
        {
            IconImage.sprite = ResourcesLoader.Instance.GetSprite("transparentPixel");
            CountText.text = "";
            return;
        }
        else
        {
            if (CurrentItem.Stack > 1)
            {
                CountText.text = "x" + CurrentItem.Stack;
            }
            else
            {
                CountText.text = "";
            }
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

    public void Clicked()
    {
        if (CurrentItem == null)
        {
            return;
        }

        InGameMainMenuUI.Instance.isDraggingItem = true;
    }

    public void UnClicked()
    {
        if (CurrentItem == null)
        {
            return;
        }

        InGameMainMenuUI.Instance.isDraggingItem = false;
    }

    public void Hover()
    {
        InGameMainMenuUI.Instance.SetHoverSlot(this);

        outline.enabled = true;
    }

    public void UnHover()
    {
        if (InGameMainMenuUI.Instance.HoveredSlot == this)
        {
            InGameMainMenuUI.Instance.UnsetHoverSlot();
        }

        outline.enabled = false;
    }

    internal void UnDrag()
    {
        IconImage.color = new Color(IconImage.color.r, IconImage.color.g, IconImage.color.b, 1f);
    }

    public void DisableInput()
    {
        GetComponent<Image>().raycastTarget = false;
    }

    public void EnableInput()
    {
        GetComponent<Image>().raycastTarget = true;
    }

}
