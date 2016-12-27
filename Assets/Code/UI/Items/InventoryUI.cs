using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour {

    [SerializeField]
    Transform Container;

    public ItemUI HoveredSlot;
    public ItemUI DraggedSlot;

    GameObject CurrentDragged;

    ActorInfo CurrentCharacter;

    public void ShowInventory(ActorInfo Character)
    {
        CurrentCharacter = Character;
        this.gameObject.SetActive(true);

        Clear();

        //TODO Might have problem when changing inventory size - (Maybe generate the slots if required).
        for(int i=0;i<Character.Inventory.Content.Length;i++)
        {
             Container.GetChild(i).GetComponent<ItemUI>().SetData(Character.Inventory.Content[i], this); 
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

    internal void Hover(ItemUI itemUI)
    {
        HoveredSlot = itemUI;
    }

    internal void UnHover(ItemUI itemUI)
    {
        if(HoveredSlot == itemUI)
        {
            HoveredSlot = null;
        }
    }

    internal void BeginDrag(ItemUI itemUI)
    {
        DraggedSlot = itemUI;

        CurrentDragged = ResourcesLoader.Instance.GetRecycledObject("DraggedItem");
        CurrentDragged.GetComponent<Image>().sprite = ResourcesLoader.Instance.GetSprite(itemUI.CurrentItem.IconKey);
        CurrentDragged.transform.SetParent(transform, false);
        CurrentDragged.transform.SetAsLastSibling();

        if (DragRoutineInstance!=null)
        {
            StopCoroutine(DragRoutineInstance);
        }
        DragRoutineInstance = StartCoroutine(DragSlotRoutine());
    }

    Coroutine DragRoutineInstance;
    private IEnumerator DragSlotRoutine()
    {
        while(Input.GetMouseButton(0))
        {
            CurrentDragged.transform.position = GameCamera.MousePosition;
            yield return 0;
        }

        ReleaseDraggedItem();
    }

    private void ReleaseDraggedItem()
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


        DraggedSlot.UnDrag();
        CurrentDragged.gameObject.SetActive(false);
        CurrentDragged = null;

        DragRoutineInstance = null;
    }
}
