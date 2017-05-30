using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestRewardsWindowUI : MonoBehaviour {

    [SerializeField]
    Transform RewardsContainer;

    string TargetQuest;

	public void Show(Quest quest)
    {
        this.gameObject.SetActive(true);

        TargetQuest = quest.Key;

        ClearContainer();

        GameObject tempObject;
        DevItemInfo tempItem;
        for (int i = 0; i < quest.RewardItems.Count;i++)
        {
            tempObject = ResourcesLoader.Instance.GetRecycledObject("RewardPanel");
            tempObject.transform.SetParent(RewardsContainer, false);

            tempItem = Content.Instance.GetItem(quest.RewardItems[i].ItemKey);
            tempObject.GetComponent<RewardPanelUI>().SetInfo(tempItem.Name, ResourcesLoader.Instance.GetSprite(tempItem.Icon), quest.RewardItems[i].MinStack);
        }

        if (quest.RewardExp > 0)
        {
            tempObject = ResourcesLoader.Instance.GetRecycledObject("RewardPanel");
            tempObject.transform.SetParent(RewardsContainer, false);

            tempObject.GetComponent<RewardPanelUI>().SetInfo("+EXP ", ResourcesLoader.Instance.GetSprite("fx_hit_small"), quest.RewardExp);
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
        SocketClient.Instance.SendQuestCompleted(TargetQuest);
        this.gameObject.SetActive(false);
    }
}
