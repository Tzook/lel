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

    Color originalTextColor;
    Color initialPerkColor;

    void Awake() 
    {
        originalTextColor = txtPrecent.color;
            
        // darken the color by 70%
        float r = originalTextColor.r * 0.3f;
        float g = originalTextColor.g * 0.3f;
        float b = originalTextColor.b * 0.3f;
        initialPerkColor = new Color(r, g, b);
    }

    public void SetInfo(DevPrimaryAbility devPA, PAPerk perk)
    {
        currentPerk = Content.Instance.GetPerk(perk.Key);
        bool isInitialValue = Content.Instance.IsInitialPerkValue(devPA, perk);

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
        txtPrecent.color = isInitialValue ? initialPerkColor : originalTextColor;
    }
}
