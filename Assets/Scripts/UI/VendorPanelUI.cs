﻿using System.Collections;
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

    [SerializeField]
    Color DefaultColor;

    [SerializeField]
    Color CantBuyColor;

    [SerializeField]
    Image BuyImage;

    NPC CurrentVendor;

    public void Show(string key, NPC Vendor)
    {
        CurrentVendor = Vendor;

        GetComponent<Canvas>().worldCamera = GameCamera.Instance.BlurCam;

        transform.GetChild(0).gameObject.SetActive(true);

        ReferenceItem = Content.Instance.GetItem(key);
        CurrentItem = new ItemInfo(ReferenceItem, ReferenceItem.Perks, 1);

        SetPurchaseButton(LocalUserInfo.Me.ClientCharacter.Gold);

        PriceText.text = ReferenceItem.GoldValue.ToString();

        m_ItemInfoUI.Show(CurrentItem);
    }

    protected void SetPurchaseButton(int gold)
    {
        if (gold < ReferenceItem.GoldValue)
        {
            BuyImage.color = CantBuyColor;
            BuyImage.GetComponent<Button>().interactable = false;
        }
        else
        {
            BuyImage.color = DefaultColor;
            BuyImage.GetComponent<Button>().interactable = true;
        }
    }

    public void Hide()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public void PurchaseItem()
    {
        AudioControl.Instance.Play("sound_purchase");

        SocketClient.Instance.SendBuyVendorItem(CurrentVendor.Key, CurrentVendor.GetItemIndex(CurrentItem.Key));
        SetPurchaseButton(LocalUserInfo.Me.ClientCharacter.Gold - ReferenceItem.GoldValue);
    }
}
