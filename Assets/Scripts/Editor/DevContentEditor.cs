/* 
 * Items:
 * item delete 'ItemKey / index'
 * 
 * Monsters:
 * monster delete 'MonserKey / Index'
 * monster addLoot 'MonserKey' 'ItemKey'
 * monster deleteLoot 'MonsterKey' 'ItemKey'
 * monster addLootAll 'ItemKey'
 * monster deleteLootAll 'ItemKey'
 * monster addLootMonsterLevel 'ItemKey' 'MonsterLevel'     - Will add this item in monsters above level
 * monster deleteLootMonsterLevel 'ItemKey' 'MonsterLevel'  - Will delete this item in monsters below level
 * monster ClearLootDuplicates
 * monster cloneLoot 'FromMonsterKey' 'ToMonsterKey' 
 * pa clonePerks 'FromPA' 'ToPa'
 */

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using SimpleJSON;
using BestHTTP;
using System.Text;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(Content))]
public class DevContentEditor : Editor
{
    string CurrentCommand;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Content currentInfo = (Content)target;

        GUILayout.BeginVertical();

        if (GUILayout.Button("Update Monsters"))
        {
            JSONNode node = new JSONClass();

            node["pass"] = "b0ss123";

            for (int i = 0; i < currentInfo.Monsters.Count; i++)
            {
                node["mobs"][i]["key"] = currentInfo.Monsters[i].MonsterKey;
                node["mobs"][i]["name"] = currentInfo.Monsters[i].MonsterName;
                node["mobs"][i]["hp"] = currentInfo.Monsters[i].MonsterHP.ToString();
                node["mobs"][i]["level"] = currentInfo.Monsters[i].MonsterLevel.ToString();
                node["mobs"][i]["minDMG"] = currentInfo.Monsters[i].MinDMG.ToString();
                node["mobs"][i]["maxDMG"] = currentInfo.Monsters[i].MaxDMG.ToString();
                node["mobs"][i]["exp"] = currentInfo.Monsters[i].RewardEXP.ToString();

                for (int a = 0; a < currentInfo.Monsters[i].PossibleLoot.Count; a++)
                {
                    node["mobs"][i]["drops"][a]["key"] = currentInfo.Monsters[i].PossibleLoot[a].ItemKey;
                    node["mobs"][i]["drops"][a]["minStack"] = currentInfo.Monsters[i].PossibleLoot[a].MinStack.ToString();
                    node["mobs"][i]["drops"][a]["maxStack"] = currentInfo.Monsters[i].PossibleLoot[a].MaxStack.ToString();
                }

                for (int a = 0; a < currentInfo.Monsters[i].Perks.Count; a++)
                {
                    node["mobs"][i]["perks"][a]["key"] = currentInfo.Monsters[i].Perks[a].Key.ToString();
                    node["mobs"][i]["perks"][a]["value"] = currentInfo.Monsters[i].Perks[a].Value.ToString();
                }

                for (int a = 0; a < currentInfo.Monsters[i].Spells.Count; a++)
                {
                    node["mobs"][i]["spells"][a]["key"] = currentInfo.Monsters[i].Spells[a].Key.ToString();

                    for (int b = 0; b < currentInfo.Monsters[i].Spells[a].Perks.Count; b++)
                    {
                        node["mobs"][i]["spells"][a]["perks"][b]["key"] = currentInfo.Monsters[i].Spells[a].Perks[b].Key.ToString();
                        node["mobs"][i]["spells"][a]["perks"][b]["value"] = currentInfo.Monsters[i].Spells[a].Perks[b].Value.ToString();
                    }
                    node["mobs"][i]["spells"][a]["minTime"] = currentInfo.Monsters[i].Spells[a].MinTime.ToString();
                    node["mobs"][i]["spells"][a]["maxTime"] = currentInfo.Monsters[i].Spells[a].MaxTime.ToString();
                }
            }

            SendMonstersInfo(node);
            WriteConstantLists.Instance.WriteMobsPopupList(currentInfo.Monsters);
        }

        if (GUILayout.Button("Update Items"))
        {
            JSONNode node = new JSONClass();

            node["pass"] = "b0ss123";

            for (int i = 0; i < currentInfo.Items.Count; i++)
            {
                node["items"][i]["key"] = currentInfo.Items[i].Key;

                if (currentInfo.Items[i].IconPlaceable != null)
                {
                    currentInfo.Items[i].Icon = currentInfo.Items[i].IconPlaceable.name.ToString();
                }

                node["items"][i]["type"] = currentInfo.Items[i].Type;
                node["items"][i]["subType"] = currentInfo.Items[i].SubType;
                node["items"][i]["dropChance"] = currentInfo.Items[i].DropChance.ToString();
                node["items"][i]["goldValue"] = currentInfo.Items[i].GoldValue.ToString();
                node["items"][i]["stackCap"] = currentInfo.Items[i].StackCap.ToString();

                node["items"][i]["req"]["lvl"] = currentInfo.Items[i].Stats.RequiresLVL.ToString();

                node["items"][i]["use"]["hp"] = currentInfo.Items[i].UseInfo.BonusHP.ToString();
                node["items"][i]["use"]["mp"] = currentInfo.Items[i].UseInfo.BonusMP.ToString();

                for (int a = 0; a < currentInfo.Items[i].Perks.Count; a++)
                {
                    node["items"][i]["perks"][a]["key"] = currentInfo.Items[i].Perks[a].Key.ToString();
                    node["items"][i]["perks"][a]["value"] = currentInfo.Items[i].Perks[a].Value.ToString();
                }

                for (int a = 0; a < currentInfo.Items[i].ItemSprites.Count; a++)
                {
                    if (currentInfo.Items[i].ItemSprites[a].SpritePlaceable != null)
                    {
                        currentInfo.Items[i].ItemSprites[a].Sprite = currentInfo.Items[i].ItemSprites[a].SpritePlaceable.name.ToString();
                    }

                }

            }

            SendItemsInfo(node);
            WriteConstantLists.Instance.WriteLootPopupList(currentInfo.Items);
        }

        if (GUILayout.Button("Update Quests"))
        {
            SendQuestsInfo(currentInfo.Quests);
            WriteConstantLists.Instance.WriteQuestsPopupList(currentInfo.Quests);
        }

        if (GUILayout.Button("Update Starting Gear"))
        {
            SendStartingGear(currentInfo.StartingGear);
        }

        if (GUILayout.Button("Update Abilities"))
        {
            SendAbilities(currentInfo.Abilities, currentInfo.Perks);
            WriteConstantLists.Instance.WritePerksPopupList(currentInfo.Perks);
        }

        GUILayout.Label("Command line");

        CurrentCommand = EditorGUILayout.TextField(CurrentCommand);

        if (GUILayout.Button("Execute"))
        {
            HandleCommand(CurrentCommand);
            CurrentCommand = "";
        }

        if (GUILayout.Button("Restart Server"))
        {
            RestartServer();
        }

        GUILayout.EndVertical();
    }

    private void SendMonstersInfo(JSONNode node)
    {
        Debug.Log("Sending monsters info " + node.ToString());

        byte[] rawdata = Encoding.UTF8.GetBytes(node.ToString());

        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("Content-Type", "application/json");

        WWW req = new WWW(Config.BASE_URL + "/mob/generate", rawdata, headers);


        ContinuationManager.Add(() => req.isDone, () =>
        {
            if (!string.IsNullOrEmpty(req.error)) Debug.Log("WWW failed: " + req.error);
            Debug.Log("WWW result : " + req.text);
        });
    }


    private void SendItemsInfo(JSONNode node)
    {
        Debug.Log("Sending items info " + node.ToString());

        byte[] rawdata = Encoding.UTF8.GetBytes(node.ToString());

        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("Content-Type", "application/json");

        WWW req = new WWW(Config.BASE_URL + "/items/generate", rawdata, headers);


        ContinuationManager.Add(() => req.isDone, () =>
        {
            if (!string.IsNullOrEmpty(req.error)) Debug.Log("WWW failed: " + req.error);
            Debug.Log("WWW result : " + req.text);
        });
    }

    private void RestartServer()
    {
        JSONNode node = new JSONClass();

        node["pass"] = "b0ss123";

        Debug.Log("Restarting Server...");

        byte[] rawdata = Encoding.UTF8.GetBytes(node.ToString());

        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("Content-Type", "application/json");


        WWW req = new WWW(Config.BASE_URL + "/restart", rawdata, headers);

        ContinuationManager.Add(() => req.isDone, () =>
        {
            if (!string.IsNullOrEmpty(req.error)) Debug.Log("WWW failed: " + req.error);
            Debug.Log("WWW result : " + req.text);
        });
    }

    private void SendStartingGear(List<string> GearList)
    {
        JSONNode node = new JSONClass();

        node["pass"] = "b0ss123";

        for (int i = 0; i < GearList.Count; i++)
        {
            node["equips"][i]["key"] = GearList[i];
        }

        Debug.Log("Sending Starting Gear...");

        byte[] rawdata = Encoding.UTF8.GetBytes(node.ToString());

        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("Content-Type", "application/json");


        WWW req = new WWW(Config.BASE_URL + "/equips/begin", rawdata, headers);

        ContinuationManager.Add(() => req.isDone, () =>
        {
            if (!string.IsNullOrEmpty(req.error)) Debug.Log("WWW failed: " + req.error);
            Debug.Log("WWW result : " + req.text);
        });
    }

    private void SendAbilities(List<DevAbility> abilities, List<DevPAPerk> perks)
    {
        JSONNode node = new JSONClass();

        node["pass"] = "b0ss123";

        for (int i = 0; i < abilities.Count; i++)
        {
            node["talents"][i]["ability"] = abilities[i].Key;

            for (int a = 0; a < abilities[i].Perks.Count; a++)
            {
                node["talents"][i]["perks"][a]["atLeastLvl"] = abilities[i].Perks[a].MinLevel.ToString();
                node["talents"][i]["perks"][a]["perksOffered"] = abilities[i].Perks[a].PerksOffered.ToString();

                for (int b = 0; b < abilities[i].Perks[a].AddToPool.Count; b++)
                {
                    node["talents"][i]["perks"][a]["addToPool"][b] = abilities[i].Perks[a].AddToPool[b].ToString();
                }
            }

            for (int a = 0; a < abilities[i].Spells.Count; a++)
            {
                node["talents"][i]["spells"][a]["key"] = abilities[i].Spells[a].Key.ToString();
                node["talents"][i]["spells"][a]["level"] = abilities[i].Spells[a].Level.ToString();
                node["talents"][i]["spells"][a]["mana"] = abilities[i].Spells[a].Mana.ToString();
                
                for (int b = 0; b < abilities[i].Spells[a].Perks.Count; b++)
                {
                    node["talents"][i]["spells"][a]["perks"][b]["key"] = abilities[i].Spells[a].Perks[b].Key.ToString();
                    node["talents"][i]["spells"][a]["perks"][b]["value"] = abilities[i].Spells[a].Perks[b].Value.ToString();
                }
            }

            for (int a = 0; a < abilities[i].InitialPerks.Count; a++)
            {
                node["talents"][i]["initialPerks"][a]["key"] = abilities[i].InitialPerks[a].Key.ToString();
                node["talents"][i]["initialPerks"][a]["value"] = abilities[i].InitialPerks[a].Value.ToString();
            }

            node["talents"][i]["hitType"] = abilities[i].HitType;
            node["talents"][i]["manaCost"] = abilities[i].ManaCost.ToString();
        }

        for (int i=0;i<perks.Count;i++)
        {
            node["perkCollection"][i]["key"] = perks[i].Key;
            node["perkCollection"][i]["value"] = perks[i].PrecentPerUpgrade.ToString();
            node["perkCollection"][i]["max"] = perks[i].UpgradeCap.ToString();
            node["perkCollection"][i]["default"] = perks[i].StartingValue.ToString();
            node["perkCollection"][i]["acc"] = "0";//perks[i].PrecentAccelerationPerUpgrade.ToString();
        }

        Debug.Log("Sending Primary Abilities...");

        byte[] rawdata = Encoding.UTF8.GetBytes(node.ToString());

        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("Content-Type", "application/json");


        WWW req = new WWW(Config.BASE_URL + "/talents/generate", rawdata, headers);

        ContinuationManager.Add(() => req.isDone, () =>
        {
            if (!string.IsNullOrEmpty(req.error)) Debug.Log("WWW failed: " + req.error);
            Debug.Log("WWW result : " + req.text);
        });
    }

    private void SendQuestsInfo(List<Quest> Quests)
    {
        JSONNode node = new JSONClass();

        node["pass"] = "b0ss123";

        for (int i = 0; i < Quests.Count; i++)
        {
            node["quests"][i]["key"] = Quests[i].Key;

            for( int c=0 ; c < Quests[i].Conditions.Count ; c++ )
            {
                node["quests"][i]["conditions"][c]["condition"] = Quests[i].Conditions[c].Condition;
                node["quests"][i]["conditions"][c]["conditionType"] = Quests[i].Conditions[c].Type;
                node["quests"][i]["conditions"][c]["targetProgress"] = Quests[i].Conditions[c].TargetProgress.ToString();
            }

            for (int rc = 0; rc < Quests[i].QuestsStates.Count; rc++)
            {
                node["quests"][i]["RequiredQuests"][rc]["key"] = Quests[i].QuestsStates[rc].QuestKey;
                node["quests"][i]["RequiredQuests"][rc]["phase"] = Quests[i].QuestsStates[rc].State;
            }

            node["quests"][i]["minLevel"] = Quests[i].MinimumLevel.ToString();


            for (int ri = 0; ri < Quests[i].RewardItems.Count; ri++)
            {
                node["quests"][i]["rewardItems"][ri]["key"] = Quests[i].RewardItems[ri].ItemKey;
                node["quests"][i]["rewardItems"][ri]["stack"] = Quests[i].RewardItems[ri].MinStack.ToString();
            }

            node["quests"][i]["rewardClass"] = Quests[i].RewardClass;

            node["quests"][i]["rewardHP"] = Quests[i].RewardHP.ToString();
            node["quests"][i]["rewardMP"] = Quests[i].RewardMP.ToString();
            node["quests"][i]["rewardPrimaryAbility"] = Quests[i].RewardPrimaryAbility.Key;

            node["quests"][i]["rewardExp"] = Quests[i].RewardExp.ToString();

        }

        Debug.Log("Sending Quests Info...");

        byte[] rawdata = Encoding.UTF8.GetBytes(node.ToString());

        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("Content-Type", "application/json");


        WWW req = new WWW(Config.BASE_URL + "/quests/generate", rawdata, headers);

        ContinuationManager.Add(() => req.isDone, () =>
        {
            if (!string.IsNullOrEmpty(req.error)) Debug.Log("WWW failed: " + req.error);
            Debug.Log("WWW result : " + req.text);
        });
    }

    private void HandleCommand(string cmd)
    {
        Content currentInfo = (Content)target;

        if (string.IsNullOrEmpty(cmd))
        {
            return;
        }

        if (WordNumber(0) == "item")
        {
            if (WordNumber(1) == "delete")
            {
                int tempIndex;
                if (int.TryParse(WordNumber(2), out tempIndex))
                {
                    Undo.RecordObject(target, "Remove Item");
                    currentInfo.Items.RemoveAt(tempIndex);
                    return;
                }
                else
                {
                    string tempWord = WordNumber(2);
                    for (int i = 0; i < currentInfo.Items.Count; i++)
                    {
                        if (currentInfo.Items[i].Key == tempWord)
                        {
                            Undo.RecordObject(target, "Remove Item");
                            currentInfo.Items.RemoveAt(i);
                            return;
                        }
                    }
                }
            }
        }
        else if (WordNumber(0) == "monster")
        {
            if (WordNumber(1) == "delete")
            {
                int tempIndex;
                if (int.TryParse(WordNumber(2), out tempIndex))
                {
                    Undo.RecordObject(target, "Remove Monster");
                    currentInfo.Monsters.RemoveAt(tempIndex);
                    return;
                }
                else
                {
                    string tempWord = WordNumber(2);
                    for (int i = 0; i < currentInfo.Monsters.Count; i++)
                    {
                        if (currentInfo.Monsters[i].MonsterKey == tempWord)
                        {
                            Undo.RecordObject(target, "Remove Monster");
                            currentInfo.Monsters.RemoveAt(i);
                            return;
                        }
                    }
                }
            }
            else if (WordNumber(1) == "addLoot")
            {
                string tempWord = WordNumber(2);
                for (int i = 0; i < currentInfo.Monsters.Count; i++)
                {
                    if (currentInfo.Monsters[i].MonsterKey == tempWord)
                    {
                        Undo.RecordObject(target, "Add Monster Loot");
                        currentInfo.Monsters[i].PossibleLoot.Add(new LootInstance(WordNumber(3)));
                        return;
                    }
                }
            }
            else if (WordNumber(1) == "deleteLoot")
            {
                string monster = WordNumber(2);
                string item = WordNumber(3);

                for (int i = 0; i < currentInfo.Monsters.Count; i++)
                {
                    if (currentInfo.Monsters[i].MonsterKey == monster)
                    {
                        Undo.RecordObject(target, "Delete Monster Loot");
                        for (int l = 0; l < currentInfo.Monsters[i].PossibleLoot.Count; l++)
                        {
                            if(currentInfo.Monsters[i].PossibleLoot[l].ItemKey == item)
                            {
                                currentInfo.Monsters[i].PossibleLoot.RemoveAt(l);
                                return;
                            }
                        }

                        return;
                    }
                }

                return;
            }
            else if (WordNumber(1) == "deleteLoot")
            {
                string tempWord = WordNumber(2);
                for (int i = 0; i < currentInfo.Monsters.Count; i++)
                {
                    if (currentInfo.Monsters[i].MonsterKey == tempWord)
                    {
                        for (int a = 0; a < currentInfo.Monsters[i].PossibleLoot.Count; a++)
                        {
                            if (currentInfo.Monsters[i].PossibleLoot[a].ItemKey == WordNumber(3))
                            {
                                Undo.RecordObject(target, "Remove Monster Loot");
                                currentInfo.Monsters[i].PossibleLoot.RemoveAt(a);
                                return;
                            }
                        }
                    }
                }
            }
            else if (WordNumber(1) == "addLootAll")
            {
                Undo.RecordObject(target, "Add All Monsters Loot");

                string tempWord = WordNumber(2);
                for (int i = 0; i < currentInfo.Monsters.Count; i++)
                {
                    currentInfo.Monsters[i].PossibleLoot.Add(new LootInstance(tempWord));
                }
            }
            else if (WordNumber(1) == "deleteLootAll")
            {
                Undo.RecordObject(target, "Delete All Monsters Loot");

                string tempWord = WordNumber(2);
                for (int i = 0; i < currentInfo.Monsters.Count; i++)
                {
                    for (int a = 0; a < currentInfo.Monsters[i].PossibleLoot.Count; a++)
                    {
                        if (currentInfo.Monsters[i].PossibleLoot[a].ItemKey == tempWord)
                        {
                            currentInfo.Monsters[i].PossibleLoot.RemoveAt(a);
                            break;
                        }
                    }
                }
            }
            else if (WordNumber(1) == "addLootMonsterLevel")
            {
                Undo.RecordObject(target, "Add Monsters Loot in monsters above level");

                string tempWord = WordNumber(2);

                int level;
                if (int.TryParse(WordNumber(3), out level))
                {
                    for (int i = 0; i < currentInfo.Monsters.Count; i++)
                    {
                        if (currentInfo.Monsters[i].MonsterLevel > level)
                        {
                            currentInfo.Monsters[i].PossibleLoot.Add(new LootInstance(tempWord));
                        }
                    }
                }
            }
            else if (WordNumber(1) == "deleteLootMonsterLevel")
            {
                Undo.RecordObject(target, "Delete Monsters Loot in monsters below level");

                string tempWord = WordNumber(2); //item

                int level;
                if (int.TryParse(WordNumber(3), out level))
                {
                    for (int i = 0; i < currentInfo.Monsters.Count; i++)
                    {
                        if (currentInfo.Monsters[i].MonsterLevel < level)
                        {
                            for (int a = 0; a < currentInfo.Monsters[i].PossibleLoot.Count; a++)
                            {
                                if (currentInfo.Monsters[i].PossibleLoot[a].ItemKey == tempWord)
                                {
                                    currentInfo.Monsters[i].PossibleLoot.RemoveAt(a);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            else if (WordNumber(1) == "ClearLootDuplicates")
            {
                Undo.RecordObject(target, "Clear Monster Loot Duplicates");

                List<LootInstance> toDelete = new List<LootInstance>();
                bool containsFlag;

                for (int t = 0; t < currentInfo.Items.Count; t++)
                {
                    for (int i = 0; i < currentInfo.Monsters.Count; i++)
                    {
                        toDelete.Clear();
                        containsFlag = false;

                        for (int a = 0; a < currentInfo.Monsters[i].PossibleLoot.Count; a++)
                        {
                            if (currentInfo.Monsters[i].PossibleLoot[a].ItemKey == currentInfo.Items[t].Key)
                            {
                                if (!containsFlag)
                                {
                                    containsFlag = true;
                                }
                                else
                                {
                                    toDelete.Add(currentInfo.Monsters[i].PossibleLoot[a]);
                                }
                            }
                        }

                        for (int b = 0; b < toDelete.Count; b++)
                        {
                            currentInfo.Monsters[i].PossibleLoot.Remove(toDelete[b]);
                        }
                    }
                }
            }
            else if(WordNumber(1) == "cloneLoot")
            {
                Undo.RecordObject(target, "Clone loot from monster to monster");

                DevMonsterInfo monsterFrom = currentInfo.GetMonster(WordNumber(2));
                DevMonsterInfo monsterTo = currentInfo.GetMonster(WordNumber(3));

                foreach(LootInstance loot in monsterFrom.PossibleLoot)
                {
                    monsterTo.PossibleLoot.Add(loot);
                }
            }

        }
        else if (WordNumber(0) == "pa")
        {
            if (WordNumber(1) == "clonePerks")
            {
                Undo.RecordObject(target, "Clone Perks");

                DevAbility paFrom = currentInfo.GetAbility(WordNumber(2));
                DevAbility paTo = currentInfo.GetAbility(WordNumber(3));

                paTo.InitialPerks.Clear();
                paTo.InitialPerks.InsertRange(0, paFrom.InitialPerks);

                paTo.Perks.Clear();
                paTo.Perks.InsertRange(0, paFrom.Perks);

                return;
            }
        }
    }

    private string WordNumber(int number)
    {
        int cnt = 0;
        string tempString = "";
        for(int i=0; i< CurrentCommand.Length; i++)
        {
            if(CurrentCommand[i] == ' ' || i == CurrentCommand.Length - 1)
            {
                if(i == CurrentCommand.Length - 1)
                {
                    tempString += CurrentCommand[i];
                }

                if(number == cnt)
                {
                    return tempString;
                }
                else
                {
                    cnt++;
                }

                tempString = "";
            }
            else
            {
                tempString += CurrentCommand[i];
            }
        }

        return "";
    }
}
