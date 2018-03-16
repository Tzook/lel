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
    protected GetRandomName m_getRandomName;

    [SerializeField]
    protected MainMenuUI m_mainMenuUI;

    public List<string> AllowedHairMale = new List<string>();
    public List<string> AllowedHairFemale = new List<string>();
    public List<string> AllowedEyes = new List<string>();
    public List<string> AllowedNose = new List<string>();
    public List<string> AllowedMouth = new List<string>();
    public const int MAX_SKIN_INDEX = 2;

    int hairIndex;
    int eyesIndex;
    int noseIndex;
    int mouthIndex;
    int skinIndex;

    private bool RandomizingName = false;

    void Start()
    {
        m_createCharacter = new CreateCharacter(CreateCharacterResponse);
        m_getRandomName = new GetRandomName(GetRandomNameResponse);
    }

    public void Init()
    {
        this.gameObject.SetActive(true);
        if (m_ActorInstance != null)
        {
            m_ActorInstance.gameObject.SetActive(false);
        }

        GameObject tempObj = Instantiate(ResourcesLoader.Instance.GetObject(("actor_male")));

        tempObj.transform.SetParent(transform);
        tempObj.transform.position = actorSpot.position;
        tempObj.transform.localScale = actorSpot.localScale;

        m_ActorInstance = tempObj.GetComponent<ActorInstance>();
        m_ActorInstance.nameHidden = true;

        m_ActorInstance.ResetChar();
        m_ActorInfo = m_ActorInstance.Info;
        m_txtGender.text = m_ActorInfo.Gender.ToString();
        m_inputName.text = "";
        
        for(int i=0;i<Content.Instance.StartingGear.Count;i++)
        {
            DevItemInfo item = Content.Instance.GetItem(Content.Instance.StartingGear[i]);
            m_ActorInfo.Equipment.SetItem(item.Type, new ItemInfo(item, item.Perks, 1));
        }

        ToggleGender();
        Randomize();
        RandomizeName();
    }

    public void ToggleGender()
    {

        m_ActorInstance.gameObject.SetActive(false);

        GameObject tempObj;
        if (m_ActorInfo.Gender == Gender.Male)
        {
            m_ActorInfo.Gender = Gender.Female;
            tempObj = Instantiate(ResourcesLoader.Instance.GetObject("actor_female"));
            m_ActorInfo.Hair = AllowedHairFemale[0];
        }
        else
        {
            m_ActorInfo.Gender = Gender.Male;
            tempObj = Instantiate(ResourcesLoader.Instance.GetObject("actor_male"));
            m_ActorInfo.Hair = AllowedHairMale[0];
        }


        tempObj.transform.SetParent(transform);
        tempObj.transform.position = actorSpot.position;
        tempObj.transform.localScale = actorSpot.localScale;

        m_ActorInstance = tempObj.GetComponent<ActorInstance>();
        m_ActorInstance.Info = m_ActorInfo;
        m_ActorInstance.nameHidden = true;
        m_ActorInstance.UpdateVisual();

        m_txtGender.text = m_ActorInfo.Gender.ToString();
    }

    public void NextHair()
    {
        hairIndex++;

        if (m_ActorInfo.Gender == Gender.Male)
        {
            if (hairIndex >= AllowedHairMale.Count)
            {
                hairIndex = 0;
            }
        }
        else
        {
            if (hairIndex >= AllowedHairFemale.Count)
            {
                hairIndex = 0;
            }
        }

        if (m_ActorInfo.Gender == Gender.Male)
        {
            m_ActorInfo.Hair = AllowedHairMale[hairIndex];
        }
        else
        {
            m_ActorInfo.Hair = AllowedHairFemale[hairIndex];
        }

        m_ActorInstance.UpdateVisual();
    }

    public void PrevHair()
    {
        hairIndex--;

        if (hairIndex < 0)
        {
            if (m_ActorInfo.Gender == Gender.Male)
            {
                hairIndex = AllowedHairMale.Count - 1;
            }
            else
            {
                hairIndex = AllowedHairFemale.Count - 1;
            }
        }

        if (m_ActorInfo.Gender == Gender.Male)
        {
            m_ActorInfo.Hair = AllowedHairMale[hairIndex];
        }
        else
        {
            m_ActorInfo.Hair = AllowedHairFemale[hairIndex];
        }

        m_ActorInstance.UpdateVisual();
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

    public void NextSkin()
    {
        skinIndex++;

        if (skinIndex > MAX_SKIN_INDEX)
        {
            skinIndex = 0;
        }

        m_ActorInfo.SkinColor = skinIndex;
        m_ActorInstance.UpdateVisual();
    }

    public void PreviousSkin()
    {
        skinIndex--;

        if (skinIndex < 0)
        {
            skinIndex = MAX_SKIN_INDEX;
        }

        m_ActorInfo.SkinColor = skinIndex;
        m_ActorInstance.UpdateVisual();
    }

    public void Randomize()
    {
        AudioControl.Instance.Play("sound_equip");

        m_ActorInfo.Hair = m_ActorInfo.Gender == Gender.Male ? AllowedHairMale[hairIndex = Random.Range(0, AllowedHairMale.Count)] : AllowedHairFemale[hairIndex = Random.Range(0, AllowedHairFemale.Count)];
        m_ActorInfo.Eyes = AllowedEyes[eyesIndex = Random.Range(0, AllowedEyes.Count)];
        m_ActorInfo.Nose = AllowedNose[noseIndex = Random.Range(0, AllowedNose.Count)];
        m_ActorInfo.Mouth = AllowedMouth[mouthIndex = Random.Range(0, AllowedMouth.Count)];
        skinIndex = Random.Range(0, MAX_SKIN_INDEX + 1);
        // if it's the last index, try again
        // this way, the chance to be black is 20% and not 33% 
        // i'm going to hell for this
        if (skinIndex == MAX_SKIN_INDEX) { skinIndex = Random.Range(0, MAX_SKIN_INDEX + 1); }
        m_ActorInfo.SkinColor = skinIndex;
        
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
            AudioControl.Instance.Play("sound_negative");
        }
        else
        {
            LocalUserInfo.Me.SetCharacters(response["data"]);
            m_mainMenuUI.MoveToMenu(1);
            m_mainMenuUI.LoadPlayerCharacters(LocalUserInfo.Me);
            m_mainMenuUI.SelectLastCharacter(LocalUserInfo.Me);
            AudioControl.Instance.Play("sound_positiveprogress");
        }
    }

    public void RandomizeName()
    {
        if (RandomizingName)
        {
            return;
        }
        RandomizingName = true;
        AudioControl.Instance.Play("sound_equip");
        m_getRandomName.Get(m_ActorInfo.Gender);
    }

    public void GetRandomNameResponse(JSONNode response)
    {
        RandomizingName = false;
        if (response["error"] != null)
        {
            WarningMessageUI.Instance.ShowMessage(response["error"].ToString());
            AudioControl.Instance.Play("sound_negative");
        }
        else
        {
            m_inputName.text = response["data"];
        }
    }
}
