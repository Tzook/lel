using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompletedQuestsWindowUI : MonoBehaviour {

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
        for (int i = 0; i < LocalUserInfo.Me.ClientCharacter.CompletedQuests.Count; i++)
        {
            tempObject = ResourcesLoader.Instance.GetRecycledObject("CompletedQuestPanel");
            tempObject.transform.SetParent(Container, false);

            tempObject.GetComponent<CompletedQuestPanelUI>().SetInfo(Content.Instance.GetQuest(LocalUserInfo.Me.ClientCharacter.CompletedQuests[i]));
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
