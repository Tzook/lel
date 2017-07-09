using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyMemberUI : MonoBehaviour {

    [SerializeField]
    Text NameText;

    [SerializeField]
    Image HealthBar;

    [SerializeField]
    GameObject DeadIcon;

    [SerializeField]
    GameObject LeaderIcon;

    [SerializeField]
    Text LocationText;

    public void SetInfo(KnownCharacter character, bool isLeader)
    {
        NameText.text = character.Name;

        if (character.isLoggedIn)
        {
            if (character.Info.CurrentRoom == LocalUserInfo.Me.ClientCharacter.CurrentRoom)
            {
                HealthBar.transform.parent.gameObject.SetActive(true);

                LocationText.gameObject.SetActive(false);

                SetHealth(character.Info.CurrentHealth / character.Info.MaxHealth);
            }
            else
            {
                HealthBar.transform.parent.gameObject.SetActive(false);
                LocationText.gameObject.SetActive(true);
                LocationText.text = "In " + character.Info.CurrentRoom;
            }
        }
        else
        {
            HealthBar.transform.parent.gameObject.SetActive(false);

            LocationText.gameObject.SetActive(true);

            LocationText.text = "OFFLINE";
        }

        LeaderIcon.SetActive(isLeader);
    }

    public void SetInfo(string sName, bool isLeader)
    {
        NameText.text = sName;

        HealthBar.transform.parent.gameObject.SetActive(false);

        LocationText.gameObject.SetActive(true);

        LocationText.text = "OFFLINE";

        LeaderIcon.SetActive(isLeader);
    }

    public void SetDed(bool state)
    {
        DeadIcon.SetActive(state);
    }

    public void SetHealth(float HPPrecent)
    {
        if (HPPrecent <= 0)
        {
            SetDed(true);
            HealthBar.fillAmount = 0f;
        }
        else
        {
            SetDed(false);
            HealthBar.fillAmount = HPPrecent;
        }
    }
}
