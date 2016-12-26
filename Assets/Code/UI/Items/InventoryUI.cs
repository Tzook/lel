using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class InventoryUI : ItemSlotsContainerUI {

    [SerializeField]
    Transform Container;

    ActorInfo CurrentCharacter;

    public void ShowInventory(ActorInfo Character)
    {
        CurrentCharacter = Character;
        this.gameObject.SetActive(true);

        RefreshInventory();
    }

    public void RefreshInventory()
    {
        Clear();

        //TODO Might have problem when changing inventory size - (Maybe generate the slots if required).
        for (int i = 0; i < CurrentCharacter.Inventory.Content.Length; i++)
        {
            Container.GetChild(i).GetComponent<ItemUI>().SetData(CurrentCharacter.Inventory.Content[i], this);
        }
    }

    public void Clear()
    {
        for(int i=0;i<Container.childCount;i++)
        {
            Container.GetChild(i).GetComponent<ItemUI>().Clear();
        }
    }

    internal void Hide()
    {
        this.gameObject.SetActive(false);
    }

    protected override void ReleaseDraggedItem()
    {
        int draggedIndex = DraggedSlot.transform.GetSiblingIndex();

        if (HoveredSlot != null)
        {
            if (HoveredSlot != DraggedSlot)
            {
                int releasedIndex = HoveredSlot.transform.GetSiblingIndex();

                CurrentCharacter.Inventory.SwapSlots(draggedIndex, releasedIndex);
                SocketClient.Instance.SendMovedItem(draggedIndex, releasedIndex);
            }

            HoveredSlot.UnDrag();
        }
        else
        {
            CurrentCharacter.Inventory.RemoveItem(draggedIndex);
            SocketClient.Instance.SendDroppedItem(draggedIndex);
        }

        ShowInventory(CurrentCharacter);

        base.ReleaseDraggedItem();
    }
}
