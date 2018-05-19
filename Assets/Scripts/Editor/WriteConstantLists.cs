using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;

public class WriteConstantLists
{       
    private const string FILE_NAME_CONTENT = "Assets/Scripts/CONTENT/Content.cs";
    private const string FILE_NAME_MONSTERS = "Assets/Scripts/Entities/MonsterSpawner.cs";
    private const string FILE_NAME_NPC = "Assets/Scripts/NPC/NPC.cs";
    private const string FILE_NAME_SCENE = "Assets/Scripts/Misc/SceneInfo.cs";
    private static WriteConstantLists _instance; 
    public static WriteConstantLists Instance
    { get { return _instance == null ? _instance = new WriteConstantLists() : _instance; } }
    
    public void WritePerksPopupList(List<DevPAPerk> perks)
    {
        string text = File.ReadAllText(FILE_NAME_CONTENT);

        Regex rgx = new Regex("/\\* AUTO_GENERATED_PERKS_START \\*/ .* /\\* AUTO_GENERATED_PERKS_END \\*/");
        
        string listString = GetListString(perks.Select(perk => perk.Key).ToList());

        string result = rgx.Replace(text, "/* AUTO_GENERATED_PERKS_START */ " + listString + " /* AUTO_GENERATED_PERKS_END */");
        File.WriteAllText(FILE_NAME_CONTENT, result);
        AssetDatabase.Refresh();
    }

    public void WriteBuffsPopupList(List<DevBuff> buffs)
    {
        string text = File.ReadAllText(FILE_NAME_CONTENT);
        
        Regex rgx = new Regex("/\\* AUTO_GENERATED_BUFFS_START \\*/ .* /\\* AUTO_GENERATED_BUFFS_END \\*/");
        string listString = GetListString(buffs.Select(buff => buff.Key).ToList());

        string replacement = "/* AUTO_GENERATED_BUFFS_START */ " + listString + " /* AUTO_GENERATED_BUFFS_END */";
        string result = rgx.Replace(text, replacement);
        File.WriteAllText(FILE_NAME_CONTENT, result);
        
        AssetDatabase.Refresh();
    }

    public void WriteAbilitiesPopupList(List<DevAbility> abilities)
    {
        string text = File.ReadAllText(FILE_NAME_CONTENT);
        
        Regex rgx = new Regex("/\\* AUTO_GENERATED_ABILITIES_START \\*/ .* /\\* AUTO_GENERATED_ABILITIES_END \\*/");
        string listString = GetListString(abilities.Select(ability => ability.Key).ToList());

        string replacement = "/* AUTO_GENERATED_ABILITIES_START */ " + listString + " /* AUTO_GENERATED_ABILITIES_END */";
        string result = rgx.Replace(text, replacement);
        File.WriteAllText(FILE_NAME_CONTENT, result);

        text = File.ReadAllText(FILE_NAME_SCENE);
        result = rgx.Replace(text, replacement);
        File.WriteAllText(FILE_NAME_SCENE, result);
        AssetDatabase.Refresh();
    }

    public void WriteLootPopupList(List<DevItemInfo> items)
    {
        string text = File.ReadAllText(FILE_NAME_CONTENT);
        
        Regex rgx = new Regex("/\\* AUTO_GENERATED_LOOT_START \\*/ .* /\\* AUTO_GENERATED_LOOT_END \\*/");
        
        string listString = GetListString(items.Select(item => item.Key).ToList());
        
        string replacement = "/* AUTO_GENERATED_LOOT_START */ " + listString + " /* AUTO_GENERATED_LOOT_END */";
        string result = rgx.Replace(text, replacement);
        File.WriteAllText(FILE_NAME_CONTENT, result);

        text = File.ReadAllText(FILE_NAME_NPC);
        result = rgx.Replace(text, replacement);
        File.WriteAllText(FILE_NAME_NPC, result);
        AssetDatabase.Refresh();
    }

    public void WriteQuestsPopupList(List<Quest> quests)
    {
        string text = File.ReadAllText(FILE_NAME_CONTENT);
        
        Regex rgx = new Regex("/\\* AUTO_GENERATED_QUESTS_START \\*/ .* /\\* AUTO_GENERATED_QUESTS_END \\*/");
        string listString = GetListString(quests.Select(quest => quest.Key).ToList());

        string replacement = "/* AUTO_GENERATED_QUESTS_START */ " + listString + " /* AUTO_GENERATED_QUESTS_END */";
        string result = rgx.Replace(text, replacement);
        File.WriteAllText(FILE_NAME_CONTENT, result);

        text = File.ReadAllText(FILE_NAME_NPC);
        result = rgx.Replace(text, replacement);
        File.WriteAllText(FILE_NAME_NPC, result);
        AssetDatabase.Refresh();
    }

    public void WriteMobsPopupList(List<DevMonsterInfo> mobs)
    {
        string text = File.ReadAllText(FILE_NAME_CONTENT);
        
        Regex rgx = new Regex("/\\* AUTO_GENERATED_MOBS_START \\*/ .* /\\* AUTO_GENERATED_MOBS_END \\*/");
        string listString = GetListString(mobs.Select(mob => mob.MonsterKey).ToList());

        string replacement = "/* AUTO_GENERATED_MOBS_START */ " + listString + " /* AUTO_GENERATED_MOBS_END */";
        string result = rgx.Replace(text, replacement);
        File.WriteAllText(FILE_NAME_CONTENT, result);
        
        text = File.ReadAllText(FILE_NAME_MONSTERS);
        result = rgx.Replace(text, replacement);
        File.WriteAllText(FILE_NAME_MONSTERS, result);
        AssetDatabase.Refresh();
    }

    private string GetListString(List<string> items)
    {
        string itemsString = "\"" + PopupAttribute.NO_VALUE_OPTION + "\", ";
        items.Sort();
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