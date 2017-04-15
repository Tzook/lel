using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using SimpleJSON;
using BestHTTP;
using System.Text;

[CustomEditor(typeof(DevContent))]
public class DevContentEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DevContent currentInfo = (DevContent)target;

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Update Monsters"))
        {
            JSONNode node = new JSONClass();

            node["pass"] = "b0ss123";

            for (int i = 0; i < currentInfo.Monsters.Count; i++)
            {
                node["mobs"][i]["key"]      = currentInfo.Monsters[i].MonsterKey;
                node["mobs"][i]["name"]     = currentInfo.Monsters[i].MonsterName;
                node["mobs"][i]["hp"]       = currentInfo.Monsters[i].MonsterHP.ToString();
                node["mobs"][i]["level"]    = currentInfo.Monsters[i].MonsterLevel.ToString();
                node["mobs"][i]["minDMG"]   = currentInfo.Monsters[i].MinDMG.ToString();
                node["mobs"][i]["maxDMG"]   = currentInfo.Monsters[i].MaxDMG.ToString();
                node["mobs"][i]["MinGoldDrop"] = currentInfo.Monsters[i].MinGoldDrop.ToString();
                node["mobs"][i]["MaxGoldDrop"] = currentInfo.Monsters[i].MaxGoldDrop.ToString();

                for(int a=0;a<currentInfo.Monsters[i].PossibleLoot.Count;a++)
                {
                    node["mobs"][i]["PossibleLoot"][a] = currentInfo.Monsters[i].PossibleLoot[a];
                }
            }

            SendMonstersInfo(node);
        }
        else if (GUILayout.Button("Update Items"))
        {
            JSONNode node = new JSONClass();

            node["pass"] = "b0ss123";

            for (int i = 0; i < currentInfo.Items.Count; i++)
            {
                node["items"][i]["name"] = currentInfo.Items[i].Name;
                node["items"][i]["icon"] = currentInfo.Items[i].Icon;
                node["items"][i]["type"] = currentInfo.Items[i].Type;
                node["items"][i]["dropChance"] = currentInfo.Items[i].DropChance.ToString();
                node["items"][i]["goldValue"] = currentInfo.Items[i].GoldValue.ToString();

                for (int a = 0; a < currentInfo.Items[i].ItemSprites.Count; a++)
                {
                    node["items"][i]["ItemSprites"][a]["partKey"] = currentInfo.Items[i].ItemSprites[a].PartKey;
                    node["items"][i]["ItemSprites"][a]["sprite"]  = currentInfo.Items[i].ItemSprites[a].Sprite;
                }
            }

            SendItemsInfo(node);
        }

        GUILayout.EndHorizontal();

    }

    private void SendMonstersInfo(JSONNode node)
    {
        Debug.Log("Sending monsters info " + node.ToString());

        byte[] rawdata = Encoding.UTF8.GetBytes(node.ToString());

        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("Content-Type", "application/json");

        WWW req = new WWW("http://lul.herokuapp.com/mob/generate", rawdata, headers);


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

        //TODO Replace with valid URL
        WWW req = new WWW("http://lul.herokuapp.com/items/generate", rawdata, headers);


        ContinuationManager.Add(() => req.isDone, () =>
        {
            if (!string.IsNullOrEmpty(req.error)) Debug.Log("WWW failed: " + req.error);
            Debug.Log("WWW result : " + req.text);
        });
    }

}
