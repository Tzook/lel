using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

public class WriteConstantLists
{
        
    private const string fileName = "Assets/Scripts/CONTENT/Content.cs";
    private static WriteConstantLists _instance; 
    public static WriteConstantLists Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new WriteConstantLists();
            }
            return _instance;
        }
    }

    public void WritePerksPopupList(List<DevPAPerk> perks)
    {
        string text = File.ReadAllText(fileName);

        Regex rgx = new Regex("/\\* AUTO_GENERATED_PERKS_START \\*/ .* /\\* AUTO_GENERATED_PERKS_END \\*/");
        
        string perksString = GetItemsString(perks.Select(perk => perk.Key).ToList());

        string result = rgx.Replace(text, "/* AUTO_GENERATED_PERKS_START */ " + perksString + " /* AUTO_GENERATED_PERKS_END */");
        File.WriteAllText(fileName, result);
    }

    public void WriteLootPopupList(List<DevItemInfo> items)
    {
        string text = File.ReadAllText(fileName);
        
        Regex rgx = new Regex("/\\* AUTO_GENERATED_LOOT_START \\*/ .* /\\* AUTO_GENERATED_LOOT_END \\*/");
        string lootString = GetItemsString(items.Select(item => item.Key).ToList());

        string result = rgx.Replace(text, "/* AUTO_GENERATED_LOOT_START */ " + lootString + " /* AUTO_GENERATED_LOOT_END */");
        File.WriteAllText(fileName, result);
    }

    private string GetItemsString(List<string> items)
    {
        string itemsString = "";
        for (int i = 0; i < items.Count; i++)
        {
            itemsString += '"' + items[i] + '"';
            if (i != items.Count - 1)
            {
                itemsString += ", ";
            }
        }

        return itemsString;
    }
}