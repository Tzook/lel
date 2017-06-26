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

    public void SetInfo(ActorInfo info, bool isLeader)
    {
        NameText.text = info.Name;

        HealthBar.transform.parent.gameObject.SetActive(true);

        LocationText.gameObject.SetActive(false);

        SetHealth(info.CurrentHealth / info.MaxHealth);

        LeaderIcon.SetActive(isLeader);
    }

    public void SetInfo(string sName, bool isLeader)
    {
        NameText.text = sName;

        HealthBar.transform.parent.gameObject.SetActive(false);

        LocationText.gameObject.SetActive(true);

        LocationText.text = "NOT IN ROOM";

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
