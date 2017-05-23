using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneInfo : MonoBehaviour {

    public string Name;

    public string NearestTownScene;

    public List<GatePortal> ScenePortals = new List<GatePortal>();

    public List<MonsterSpawner> Spawners = new List<MonsterSpawner>();

    public List<NPC> Npcs = new List<NPC>();
}
