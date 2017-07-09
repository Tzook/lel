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

    [SerializeField]
    GameObject KickIcon;

    KnownCharacter CurrentCharacter;

    public void SetInfo(KnownCharacter character, bool isLeader)
    {
        CurrentCharacter = character;

        NameText.text = character.Name;

        if (character.isLoggedIn)
        {
            if (character.Info.CurrentRoom == LocalUserInfo.Me.ClientCharacter.CurrentRoom)
            {
                HealthBar.transform.parent.gameObject.SetActive(true);

                LocationText.gameObject.SetActive(false);


                SetHealth((character.Info.CurrentHealth*1f) / (character.Info.MaxHealth*1f));
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
        KickIcon.SetActive(LocalUserInfo.Me.CurrentParty.Leader == LocalUserInfo.Me.ClientCharacter.Name && LocalUserInfo.Me.ClientCharacter.Name != CurrentCharacter.Name);
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

    public void KickPlayer()
    {
        SocketClient.Instance.SendKickFromParty(CurrentCharacter.Name);
        this.gameObject.SetActive(false);
    }
}
