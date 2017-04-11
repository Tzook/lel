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

        if (GUILayout.Button("Update Scene Info"))
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
            }

            SendMonstersInfo(node);
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

}
