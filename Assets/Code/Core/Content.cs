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

    public List<DevItemInfo> Items = new List<DevItemInfo>();

    public List<ContentPiece> InfoBank = new List<ContentPiece>();

    public List<PrimaryAbility> PrimaryAbilities = new List<PrimaryAbility>();

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

    public PrimaryAbility GetPrimaryAbility(string key)
    {
        for (int i = 0; i < PrimaryAbilities.Count; i++)
        {
            if (PrimaryAbilities[i].Name == key)
            {
                return PrimaryAbilities[i];
            }
        }

        return null;
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

    public Texture2D DefaultCursor;
    public Texture2D ClickableCursor;

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
    public int MinDMG;

    [SerializeField]
    public int MaxDMG;

    [SerializeField]
    public int RewardEXP;

    [SerializeField]
    public EnemyType Type;

    [SerializeField]
    public List<LootInstance> PossibleLoot = new List<LootInstance>();



}

[System.Serializable]
public class LootInstance
{
    public LootInstance Clone()
    {
        LootInstance tempLoot = new LootInstance();

        tempLoot.ItemKey = this.ItemKey;
        tempLoot.MinStack = this.MinStack;
        tempLoot.MaxStack = this.MaxStack;

        return tempLoot;
    }

    public string ItemKey;
    public int MinStack = 1;
    public int MaxStack = 1;
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

    public List<DevItemSprite> ItemSprites = new List<DevItemSprite>();
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
public class ContentPiece
{
    public string Title;

    [TextArea(16, 9)]
    public string Description;

    public Sprite Icon;
}

[System.Serializable]
public class PrimaryAbility
{
    public string Name;

    public Sprite Icon;
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

    public List<string> RequiredCompletedQuests = new List<string>();

    public string RequiredClass;

    public int MinimumLevel;

    public List<LootInstance> RewardItems = new List<LootInstance>();

    public int RewardSTR;
    public int RewardMAG;
    public int RewardDEX;
    public int RewardHP;
    public int RewardMP;

    public PrimaryAbility RewardPrimaryAbility;

    public string RewardClass;
    
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

        //Is not for my class?
        if (!string.IsNullOrEmpty(RequiredClass) && RequiredClass != character.Class)
        {
            return false;
        }

        //Is for a higher level?
        if(character.LVL < MinimumLevel)
        {
            return false;
        }

        //Is it a chain quest?
        for (int i=0;i< RequiredCompletedQuests.Count;i++)
        {
            if(!character.CompletedQuests.Contains(RequiredCompletedQuests[i]))
            {
                return false;
            }
        }


        return true;
    }

    public bool CanBeCompleted
    {
        get
        {
            //if(RewardClass != LocalUserInfo.Me.SelectedCharacter.Class)
            //{
            //    return false;
            //}

            //if(MinimumLevel > LocalUserInfo.Me.SelectedCharacter.LVL)
            //{
            //    return false;
            //}

            //for (int i = 0; i < RequiredCompletedQuests.Count; i++)
            //{
            //    if(!LocalUserInfo.Me.SelectedCharacter.CompletedQuests.Contains(RequiredCompletedQuests[i]))
            //    {
            //        return false;
            //    }
            //}

            for (int i=0;i<Conditions.Count;i++)
            {
                if(Conditions[i].CurrentProgress < Conditions[i].TargetProgress)
                {
                    return false;
                }
            }


            return true;
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

        for(int i=0;i<this.Conditions.Count;i++)
        {
            tempQuest.Conditions.Add(this.Conditions[i].Clone());
        }

        tempQuest.RequiredCompletedQuests.AddRange(this.RequiredCompletedQuests);
        tempQuest.RequiredClass = this.RequiredClass;
        tempQuest.MinimumLevel  = this.MinimumLevel;

        for (int i = 0; i < this.RewardItems.Count; i++)
        {
            tempQuest.RewardItems.Add(this.RewardItems[i].Clone());
        }

        tempQuest.RewardSTR = this.RewardSTR;
        tempQuest.RewardMAG = this.RewardMAG;
        tempQuest.RewardDEX = this.RewardDEX;
        tempQuest.RewardHP = this.RewardHP;
        tempQuest.RewardMP = this.RewardMP;

        tempQuest.RewardClass = this.RewardClass;
        tempQuest.RewardPrimaryAbility = this.RewardPrimaryAbility;

        tempQuest.RewardExp = this.RewardExp;

        return tempQuest;
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

        return tempCondition;
    }

    public string Condition;
    public string Type;
    public int CurrentProgress;
    public int TargetProgress;
}

