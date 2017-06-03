﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrimaryAbilitiesGridUI : MonoBehaviour {

    [SerializeField]
    CanvasGroup m_CG;

    [SerializeField]
    Transform Container;

	public void Show()
    {
        this.gameObject.SetActive(true);

        ClearGrid();

        GameObject tempObj;
        PrimaryAbility tempAbility;
        for(int i=0;i<LocalUserInfo.Me.ClientCharacter.PrimaryAbilities.Count;i++)
        {
            tempObj = ResourcesLoader.Instance.GetRecycledObject("PrimaryAbilityOption");
            tempObj.transform.SetParent(Container, false);

            tempObj
                .transform.localScale = Vector2.one;

            tempAbility = Content.Instance.GetPrimaryAbility(LocalUserInfo.Me.ClientCharacter.PrimaryAbilities[i]);

            tempObj.transform.GetChild(0).GetComponent<Image>().sprite = tempAbility.Icon;
            AddAbilityListener(tempObj.GetComponent<Button>(), LocalUserInfo.Me.ClientCharacter.PrimaryAbilities[i]);
        }

        StartCoroutine(ShowRoutine());
    }

    private IEnumerator ShowRoutine()
    {
        m_CG.alpha = 0f;

        while(m_CG.alpha < 1f)
        {
            m_CG.alpha += 3f * Time.deltaTime;

            yield return 0;
        }
    }

    public void Hide()
    {
        StopAllCoroutines();
        this.gameObject.SetActive(false);
    }

    public void SetPrimaryAbilitiy(string key)
    {
        LocalUserInfo.Me.ClientCharacter.SwitchPrimaryAbility(key);
        Hide();
    }

    private void AddAbilityListener(Button button, string abilityKey)
    {
        button.onClick.AddListener(delegate
        {
            SetPrimaryAbilitiy(abilityKey);
        });
    }

    private void ClearGrid()
    {
        while(Container.childCount > 0)
        {
            Container.GetChild(0).gameObject.SetActive(false);
            Container.GetChild(0).SetParent(null);
        }
    }
}