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
        if (LocalUserInfo.Me.ClientCharacter.CurrentParty == null || LocalUserInfo.Me.ClientCharacter.CurrentParty.Members.Count == 0)
        {
            LocalUserInfo.Me.ClientCharacter.CurrentParty = null;
            this.gameObject.SetActive(false);
            return;
        }

        ActorInfo actor;
        GameObject tempObj;
        List<string> partyMembers = LocalUserInfo.Me.ClientCharacter.CurrentParty.Members;

        for (int i = 0; i < partyMembers.Count; i++)
        {
            tempObj = ResourcesLoader.Instance.GetRecycledObject("PartyMemberUI");
            tempObj.transform.SetParent(Container, false);

            actor = Game.Instance.CurrentScene.GetActorByName(partyMembers[i]);//TODO Get actors also not from scene...

            if (actor != null)
            {
                tempObj.GetComponent<PartyMemberUI>().SetInfo(actor, (partyMembers[i] == LocalUserInfo.Me.ClientCharacter.CurrentParty.Leader));
            }
            else
            {
                tempObj.GetComponent<PartyMemberUI>().SetInfo(partyMembers[i], (partyMembers[i] == LocalUserInfo.Me.ClientCharacter.CurrentParty.Leader));
            }
        }
    }
}
