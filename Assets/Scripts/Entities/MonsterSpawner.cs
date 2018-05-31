using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour {

    [Popup(/* AUTO_GENERATED_MOBS_START */ "NO VALUE", "Bat", "BerriesBush", "BlueBerriesBush", "BlueMushroom", "BossTurtle", "BuffedSquirrel", "BuffedSquirrel_VaultKeeper", "FatRabbit", "FlowerManBoss", "FrostWizardBoss", "GiantBat", "GreenWorm", "OldTurtle", "PirateSailor1", "PirateSailor10", "PirateSailor2", "PirateSailor3", "PirateSailor4", "PirateSailor5", "PirateSailor6", "PirateSailor7", "PirateSailor8", "PirateSailor9", "Plant", "Rabbit", "RabbitBoss", "RedWorm", "Sack", "SmallSquirrel", "Spike", "SpikedTurtle", "Squirrel", "SquirrelBoss", "Thorns", "TomatoesBush", "Turtle", "VilePlant", "Vulture", "VultureRabbit", "Worm", "WormBoss" /* AUTO_GENERATED_MOBS_END */)]
    public string MonsterKey;

    public int SpawnCap;

    public float RespawnTime;
    
    [SerializeField]
    public BulkEnumState bulkEnumState;
    public enum BulkEnumState
    {
        None, Wave, Asap
    }
    public int SpawnTimes;
}
