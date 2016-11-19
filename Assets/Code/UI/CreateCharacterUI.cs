using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using SimpleJSON;
using System.Collections.Generic;

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

    public List<string> AllowedEyes = new List<string>();
    public List<string> AllowedNose = new List<string>();
    public List<string> AllowedMouth = new List<string>();
    int eyesIndex;
    int noseIndex;
    int mouthIndex;

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

        ToggleGender();
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

    public void NextEyes()
    {
        eyesIndex++;

        if (eyesIndex >= AllowedEyes.Count)
        {
            eyesIndex = 0;
        }

        m_ActorInfo.Eyes = AllowedEyes[eyesIndex];
        m_ActorInstance.UpdateVisual();
    }

    public void PrevEyes()
    {
        eyesIndex--;

        if (eyesIndex < 0)
        {
            eyesIndex = AllowedEyes.Count-1;
        }

        m_ActorInfo.Eyes = AllowedEyes[eyesIndex];
        m_ActorInstance.UpdateVisual();
    }

    public void NextNose()
    {
        noseIndex++;

        if (noseIndex >= AllowedNose.Count)
        {
            noseIndex = 0;
        }

        m_ActorInfo.Nose = AllowedNose[noseIndex];
        m_ActorInstance.UpdateVisual();
    }

    public void PrevNose()
    {
        noseIndex--;

        if (noseIndex < 0)
        {
            noseIndex = AllowedNose.Count - 1;
        }

        m_ActorInfo.Nose = AllowedNose[noseIndex];
        m_ActorInstance.UpdateVisual();
    }

    public void NextMouth()
    {
        mouthIndex++;

        if (mouthIndex >= AllowedMouth.Count)
        {
            mouthIndex = 0;
        }

        m_ActorInfo.Mouth = AllowedMouth[mouthIndex];
        m_ActorInstance.UpdateVisual();
    }

    public void PreviousMouth()
    {
        mouthIndex--;

        if (mouthIndex < 0)
        {
            mouthIndex = AllowedMouth.Count - 1;
        }

        m_ActorInfo.Mouth = AllowedMouth[mouthIndex];
        m_ActorInstance.UpdateVisual();
    }


    public void MoveToCharactersList()
    {
        m_mainMenuUI.MoveToMenu(1);
    }

    public void AttemptCreateCharacter()
    {

        if (string.IsNullOrEmpty(m_inputName.text))
        {
            WarningMessageUI.Instance.ShowMessage("You must type a name!", 2f);
            return;
        }

        m_ActorInfo.Name = m_inputName.text;

        LoadingWindowUI.Instance.Register(this);
        m_createCharacter.Create(m_ActorInfo);
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
