using System.Collections.Generic;

[System.Serializable]
public class DevDungeonContent
{
    public string Key;
    public int MinLvl;
    public int MaxLvl;
    public int TimeLimitInMinutes;
    [Popup(/* AUTO_GENERATED_SCENES_START */ "NO VALUE", "AcolyteCave", "AcolyteChamber", "dungeon_arh_grass_1", "dungeon_arh_grass_2", "dungeon_arh_intro", "extraAssetsScene", "FlowerArena", "FlowerCave", "FlowerGarden", "FrostArena", "FrostPracticeChamber", "GrassLands_Watercliffs", "GrassLands_Watercliffs_Pirates", "GrassLands1", "GrassLands1_2", "GrassLands1_2_5", "GrassLands1_3", "GrassLands2", "GrassLands3", "GrassLands3_2", "GrassLands3_3", "GrassLands4", "GrassLands4_1", "GrassLands4_2", "GrassLands4_3", "GrassLands5", "GrassLands5_1", "GrassLands5_1_1", "GrassLands5_1_2", "GrassLands5_1_3", "GrassLands5_2", "GrassLands5_22", "GrassLands6", "GrassLands6_2", "GrassLands7", "GrassLands8", "HairyHouse", "HealingPracticeChamber", "HugeTree1", "HugeTree2", "HugeTree2_1", "HugeTree3", "HugeTreeBranch_Heaven", "HugeTreeBranch_Owl", "HugeTreeBranch_Squirrels", "IntroScene", "MainMenu", "NutVault", "NutVaultCombat", "PurpleSpa", "PurpleSpaJacuzzi", "PurpleSpaJacuzziFight", "RabbitStorageRoom", "RabbitStorageRoom_1", "RabbitStorageRoom_2", "RabbitTower", "RangerTrainingGround", "RangerTrainingGround2", "RootsCave", "RootsCave_FlawHouse", "RootsCave_Shrine", "ShamanCave", "ShellHill", "ShellHill_Arena", "ShellHillAlley", "ShellHillGranery", "ShellHillHousehold_A", "ShellHillHousehold_B", "ShellHillInn", "ShellHillInnBasement", "ShellHillInnBasement_1", "ShellHillInnRooms", "ShellHillShrine", "ShroomCo", "ShroomCoArchive", "ShroomCoLab", "ShroomCoSafe", "ShroomCoVault", "SquirrelTunnel", "SquirrelTunnel_1", "TestingRoom", "TestingRoom2", "TowerOfPower", "TurtleCave", "WarlockHouse", "WaterCave_Pirates", "WaterCave_Pirates2", "WaterCave1", "WaterCave2", "WeatherVendorBasement", "WormTunnel", "WormTunnel2", "WormTunnelBoss" /* AUTO_GENERATED_SCENES_END */)]
    public string EntranceScene;
    public List<DevDungeonStage> Stages = new List<DevDungeonStage>();
    public List<DevPerkMap> PossiblePerksEmpowers = new List<DevPerkMap>();
    public List<EmpowerCombinations> PossibleEmpowersCombinations = new List<EmpowerCombinations>();
    public List<string> PossibleSpecialEmpowers = new List<string>();

    public bool isReady
    {
        get
        {
            bool Flag = true;

            if(LocalUserInfo.Me.CurrentParty == null)
            {
                return false;
            }

            Flag = !LocalUserInfo.Me.CurrentParty.HasPlayerBelowLVL(MinLvl);

            Flag = !LocalUserInfo.Me.CurrentParty.HasPlayerAboveLVL(MaxLvl);

            return Flag;
        }
        set
        {

        }
    }
}

[System.Serializable]
public class DevDungeonStage
{
    public List<DevDungeonStageScene> PossibleScenes = new List<DevDungeonStageScene>();
    public List<DevDungeonStageScene> PossibleRareScenes = new List<DevDungeonStageScene>();
    public List<DevDungeonReward> Rewards = new List<DevDungeonReward>();
}

[System.Serializable]
public class DevDungeonStageScene
{
    [Popup(/* AUTO_GENERATED_SCENES_START */ "NO VALUE", "AcolyteCave", "AcolyteChamber", "dungeon_arh_grass_1", "dungeon_arh_grass_2", "dungeon_arh_intro", "extraAssetsScene", "FlowerArena", "FlowerCave", "FlowerGarden", "FrostArena", "FrostPracticeChamber", "GrassLands_Watercliffs", "GrassLands_Watercliffs_Pirates", "GrassLands1", "GrassLands1_2", "GrassLands1_2_5", "GrassLands1_3", "GrassLands2", "GrassLands3", "GrassLands3_2", "GrassLands3_3", "GrassLands4", "GrassLands4_1", "GrassLands4_2", "GrassLands4_3", "GrassLands5", "GrassLands5_1", "GrassLands5_1_1", "GrassLands5_1_2", "GrassLands5_1_3", "GrassLands5_2", "GrassLands5_22", "GrassLands6", "GrassLands6_2", "GrassLands7", "GrassLands8", "HairyHouse", "HealingPracticeChamber", "HugeTree1", "HugeTree2", "HugeTree2_1", "HugeTree3", "HugeTreeBranch_Heaven", "HugeTreeBranch_Owl", "HugeTreeBranch_Squirrels", "IntroScene", "MainMenu", "NutVault", "NutVaultCombat", "PurpleSpa", "PurpleSpaJacuzzi", "PurpleSpaJacuzziFight", "RabbitStorageRoom", "RabbitStorageRoom_1", "RabbitStorageRoom_2", "RabbitTower", "RangerTrainingGround", "RangerTrainingGround2", "RootsCave", "RootsCave_FlawHouse", "RootsCave_Shrine", "ShamanCave", "ShellHill", "ShellHill_Arena", "ShellHillAlley", "ShellHillGranery", "ShellHillHousehold_A", "ShellHillHousehold_B", "ShellHillInn", "ShellHillInnBasement", "ShellHillInnBasement_1", "ShellHillInnRooms", "ShellHillShrine", "ShroomCo", "ShroomCoArchive", "ShroomCoLab", "ShroomCoSafe", "ShroomCoVault", "SquirrelTunnel", "SquirrelTunnel_1", "TestingRoom", "TestingRoom2", "TowerOfPower", "TurtleCave", "WarlockHouse", "WaterCave_Pirates", "WaterCave_Pirates2", "WaterCave1", "WaterCave2", "WeatherVendorBasement", "WormTunnel", "WormTunnel2", "WormTunnelBoss" /* AUTO_GENERATED_SCENES_END */)]
    public string Key;
    public float Time;
}

[System.Serializable]
public class DevDungeonReward
{
    [Popup(/* AUTO_GENERATED_LOOT_START */ "NO VALUE", "acolytehood", "acolyterobe", "acorn", "adventurerShirt", "apprenticeRobeBlack", "apprenticeRobeWhite", "archershat", "batWing", "bigFish", "blackbandana", "blackclothshirt", "blackGloves", "blackJellyBean", "blackkercheif", "blackPants", "blackpeasentshirt", "blackShoes", "blueBerries", "blueJellyBean", "bluekercheif", "blueMushroomCap", "bluepeasentshirt", "blueponcho", "brownpeasentshirt", "brownponcho", "cabbage", "captainhat", "captianscoat", "carrot", "carrotSack", "chainlinkhelmet", "charredrobe", "clothPants", "commonaxe", "commonschimitar", "commonsword", "cosmoTunnelKey", "cozyslippers", "cutlass", "dirk", "divinebook", "drainingstaff", "executionerbandana", "Fishing Rod", "forbiddendirk", "gold", "greenGloves", "greenJellyBean", "greenPants", "greenpeasentshirt", "greenponcho", "icystaff", "idoloftrust", "improvedshortbow", "leatherarmor", "leatherarmor", "leatherGloves", "leatherpants", "leatherShoes", "leatherVest", "lightgambesson", "longdagger", "longhammer", "magicCarrotSeeds", "mailarmor", "mailpants", "mailsabatons", "mailshoes", "meltingrod", "metalbow", "metalhelmet", "nutVaultKey", "oldTurtleShell", "orangeclothshirt", "orangeJellyBean", "orbofenergy", "peasantHat", "pinkclothshirt", "pinkJellyBean", "PirateSupply", "plantFlower", "priestrobe", "pyromancerrobe", "rabbitBossEars", "rabbitCostume", "rabbitEar", "rabbitfurshoes", "rabbitFurVest", "rabbitLandEntrancePremission", "redApple", "redbandna", "redBerries", "redGloves", "redJellyBean", "redpeasentshirt", "shortAxe", "shortBow", "shortClub", "shortcutlass", "shortDagger", "shortScimitar", "shortSword", "slipers", "smallFish", "snailhat", "spear", "squirrelBossMustache", "strapShoes", "strawHat", "swordOfElad", "tatteredblackpants", "tatteredbrownpants", "tatteredgreenpants", "tatteredwhitepants", "tauntingfork", "threateningfork", "tomato", "torch", "turtleShell", "turtleshellarmor", "turtleShellOld", "turtleShellSpiked", "turtleSoup", "twohandedaxe", "twohandedschimitar", "twohandedsword", "VilePetal", "wand", "wandoffrost", "warbow", "warlockrobe", "whiteclothshirt", "whiteGloves", "whiteJellyBean", "wizardhood", "wizardrobe", "woodenhammer", "woodenpole", "wormBossAntenna", "yellowbandana", "yellowJellyBean", "yellowkercheif", "yellowshoes" /* AUTO_GENERATED_LOOT_END */)]
    public string ItemKey;
    public int Rarity;
    public int Stack;
}


[System.Serializable]
public class EmpowerCombinations
{
    public List<float> BuffsMultiplyers = new List<float>();
}