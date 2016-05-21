using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using BestHTTP.SocketIO;
using System;
using SimpleJSON;

public class CharacterInfoPageUI : MonoBehaviour {

    [SerializeField]
    protected ActorInstance actorInstance;

    [SerializeField]
    protected Text txtName;

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
        actorInstance.UpdateVisual(info);
        txtName.text = info.Name;
        LocalUserInfo.Me.SelectedCharacter = info;
    }

    public void AttemptDeleteCharacter()
    {
        SM.LoadingWindow.Register(this);
        deleteCharacter.Delete(actorInstance.Info.ID);
    }

    public void DeleteCharacterResponse(JSONNode response)
    {
        SM.LoadingWindow.Leave(this);

        if (response["error"] != null)
        {
            SM.WarningMessage.ShowMessage(response["error"].ToString());
        }
        else
        {
            SM.WarningMessage.ShowMessage(response["data"].ToString());
            LocalUserInfo.Me.SetCharacters(response["data"]);
            m_CharacterSelectionPageMenu.SwitchTo(0);
            m_mainMenuUI.LoadPlayerCharacters(LocalUserInfo.Me);
        }
    }
}
