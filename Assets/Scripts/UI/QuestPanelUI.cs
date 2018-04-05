using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestPanelUI : MonoBehaviour {

    [SerializeField]
    Text txtTitle;

    [SerializeField]
    Text txtDescription;

    [SerializeField]
    Transform AvatarSpot;

    [SerializeField]
    Transform ConditionContainer;

    Quest Info;

    public void SetInfo(Quest quest)
    {
        if (this.gameObject.activeInHierarchy)
        {
            StartCoroutine(SetInfoRoutine(quest));
        }
    }

    IEnumerator SetInfoRoutine(Quest quest)
    {
        Info = quest;

        txtTitle.text = quest.Name;
        txtDescription.text = quest.InProgressDescription;

        ClearPanel();

        GameObject tempObject;

        if (!string.IsNullOrEmpty(quest.FacePrefab))
        {
            tempObject = ResourcesLoader.Instance.GetRecycledObject(quest.FacePrefab);
        }
        else
        {
            tempObject = ResourcesLoader.Instance.GetRecycledObject("StandardQuest");
        }

        tempObject.transform.SetParent(AvatarSpot, false);

        yield return 0;

        tempObject.transform.position = AvatarSpot.position;

        string conContent = "";
        for (int i = 0; i < quest.Conditions.Count; i++)
        {
            tempObject = ResourcesLoader.Instance.GetRecycledObject("QuestTask");
            tempObject.transform.SetParent(ConditionContainer, false);

            switch (quest.Conditions[i].Condition)
            {
                case "hunt":
                    {
                        conContent = Content.Instance.GetMonster(quest.Conditions[i].Type).MonsterName + " Slain ";

                        break;
                    }
                case "loot":
                    {
                        conContent = Content.Instance.GetItem(quest.Conditions[i].Type).Name + " Collected ";

                        break;
                    }
                case "ok":
                    {
                        conContent = quest.Conditions[i].ExtraDescription;

                        break;
                    }
            }

            tempObject.transform.GetChild(0).GetComponent<Text>().text = conContent;
            tempObject.transform.GetChild(1).GetComponent<Text>().text = quest.Conditions[i].CurrentProgress + " / " + quest.Conditions[i].TargetProgress;
        }

    }

    public void ClearPanel()
    {
        while (AvatarSpot.transform.childCount > 0)
        {
            AvatarSpot.transform.GetChild(0).gameObject.SetActive(false);
            AvatarSpot.transform.GetChild(0).SetParent(transform);
        }

        while(ConditionContainer.childCount > 0)
        {
            ConditionContainer.transform.GetChild(0).gameObject.SetActive(false);
            ConditionContainer.transform.GetChild(0).SetParent(transform);
        }
    }

    public void AbandonQuest()
    {
        InGameMainMenuUI.Instance.AbandonQuest(Info);
    }
}
