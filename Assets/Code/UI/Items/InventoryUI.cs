using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class InventoryUI : ItemSlotsContainerUI
{

    [SerializeField]
    Transform Container;

    public ActorInfo CurrentCharacter;

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if(InGameMainMenuUI.Instance.HoveredSlot!=null && InGameMainMenuUI.Instance.HoveredSlot.ParentContainer == this)
            {
                SocketClient.Instance.SendUsedItem(InGameMainMenuUI.Instance.HoveredSlot.transform.GetSiblingIndex());
            }
        }
    }

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

    public override void DisableInput()
    {
        for(int i=0;i<Container.childCount;i++)
        {
            Container.GetChild(i).GetComponent<ItemUI>().DisableInput();
        }
    }

    public override void EnableInput()
    {
        for (int i = 0; i < Container.childCount; i++)
        {
            Container.GetChild(i).GetComponent<ItemUI>().EnableInput();
        }
    }
}
