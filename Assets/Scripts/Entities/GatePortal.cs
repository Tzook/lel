using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GatePortal : MonoBehaviour {

    public string Key;
    public string TargetPortal;
    [Popup(/* AUTO_GENERATED_SCENES_START */ "NO VALUE", "AcolyteCave", "AcolyteChamber", "dungeon_arh_grass_1", "dungeon_arh_grass_2", "dungeon_arh_intro", "extraAssetsScene", "FlowerArena", "FlowerCave", "FlowerGarden", "FrostArena", "FrostPracticeChamber", "GrassLands_Watercliffs", "GrassLands_Watercliffs_Pirates", "GrassLands1", "GrassLands1_2", "GrassLands1_2_5", "GrassLands1_3", "GrassLands2", "GrassLands3", "GrassLands3_2", "GrassLands3_3", "GrassLands4", "GrassLands4_1", "GrassLands4_2", "GrassLands4_3", "GrassLands5", "GrassLands5_1", "GrassLands5_1_1", "GrassLands5_1_2", "GrassLands5_1_3", "GrassLands5_2", "GrassLands5_22", "GrassLands6", "GrassLands6_2", "GrassLands7", "GrassLands8", "HairyHouse", "HealingPracticeChamber", "HugeTree1", "HugeTree2", "HugeTree2_1", "HugeTree3", "HugeTreeBranch_Heaven", "HugeTreeBranch_Owl", "HugeTreeBranch_Squirrels", "IntroScene", "MainMenu", "NutVault", "NutVaultCombat", "PurpleSpa", "PurpleSpaJacuzzi", "PurpleSpaJacuzziFight", "RabbitStorageRoom", "RabbitStorageRoom_1", "RabbitStorageRoom_2", "RabbitTower", "RangerTrainingGround", "RangerTrainingGround2", "RootsCave", "RootsCave_FlawHouse", "RootsCave_Shrine", "ShamanCave", "ShellHill", "ShellHill_Arena", "ShellHillAlley", "ShellHillGranery", "ShellHillHousehold_A", "ShellHillHousehold_B", "ShellHillInn", "ShellHillInnBasement", "ShellHillInnBasement_1", "ShellHillInnRooms", "ShellHillShrine", "ShroomCo", "ShroomCoArchive", "ShroomCoLab", "ShroomCoSafe", "ShroomCoVault", "SquirrelTunnel", "SquirrelTunnel_1", "TestingRoom", "TestingRoom2", "TowerOfPower", "TurtleCave", "WarlockHouse", "WaterCave_Pirates", "WaterCave_Pirates2", "WaterCave1", "WaterCave2", "WeatherVendorBasement", "WormTunnel", "WormTunnel2", "WormTunnelBoss" /* AUTO_GENERATED_SCENES_END */)]    
    public string TargetLevel;

    [SerializeField]
    bool StartShut = false;

    public List<string> RequiresItems = new List<string>();

    void Start()
    {
        StartCoroutine(Initialize());
    }

    private IEnumerator Initialize()
    {
        while(Game.Instance.CurrentScene==null)
        {
            yield return 0;
        }

        Game.Instance.CurrentScene.AddScenePortal(this); 

        if(StartShut)
        {
            this.gameObject.SetActive(false);
        }
    }
}
