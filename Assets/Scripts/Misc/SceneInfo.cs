﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneInfo : MonoBehaviour {

    public string Name;

    public string NearestTownScene;

    public string BGMusic;

    public List<GatePortal> ScenePortals = new List<GatePortal>();

    public List<MonsterSpawner> Spawners = new List<MonsterSpawner>();

    public List<NPC> Npcs = new List<NPC>();

    public MiniMapInfo miniMapInfo = new MiniMapInfo();

    public static SceneInfo Instance;

    void Awake()
    {
        Instance = this;
    }
}