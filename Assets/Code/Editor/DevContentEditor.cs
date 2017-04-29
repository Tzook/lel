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
                node["items"][i]["dropChance"] = currentInfo.Items[i].DropChance.ToString();
                node["items"][i]["goldValue"] = currentInfo.Items[i].GoldValue.ToString();
                node["items"][i]["stackCap"] = currentInfo.Items[i].StackCap.ToString();

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
}
