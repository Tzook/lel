using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyWindowUI : MonoBehaviour {

    [SerializeField]
    Animator m_Anim;

    [SerializeField]
    Transform Container;

    [SerializeField]
    Outline LockOutline;

    public bool Locked = false;

	public void Open()
    {
        ClearContainer();

        m_Anim.SetBool("isOpen", true);

        Refresh();
    }

    public void Close()
    {
        if (Locked)
        {
            return;
        }

        m_Anim.SetBool("isOpen", false);
    }

    void ClearContainer()
    {
        while(Container.childCount > 0)
        {
            Container.GetChild(0).gameObject.SetActive(false);
            Container.GetChild(0).parent = transform;
        }
    }

    public void ToggleLock()
    {
        Locked = !Locked;
        LockOutline.enabled = Locked;
    }

    public void Refresh()
    {
        //Party still exists?
        if (LocalUserInfo.Me.CurrentParty == null || LocalUserInfo.Me.CurrentParty.Members.Count == 0)
        {
            LocalUserInfo.Me.CurrentParty = null;
            this.gameObject.SetActive(false);
            return;
        }

        KnownCharacter knownActor;
        GameObject tempObj;
        List<string> partyMembers = LocalUserInfo.Me.CurrentParty.Members;

        for (int i = 0; i < partyMembers.Count; i++)
        {
            tempObj = ResourcesLoader.Instance.GetRecycledObject("PartyMemberUI");
            tempObj.transform.SetParent(Container, false);

            if (partyMembers[i] == LocalUserInfo.Me.ClientCharacter.Name)
            {
                knownActor = new KnownCharacter(LocalUserInfo.Me.ClientCharacter);
                knownActor.isLoggedIn = true;
            }
            else
            {
                knownActor = LocalUserInfo.Me.GetKnownCharacter(partyMembers[i]);
            }

            if (knownActor != null)
            {
                tempObj.GetComponent<PartyMemberUI>().SetInfo(knownActor, (partyMembers[i] == LocalUserInfo.Me.CurrentParty.Leader));
            }
            else
            {
                //TODO Get tzook to send known info on logged off chars!
                tempObj.GetComponent<PartyMemberUI>().SetInfo(partyMembers[i], (partyMembers[i] == LocalUserInfo.Me.CurrentParty.Leader));
            }
    }
    }

    public void LeaveParty()
    {
        SocketClient.Instance.SendLeaveParty();

    }
}
