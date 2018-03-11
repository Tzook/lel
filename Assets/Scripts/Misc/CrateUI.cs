using System.Collections;
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
        string prefix = PercentValue >= 0 ? "+" : "";
        string suffix = "";
        if (CurrentPerk.PerkType == DevPAPerk.PerkTypeEnum.Percent) {
            PercentValue *= 100f;
            suffix = "%";
        } else if (CurrentPerk.PerkType == DevPAPerk.PerkTypeEnum.Time) {
            suffix = "s";
        } 
        bool hasFloatingPoint = PercentValue % 1 == 0;
        string showValue = (hasFloatingPoint ? Mathf.FloorToInt(PercentValue) : PercentValue).ToString();
        m_Title.text = prefix + showValue + suffix + " " + CurrentPerk.Name;
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
