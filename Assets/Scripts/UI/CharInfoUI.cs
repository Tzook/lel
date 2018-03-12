using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CharInfoUI : MonoBehaviour {

    [SerializeField]
    Text m_txtName;

    [SerializeField]
    Text m_txtGender;

    [SerializeField]
    Text m_txtLevel;

    [SerializeField]
    Button m_btnAddFriend;

    [SerializeField]
    Button m_btnAddParty;


    [SerializeField]
    Transform CharSpot;

    ActorInstance CharInstance;

    ActorInfo CurrentInfo;

    public void Open(ActorInfo Info)
    {
        CurrentInfo = Info;

        this.gameObject.SetActive(true);

        Refresh();
    }

    public void Refresh()
    {
        StartCoroutine(RefreshRoutine(CurrentInfo));
    } 

    private IEnumerator RefreshRoutine(ActorInfo Info)
    {
        m_txtName.text = Info.Name;
        m_txtGender.text = "Gender: " + Info.Gender.ToString();
        m_txtLevel.text = "Level: " + Info.LVL;

        m_btnAddFriend.interactable = false;
        m_btnAddParty.interactable = CanSendPartyInvite(Info.Name);

        if (CharSpot.childCount > 0)
        {
            Destroy(CharSpot.GetChild(0).gameObject);
        }

        yield return 0;
        
        if (Info.Gender == Gender.Male)
        {
            Instantiate(ResourcesLoader.Instance.GetObject("actor_male")).transform.SetParent(CharSpot);
        }
        else
        {
            Instantiate(ResourcesLoader.Instance.GetObject("actor_female")).transform.SetParent(CharSpot);
        }

        CharSpot.GetChild(0).position = CharSpot.position;
        CharSpot.GetChild(0).transform.localScale = Vector3.one;
        CharInstance = CharSpot.GetChild(0).GetComponent<ActorInstance>();

        CharInstance.GetComponent<PlayerShortcuts>().enabled = false;
        CharInstance.Info = Info;
        CharInstance.nameHidden = true;

        CharInstance.SetElementsLayer("OverUI", 1);
        CharInstance.UpdateVisual();
    }

    public void InviteToParty()
    {
        if (LocalUserInfo.Me.CurrentParty == null)
        {
            SocketClient.Instance.SendCreateParty();
        }

        SocketClient.Instance.SendInviteToParty(CurrentInfo.Name);

        Refresh();
    }

    public bool CanSendPartyInvite(string charName)
    {
        if(LocalUserInfo.Me.CurrentParty == null)
        {
            return true;
        }

        Party party = LocalUserInfo.Me.CurrentParty;

        return (!party.Members.Contains(charName)
                && party.Leader == LocalUserInfo.Me.ClientCharacter.Name
                && party.Members.Count < 5);
    }
}
