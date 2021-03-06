﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using SimpleJSON;
using BestHTTP;
using System.Text;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(SceneInfo))]
public class SceneInfoEditor : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SceneInfo currentInfo = (SceneInfo)target;

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Update Scene Info"))
        {   
            WriteConstantLists.Instance.WriteScenesPopupList();
            WriteMiniMap(currentInfo);

            JSONNode node = new JSONClass();

            node["scene"]["name"] = currentInfo.Key;
            node["scene"]["nearestTownScene"] = currentInfo.NearestTownScene;
            node["scene"]["pvp"].AsBool = currentInfo.SupportsPVP;
            node["scene"]["canSetMainAbility"].AsBool = currentInfo.CanSetMainAbility;
            node["scene"]["all"] = currentInfo.Key + "," + string.Join(",", SceneInfo.SUPPORTED_SCENES);

            for (int i = 0; i < currentInfo.RoomAbilities.Count; i++)
            {
                node["scene"]["abilities"][i] = currentInfo.RoomAbilities[i];
            }

            for (int i=0;i<currentInfo.ScenePortals.Count;i++)
            {
                node["scene"]["Portals"][i]["key"] = currentInfo.ScenePortals[i].Key;
                node["scene"]["Portals"][i]["targetPortal"] = currentInfo.ScenePortals[i].TargetPortal;
                node["scene"]["Portals"][i]["TargetLevel"] = currentInfo.ScenePortals[i].TargetLevel;
                node["scene"]["Portals"][i]["PositionX"] = currentInfo.ScenePortals[i].transform.position.x.ToString();
                node["scene"]["Portals"][i]["PositionY"] = currentInfo.ScenePortals[i].transform.position.y.ToString();
            }

            for (int i = 0; i < currentInfo.Spawners.Count; i++)
            {
                node["scene"]["Spawners"][i]["MonsterKey"] = currentInfo.Spawners[i].MonsterKey;
                node["scene"]["Spawners"][i]["SpawnCap"] = currentInfo.Spawners[i].SpawnCap.ToString();
                node["scene"]["Spawners"][i]["RespawnTime"] = currentInfo.Spawners[i].RespawnTime.ToString();
                node["scene"]["Spawners"][i]["InitialDelay"] = currentInfo.Spawners[i].InitialDelay.ToString();
                node["scene"]["Spawners"][i]["PositionX"] = currentInfo.Spawners[i].transform.position.x.ToString();
                node["scene"]["Spawners"][i]["PositionY"] = currentInfo.Spawners[i].transform.position.y.ToString();
                node["scene"]["Spawners"][i]["Bulk"] = currentInfo.Spawners[i].bulkEnumState.ToString();
                node["scene"]["Spawners"][i]["SpawnTimes"] = currentInfo.Spawners[i].SpawnTimes.ToString();
            }

            for (int i = 0; i < currentInfo.Npcs.Count; i++)
            {
                node["scene"]["NPC"][i]["npcKey"] = currentInfo.Npcs[i].Key;

                for(int a = 0 ; a < currentInfo.Npcs[i].SellingItems.Count ; a++ )
                {
                    node["scene"]["NPC"][i]["sell"][a]["itemKey"] = currentInfo.Npcs[i].SellingItems[a].itemKey;
                }

                for (int b = 0; b < currentInfo.Npcs[i].GivingQuests.Count; b++)
                {
                    node["scene"]["NPC"][i]["GivingQuests"][b] = currentInfo.Npcs[i].GivingQuests[b];
                }

                for (int c = 0; c < currentInfo.Npcs[i].EndingQuests.Count; c++)
                {
                    node["scene"]["NPC"][i]["EndingQuests"][c] = currentInfo.Npcs[i].EndingQuests[c];
                }

                for (int d = 0; d < currentInfo.Npcs[i].TeleportableScenes.Count; d++)
                {
                    node["scene"]["NPC"][i]["teleportRooms"][d]["room"] = currentInfo.Npcs[i].TeleportableScenes[d].sceneKey;
                    node["scene"]["NPC"][i]["teleportRooms"][d]["portal"] = currentInfo.Npcs[i].TeleportableScenes[d].portalKey;
                    node["scene"]["NPC"][i]["teleportRooms"][d]["party"].AsBool = currentInfo.Npcs[i].TeleportableScenes[d].allowParty;
                }
            }

            SendSceneInfo(node);
        }

        GUILayout.EndHorizontal();

    }

    protected void WriteMiniMap(SceneInfo currentInfo)
    {
        float left = 9999;
        float right = -9999;
        float top = -9999;
        float bottom = 9999;
        List<ColiderGroup> coliders = new List<ColiderGroup>();
        Ferr2DT_PathTerrain[] terrains = Resources.FindObjectsOfTypeAll<Ferr2DT_PathTerrain>();
        if (terrains.Length > 0)
        {
            foreach (Ferr2DT_PathTerrain terrain in terrains) 
            {
                List<List<Vector2>> currentColliders = terrain.GetColliderVerts();
                foreach (List<Vector2> collidersGroup in currentColliders) 
                {
                    ColiderGroup worldColliders = new ColiderGroup();

                    // find the top/bottom/right/left so we can draw things relatively
                    foreach (Vector2 vector in collidersGroup) 
                    {
                        // convert the vector to be relative to the world
                        Vector2 worldVector = terrain.transform.TransformPoint(vector);
                        worldColliders.coliders.Add(worldVector);

                        if (worldVector.x < left)   left = worldVector.x;
                        if (right < worldVector.x)  right = worldVector.x;
                        if (top < worldVector.y)    top = worldVector.y;
                        if (worldVector.y < bottom) bottom = worldVector.y;
                    }

                    if (worldColliders.coliders.Count > 0) 
                    {
                        coliders.Add(worldColliders);
                    }
                    
                }
            }
        }
        else
        {
            BoxCollider2D[] boxColliders = Resources.FindObjectsOfTypeAll<BoxCollider2D>();
            foreach (var collider in boxColliders)
            {
                if (collider.isTrigger)
                {
                    continue;
                }
                ColiderGroup collidersGroup = new ColiderGroup();

                float colliderTop = collider.offset.y + (collider.size.y / 2f);
                float colliderBottom = collider.offset.y - (collider.size.y / 2f);
                float colliderLeft = collider.offset.x - (collider.size.x / 2f);
                float colliderRight = collider.offset.x + (collider.size.x /2f);
                
                Vector3 topLeft = collider.transform.TransformPoint(new Vector3( colliderLeft, colliderTop, 0f));
                Vector3 topRight = collider.transform.TransformPoint(new Vector3( colliderRight, colliderTop, 0f));
                Vector3 bottomLeft = collider.transform.TransformPoint(new Vector3( colliderLeft, colliderBottom, 0f));
                Vector3 bottomRight = collider.transform.TransformPoint(new Vector3( colliderRight, colliderBottom, 0f));

                if (topLeft.x < left)   left = topLeft.x;
                if (right < topRight.x)  right = topRight.x;
                if (top < topLeft.y)    top = topLeft.y;
                if (bottomLeft.y < bottom) bottom = bottomLeft.y;

                collidersGroup.coliders.Add(topLeft);
                collidersGroup.coliders.Add(topRight);
                collidersGroup.coliders.Add(bottomLeft);
                collidersGroup.coliders.Add(bottomRight);

                coliders.Add(collidersGroup);
            }
        }


        currentInfo.miniMapInfo.left = left;
        currentInfo.miniMapInfo.right = right;
        currentInfo.miniMapInfo.top = top;
        currentInfo.miniMapInfo.bottom = bottom;
        currentInfo.miniMapInfo.coliders = coliders;
        EditorSceneManager.SaveScene(EditorSceneManager.GetSceneByName(currentInfo.Key));
    }

    private void SendSceneInfo(JSONNode node)
    {
        Debug.Log("Sending scene info " + node.ToString());

        byte[] rawdata = Encoding.UTF8.GetBytes(node.ToString());

        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("Content-Type", "application/json");
        headers.Add("Cookie", CookiesManager.Instance.GetCookiesString());

        WWW req = new WWW(Config.BASE_URL + "/room/generate" ,rawdata ,headers);


        ContinuationManager.Add(() => req.isDone, () =>
        {
            if (!string.IsNullOrEmpty(req.error)) Debug.Log("WWW failed: " + req.error);
            Debug.Log("WWW result : " + req.text);
        });
    }
}
