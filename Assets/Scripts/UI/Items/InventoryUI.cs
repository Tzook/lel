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
            InGameMainMenuUI.Instance.isDraggingItem = false;

            if (InGameMainMenuUI.Instance.HoveredSlot != null && InGameMainMenuUI.Instance.HoveredSlot.ParentContainer == this)
            {
                if (Game.Instance.CanUseUI)
                {
                    UseItem(InGameMainMenuUI.Instance.HoveredSlot.transform.GetSiblingIndex());

                }
            }
        }
    }

    void UseItem(int gIndex)
    {
        ItemInfo item = CurrentCharacter.Inventory.ContentArray[gIndex];

        if (item.SubType == "consumable" || item.SubType == "food" || item.SubType == "drink")
        {
            LocalUserInfo.Me.ClientCharacter.Instance.InputController.UseConsumable(gIndex, item);
        }
        else
        {
            SocketClient.Instance.SendUsedItem(InGameMainMenuUI.Instance.HoveredSlot.transform.GetSiblingIndex());
        }

        InGameMainMenuUI.Instance.ForceHideItemInfo();
    }

    public void ShowInventory(ActorInfo Character)
    {
        CurrentCharacter = Character;
        this.gameObject.SetActive(true);

        RefreshInventory();
    }

    public void ShowInventory()
    {
        ShowInventory(LocalUserInfo.Me.ClientCharacter);
    }

    public void RefreshInventory()
    {
        Clear();

        GoldText.text = CurrentCharacter.Gold.ToString("N0");

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
