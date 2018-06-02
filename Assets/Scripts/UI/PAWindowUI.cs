using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PAWindowUI : MonoBehaviour 
{
    [SerializeField]
    Text WindowTitle;
    [SerializeField]
    Text ToggleText;

    [SerializeField]
    Image PAIcon;

    [SerializeField]
    Text PAName;

    [SerializeField]
    Text PALevel;

    [SerializeField]
    Text PAEXPPrecent;

    [SerializeField]
    Image PABar;

    [SerializeField]
    Transform UpgradeContainer;

    [SerializeField]
    Transform PAContainer;

    [SerializeField]
    Image m_FrameImage;

    [SerializeField]
    Text m_mainText;

    [SerializeField]
    Button m_mainButton;

    public int LastSelection = 0;
    public int SelectedPA = 0;

    Ability CurrentPA;
    DevAbility CurrentDevPA;

    bool ShowingCharAbilities = false;

    public void Show()
    {
        this.gameObject.SetActive(true);

        if (ShowingCharAbilities)
        {
            ToggleCharAbilities();
        }
        else 
        {
            RefreshWindow();
        }
    }

    public void RefreshWindow(int AbilityIndex)
    {
        SelectedPA = AbilityIndex;
        RefreshWindow();
    }

    public void RefreshWindow()
    {
        List<Ability> Abilities = ShowingCharAbilities ? LocalUserInfo.Me.ClientCharacter.CharAbilities : LocalUserInfo.Me.ClientCharacter.PrimaryAbilities;
        CurrentPA = Abilities[SelectedPA];
        CurrentDevPA = Content.Instance.GetAbility(CurrentPA.Key);

        ClearContainers();
        
        GameObject tempObj;
        DevAbility tempDevPA;
        for (int i = 0; i < Abilities.Count; i++)
        {
            tempObj = ResourcesLoader.Instance.GetRecycledObject("PASelectionButtonUI");
            tempObj.transform.SetParent(PAContainer, false);

            AddPASelectionListener(tempObj.GetComponent<Button>(), i);

            tempDevPA = Content.Instance.GetAbility(Abilities[i].Key);

            tempObj.GetComponent<Image>().color = tempDevPA.PAColor;

            tempObj.transform.GetChild(tempObj.transform.childCount-1).GetComponent<Image>().sprite = tempDevPA.Icon;
            tempObj.transform.GetChild(tempObj.transform.childCount - 1).GetComponent<Outline>().effectColor = tempDevPA.PAColor;

            if(SelectedPA == i)
            {
                tempObj.transform.localScale = Vector3.one * 1.2f;
            }
            else
            {
                tempObj.transform.localScale = Vector3.one;
            }
        }

        PAName.text = CurrentDevPA.Name;
        PAIcon.sprite = CurrentDevPA.Icon;
        PALevel.text = CurrentPA.LVL.ToString();

        float toLevelUpPrecent;

        if (CurrentPA.Key == "charTalent")
        {
            toLevelUpPrecent = (LocalUserInfo.Me.ClientCharacter.EXP / (LocalUserInfo.Me.ClientCharacter.NextLevelXP * 1f));
        }
        else
        {
            toLevelUpPrecent = (CurrentPA.Exp / (CurrentPA.NextLevelXP * 1f));
        }

        PAEXPPrecent.text = (Mathf.FloorToInt(toLevelUpPrecent * 100)) + "%";
        PABar.fillAmount = toLevelUpPrecent;


        PAIcon.GetComponent<Outline>().effectColor = CurrentDevPA.PAColor;
        m_FrameImage.color = CurrentDevPA.PAColor;

        foreach (KeyValuePair<string, PAPerk> keyValuePair in CurrentPA.Perks)
        {
            tempObj = ResourcesLoader.Instance.GetRecycledObject("UpgradeInfoPanel");
            tempObj.transform.SetParent(UpgradeContainer, false);
            tempObj.GetComponent<UpgradeInfoUI>().SetInfo(CurrentDevPA, keyValuePair.Value);
        }

        if (LocalUserInfo.Me.ClientCharacter.IsMainAbility(CurrentPA.Key))
        {
            m_mainButton.gameObject.SetActive(false);
            m_mainText.gameObject.SetActive(true);
        } else if (ShowingCharAbilities || !SceneInfo.Instance.CanSetMainAbility)
        {
            m_mainButton.gameObject.SetActive(false);
            m_mainText.gameObject.SetActive(false);
        }
        else
        {
            m_mainButton.gameObject.SetActive(true);
            m_mainText.gameObject.SetActive(false);
        }
    }

    public void AddPASelectionListener(Button btn, int gIndex)
    {
        btn.onClick.AddListener(delegate
        {
            RefreshWindow(gIndex);
        });
    }

    public void ToggleCharAbilities()
    {
        ShowingCharAbilities = !ShowingCharAbilities;
        WindowTitle.text = ShowingCharAbilities ? "Char Abilities" : "Primary Abilities";
        ToggleText.text = ShowingCharAbilities ? "Primary" : "Char";
        int temp = LastSelection;
        LastSelection = SelectedPA;
        RefreshWindow(temp);
    }

    public void SetMainAbility()
    {
        LocalUserInfo.Me.ClientCharacter.Instance.InputController.SetMainAbility(CurrentPA.Key);
        RefreshWindow();
    }

    public void ClearContainers()
    {
        while(UpgradeContainer.childCount > 0)
        {
            UpgradeContainer.GetChild(0).gameObject.SetActive(false);
            UpgradeContainer.GetChild(0).SetParent(transform);
        }

        while (PAContainer.childCount > 0)
        {
            PAContainer.GetChild(0).GetComponent<Button>().onClick.RemoveAllListeners();
            PAContainer.GetChild(0).gameObject.SetActive(false);
            PAContainer.GetChild(0).SetParent(transform);
        }
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
}
