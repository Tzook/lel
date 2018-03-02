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

    public void SetInfo(DevAbility devPA, PAPerk perk)
    {
        currentPerk = Content.Instance.GetPerk(perk.Key);
        bool isInitialValue = Content.Instance.IsInitialPerkValue(devPA, perk);

        txtTitle.text = currentPerk.Name;
        iconImage.sprite = currentPerk.Icon;

        float percentValue = GetPerkValue(currentPerk, perk);

        if (currentPerk.PrecentPerUpgrade < 1)
        {
            txtPrecent.text = Mathf.FloorToInt(percentValue * 100f) + "%";
        }
        else
        {
            txtPrecent.text = percentValue.ToString();
        }

        txtPrecent.color = isInitialValue ? initialPerkColor : originalTextColor;
    }

    private float GetPerkValue(DevPAPerk currentPerk, PAPerk perk)
    {
        // Gauss formula => 1 + 2 + 3 + ... + n = n * (n + 1) / 2
        float sumValuesUntilLevel = (perk.Points - 1) * perk.Points / 2;
        // 1 * x + 2 * x + ... + n * x = (1 + 2 + 3 + ... + n) * x
        float accelerationPoint = sumValuesUntilLevel * currentPerk.PrecentAccelerationPerUpgrade;
        
        float percentValue = currentPerk.StartingValue + perk.Points * currentPerk.PrecentPerUpgrade + accelerationPoint;
        return percentValue;
    }
}
