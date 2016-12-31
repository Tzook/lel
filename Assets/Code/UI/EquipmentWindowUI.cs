using UnityEngine;
using System.Collections;
using System;

public class EquipmentWindowUI : ItemSlotsContainerUI
{
    [SerializeField]
    Transform CharSpot;
    ActorInstance CharInstance;


    [SerializeField]
    ItemUI HeadSlot;

    [SerializeField]
    ItemUI ChestSlot;

    [SerializeField]
    ItemUI GlovesSlot;

    [SerializeField]
    ItemUI LegsSlot;

    [SerializeField]
    ItemUI ShoesSlot;

    ActorInfo CurrentCharacter;

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (InGameMainMenuUI.Instance.HoveredSlot != null && InGameMainMenuUI.Instance.HoveredSlot.ParentContainer == this)
            {
                SocketClient.Instance.SendUsedEquip(InGameMainMenuUI.Instance.HoveredSlot.slotKey);
            }
        }
    }


    public void Open(ActorInfo Info)
    {
        this.gameObject.SetActive(true);

        CurrentCharacter = Info;

        StartCoroutine(OpenRoutine());
    }
    
    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    private IEnumerator OpenRoutine()
    {
        if (CharSpot.childCount > 0)
        {
            Destroy(CharSpot.GetChild(0).gameObject);
        }

        yield return 0;

        if (CurrentCharacter.Gender == Gender.Male)
        {
            Instantiate(ResourcesLoader.Instance.GetObject("actor_male")).transform.SetParent(CharSpot);
        }
        else
        {
            Instantiate(ResourcesLoader.Instance.GetObject("actor_female")).transform.SetParent(CharSpot);
        }

        CharSpot.GetChild(0).position = CharSpot.position;
        CharSpot.GetChild(0).transform.localScale = Vector3.one;
        CharInstance = CharSpot.GetChild(0).GetComponent<ActorInstance>();

        CharInstance.Info = CurrentCharacter;
        CharInstance.nameHidden = true;

        CharInstance.SetElementsUILayer();

        RefreshEquipment();

    }

    public void RefreshEquipment()
    {
        HeadSlot.SetData(CurrentCharacter.Equipment.Head, this);
        ChestSlot.SetData(CurrentCharacter.Equipment.Chest, this);
        GlovesSlot.SetData(CurrentCharacter.Equipment.Gloves, this);
        LegsSlot.SetData(CurrentCharacter.Equipment.Legs, this);
        ShoesSlot.SetData(CurrentCharacter.Equipment.Shoes, this);

        CharInstance.UpdateVisual();
    }

    public override void DisableInput()
    {
        HeadSlot.GetComponent<ItemUI>().DisableInput();
        ChestSlot.GetComponent<ItemUI>().DisableInput();
        GlovesSlot.GetComponent<ItemUI>().DisableInput();
        LegsSlot.GetComponent<ItemUI>().DisableInput();
        ShoesSlot.GetComponent<ItemUI>().DisableInput();
    }

    public override void EnableInput()
    {
        HeadSlot.GetComponent<ItemUI>().EnableInput();
        ChestSlot.GetComponent<ItemUI>().EnableInput();
        GlovesSlot.GetComponent<ItemUI>().EnableInput();
        LegsSlot.GetComponent<ItemUI>().EnableInput();
        ShoesSlot.GetComponent<ItemUI>().EnableInput();
    }

    public bool CanEquip(ItemInfo item, ItemUI slot)
    {
        return CurrentCharacter.Equipment.CanEquip(item.Type, slot.slotKey);
    }
}
