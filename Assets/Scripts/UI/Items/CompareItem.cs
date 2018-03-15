using UnityEngine;

public class CompareItem : MonoBehaviour 
{
    [SerializeField]
    EquipmentWindowUI m_equipmentPanel;
    [SerializeField]
    private ItemInfoUI ComparedItem;

    private bool InCompareMode = false;

    private ItemInfoUI OriginalItem;


    void Awake()
    {
        OriginalItem = GetComponent<ItemInfoUI>();
    }

    void Update()
    {
        if (this.isActiveAndEnabled && InGameMainMenuUI.Instance.HoveredSlot && InGameMainMenuUI.Instance.HoveredSlot.ParentContainer != m_equipmentPanel)
        {
            if (!InCompareMode)
            {
                InCompareMode = ShouldBeInCompareMode();
                if (InCompareMode)
                {
                    DevItemInfo itemInfo = Content.Instance.GetItem(OriginalItem.currentItemKey);
                    ItemInfo matchingItem = LocalUserInfo.Me.ClientCharacter.Equipment.GetItem(itemInfo.Type);
                    if (matchingItem != null)
                    {
                        ComparedItem.Show(matchingItem);
                    }
                }
            }
            if (InCompareMode)
            {
                InCompareMode = ShouldBeInCompareMode();
                if (!InCompareMode)
                {
                    ComparedItem.Hide();
                }

            }
        }
    }

    private bool ShouldBeInCompareMode()
    {
        return Input.GetKey(KeyCode.LeftShift) || m_equipmentPanel.isActiveAndEnabled;
    }

    void OnDisable()
    {
        if (InCompareMode)
        {
            InCompareMode = false;
            ComparedItem.Hide();
        }
    }
}