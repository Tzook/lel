using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ItemSlotsContainerUI : MonoBehaviour {

    public ItemUI DraggedSlot;
    public ItemUI HoveredSlot;

    protected GameObject CurrentDragged;

    public virtual void BeginDrag(ItemUI slot)
    {
        DraggedSlot = slot;

        CurrentDragged = ResourcesLoader.Instance.GetRecycledObject("DraggedItem");
        CurrentDragged.GetComponent<Image>().sprite = ResourcesLoader.Instance.GetSprite(slot.CurrentItem.IconKey);
        CurrentDragged.transform.SetParent(InGameMainMenuUI.Instance.transform, false);
        CurrentDragged.transform.SetAsLastSibling();

        if (DragRoutineInstance != null)
        {
            StopCoroutine(DragRoutineInstance);
        }
        DragRoutineInstance = StartCoroutine(DragSlotRoutine());
    }

    public virtual void Hover(ItemUI slot)
    {
        HoveredSlot = slot;
    }

    public virtual void UnHover(ItemUI slot)
    {
        if (HoveredSlot == slot)
        {
            HoveredSlot = null;
        }
    }

    protected Coroutine DragRoutineInstance;
    protected virtual IEnumerator DragSlotRoutine()
    {
        while (Input.GetMouseButton(0))
        {
            CurrentDragged.transform.position = GameCamera.MousePosition;
            yield return 0;
        }

        ReleaseDraggedItem();
    }

    protected virtual void ReleaseDraggedItem()
    {
        DraggedSlot.UnDrag();
        CurrentDragged.gameObject.SetActive(false);
        CurrentDragged = null;

        DragRoutineInstance = null;
    }
}
