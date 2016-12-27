using UnityEngine;
using System.Collections;
using System;

public class EquipmentWindowUI : ItemSlotsContainerUI
{
    [SerializeField]
    Transform CharSpot;

    [SerializeField]
    ItemUI HeadSlot;

    [SerializeField]
    ItemUI ChestSlot;

    ActorInstance CharInstance;
    
    public void Open(ActorInfo Info)
    {
        this.gameObject.SetActive(true);

        StartCoroutine(OpenRoutine(Info));

        HeadSlot.SetData(null, this);
        ChestSlot.SetData(null, this);

    }
    
    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    private IEnumerator OpenRoutine(ActorInfo Info)
    {
        if (CharSpot.childCount > 0)
        {
            Destroy(CharSpot.GetChild(0).gameObject);
        }

        yield return 0;

        if (Info.Gender == Gender.Male)
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

        CharInstance.Info = Info;
        CharInstance.nameHidden = true;

        CharInstance.SetElementsUILayer();
        CharInstance.UpdateVisual();
    }

    public void RefreshEquipment()
    {
        
    }
}
