using UnityEngine;
using System.Collections;
using SimpleJSON;
using System.Collections.Generic;
using System;

public class ActorInfo
{
    public string ID;
    public string Name;
    public Gender Gender;
    public ActorInstance Instance;
    public string CurrentRoom;
    public Vector3 LastPosition;
    public bool Climbing;
    public int SkinColor;
    public string Hair = "hair_0";
    public string Eyes = "eyes_0a";
    public string Nose = "nose_0";
    public string Mouth= "mouth_0";

    public int LVL;

    public int EXP;
    public int NextLevelXP
    {
        get
        {
            // see http://tibia.wikia.com/wiki/Experience_Formula
            return Mathf.FloorToInt(((50f / 3f) * ((LVL+1) * (LVL + 1) * (LVL + 1) - 6f * (LVL + 1) * (LVL + 1) + 17f * (LVL + 1) - 12f)) 
                                    - ((50f / 3f) * (LVL * LVL * LVL - 6f * LVL * LVL + 17f * LVL - 12f)));
        }
        private set { }
    }

    public string Class;

    public int STR;
    public int MAG;
    public int DEX;

    public int BonusSTR;
    public int BonusMAG;
    public int BonusDEX;
    public int BonusHP;
    public int BonusMP;

    public int MaxHealth
    {
        get
        {
            return (STR * 5) + (BonusSTR * 5) + BonusHP;
        }
    }
    public int CurrentHealth;

    public int MaxMana
    {
        get
        {
            return (MAG * 5) + (BonusMAG * 5) + BonusMP;
        }
    }
    public int CurrentMana;

    public int TotalSTR
    {
        get { return BonusSTR + STR; }
    }
    public int TotalMAG
    {
        get { return BonusMAG + MAG; }
    }
    public int TotalDEX
    {
        get { return BonusDEX + DEX; }
    }


    public List<string> PrimaryAbilities = new List<string>();
    public string CurrentPrimaryAbility;

    public Inventory Inventory;

    public Equipment Equipment;

    public List<Quest> QuestsInProgress = new List<Quest>();
    public List<string> CompletedQuests = new List<string>();

    public int Gold;

    public ActorInfo()
    {
        Equipment = new Equipment(new JSONClass(), this);
    }

    public ActorInfo(JSONNode node)
    {
        this.ID = node["_id"].Value;
        this.Name = node["name"].Value;
        this.CurrentRoom = node["room"].Value;
        this.LastPosition = new Vector3(node["position"]["x"].AsFloat, node["position"]["y"].AsFloat, node["position"]["z"].AsFloat);
        this.Climbing = node["position"]["climbing"].AsBool;
        this.Gold = node["gold"].AsInt;
        

        if (node["looks"]["g"].AsBool)
        {
            this.Gender = Gender.Male;
        }
        else
        {
            this.Gender = Gender.Female;
        }

        Hair = node["looks"]["hair"].Value;
        Eyes = node["looks"]["eyes"].Value;
        Nose = node["looks"]["nose"].Value;
        Mouth = node["looks"]["mouth"].Value;
        SkinColor = node["looks"]["skin"].AsInt;

        Inventory = new Inventory(node["items"]);

        Equipment = new Equipment(node["equips"], this);

        if (node["stats"] != null)
        {
            SetStats(node["stats"]);
        }
        
        // Get the JSON keys
        IEnumerator enumerator = node["quests"]["done"].AsObject.GetEnumerator();
        while (enumerator.MoveNext()) 
        {
            KeyValuePair<string, JSONNode> current = (KeyValuePair<string, JSONNode>)enumerator.Current;
            CompletedQuests.Add(current.Key);
        }

        // Get the JSON keys
        enumerator = node["quests"]["progress"].AsObject.GetEnumerator();
        while (enumerator.MoveNext()) 
        {
            KeyValuePair<string, JSONNode> current = (KeyValuePair<string, JSONNode>)enumerator.Current;
            string questKey = current.Key;
            Quest quest = AddQuest(questKey);
            fillInitialProgress(quest, node["quests"]);
        }

        RefreshBonuses();
    }

    public Quest GetQuest(string questKey)
    {
        for(int i=0;i<QuestsInProgress.Count;i++)
        {
            if (QuestsInProgress[i].Key == questKey)
            {
                return QuestsInProgress[i];
            }
        }

        return null;
    }

    public QuestCondition GetQuestCondition(Quest tempQuest, string typeKey)
    {
        for(int i=0 ; i < tempQuest.Conditions.Count ; i++)
        {
            if(tempQuest.Conditions[i].Type == typeKey)
            {
                return tempQuest.Conditions[i];
            }
        }

        return null;
    }

    //Would switch to a primary ability 
    //if current equipment won't allow the current ability.
    public void ValidatePrimaryAbility()
    {
        switch(CurrentPrimaryAbility)
        {
            case "range":
                {
                    if(Equipment.Weapon.SubType != "range")
                    {
                        SwitchPrimaryAbility("melee");
                        return;
                    }
                    break;
                }
        }
    }

    public void ChangeGold(int amount)
    {
        Gold += amount;
        InGameMainMenuUI.Instance.RefreshInventory();

        if (amount > 0)
        {
            InGameMainMenuUI.Instance.MinilogMessage("Gained " + amount.ToString("N0") + " gold");
        }
        else
        {
            InGameMainMenuUI.Instance.MinilogMessage("Lost " + (-amount).ToString("N0") + " gold");
        }
    }

    public void SetStats(JSONNode node)
    {
        this.LVL = node["lvl"].AsInt;

        this.EXP = node["exp"].AsInt;

        this.Class = node["class"].Value;

        this.STR = node["str"].AsInt;
        this.MAG = node["mag"].AsInt;
        this.DEX = node["dex"].AsInt;

        this.CurrentHealth = node["hp"]["now"].AsInt;

        this.CurrentMana = node["mp"]["now"].AsInt;

        this.PrimaryAbilities.Clear();
        for(int i=0;i<node["abilities"].Count;i++)
        {
            this.PrimaryAbilities.Add(node["abilities"][i].Value);
        }

        SetPrimaryAbility(node["primaryAbility"].Value);

    }

    public void SwitchPrimaryAbility(string key)
    {
        if(PrimaryAbilities.Contains(key) && CanUsePrimaryAbility(key))
        {
            SocketClient.Instance.SendChangedAbility(key);

            SetPrimaryAbility(key);

        }
    }

    public void SetPrimaryAbility(string key)
    {
        CurrentPrimaryAbility = key;

        if (this == LocalUserInfo.Me.ClientCharacter)
        {
            InGameMainMenuUI.Instance.RefreshCurrentPrimaryAbility();
        }
    }

    public bool CanUsePrimaryAbility(string key)
    {
        switch(key)
        {
            case "range":
                {
                    if(Equipment.Weapon == null)
                    {
                        return false;
                    }

                    if(Equipment.Weapon.SubType != "range")
                    {
                        InGameMainMenuUI.Instance.ShockMessageCenter.CallMessage("'Range' ability requires a BOW equipped!");
                        return false;
                    }

                    break;
                }
        }

        return true;
    }

    public bool CanEquipItem(ItemInfo item)
    {
        if (item.Stats.RequiresDEX > this.DEX)
        {
            return false;
        }
        if (item.Stats.RequiresSTR > this.STR)
        {
            return false;
        }
        if (item.Stats.RequiresMAG > this.MAG)
        {
            return false;
        }
        if (item.Stats.RequiresLVL > this.LVL)
        {
            return false;
        }

        return true;
    }

    public void RefreshBonuses()
    {
        BonusSTR = 0;
        BonusMAG = 0;
        BonusDEX = 0;
        BonusHP = 0;
        BonusMP = 0;

        AddItemBonus(Equipment.Chest);
        AddItemBonus(Equipment.Gloves);
        AddItemBonus(Equipment.Head);
        AddItemBonus(Equipment.Legs);
        AddItemBonus(Equipment.Shoes);
        AddItemBonus(Equipment.Weapon);
    }

    public void RefreshBonuses(Equipment equips)
    {
        BonusSTR = 0;
        BonusMAG = 0;
        BonusDEX = 0;
        BonusHP = 0;
        BonusMP = 0;

        AddItemBonus(equips.Chest);
        AddItemBonus(equips.Gloves);
        AddItemBonus(equips.Head);
        AddItemBonus(equips.Legs);
        AddItemBonus(equips.Shoes);
        AddItemBonus(equips.Weapon);
    }

    private void AddItemBonus(ItemInfo item)
    {
        if(item == null)
        {
            return;
        }

        BonusSTR += item.Stats.BonusSTR;
        BonusMAG += item.Stats.BonusMAG;
        BonusDEX += item.Stats.BonusDEX;
        BonusHP += item.Stats.BonusHP;
        BonusMP += item.Stats.BonusMP;
    }

    public Quest AddQuest(string questKey)
    {
        Quest tempQuest = Content.Instance.GetQuest(questKey).Clone();
        Dictionary<string, int> inventoryCounts = Inventory.GetInventoryCounts();

        // loop through all conditions and add the item count from inventory to the quest progress
        for (int i = 0; i < tempQuest.Conditions.Count; i++)
        {
            QuestCondition cond = tempQuest.Conditions[i];
            if (inventoryCounts.ContainsKey(cond.Type)) 
            {
                cond.CurrentProgress = inventoryCounts[cond.Type];
            }
        }
        QuestsInProgress.Add(tempQuest);
        return tempQuest;
    }

    protected void fillInitialProgress(Quest quest, JSONNode initialHunt)
    {
        for (int i = 0; i < quest.Conditions.Count; i++)
        {
            QuestCondition cond = quest.Conditions[i];
            if (initialHunt["hunt"][cond.Type] != null) 
            {
                cond.CurrentProgress = initialHunt["hunt"][cond.Type][quest.Key].AsInt;
            }
            else if (initialHunt["ok"][cond.Type] != null)
            {
                cond.CurrentProgress = initialHunt["ok"][cond.Type][quest.Key].AsInt;
            }

        }
    }

    public void CompleteQuest(string questKey)
    {
        CompletedQuests.Add(questKey);
        QuestsInProgress.Remove(GetQuest(questKey));
    }

    public void UpdateProgress(string type, int value) 
    {
        bool updatedAnything = false;
        for (int i = 0; i < QuestsInProgress.Count; i++)
        {
            QuestCondition cond = GetQuestCondition(QuestsInProgress[i], type);
            if (cond != null) 
            {
                cond.CurrentProgress += value;
                updatedAnything = true;
                Game.Instance.CurrentScene.UpdateQuestProgress(QuestsInProgress[i].Key);

                switch(cond.Condition)
                {
                    case "collect":
                        {

                            if (QuestsInProgress[i].WasNowCompleted)
                            {
                                InGameMainMenuUI.Instance.ShockMessageCenter.CallMessage(QuestsInProgress[i].Name + " Completed!", Color.black, false);
                            }
                            // if the quest condition wasn't completed already or if it was and now it isn't
                            else if (cond.CurrentProgress - value < cond.TargetProgress || cond.CurrentProgress < cond.TargetProgress)
                            {
                                InGameMainMenuUI.Instance.ShockMessageCenter.CallMessage(Content.Instance.GetItem(type).Name + " " + cond.CurrentProgress + " / " + cond.TargetProgress, Color.black, false);
                            }

                            break;
                        }
                    case "hunt":
                        {
                            UpdateQuestProgress(QuestsInProgress[i].Key, cond.Type, cond.CurrentProgress);
                            break;
                        }
                }
                
            }
        }
        if (updatedAnything) 
        {
            // refresh only if anything has been updated
            InGameMainMenuUI.Instance.RefreshQuestProgress();
            InGameMainMenuUI.Instance.RefreshCompletedQuestProgress();
        }
    }

    public void UpdateQuestProgress(string questKey, string typeKey, int Value)
    {
        Quest tempQuest = GetQuest(questKey);
        QuestCondition condition = GetQuestCondition(tempQuest, typeKey);

        condition.CurrentProgress = Value;


        if (tempQuest.WasNowCompleted)
        {
            InGameMainMenuUI.Instance.ShockMessageCenter.CallMessage(tempQuest.Name + " Completed!", Color.black, false);
            return;
        }

        if (condition.Condition == "hunt")
        {
            InGameMainMenuUI.Instance.ShockMessageCenter.CallMessage(Content.Instance.GetMonster(typeKey).MonsterName + " " + Value + " / " + condition.TargetProgress, Color.black, false);
        }
        else if (condition.Condition == "ok")
        {
            InGameMainMenuUI.Instance.ShockMessageCenter.CallMessage(typeKey + " " + Value + " / " + condition.TargetProgress, Color.black, false);
        }
    }


    public void AbandonQuest(string QuestKey)
    {
        QuestsInProgress.Remove(GetQuest(QuestKey));
    }
}

public enum Gender
{
    Male, Female
}
