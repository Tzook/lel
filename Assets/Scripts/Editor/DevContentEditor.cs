/* 
 * Items:
 * item delete 'ItemKey / index'
 * item modifyDrop 'Times'
 * item balanceDropTargets
 * 
 * Monsters:
 * monster clone 'MonsterKey'
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
 * 
 * Quests:
 * quest delete 'QuestKey'
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using SimpleJSON;
using BestHTTP;
using System.Text;
using UnityEditor.SceneManagement;
using System;

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
            SendMonstersInfo(currentInfo.Monsters, currentInfo.Multiplyers);
            WriteConstantLists.Instance.WriteMobsPopupList(currentInfo.Monsters);
        }

        if (GUILayout.Button("Update Items"))
        {
            SendItemsInfo(currentInfo.Items);
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
            WriteConstantLists.Instance.WriteBuffsPopupList(currentInfo.Buffs);
            WriteConstantLists.Instance.WriteAbilitiesPopupList(currentInfo.Abilities);
        }

        if (GUILayout.Button("Update Dungeons"))
        {
            SendDungeons(currentInfo.Dungeons);
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

    private void SendMonstersInfo(List<DevMonsterInfo> Monsters, ContentMultiplyers Multiplyers)
    {
        JSONNode node = new JSONClass();        
        
        for (int i = 0; i < Monsters.Count; i++)
        {
            node["mobs"][i]["key"] = Monsters[i].MonsterKey;
            node["mobs"][i]["name"] = Monsters[i].MonsterName;
            node["mobs"][i]["hp"] = Mathf.FloorToInt(Monsters[i].MonsterHP * Multiplyers.mobsHp).ToString();
            node["mobs"][i]["level"] = Monsters[i].MonsterLevel.ToString();
            node["mobs"][i]["dmg"] = Mathf.FloorToInt(Monsters[i].DMG * Multiplyers.mobsDmg).ToString();
            node["mobs"][i]["exp"] = Mathf.FloorToInt(Monsters[i].RewardEXP * Multiplyers.mobsExp).ToString();

            for (int a = 0; a < Monsters[i].PossibleLoot.Count; a++)
            {
                node["mobs"][i]["drops"][a]["key"] = Monsters[i].PossibleLoot[a].ItemKey;
                node["mobs"][i]["drops"][a]["minStack"] = Monsters[i].PossibleLoot[a].MinStack.ToString();
                node["mobs"][i]["drops"][a]["maxStack"] = Monsters[i].PossibleLoot[a].MaxStack.ToString();
            }

            for (int a = 0; a < Monsters[i].Perks.Count; a++)
            {
                node["mobs"][i]["perks"][a]["key"] = Monsters[i].Perks[a].Key.ToString();
                node["mobs"][i]["perks"][a]["value"] = Monsters[i].Perks[a].Value.ToString();
            }

            for (int a = 0; a < Monsters[i].Spells.SpellsList.Count; a++)
            {
                node["mobs"][i]["spells"][a]["chance"] = Monsters[i].Spells.SpellsList[a].Chance.ToString();
                FillMonsterSpellInfo(Monsters[i].Spells.SpellsList[a], node["mobs"][i]["spells"][a]);
            }
            if (!String.IsNullOrEmpty(Monsters[i].Spells.DeathRattle.Key))
            {
                node["mobs"][i]["deathRattle"]["duration"] = Monsters[i].Spells.DeathRattle.Duration.ToString();
                FillMonsterSpellInfo(Monsters[i].Spells.DeathRattle, node["mobs"][i]["deathRattle"]);
            }

            node["mobs"][i]["spellsMinTime"] = Monsters[i].Spells.MinTime.ToString();
            node["mobs"][i]["spellsMaxTime"] = Monsters[i].Spells.MaxTime.ToString();
        }

        UpdateContent(node, "/mob/generate");
    }

    private static void FillMonsterSpellInfo(DevMobSpellBase devSpell, JSONNode node)
    {
        node["key"] = devSpell.Key.ToString();

        for (int b = 0; b < devSpell.Perks.Count; b++)
        {
            node["perks"][b]["key"] = devSpell.Perks[b].Key.ToString();
            node["perks"][b]["value"] = devSpell.Perks[b].Value.ToString();
        }

        for (int b = 0; b < devSpell.SpawnMobs.Length; b++)
        {
            node["spawnMobs"][b] = devSpell.SpawnMobs[b];
        }

        for (int b = 0; b < devSpell.HitIfTargetHasBuff.Count; b++)
        {
            node["hitIfTargetHasBuff"][b] = devSpell.HitIfTargetHasBuff[b];
        }

        for (int b = 0; b < devSpell.ClearTargetBuffs.Count; b++)
        {
            node["clearTargetBuffs"][b] = devSpell.ClearTargetBuffs[b];
        }
    }

    private void SendItemsInfo(List<DevItemInfo> Items)
    {
        JSONNode node = new JSONClass();

        for (int i = 0; i < Items.Count; i++)
        {
            node["items"][i]["key"] = Items[i].Key;

            if (Items[i].IconPlaceable != null)
            {
                Items[i].Icon = Items[i].IconPlaceable.name.ToString();
            }

            node["items"][i]["type"] = Items[i].Type;
            node["items"][i]["subType"] = Items[i].SubType;
            node["items"][i]["dropChance"] = Items[i].DropChance.ToString();
            node["items"][i]["goldValue"] = Items[i].GoldValue.ToString();
            node["items"][i]["stackCap"] = Items[i].StackCap.ToString();

            node["items"][i]["req"]["lvl"] = Items[i].Stats.RequiresLVL.ToString();

            node["items"][i]["use"]["hp"] = Items[i].UseInfo.BonusHP.ToString();
            node["items"][i]["use"]["mp"] = Items[i].UseInfo.BonusMP.ToString();

            for (int a = 0; a < Items[i].Perks.Count; a++)
            {
                node["items"][i]["perks"][a]["key"] = Items[i].Perks[a].Key.ToString();
                node["items"][i]["perks"][a]["value"] = Items[i].Perks[a].Value.ToString();
            }

            for (int a = 0; a < Items[i].ItemSprites.Count; a++)
            {
                if (Items[i].ItemSprites[a].SpritePlaceable != null)
                {
                    Items[i].ItemSprites[a].Sprite = Items[i].ItemSprites[a].SpritePlaceable.name.ToString();
                }

            }

            node["items"][i]["minLvlMobs"] = Items[i].AppearsAt.MinLvlMobs.ToString();
            node["items"][i]["maxLvlMobs"] = Items[i].AppearsAt.MaxLvlMobs.ToString();
            node["items"][i]["maxMobsStack"] = Items[i].AppearsAt.MaxStack.ToString();
            node["items"][i]["minMobsStack"] = Items[i].AppearsAt.MinStack.ToString();
        }

        UpdateContent(node, "/items/generate");
    }

    private void RestartServer()
    {
        JSONNode node = new JSONClass();

        UpdateContent(node, "/restart");
    }

    private void SendStartingGear(List<string> GearList)
    {
        JSONNode node = new JSONClass();

        for (int i = 0; i < GearList.Count; i++)
        {
            node["equips"][i]["key"] = GearList[i];
        }

        UpdateContent(node, "/equips/begin");
    }

    private void SendAbilities(List<DevAbility> abilities, List<DevPAPerk> perks)
    {
        JSONNode node = new JSONClass();

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
                node["talents"][i]["spells"][a]["cooldown"] = abilities[i].Spells[a].Cooldown.ToString();
                
                for (int b = 0; b < abilities[i].Spells[a].Perks.Count; b++)
                {
                    node["talents"][i]["spells"][a]["perks"][b]["key"] = abilities[i].Spells[a].Perks[b].Key.ToString();
                    node["talents"][i]["spells"][a]["perks"][b]["value"] = abilities[i].Spells[a].Perks[b].Value.ToString();
                }
                
                for (int b = 0; b < abilities[i].Spells[a].HitIfTargetHasBuff.Count; b++)
                {
                    node["talents"][i]["spells"][a]["hitIfTargetHasBuff"][b] = abilities[i].Spells[a].HitIfTargetHasBuff[b].ToString();
                }
                
                for (int b = 0; b < abilities[i].Spells[a].ClearTargetBuffs.Count; b++)
                {
                    node["talents"][i]["spells"][a]["clearTargetBuffs"][b] = abilities[i].Spells[a].ClearTargetBuffs[b].ToString();
                }
            }

            for (int a = 0; a < abilities[i].InitialPerks.Count; a++)
            {
                node["talents"][i]["initialPerks"][a]["key"] = abilities[i].InitialPerks[a].Key.ToString();
                node["talents"][i]["initialPerks"][a]["value"] = abilities[i].InitialPerks[a].Level.ToString();
            }

            node["talents"][i]["hitType"] = abilities[i].HitType;
            node["talents"][i]["powerType"] = abilities[i].PowerType;
            node["talents"][i]["manaCost"] = abilities[i].ManaCost.ToString();
        }

        for (int i=0;i<perks.Count;i++)
        {
            node["perkCollection"][i]["key"] = perks[i].Key;
            node["perkCollection"][i]["value"] = perks[i].PrecentPerUpgrade.ToString();
            node["perkCollection"][i]["max"] = perks[i].UpgradeCap.ToString();
            node["perkCollection"][i]["default"] = perks[i].StartingValue.ToString();
            node["perkCollection"][i]["client"].AsBool = perks[i].IsClient;
            node["perkCollection"][i]["type"] = perks[i].Type;
            
            for (int a = 0; a < perks[i].Buff.BonusPerks.Count; a++)
            {
                node["perkCollection"][i]["bonusPerks"][a]["key"] = perks[i].Buff.BonusPerks[a].Key.ToString();
                node["perkCollection"][i]["bonusPerks"][a]["value"] = perks[i].Buff.BonusPerks[a].Value.ToString();
            }
            node["perkCollection"][i]["party"].AsBool = perks[i].Buff.PartyBuff;
            
            node["perkCollection"][i]["acc"] = "0";//perks[i].PrecentAccelerationPerUpgrade.ToString();
        }

        UpdateContent(node, "/talents/generate");
    }

    private void SendQuestsInfo(List<Quest> Quests)
    {
        JSONNode node = new JSONClass();

        for (int i = 0; i < Quests.Count; i++)
        {
            node["quests"][i]["key"] = Quests[i].Key;

            for (int c = 0; c < Quests[i].Conditions.Count; c++)
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

            node["quests"][i]["rewardHP"] = Quests[i].RewardHP.ToString();
            node["quests"][i]["rewardMP"] = Quests[i].RewardMP.ToString();
            node["quests"][i]["rewardPrimaryAbility"] = Quests[i].RewardPrimaryAbility;

            node["quests"][i]["rewardExp"] = Quests[i].RewardExp.ToString();

        }

        UpdateContent(node, "/quests/generate");
    }

    private void SendDungeons(List<DevDungeonContent> dungeons)
    {
        JSONNode node = new JSONClass();

        for (int i = 0; i < dungeons.Count; i++)
        {
            node["dungeons"][i]["key"] = dungeons[i].Key;
            node["dungeons"][i]["minLvl"] = dungeons[i].MinLvl.ToString();
            node["dungeons"][i]["maxLvl"] = dungeons[i].MaxLvl.ToString();
            node["dungeons"][i]["time"] = dungeons[i].TimeLimitInMinutes.ToString();
            node["dungeons"][i]["beginRoom"] = dungeons[i].EntranceScene;

            for (int a = 0; a < dungeons[i].Stages.Count; a++)
            {
                for (int b = 0; b < dungeons[i].Stages[a].PossibleScenes.Count; b++)
                {
                    node["dungeons"][i]["stages"][a]["rooms"][b]["key"] = dungeons[i].Stages[a].PossibleScenes[b].Key;
                    node["dungeons"][i]["stages"][a]["rooms"][b]["time"] = dungeons[i].Stages[a].PossibleScenes[b].Time.ToString();
                }
                
                for (int b = 0; b < dungeons[i].Stages[a].PossibleRareScenes.Count; b++)
                {
                    node["dungeons"][i]["stages"][a]["rareRooms"][b]["key"] = dungeons[i].Stages[a].PossibleRareScenes[b].Key;
                    node["dungeons"][i]["stages"][a]["rareRooms"][b]["time"] = dungeons[i].Stages[a].PossibleRareScenes[b].Time.ToString();
                }

                for (int b = 0; b < dungeons[i].Stages[a].Rewards.Count; b++)
                {
                    node["dungeons"][i]["stages"][a]["rewards"][b]["key"] = dungeons[i].Stages[a].Rewards[b].ItemKey;
                    node["dungeons"][i]["stages"][a]["rewards"][b]["rarity"] = dungeons[i].Stages[a].Rewards[b].Rarity.ToString();
                    node["dungeons"][i]["stages"][a]["rewards"][b]["stack"] = dungeons[i].Stages[a].Rewards[b].Stack.ToString();
                }
            }

            for (int a = 0; a < dungeons[i].PossiblePerksEmpowers.Count; a++)
            {
                node["dungeons"][i]["perksPool"][a]["key"] = dungeons[i].PossiblePerksEmpowers[a].Key;
                node["dungeons"][i]["perksPool"][a]["value"] = dungeons[i].PossiblePerksEmpowers[a].Value.ToString();
            }

            for (int a = 0; a < dungeons[i].PossibleEmpowersCombinations.Count; a++)
            {
                for (int b = 0; b < dungeons[i].PossibleEmpowersCombinations[a].BuffsMultiplyers.Count; b++)
                {
                    float val = dungeons[i].PossibleEmpowersCombinations[a].BuffsMultiplyers[b];
                    node["dungeons"][i]["perksCombinations"][a]["multiplyers"][b] = val.ToString();
                }
            }

            for (int a = 0; a < dungeons[i].PossibleSpecialEmpowers.Count; a++)
            {
                node["dungeons"][i]["rareBonuses"][a] = dungeons[i].PossibleSpecialEmpowers[a];
            }
        }

        UpdateContent(node, "/dungeons/generate");
    }

    private void UpdateContent(JSONNode node, string path)
    {
        Debug.Log("Updating: " + path);
        byte[] rawdata = Encoding.UTF8.GetBytes(node.ToString());

        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("Content-Type", "application/json");
        headers.Add("Cookie", CookiesManager.Instance.GetCookiesString());

        WWW req = new WWW(Config.BASE_URL + path, rawdata, headers);

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
            else if (WordNumber(1) == "modifyDrop")
            {
                Undo.RecordObject(target, "Times modifyDrop");
                for (int i = 0; i < currentInfo.Items.Count; i++)
                {
                    if (currentInfo.Items[i].DropChance < 0.5f)
                    {
                        currentInfo.Items[i].DropChance *= float.Parse(WordNumber(2));
                    }
                }

            }
            else if (WordNumber(1) == "balanceDropTargets")
            {
                Undo.RecordObject(target, "Times modifyDrop");
                for (int i = 0; i < currentInfo.Items.Count; i++)
                {
                    if (currentInfo.Items[i].DropChance < 0.5f && currentInfo.Items[i].AppearsAt.MinLvlMobs > 0 && currentInfo.Items[i].AppearsAt.MaxLvlMobs > 0)
                    {
                        currentInfo.Items[i].AppearsAt.MaxLvlMobs = currentInfo.Items[i].Stats.RequiresLVL + 2;
                        currentInfo.Items[i].AppearsAt.MinLvlMobs = currentInfo.Items[i].Stats.RequiresLVL - 2;
                    }
                }
            }
        }
        else if (WordNumber(0) == "monster")
        {
            if (WordNumber(1) == "clone")
            {
                Undo.RecordObject(target, "Clone Monster");
                currentInfo.Monsters.Add(currentInfo.GetMonster(WordNumber(2)).Clone());
                return;
            }
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
                            if (currentInfo.Monsters[i].PossibleLoot[l].ItemKey == item)
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
            else if (WordNumber(1) == "cloneLoot")
            {
                Undo.RecordObject(target, "Clone loot from monster to monster");

                DevMonsterInfo monsterFrom = currentInfo.GetMonster(WordNumber(2));
                DevMonsterInfo monsterTo = currentInfo.GetMonster(WordNumber(3));

                foreach (LootInstance loot in monsterFrom.PossibleLoot)
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
        else if (WordNumber(0) == "quest")
        {
            if (WordNumber(1) == "delete")
            {
                int tempIndex;
                if (int.TryParse(WordNumber(2), out tempIndex))
                {
                    Undo.RecordObject(target, "Remove Quest");
                    currentInfo.Quests.RemoveAt(tempIndex);
                    return;
                }
                else
                {
                    string tempWord = WordNumber(2);
                    for (int i = 0; i < currentInfo.Quests.Count; i++)
                    {
                        if (currentInfo.Quests[i].Key == tempWord)
                        {
                            Undo.RecordObject(target, "Remove Quest");
                            currentInfo.Quests.RemoveAt(i);
                            return;
                        }
                    }
                }
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
