using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeInfoUI : MonoBehaviour {

    [SerializeField]
    Text txtTitle;

    [SerializeField]
    Text txtPrecent;

    [SerializeField]
    Image iconImage;

    DevPAPerk currentPerk;

    public void SetInfo(PAPerk perk)
    {
        currentPerk = Content.Instance.GetPerk(perk.Key);

        txtTitle.text = currentPerk.Name;
        iconImage.sprite = currentPerk.Icon;

        if (currentPerk.PrecentPerUpgrade < 1)
        {
            txtPrecent.text = Mathf.FloorToInt((perk.Points * currentPerk.PrecentPerUpgrade) * 100f) + "%";
        }
        else
        {
            txtPrecent.text = perk.Points.ToString();
        }
    }
}
