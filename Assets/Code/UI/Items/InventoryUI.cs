using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class InventoryUI : ItemSlotsContainerUI
{

    [SerializeField]
    Transform Container;

    [SerializeField]
    Text GoldText;

    public ActorInfo CurrentCharacter;

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if(InGameMainMenuUI.Instance.HoveredSlot!=null && InGameMainMenuUI.Instance.HoveredSlot.ParentContainer == this)
            {
                if (Game.Instance.CanUseUI)
                {
                    SocketClient.Instance.SendUsedItem(InGameMainMenuUI.Instance.HoveredSlot.transform.GetSiblingIndex());
                }
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

        GoldText.text = "0";

        //TODO Might have problem when changing inventory size - (Maybe generate the slots if required).
        for (int i = 0; i < CurrentCharacter.Inventory.ContentArray.Length; i++)
        {
            Container.GetChild(i).GetComponent<ItemUI>().SetData(CurrentCharacter.Inventory.ContentArray[i], this);
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
