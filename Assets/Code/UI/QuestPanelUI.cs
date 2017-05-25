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

    public void SetInfo(Quest quest)
    {
        txtTitle.text = quest.Name;
        txtDescription.text = quest.InProgressDescription;

        GameObject tempObject;

        if (!string.IsNullOrEmpty(quest.FacePrefab))
        {
            tempObject = ResourcesLoader.Instance.GetRecycledObject(quest.FacePrefab);
        }
        else
        {
            tempObject = ResourcesLoader.Instance.GetRecycledObject("StandardQuest");
        }

        tempObject.transform.SetParent(ConditionContainer, false);

        string conContent = "";
        for (int i = 0; i < quest.Conditions.Count; i++)
        {
            tempObject = ResourcesLoader.Instance.GetRecycledObject("QuestTask");
            tempObject.transform.SetParent(ConditionContainer, false);

            switch(quest.Conditions[i].Condition)
            {
                case "kill":
                    {
                        conContent = Content.Instance.GetMonster(quest.Conditions[i].Type).MonsterName + " Slain ";
                       
                        break;
                    }
                case "collect":
                    {
                        conContent = Content.Instance.GetItem(quest.Conditions[i].Type).Name + " Collected ";

                        break;
                    }
            }

            tempObject.transform.GetChild(0).GetComponent<Text>().text = conContent;
            tempObject.transform.GetChild(1).GetComponent<Text>().text = quest.Conditions[i].CurrentProgress + " / " + quest.Conditions[i].TargetProgress;
        }

    }
}
