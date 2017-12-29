using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompletedQuestPanelUI : MonoBehaviour {

    [SerializeField]
    Text txtTitle;

    [SerializeField]
    Text txtDescription;

    [SerializeField]
    Transform AvatarSpot;

    public void SetInfo(Quest quest)
    {
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
        tempObject.transform.position = AvatarSpot.position;
    }

    public void ClearPanel()
    {
        while (AvatarSpot.transform.childCount > 0)
        {
            AvatarSpot.transform.GetChild(0).gameObject.SetActive(false);
            AvatarSpot.transform.GetChild(0).SetParent(transform);
        }
    }
}
