using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvailableQuestsWindowUI : MonoBehaviour {

    [SerializeField]
    Transform Container;

    public void Show()
    {
        this.gameObject.SetActive(true);

        Refresh();
    }

    public void Refresh()
    {
        ClearContainer();

        GameObject tempObject;
        for (int i = 0; i < Content.Instance.Quests.Count; i++)
        {
            if (Content.Instance.Quests[i].IsAvailable(LocalUserInfo.Me.ClientCharacter))
            {
                tempObject = ResourcesLoader.Instance.GetRecycledObject("AvailableQuestPanel");
                tempObject.transform.SetParent(Container, false);

                tempObject.GetComponent<AvailableQuestPanelUI>().SetInfo(Content.Instance.Quests[i]);
            }
        }
    }

    void ClearContainer()
    {
        while (Container.childCount > 0)
        {
            Container.GetChild(0).gameObject.SetActive(false);
            Container.GetChild(0).transform.SetParent(transform);
        }
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
}
