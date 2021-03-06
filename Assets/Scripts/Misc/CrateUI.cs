﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrateUI : MonoBehaviour
{

    DevPAPerk CurrentPerk;

    [SerializeField]
    Image m_Image;

    [SerializeField]
    Text m_Title;

    [SerializeField]
    Animator m_Animator;

    [SerializeField]
    ParticleSystem m_ParticleSystemA;

    [SerializeField]
    ParticleSystem m_ParticleSystemB;

    [SerializeField]
    Button CardButton;

    public bool Interactable
    {
        set
        {
            CardButton.interactable = value;
        }
        get
        {

            return CardButton.interactable;
        }
    }

    public void PlaySound(string Key)
    {
        AudioControl.Instance.Play(Key);
    }

    public void StopSound(string Key)
    {
        AudioControl.Instance.StopSound(Key);
    }

    public void Set(string perkKey, PAPerk currentAbilityPerk = null)
    {
        Debug.Log("SD" + perkKey);
        CurrentPerk = Content.Instance.GetPerk(perkKey);

        m_Image.sprite = CurrentPerk.Icon;

        m_Animator.SetTrigger("Reset");

        int precentAccelerationPerUpgrade = 0;//CurrentPerk.PrecentAccelerationPerUpgrade;
        float PercentValue = CurrentPerk.PrecentPerUpgrade + precentAccelerationPerUpgrade * (currentAbilityPerk == null ? 0 : currentAbilityPerk.Points);

        m_Title.text = PerkValueFormatter.Instance.GetFormattedValue(CurrentPerk, PercentValue, true) + " " + CurrentPerk.Name;
    }

    public void Unpack()
    {
        m_Animator.SetTrigger("Unpack");
    }

    public void PlayParticles()
    {
        m_ParticleSystemA.Play();
        m_ParticleSystemB.Play();
    }

    public void SelectUpgrade()
    {
        InGameMainMenuUI.Instance.ChooseMasteryUpgradePerk(CurrentPerk.Key);
        StartCoroutine(SelectMasteryRoutine());
    }

    IEnumerator SelectMasteryRoutine()
    {
        m_Animator.SetTrigger("Select");

        yield return new WaitForSeconds(1f);

        InGameMainMenuUI.Instance.HideMasteryUpgradeWindow();
    }
}
