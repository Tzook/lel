using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using BestHTTP.SocketIO;
using System;
using SimpleJSON;

public class CharacterInfoPageUI : MonoBehaviour {

    [SerializeField]
    protected Transform actorSpot;

    protected ActorInstance actorInstance;

    [SerializeField]
    protected Text txtName;

    [SerializeField]
    protected Text txtLevel;

    [SerializeField]
    protected Text txtClass;

    [SerializeField]
    protected MainMenuUI m_mainMenuUI;

    [SerializeField]
    protected ui_pageMenu m_CharacterSelectionPageMenu;

    protected DeleteCharacter deleteCharacter;

    void Start()
    {
        deleteCharacter = new DeleteCharacter(DeleteCharacterResponse);
    }

    public void SetInfo(ActorInfo info)
    {

        if (actorInstance != null)
        {
            actorInstance.gameObject.SetActive(false);
        }

        GameObject tempObj;
        if (info.Gender == Gender.Male)
        {
            tempObj = Instantiate(ResourcesLoader.Instance.GetObject("actor_male"));
        }
        else
        {
            tempObj = Instantiate(ResourcesLoader.Instance.GetObject("actor_female"));
        }

        tempObj.transform.SetParent(transform);
        tempObj.transform.position = actorSpot.position;
        tempObj.transform.localScale = actorSpot.localScale;

        actorInstance = tempObj.GetComponent<ActorInstance>();

        actorInstance.nameHidden = true;
        actorInstance.UpdateVisual(info);
        txtName.text = info.Name;
        txtLevel.text = "Level " + info.LVL;
        txtClass.text = info.Class;

        LocalUserInfo.Me.ClientCharacter = info;
    }

    public void AttemptDeleteCharacter()
    {
        LoadingWindowUI.Instance.Register(this);
        deleteCharacter.Delete(actorInstance.Info.ID);
        AudioControl.Instance.Play("sound_magic");
    }

    public void DeleteCharacterResponse(JSONNode response)
    {
        LoadingWindowUI.Instance.Leave(this);

        if (response["error"] != null)
        {
            WarningMessageUI.Instance.ShowMessage(response["error"].ToString());
        }
        else
        {
            OnLeaveCharacterInfoPage();
            LocalUserInfo.Me.SetCharacters(response["data"]);
            m_CharacterSelectionPageMenu.SwitchTo(0);
            m_mainMenuUI.LoadPlayerCharacters(LocalUserInfo.Me);
        }
    }

    public void OnLeaveCharacterInfoPage()
    {
        PlayerPrefs.DeleteKey(MainMenuUI.LAST_LOGGED_IN_CHAR_ID_PREF_KEY);
    }
}
