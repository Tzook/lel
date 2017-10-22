using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using SimpleJSON;
using BestHTTP;
using System.Text;

[CustomEditor(typeof(Content))]
public class DevContentEditor : Editor
{
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
            }

            SendMonstersInfo(node);
        }

        if (GUILayout.Button("Update Items"))
        {
            JSONNode node = new JSONClass();

            node["pass"] = "b0ss123";

            for (int i = 0; i < currentInfo.Items.Count; i++)
            {
                node["items"][i]["key"] = currentInfo.Items[i].Key;
                //node["items"][i]["name"] = currentInfo.Items[i].Name;

                if (currentInfo.Items[i].IconPlaceable != null)
                {
                    currentInfo.Items[i].Icon = currentInfo.Items[i].IconPlaceable.name.ToString();
                }

                node["items"][i]["type"] = currentInfo.Items[i].Type;
                node["items"][i]["subType"] = currentInfo.Items[i].SubType;
                node["items"][i]["dropChance"] = currentInfo.Items[i].DropChance.ToString();
                node["items"][i]["goldValue"] = currentInfo.Items[i].GoldValue.ToString();
                node["items"][i]["stackCap"] = currentInfo.Items[i].StackCap.ToString();

                node["items"][i]["req"]["str"] = currentInfo.Items[i].Stats.RequiresSTR.ToString();
                node["items"][i]["req"]["mag"] = currentInfo.Items[i].Stats.RequiresMAG.ToString();
                node["items"][i]["req"]["dex"] = currentInfo.Items[i].Stats.RequiresDEX.ToString();
                node["items"][i]["req"]["lvl"] = currentInfo.Items[i].Stats.RequiresLVL.ToString();

                node["items"][i]["stats"]["str"] = currentInfo.Items[i].Stats.BonusSTR.ToString();
                node["items"][i]["stats"]["mag"] = currentInfo.Items[i].Stats.BonusMAG.ToString();
                node["items"][i]["stats"]["dex"] = currentInfo.Items[i].Stats.BonusDEX.ToString();
                node["items"][i]["stats"]["hp"] = currentInfo.Items[i].Stats.BonusHP.ToString();
                node["items"][i]["stats"]["mp"] = currentInfo.Items[i].Stats.BonusMP.ToString();


                for (int a = 0; a < currentInfo.Items[i].ItemSprites.Count; a++)
                {
                    if (currentInfo.Items[i].ItemSprites[a].SpritePlaceable != null)
                    {
                        currentInfo.Items[i].ItemSprites[a].Sprite = currentInfo.Items[i].ItemSprites[a].SpritePlaceable.name.ToString();
                    }

                }

                //for (int a = 0; a < currentInfo.Items[i].ItemSprites.Count; a++)
                //{
                //    node["items"][i]["ItemSprites"][a]["partKey"] = currentInfo.Items[i].ItemSprites[a].PartKey;

                //    if (currentInfo.Items[i].ItemSprites[a].SpritePlaceable != null)
                //    {
                //        node["items"][i]["ItemSprites"][a]["sprite"] = currentInfo.Items[i].ItemSprites[a].SpritePlaceable.name.ToString();
                //    }
                //    else
                //    {
                //        node["items"][i]["ItemSprites"][a]["sprite"] = currentInfo.Items[i].ItemSprites[a].Sprite;
                //    }
                //}

            }

            SendItemsInfo(node);
        }

        if (GUILayout.Button("Update Quests"))
        {
            SendQuestsInfo(currentInfo.Quests);
        }

        if (GUILayout.Button("Update Starting Gear"))
        {
            SendStartingGear(currentInfo.StartingGear);
        }

        if (GUILayout.Button("Update Primary Abilities"))
        {
            SendPrimaryAbilities(currentInfo.PrimaryAbilities);
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

    private void SendPrimaryAbilities(List<PrimaryAbility> primaryAbilities)
    {
        JSONNode node = new JSONClass();

        node["pass"] = "b0ss123";

        for (int i = 0; i < primaryAbilities.Count; i++)
        {
            node["primaryAbilities"][i]["key"] = primaryAbilities[i].Name;
        }

        Debug.Log("Sending Primary Abilities...");

        byte[] rawdata = Encoding.UTF8.GetBytes(node.ToString());

        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("Content-Type", "application/json");


        WWW req = new WWW(Config.BASE_URL + "/primaryAbilities", rawdata, headers);

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

            node["quests"][i]["requiredClass"] = Quests[i].RequiredClass;

            node["quests"][i]["minLevel"] = Quests[i].MinimumLevel.ToString();


            for (int ri = 0; ri < Quests[i].RewardItems.Count; ri++)
            {
                node["quests"][i]["rewardItems"][ri]["key"] = Quests[i].RewardItems[ri].ItemKey;
                node["quests"][i]["rewardItems"][ri]["stack"] = Quests[i].RewardItems[ri].MinStack.ToString();
            }

            node["quests"][i]["rewardClass"] = Quests[i].RewardClass;

            node["quests"][i]["rewardSTR"] = Quests[i].RewardSTR.ToString();
            node["quests"][i]["rewardMAG"] = Quests[i].RewardMAG.ToString();
            node["quests"][i]["rewardDEX"] = Quests[i].RewardDEX.ToString();
            node["quests"][i]["rewardHP"] = Quests[i].RewardHP.ToString();
            node["quests"][i]["rewardMP"] = Quests[i].RewardMP.ToString();
            node["quests"][i]["rewardPrimaryAbility"] = Quests[i].RewardPrimaryAbility.Name;

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
}
