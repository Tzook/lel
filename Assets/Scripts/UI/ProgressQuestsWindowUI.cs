﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressQuestsWindowUI : MonoBehaviour {

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
        for (int i = 0; i < LocalUserInfo.Me.ClientCharacter.QuestsInProgress.Count; i++)
        {
            tempObject = ResourcesLoader.Instance.GetRecycledObject("QuestPanel");
            tempObject.transform.SetParent(Container, false);
            tempObject.GetComponent<QuestPanelUI>().SetInfo(LocalUserInfo.Me.ClientCharacter.QuestsInProgress[i]);
        }
    }

    void ClearContainer()
    {
        while(Container.childCount > 0)
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
