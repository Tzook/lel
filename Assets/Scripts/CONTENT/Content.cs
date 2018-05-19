using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public class Content : MonoBehaviour {

    public static Content Instance;

    void Awake()
    {
        Instance = this;
    }

    public List<DevMonsterInfo> Monsters = new List<DevMonsterInfo>();

    public ContentMultiplyers Multiplyers = new ContentMultiplyers();


    public List<DevItemInfo> Items = new List<DevItemInfo>();

    public List<ContentPiece> InfoBank = new List<ContentPiece>();

    public List<DevAbility> Abilities = new List<DevAbility>();

    public List<DevPAPerk> Perks = new List<DevPAPerk>();

    public List<DevBuff> Buffs = new List<DevBuff>();

    public ContentPiece GetInfo(string Key)
    {
        for (int i = 0; i < InfoBank.Count; i++)
        {
            if (InfoBank[i].Title == Key)
            {
                return InfoBank[i];
            }
        }

        return null;
    }

    public DevAbility GetAbility(string key)
    {
        for (int i = 0; i < Abilities.Count; i++)
        {
            if (Abilities[i].Key == key)
            {
                return Abilities[i];
            }
        }

        return null;
    }

    public int GetAbilityIndex(string key)
    {
        for (int i = 0; i < Abilities.Count; i++)
        {
            if (Abilities[i].Key == key)
            {
                return i;
            }
        }

        return 0;
    }

    public DevItemInfo GetItem(string itemKey)
    {
        for(int i=0;i<Items.Count;i++)
        {
            if(Items[i].Key == itemKey)
            {
                return Items[i];
            }
        }

        return null;
    }

    public DevMonsterInfo GetMonster(string monsterKey)
    {
        for (int i = 0; i < Monsters.Count; i++)
        {
            if (Monsters[i].MonsterKey == monsterKey)
            {
                return Monsters[i];
            }
        }

        return null;
    }

    public DevMonsterInfo GetMonsterByName(string monsterName)
    {
        for (int i = 0; i < Monsters.Count; i++)
        {
            if (Monsters[i].MonsterName == monsterName)
            {
                return Monsters[i];
            }
        }

        return null;
    }

    public Texture2D DefaultCursor;
    public Texture2D ClickableCursor;
    public Texture2D CrosshairCursor;

    public List<string> StartingGear = new List<string>();

    public List<Quest> Quests = new List<Quest>();

    public Quest GetQuest(string questKey)
    {
        for(int i=0;i<Quests.Count;i++)
        {
            if(Quests[i].Key == questKey)
            {
                return Quests[i];
            }
        }

        return null;
    }

    public DevPAPerk GetPerk(string Key)
    {
        for(int i=0;i<Perks.Count;i++)
        {
            if(Perks[i].Key == Key)
            {
                return Perks[i];
            }
        }

        return null;
    }

    public bool IsInitialPerkValue(DevAbility ability, PAPerk perk)
    {
        for(int i = 0; i < ability.InitialPerks.Count;i++)
        {
            if(ability.InitialPerks[i].Key == perk.Key)
            {
                return ability.InitialPerks[i].Value == perk.Points;
            }
        }
        return false;
    }

    public DevBuff GetBuff(string buffKey)
    {
        for (int i = 0; i < Buffs.Count; i++)
        {
            if (Buffs[i].Key == buffKey)
            {
                return Buffs[i];
            }
        }

        return null;
    }

    public DevSpell GetPlayerSpell(string spellKey)
    {
        for(int i=0; i < Abilities.Count; i++)
        {
            for(int a=0; a < Abilities[i].Spells.Count; a++)
            {
                if(Abilities[i].Spells[a].Key == spellKey)
                {
                    return Abilities[i].Spells[a];
                }
            }
        }

        return null;
    }

    public DevMobSpellBase GetMobSpell(string spellKey)
    {
        for (int i = 0; i < Monsters.Count; i++)
        {
            for (int a = 0; a < Monsters[i].Spells.SpellsList.Count; a++)
            {
                if (Monsters[i].Spells.SpellsList[a].Key == spellKey)
                {
                    return Monsters[i].Spells.SpellsList[a];
                }
            }

            if (Monsters[i].Spells.DeathRattle.Key == spellKey)
            {
                return Monsters[i].Spells.DeathRattle;
            }
        }

        return null;
    }

    public DevSpell GetSpellAtLevel(int lvl)
    {
        for (int i = 0; i < Abilities.Count; i++)
        {
            for (int a = 0; a < Abilities[i].Spells.Count; a++)
            {
                if (Abilities[i].Spells[a].Level == lvl)
                {
                    return Abilities[i].Spells[a];
                }
            }
        }

        return null;
    }

    public DevSpell GetSpellAtIndex(int iIndex)
    {
        DevAbility tempPA = GetAbility(LocalUserInfo.Me.ClientCharacter.CurrentPrimaryAbility.Key);

        if (iIndex < tempPA.Spells.Count)
        {
            return tempPA.Spells[iIndex];
        }

        return null;
    }



    public int GetSpellIndex(DevSpell spell)
    {
        for (int i = 0; i < Abilities.Count; i++)
        {
            for (int a = 0; a < Abilities[i].Spells.Count; a++)
            {
                if (Abilities[i].Spells[a] == spell)
                {
                    return a;
                }
            }
        }

        return 0;
    }
    
    //void Start()
    //{
    //    Content.Instance.Monsters.InsertRange(0, Monsters);
    //}
}

[System.Serializable]
public class DevMonsterInfo
{
    [SerializeField]
    public string MonsterKey;

    [SerializeField]
    public string MonsterName;

    [SerializeField]
    public int MonsterHP;

    [SerializeField]
    public int MonsterLevel;

    [SerializeField]
    public int DMG;

    [SerializeField]
    public int RewardEXP;

    [SerializeField]
    public EnemyType Type;

    [SerializeField]
    public List<LootInstance> PossibleLoot = new List<LootInstance>();

    [SerializeField]
    public bool isBoss;

    public List<DevPerkMap> Perks = new List<DevPerkMap>();
    
    public DevMobSpells Spells;

    public DevMonsterInfo Clone()
    {
        return (DevMonsterInfo)this.MemberwiseClone();
    }
    
    public DevPerkMap GetPerk(string perkKey)
    {
        for(int i=0;i<Perks.Count;i++)
        {
            if(Perks[i].Key == perkKey)
            {
                return Perks[i];
            }
        }

        return null;
    }
}

[System.Serializable]
public class LootInstance
{
    [Popup(/* AUTO_GENERATED_LOOT_START */ "NO VALUE", "acolytehood", "acolyterobe", "acorn", "adventurerShirt", "apprenticeRobeBlack", "apprenticeRobeWhite", "archershat", "batWing", "bigFish", "blackbandana", "blackclothshirt", "blackGloves", "blackJellyBean", "blackkercheif", "blackPants", "blackpeasentshirt", "blackShoes", "blueBerries", "blueJellyBean", "bluekercheif", "blueMushroomCap", "bluepeasentshirt", "blueponcho", "brownpeasentshirt", "brownponcho", "cabbage", "captainhat", "captianscoat", "carrot", "carrotSack", "chainlinkhelmet", "charredrobe", "clothPants", "commonaxe", "commonschimitar", "commonsword", "cosmoTunnelKey", "cozyslippers", "cutlass", "dirk", "divinebook", "drainingstaff", "executionerbandana", "Fishing Rod", "forbiddendirk", "gold", "greenGloves", "greenJellyBean", "greenPants", "greenpeasentshirt", "greenponcho", "icystaff", "idoloftrust", "improvedshortbow", "leatherarmor", "leatherarmor", "leatherGloves", "leatherpants", "leatherShoes", "leatherVest", "lightgambesson", "longdagger", "longhammer", "magicCarrotSeeds", "mailarmor", "mailpants", "mailsabatons", "mailshoes", "meltingrod", "metalbow", "metalhelmet", "nutVaultKey", "oldTurtleShell", "orangeclothshirt", "orangeJellyBean", "orbofenergy", "peasantHat", "pinkclothshirt", "pinkJellyBean", "PirateSupply", "plantFlower", "priestrobe", "pyromancerrobe", "rabbitBossEars", "rabbitCostume", "rabbitEar", "rabbitfurshoes", "rabbitFurVest", "rabbitLandEntrancePremission", "redApple", "redbandna", "redBerries", "redGloves", "redJellyBean", "redpeasentshirt", "shortAxe", "shortBow", "shortClub", "shortcutlass", "shortDagger", "shortScimitar", "shortSword", "slipers", "smallFish", "snailhat", "spear", "squirrelBossMustache", "strapShoes", "strawHat", "swordOfElad", "tatteredblackpants", "tatteredbrownpants", "tatteredgreenpants", "tatteredwhitepants", "tauntingfork", "threateningfork", "tomato", "torch", "turtleShell", "turtleshellarmor", "turtleShellOld", "turtleShellSpiked", "turtleSoup", "twohandedaxe", "twohandedschimitar", "twohandedsword", "VilePetal", "wand", "wandoffrost", "warbow", "warlockrobe", "whiteclothshirt", "whiteGloves", "whiteJellyBean", "wizardhood", "wizardrobe", "woodenhammer", "woodenpole", "wormBossAntenna", "yellowbandana", "yellowJellyBean", "yellowkercheif", "yellowshoes" /* AUTO_GENERATED_LOOT_END */)]
    public string ItemKey;
    public int MinStack = 1;
    public int MaxStack = 1;

    public LootInstance(string key = "")
    {
        this.ItemKey = key;
    }

    public LootInstance Clone()
    {
        LootInstance tempLoot = new LootInstance();

        tempLoot.ItemKey = this.ItemKey;
        tempLoot.MinStack = this.MinStack;
        tempLoot.MaxStack = this.MaxStack;

        return tempLoot;
    }

}


[System.Serializable]
public class DevItemInfo
{
    public string Key;
    public string Name;

    [TextArea(16, 9)]
    public string Description;

    //Will send "Icon" only if "IconPlaceable" is null.
    public Sprite IconPlaceable;
    public string Icon;

    public string UseSound;

    public string Type;
    public string SubType;

    public float DropChance;
    public int GoldValue;

    public int StackCap = 1;

    public ItemStats Stats;
    
    public List<DevPerkMap> Perks = new List<DevPerkMap>();

    public UseItemInfo UseInfo;

    public List<DevItemSprite> ItemSprites = new List<DevItemSprite>();
    
    public DevAppearsAt AppearsAt;
}

[System.Serializable]
public class DevItemSprite
{
    public string PartKey;

    //Will send "Sprite" only if "SpritePlaceable" is null.
    public Sprite SpritePlaceable = null;

    public string Sprite;
}

[System.Serializable]
public class DevAppearsAt
{
    public int MinLvlMobs;
    
    public int MaxLvlMobs;

    public int MinStack = 1;
    
    public int MaxStack = 1;
}

[System.Serializable]
public class ContentPiece
{
    public string Title;

    [TextArea(16, 9)]
    public string Description;

    public Sprite Icon;
}

[System.Serializable]
public class DevAbility
{
    public string Key;
    public string Name;

    public Sprite Icon;

    public Color PAColor;

    public List<PerkStage> Perks = new List<PerkStage>();

    public List<DevSpell> Spells = new List<DevSpell>();
    
    public List<DevPerkMap> InitialPerks = new List<DevPerkMap>();

    public string HitType
    {
        get
        {
            switch (hitTypeEnumState)
            {
                default:
                case HitTypeEnumState.Attack:
                    {
                        return "atk";
                    }
                case HitTypeEnumState.Heal:
                    {
                        return "heal";
                    }
                case HitTypeEnumState.CharTalent:
                    {
                        return "";
                    }
            }
        }
    }
    [SerializeField]
    HitTypeEnumState hitTypeEnumState;
    public enum HitTypeEnumState
    {
        Attack,Heal,CharTalent
    }

    [SerializeField]
    public HitTargetEnumState hitTargetEnumState;
    public enum HitTargetEnumState
    {
        Enemy, Actor
    }

    public string PowerType
    {
        get
        {
            switch (powerTypeEnumState)
            {
                default:
                case PowerTypeEnumState.Melee:
                    {
                        return "melee";
                    }
                case PowerTypeEnumState.Range:
                    {
                        return "range";
                    }
                case PowerTypeEnumState.Magic:
                    {
                        return "magic";
                    }
            }
        }
    }
    [SerializeField]
    PowerTypeEnumState powerTypeEnumState;
    public enum PowerTypeEnumState
    {
        Melee,Range,Magic
    }

    [SerializeField]
    public SpellTypeEnumState attackTypeEnumState;

    public int ManaCost;

    [SerializeField]
    public List<int> OneHandAttackAnimations = new List<int>();

    [SerializeField]
    public List<int> TwoHandAttackAnimations = new List<int>();

    [SerializeField]
    public string HitEffect = "HitEffect";

    [SerializeField]
    public List<string> HitSounds = new List<string>();

    [SerializeField]
    public string GrowingEffect;

    [SerializeField]
    public string ProjectilePrefab;

    [SerializeField]
    public string ProjectileHitSound;

    [SerializeField]
    public string ProjectileWallSound;

    [SerializeField]
    public bool ProjectileStayAfterHit;
}

[System.Serializable]
public class PerkStage
{
    public int MinLevel;
    public int PerksOffered;
    [Popup(/* AUTO_GENERATED_PERKS_START */ "NO VALUE", "aoeCap", "aoeChance", "attackSpeedModifier", "bleedChance", "bleedDuration", "bleedResistance", "blockChance", "burnChance", "burnDuration", "burnResistance", "burntTargetModifier", "cooldownModifier", "crippleChance", "crippleDuration", "crippleResistance", "critChance", "critDamageModifier", "damageBonus", "damageModifier", "damageReduction", "defenceBonus", "freezeChance", "freezeDuration", "freezeResistance", "frozenTargetModifier", "fullyChargeModifier", "hpBonus", "hpRegenInterval", "hpRegenModifier", "hpStealChance", "hpStealModifier", "knockbackModifier", "magicDamageBonus", "meleeDamageBonus", "minDamageModifier", "mpBonus", "mpCost", "mpRegenInterval", "mpRegenModifier", "mpStealChance", "mpStealModifier", "questExpBonus", "questGoldBonus", "rangeDamageBonus", "saleModifier", "shopsDiscount", "spikesModifier", "stunChance", "stunDuration", "stunResistance", "threatModifier" /* AUTO_GENERATED_PERKS_END */)]
    public List<string> AddToPool = new List<string>();
}

[System.Serializable]
public class DevMobSpells
{
    public List<DevMobSpell> SpellsList = new List<DevMobSpell>();

    public float MinTime;

    public float MaxTime;

    public DevDeathrattleSpell DeathRattle;
}

public class DevSpellBase 
{
    public string Key;
    public string ColliderPrefab;
    public List<DevPerkMap> Perks = new List<DevPerkMap>();
    [Popup(/* AUTO_GENERATED_BUFFS_START */ "NO VALUE", "bleedChance", "crippleChance", "freezeChance", "stunChance" /* AUTO_GENERATED_BUFFS_END */)]
    public List<string> HitIfTargetHasBuff = new List<string>();
    [Popup(/* AUTO_GENERATED_BUFFS_START */ "NO VALUE", "bleedChance", "crippleChance", "freezeChance", "stunChance" /* AUTO_GENERATED_BUFFS_END */)]
    public List<string> ClearTargetBuffs = new List<string>();
}

[System.Serializable]
public class DevSpell: DevSpellBase
{
    public int Level;
    public int Mana;
    public int Cooldown;
    public Sprite Icon;
    public string HitSound;

    [SerializeField]
    public HitTargetEnumState hitTargetEnumState;
    public enum HitTargetEnumState
    {
        Enemy, Actor
    }

    [SerializeField]
    public SpellTypeEnumState spellTypeEnumState;

}

[System.Serializable]
public class DevMobSpellBase : DevSpellBase
{
    [Popup(/* AUTO_GENERATED_MOBS_START */ "NO VALUE", "Bat", "BerriesBush", "BlueBerriesBush", "BlueMushroom", "BossTurtle", "BuffedSquirrel", "BuffedSquirrel_VaultKeeper", "FatRabbit", "FlowerManBoss", "FrostWizardBoss", "GiantBat", "GreenWorm", "OldTurtle", "PirateSailor1", "PirateSailor10", "PirateSailor2", "PirateSailor3", "PirateSailor4", "PirateSailor5", "PirateSailor6", "PirateSailor7", "PirateSailor8", "PirateSailor9", "Plant", "Rabbit", "RabbitBoss", "RedWorm", "Sack", "SmallSquirrel", "Spike", "SpikedTurtle", "Squirrel", "SquirrelBoss", "Thorns", "TomatoesBush", "Turtle", "VilePlant", "Worm", "WormBoss" /* AUTO_GENERATED_MOBS_END */)]
    public string[] SpawnMobs;
}

[System.Serializable]
public class DevMobSpell : DevMobSpellBase
{
    public float Chance;
}

[System.Serializable]
public class DevDeathrattleSpell : DevMobSpellBase
{
    public int Duration;
}

[System.Serializable]
public class DevPerkMap
{
    [Popup(/* AUTO_GENERATED_PERKS_START */ "NO VALUE", "aoeCap", "aoeChance", "attackSpeedModifier", "bleedChance", "bleedDuration", "bleedResistance", "blockChance", "burnChance", "burnDuration", "burnResistance", "burntTargetModifier", "cooldownModifier", "crippleChance", "crippleDuration", "crippleResistance", "critChance", "critDamageModifier", "damageBonus", "damageModifier", "damageReduction", "defenceBonus", "freezeChance", "freezeDuration", "freezeResistance", "frozenTargetModifier", "fullyChargeModifier", "hpBonus", "hpRegenInterval", "hpRegenModifier", "hpStealChance", "hpStealModifier", "knockbackModifier", "magicDamageBonus", "meleeDamageBonus", "minDamageModifier", "mpBonus", "mpCost", "mpRegenInterval", "mpRegenModifier", "mpStealChance", "mpStealModifier", "questExpBonus", "questGoldBonus", "rangeDamageBonus", "saleModifier", "shopsDiscount", "spikesModifier", "stunChance", "stunDuration", "stunResistance", "threatModifier" /* AUTO_GENERATED_PERKS_END */)]
    public string Key;
    public float Value;
}

[System.Serializable]
public class DevPAPerk
{
    public string Key;
    public string Name;
    public Sprite Icon;
    public float PrecentPerUpgrade;
    public float UpgradeCap;
    public float StartingValue;
    public List<DevPerkMap> BonusPerks = new List<DevPerkMap>();
    public bool IsClient = false;
    
    // acceleration - if value is 10 and acceleration is 5, the value at lvl 1 is 10, at lvl 2 the value is 10+15=25 and so on
    // currently disabled. will be re-used if we need it
    // public float PrecentAccelerationPerUpgrade;

    public string Type
    {
        get
        {
            switch(this.PerkType)
            {
                case PerkTypeEnum.Percent:
                    {
                        return "Percent";
                    }
                case PerkTypeEnum.Time:
                    {
                        return "Time";
                    }
                case PerkTypeEnum.Number:
                    {
                        return "Number";
                    }
            }

            return "NOTYPE";
        }
    }

    [SerializeField]
    public PerkTypeEnum PerkType;
    
    public enum PerkTypeEnum
    {
        Percent,Time,Number
    }
}

[System.Serializable]
public class DevBuff
{
    [Popup(/* AUTO_GENERATED_PERKS_START */ "NO VALUE", "aoeCap", "aoeChance", "attackSpeedModifier", "bleedChance", "bleedDuration", "bleedResistance", "blockChance", "burnChance", "burnDuration", "burnResistance", "burntTargetModifier", "cooldownModifier", "crippleChance", "crippleDuration", "crippleResistance", "critChance", "critDamageModifier", "damageBonus", "damageModifier", "damageReduction", "defenceBonus", "freezeChance", "freezeDuration", "freezeResistance", "frozenTargetModifier", "fullyChargeModifier", "hpBonus", "hpRegenInterval", "hpRegenModifier", "hpStealChance", "hpStealModifier", "knockbackModifier", "magicDamageBonus", "meleeDamageBonus", "minDamageModifier", "mpBonus", "mpCost", "mpRegenInterval", "mpRegenModifier", "mpStealChance", "mpStealModifier", "questExpBonus", "questGoldBonus", "rangeDamageBonus", "saleModifier", "shopsDiscount", "spikesModifier", "stunChance", "stunDuration", "stunResistance", "threatModifier" /* AUTO_GENERATED_PERKS_END */)]
    public string Key;

    public string Name;

    public Sprite Icon;

    public string EffectPrefab;

    public string AudioKey;
}

[System.Serializable]
public class Quest
{
    public string Name;

    public string Key;

    [TextArea(16, 9)]
    public string InProgressDescription;

    [TextArea(16, 9)]
    public string QuestCompleteDescription;

    public string FacePrefab;

    public List<QuestCondition> Conditions = new List<QuestCondition>();

    public List<QuestState> QuestsStates = new List<QuestState>();

    public int MinimumLevel;

    public List<LootInstance> RewardItems = new List<LootInstance>();

    public int RewardHP;
    public int RewardMP;

    [Popup(/* AUTO_GENERATED_ABILITIES_START */ "NO VALUE", "charTalent", "frost", "heal", "melee", "quests", "range" /* AUTO_GENERATED_ABILITIES_END */)]
    public string RewardPrimaryAbility;
    
    public int RewardExp;

    public bool IsAvailable(ActorInfo character)
    {
        //Is already complete?
        if(LocalUserInfo.Me.ClientCharacter.CompletedQuests.Contains(this.Key))
        {
            return false;
        }

        //Is already in progress?
        if(LocalUserInfo.Me.ClientCharacter.GetQuest(this.Key) != null)
        {
            return false;
        }

        //Is for a higher level?
        if(character.LVL < MinimumLevel)
        {
            return false;
        }

        //Is it a chain quest?
        for (int i=0;i< QuestsStates.Count;i++)
        {
            switch (QuestsStates[i].State)
            {
                case "InProgress":
                    {
                        if(character.GetQuest(QuestsStates[i].QuestKey) == null)
                        {
                            return false;
                        }

                        break;
                    }
                case "Completed":
                    {
                        if (!character.CompletedQuests.Contains(QuestsStates[i].QuestKey))
                        {
                            return false;
                        }

                        break;
                    }
                case "NotInProgress":
                    {
                        if (character.GetQuest(QuestsStates[i].QuestKey) != null)
                        {
                            return false;
                        }

                        break;
                    }
                case "NotCompleted":
                    {
                        if (character.CompletedQuests.Contains(QuestsStates[i].QuestKey))
                        {
                            return false;
                        }

                        break;
                    }
            }
        }


        return true;
    }

    public bool CanBeCompleted
    {
        get
        {
            for (int i=0;i<Conditions.Count;i++)
            {
                if (Conditions[i].CurrentProgress < Conditions[i].TargetProgress)
                {
                    return false;
                }
            }


            return true;
        }
    }

    //Will check if quest was completed now
    public bool WasNowCompleted
    {
        get
        {
            bool HasEqual = false;
            for (int i = 0; i < Conditions.Count; i++)
            {
                if (Conditions[i].CurrentProgress < Conditions[i].TargetProgress)
                {
                    return false;
                }
                else
                {
                    HasEqual = true;
                }
            }

            return HasEqual;
        }
    }

    public Quest Clone()
    {
        Quest tempQuest = new Quest();

        tempQuest.Key = this.Key;
        tempQuest.Name = this.Name;
        tempQuest.InProgressDescription = this.InProgressDescription;
        tempQuest.QuestCompleteDescription = this.QuestCompleteDescription;
        tempQuest.FacePrefab = this.FacePrefab;

        for (int i = 0; i < this.Conditions.Count; i++)
        {
            tempQuest.Conditions.Add(this.Conditions[i].Clone());
        }

        tempQuest.QuestsStates.AddRange(this.QuestsStates);
        tempQuest.MinimumLevel = this.MinimumLevel;

        for (int i = 0; i < this.RewardItems.Count; i++)
        {
            tempQuest.RewardItems.Add(this.RewardItems[i].Clone());
        }

        tempQuest.RewardHP = this.RewardHP;
        tempQuest.RewardMP = this.RewardMP;

        tempQuest.RewardPrimaryAbility = this.RewardPrimaryAbility;

        tempQuest.RewardExp = this.RewardExp;

        return tempQuest;
    }

    public QuestCondition GetConditionByType(string type)
    {
        for(int i=0;i<Conditions.Count;i++)
        {
            if(Conditions[i].Type == type)
            {
                return Conditions[i];
            }
        }

        return null;
    }
}

[System.Serializable]
public class QuestCondition
{

    public QuestCondition Clone()
    {
        QuestCondition tempCondition = new QuestCondition();

        tempCondition.Condition = this.Condition;
        tempCondition.Type = this.Type;
        tempCondition.CurrentProgress = this.CurrentProgress;
        tempCondition.TargetProgress = this.TargetProgress;
        tempCondition.ExtraDescription = this.ExtraDescription;

        return tempCondition;
    }

    public string Condition;
    public string Type;
    public int CurrentProgress;
    public int TargetProgress;

    public string ExtraDescription;
}

[System.Serializable]
public class QuestState
{
    [Popup(/* AUTO_GENERATED_QUESTS_START */ "NO VALUE", "ABeanyRequest", "aidingTheCommunity", "bigOnes", "bigOnesDivine", "bigOnesIce", "bigOnesMelting", "bigOnesScimitar", "blockingTheView", "breakIntoSpa", "bustNuts", "carrotSupply", "CleaningUp", "divinePlace", "examineLostSupplies", "FatAlbert", "findAlex", "findCosmo", "findCosmo2", "findCosmo3", "findKaren", "frostPractice", "FrostTest", "hairPotion", "helpAlex", "helpJaxTheDog", "helpJaxTheDog2", "helpMaya", "jacksVengeance", "jacksVengeance2", "jacksVengeance3", "joinShrine", "mayaSpikedTurtles", "OldFriends", "petRansom", "petRansom2", "picnicSupplies", "piratesAmbush", "practiceHealing", "rabbitRaids", "rangerPractice", "RangerPractice2", "RangerTest", "ruinedPainting", "sacrificeFrostGod", "summonAnAngel", "thisIsNecessary1", "thisIsNecessary2", "thisIsNecessary3", "turtleProblem", "TurtleQuizz", "turtleSoup", "untieNurtle" /* AUTO_GENERATED_QUESTS_END */)]
    public string QuestKey;

    public string State
    {
        get
        {
            switch(this.EnumState)
            {
                case QuestEnumState.InProgress :
                    {
                        return "InProgress";
                    }
                case QuestEnumState.Completed:
                    {
                        return "Completed";
                    }
                case QuestEnumState.NotInProgress:
                    {
                        return "NotInProgress";
                    }
                case QuestEnumState.NotCompleted:
                    {
                        return "NotCompleted";
                    }
                case QuestEnumState.CanBeCompleted:
                    {
                        return "CanBeCompleted";
                    }
                case QuestEnumState.IsAvailable:
                    {
                        return "IsAvailable";
                    }
                case QuestEnumState.IsUnavailable:
                    {
                        return "IsUnavailable";
                    }
            }

            return "NOSTATE";
        }
    }

    [SerializeField]
    QuestEnumState EnumState;
    


    public enum QuestEnumState
    {
        InProgress,Completed,NotInProgress,NotCompleted,CanBeCompleted, NeverStarted, IsAvailable, IsUnavailable
    }
}

public enum SpellTypeEnumState
{
    normal, projectile, explosion, channeling, movement
}

[System.Serializable]
public class ContentMultiplyers
{
    public float mobsHp = 1f;
    public float mobsExp = 1f;
    public float mobsDmg = 1f;
}