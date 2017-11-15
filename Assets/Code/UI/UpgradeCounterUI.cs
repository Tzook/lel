﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeCounterUI : MonoBehaviour {

    int CurrentValue;

    [SerializeField]
    ParticleSystem BurstParticles;

    [SerializeField]
    ParticleSystem LoopParticles;

    [SerializeField]
    Text m_txtValue;

    [SerializeField]
    Button m_Button;

    public bool Interactable
    {
        get
        {
            return m_Button.interactable;
        }
        set
        {
            m_Button.interactable = value;
        }
    }


    public void SetValue(int gValue)
    {
        if (gValue > 0)
        {
            this.gameObject.SetActive(true);
            LoopParticles.Play();
        }
        else
        {
            this.gameObject.SetActive(false);
        }

        if (CurrentValue < gValue)
        {
            Interactable = true;

            StartCoroutine(GainEffectRoutine());
        }

        CurrentValue = gValue;
        m_txtValue.text = CurrentValue.ToString();
    }

    IEnumerator GainEffectRoutine()
    {
        GameObject tempBlob = ResourcesLoader.Instance.GetRecycledObject("PowerBlobEffect");

        yield return tempBlob.GetComponent<SplineFloatEffect>().LaunchRoutine(LocalUserInfo.Me.ClientCharacter.Instance.transform, transform, 1f, Random.Range(-3f, 3f));

        BurstParticles.Play();
    }
}
