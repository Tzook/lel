using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardPanelUI : MonoBehaviour {

    [SerializeField]
    Image Icon;

    [SerializeField]
    Text ItemName;

    [SerializeField]
    Text ItemStack;

    public void SetInfo(string itemName, Sprite itemIcon = null, int itemStack = 0)
    {
        ItemName.text = itemName;

        if(itemIcon != null)
        {
            Icon.sprite = itemIcon;
        }
        else
        {
            Icon.sprite = null;
        }

        if(itemStack > 0)
        {
            ItemStack.text = "x" + itemStack.ToString("N0");
        }
    }
}
