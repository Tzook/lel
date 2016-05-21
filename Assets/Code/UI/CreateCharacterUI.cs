using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using SimpleJSON;

public class CreateCharacterUI : MonoBehaviour {

    [SerializeField]
    protected Text m_txtGender;
    
    [SerializeField]
    protected InputField m_inputName;

    [SerializeField]
    protected ActorInstance m_ActorInstance;

    protected ActorInfo m_ActorInfo;

    protected CreateCharacter m_createCharacter;
    
    [SerializeField]
    protected MainMenuUI m_mainMenuUI;

    void Start()
    {
        m_createCharacter = new CreateCharacter(CreateCharacterResponse);
    }

    public void Init()
    {
        m_ActorInstance.Reset();
        m_ActorInfo = m_ActorInstance.Info;
        m_txtGender.text = m_ActorInfo.Gender.ToString();
        m_inputName.text = "";
    }

    public void ToggleGender()
    {
        if(m_ActorInfo.Gender == Gender.Male)
        {
            m_ActorInfo.Gender = Gender.Female;
        }
        else
        if (m_ActorInfo.Gender == Gender.Female)
        {
            m_ActorInfo.Gender = Gender.Male;
        }

        m_ActorInstance.UpdateVisual();

        m_txtGender.text = m_ActorInfo.Gender.ToString();
    }
    
    public void MoveToCharactersList()
    {
        m_mainMenuUI.MoveToMenu(1);        
    }
    
    public void AttemptCreateCharacter()
    {
        string name = m_inputName.text;
        Gender gender = m_ActorInfo.Gender;
        
        if (string.IsNullOrEmpty(name))
        {
            SM.WarningMessage.ShowMessage("You must type a name!", 2f);
            return;
        }
        
        SM.LoadingWindow.Register(this);
        m_createCharacter.Create(name, gender);
    }
    
    public void CreateCharacterResponse(JSONNode response)
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
            m_mainMenuUI.MoveToMenu(1);
            m_mainMenuUI.LoadPlayerCharacters(LocalUserInfo.Me);
        }
    }
}
