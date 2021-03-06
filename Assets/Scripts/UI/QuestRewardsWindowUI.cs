﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestRewardsWindowUI : MonoBehaviour {

    [SerializeField]
    Transform RewardsContainer;

    [SerializeField]
    Text QuestTitle;

    string TargetQuest;

    string TargetNpcKey;

	public void Show(Quest quest, string npcKey)
    {
        this.gameObject.SetActive(true);

        TargetQuest = quest.Key;
        TargetNpcKey = npcKey;

        QuestTitle.text = quest.Name;

        ClearContainer();

        GameObject tempObject;
        DevItemInfo tempItem;
        for (int i = 0; i < quest.RewardItems.Count;i++)
        {
            tempObject = ResourcesLoader.Instance.GetRecycledObject("RewardPanel");
            tempObject.transform.SetParent(RewardsContainer, false);

            tempItem = Content.Instance.GetItem(quest.RewardItems[i].ItemKey);
            int stack = quest.RewardItems[i].MinStack;
            if (tempItem.Key == "gold") 
            {
                stack = Mathf.RoundToInt(stack * (LocalUserInfo.Me.ClientCharacter.ClientPerks.QuestGoldBonus + 1f));
            }
            tempObject.GetComponent<RewardPanelUI>().SetInfo(tempItem.Name, ResourcesLoader.Instance.GetSprite(tempItem.Icon), stack);
        }

        if (quest.RewardExp > 0)
        {
            tempObject = ResourcesLoader.Instance.GetRecycledObject("RewardPanel");
            tempObject.transform.SetParent(RewardsContainer, false);

            int rewardExp = Mathf.RoundToInt(quest.RewardExp * (LocalUserInfo.Me.ClientCharacter.ClientPerks.QuestExpBonus + 1f));
            tempObject.GetComponent<RewardPanelUI>().SetInfo("+EXP ", ResourcesLoader.Instance.GetSprite("fx_hit_small"), rewardExp);
        }
    }

    public void ClearContainer()
    {
        while(RewardsContainer.childCount > 0)
        {
            RewardsContainer.GetChild(0).gameObject.SetActive(false);
            RewardsContainer.GetChild(0).SetParent(transform);
        }
    }

    public void RecieveReward()
    {
        SocketClient.Instance.SendQuestCompleted(TargetQuest, TargetNpcKey);
        this.gameObject.SetActive(false);
    }
}
