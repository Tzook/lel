using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VendorPanelUI : MonoBehaviour {

    [SerializeField]
    Text PriceText;

    [SerializeField]
    ItemInfoUI m_ItemInfoUI;

    ItemInfo CurrentItem;
    DevItemInfo ReferenceItem;

    public void Show(string key)
    {
        GetComponent<Canvas>().worldCamera = GameCamera.Instance.BlurCam;

        transform.GetChild(0).gameObject.SetActive(true);

        ReferenceItem = Content.Instance.GetItem(key);
        CurrentItem = new ItemInfo(ReferenceItem, 1);

        PriceText.text = ReferenceItem.GoldValue.ToString();

        m_ItemInfoUI.Show(CurrentItem);
    }

    public void Hide()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }
}
