using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using SimpleJSON;
using BestHTTP;
using System.Text;

[CustomEditor(typeof(SceneInfo))]
public class SceneInfoEditor : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SceneInfo currentInfo = (SceneInfo)target;

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Update Scene Info"))
        {
            JSONNode node = new JSONClass();

            node["scene"]["name"] = currentInfo.Name;
            
            for(int i=0;i<currentInfo.ScenePortals.Count;i++)
            {
                node["scene"]["Portals"][i]["TargetLevel"] = currentInfo.ScenePortals[i].TargetLevel;
                node["scene"]["Portals"][i]["PositionX"]   = currentInfo.ScenePortals[i].transform.position.x.ToString();
                node["scene"]["Portals"][i]["PositionY"]   = currentInfo.ScenePortals[i].transform.position.y.ToString();
            }

            for (int i = 0; i < currentInfo.Spawners.Count; i++)
            {
                node["scene"]["Spawners"][i]["MonsterKey"]  = currentInfo.Spawners[i].MonsterKey;
                node["scene"]["Spawners"][i]["SpawnCap"]    = currentInfo.Spawners[i].SpawnCap.ToString();
                node["scene"]["Spawners"][i]["RespawnTime"] = currentInfo.Spawners[i].RespawnTime.ToString();
                node["scene"]["Spawners"][i]["PositionX"]   = currentInfo.Spawners[i].transform.position.x.ToString();
                node["scene"]["Spawners"][i]["PositionY"]   = currentInfo.Spawners[i].transform.position.y.ToString();
            }

            SendSceneInfo(node);
        }

        GUILayout.EndHorizontal();

    }

    private void SendSceneInfo(JSONNode node)
    {
        Debug.Log("Sending scene info " + node.ToString());

        byte[] rawdata = Encoding.UTF8.GetBytes(node.ToString());

        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("Content-Type", "application/json");

        WWW req = new WWW("http://www.lul.herokuapp.com/room/generate" ,rawdata ,headers);
    }
}
