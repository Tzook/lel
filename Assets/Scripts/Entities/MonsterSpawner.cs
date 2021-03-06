using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour {

    [Popup(/* AUTO_GENERATED_MOBS_START */ "NO VALUE", "Bat", "BerriesBush", "BigTurtle", "BlueBerriesBush", "BlueMushroom", "BossTurtle", "BuffedSquirrel", "BuffedSquirrel_VaultKeeper", "FatRabbit", "FlowerManBandit", "FlowerManBoss", "FrostWizardBoss", "GiantBat", "GreenWorm", "Mole", "OldTurtle", "PirateSailor1", "PirateSailor10", "PirateSailor2", "PirateSailor3", "PirateSailor4", "PirateSailor5", "PirateSailor6", "PirateSailor7", "PirateSailor8", "PirateSailor9", "Plant", "Rabbit", "RabbitBoss", "RedWorm", "Sack", "SmallSquirrel", "Spike", "SpikedTurtle", "Squirrel", "SquirrelBoss", "StoneBlock", "StoneBrick", "StoneGolem", "Thorns", "TomatoesBush", "Turtle", "VilePlant", "Vulture", "VultureRabbit", "Worm", "WormBoss" /* AUTO_GENERATED_MOBS_END */)]
    public string MonsterKey;

    public int SpawnCap;

    public float RespawnTime;
    public float InitialDelay;
    
    [SerializeField]
    public BulkEnumState bulkEnumState;
    public enum BulkEnumState
    {
        None, Wave, Asap
    }
    public int SpawnTimes;
}
