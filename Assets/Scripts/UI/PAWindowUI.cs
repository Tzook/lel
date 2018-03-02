using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PAWindowUI : MonoBehaviour {

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

    public int SelectedPA = 0;

    Ability CurrentPA;
    DevAbility CurrentDevPA;

    public void Show()
    {
        this.gameObject.SetActive(true);

        RefreshWindow();
    }

    public void RefreshWindow(int AbilityIndex)
    {
        SelectedPA = AbilityIndex;
        RefreshWindow();
    }

    public void RefreshWindow()
    {
        List<Ability> Abilities = LocalUserInfo.Me.ClientCharacter.AllAbilities;
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

            tempObj.transform.GetChild(0).GetComponent<Image>().sprite = tempDevPA.Icon;
            tempObj.transform.GetChild(0).GetComponent<Outline>().effectColor = tempDevPA.PAColor;

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

        float toLevelUpPrecent = (CurrentPA.Exp / (CurrentPA.NextLevelXP * 1f));
        PAEXPPrecent.text = (Mathf.FloorToInt(toLevelUpPrecent * 100)) + "%";
        PABar.fillAmount = toLevelUpPrecent;


        PAIcon.GetComponent<Outline>().effectColor = CurrentDevPA.PAColor;
        m_FrameImage.color = CurrentDevPA.PAColor;

        for(int i=0; i < CurrentPA.Perks.Count; i++)
        {
            tempObj = ResourcesLoader.Instance.GetRecycledObject("UpgradeInfoPanel");
            tempObj.transform.SetParent(UpgradeContainer, false);
            tempObj.GetComponent<UpgradeInfoUI>().SetInfo(CurrentDevPA, CurrentPA.Perks[i]);
        }
    }

    public void AddPASelectionListener(Button btn, int gIndex)
    {
        btn.onClick.AddListener(delegate
        {
            RefreshWindow(gIndex);
        });
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
