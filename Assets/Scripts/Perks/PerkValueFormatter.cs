using UnityEngine;

public class PerkValueFormatter
{
    private static PerkValueFormatter _instance; 
    public static PerkValueFormatter Instance
    { get { return _instance == null ? _instance = new PerkValueFormatter() : _instance; } }

    public string GetFormattedValue(DevPAPerk perkInfo, float perkValue, bool includePrefix)
    {
        string prefix = includePrefix && perkValue >= 0 ? "+" : "";
        string suffix = "";
        if (perkInfo.PerkType == DevPAPerk.PerkTypeEnum.Percent) {
            perkValue *= 100f;
            suffix = "%";
        } else if (perkInfo.PerkType == DevPAPerk.PerkTypeEnum.Time) {
            suffix = "s";
        } 
        bool hasFloatingPoint = perkValue % 1 == 0;
        string showValue = (hasFloatingPoint ? Mathf.FloorToInt(perkValue) : perkValue).ToString();
        return prefix + showValue + suffix;
    }
}