using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneInfo : MonoBehaviour {

    public string Name;

    public string NearestTownScene;

    public string BGMusic;

    [Popup(/* AUTO_GENERATED_ABILITIES_START */ "", "charTalent", "frost", "heal", "melee", "range" /* AUTO_GENERATED_ABILITIES_END */)]    
    public List<string> RoomAbilities = new List<string>();

    public List<GatePortal> ScenePortals = new List<GatePortal>();

    public List<MonsterSpawner> Spawners = new List<MonsterSpawner>();

    public List<NPC> Npcs = new List<NPC>();

    public MiniMapInfo miniMapInfo = new MiniMapInfo();

    public static SceneInfo Instance;

    public List<NetStateEntity> NetStateEntities = new List<NetStateEntity>();

    public void UpdateStateChange(string entityKey, string stateValue)
    {
        for(int i=0;i<NetStateEntities.Count;i++)
        {
            if(NetStateEntities[i].Key == entityKey)
            {
                NetStateEntities[i].OnNetStateChange(stateValue);
                return;
            }
        }
    }

    void Awake()
    {
        Instance = this;
    }
}