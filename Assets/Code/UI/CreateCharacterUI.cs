using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using SimpleJSON;

public class CreateCharacterUI : MonoBehaviour
{

    [SerializeField]
    protected Text m_txtGender;

    [SerializeField]
    protected InputField m_inputName;

    [SerializeField]
    Transform actorSpot;

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
        if (m_ActorInstance != null)
        {
            m_ActorInstance.gameObject.SetActive(false);
        }

        GameObject tempObj = ResourcesLoader.Instance.GetRecycledObject("actor_male");

        tempObj.transform.SetParent(transform);
        tempObj.transform.position = actorSpot.position;
        tempObj.transform.localScale = actorSpot.localScale;

        m_ActorInstance = tempObj.GetComponent<ActorInstance>();
        m_ActorInstance.HideName = true;

        m_ActorInstance.Reset();
        m_ActorInfo = m_ActorInstance.Info;
        m_txtGender.text = m_ActorInfo.Gender.ToString();
        m_inputName.text = "";
    }

    public void ToggleGender()
    {

        m_ActorInstance.gameObject.SetActive(false);

        GameObject tempObj;
        if (m_ActorInfo.Gender == Gender.Male)
        {
            m_ActorInfo.Gender = Gender.Female;
            tempObj = ResourcesLoader.Instance.GetRecycledObject("actor_female");
        }
        else
        {
            m_ActorInfo.Gender = Gender.Male;
            tempObj = ResourcesLoader.Instance.GetRecycledObject("actor_male");
        }


        tempObj.transform.SetParent(transform);
        tempObj.transform.position = actorSpot.position;
        tempObj.transform.localScale = actorSpot.localScale;

        m_ActorInstance = tempObj.GetComponent<ActorInstance>();
        m_ActorInstance.Info = m_ActorInfo;
        m_ActorInstance.HideName = true;
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
            WarningMessageUI.Instance.ShowMessage("You must type a name!", 2f);
            return;
        }

        LoadingWindowUI.Instance.Register(this);
        m_createCharacter.Create(name, gender);
    }

    public void CreateCharacterResponse(JSONNode response)
    {
        LoadingWindowUI.Instance.Leave(this);

        if (response["error"] != null)
        {
            WarningMessageUI.Instance.ShowMessage(response["error"].ToString());
        }
        else
        {
            WarningMessageUI.Instance.ShowMessage(response["data"].ToString());
            LocalUserInfo.Me.SetCharacters(response["data"]);
            m_mainMenuUI.MoveToMenu(1);
            m_mainMenuUI.LoadPlayerCharacters(LocalUserInfo.Me);
        }
    }
}
