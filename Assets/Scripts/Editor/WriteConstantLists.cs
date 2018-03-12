using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

public class WriteConstantLists
{
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
        string fileName = "Assets/Scripts/CONTENT/Content.cs";
        string text = File.ReadAllText(fileName);
        
        Regex rgx = new Regex("/\\* AUTO_GENERATED_PERKS_START \\*/ .* /\\* AUTO_GENERATED_PERKS_END \\*/");
        string perksString = "";
        for (int i = 0; i < perks.Count; i++)
        {
            perksString += '"' + perks[i].Key + '"';
            if (i != perks.Count - 1)
            {
                perksString += ", ";
            }
        }

        string result = rgx.Replace(text, "/* AUTO_GENERATED_PERKS_START */ " + perksString + " /* AUTO_GENERATED_PERKS_END */");
        File.WriteAllText(fileName, result);
    }
}