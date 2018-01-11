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
    protected Text m_txtSTR;

    [SerializeField]
    protected Text m_txtMAG;

    [SerializeField]
    protected Text m_txtDEX;


    [SerializeField]
    protected InputField m_inputName;

    [SerializeField]
    Transform actorSpot;

    protected ActorInstance m_ActorInstance;

    protected ActorInfo m_ActorInfo;

    protected CreateCharacter m_createCharacter;

    [SerializeField]
    protected MainMenuUI m_mainMenuUI;

    public List<string> AllowedHairMale = new List<string>();
    public List<string> AllowedHairFemale = new List<string>();
    public List<string> AllowedEyes = new List<string>();
    public List<string> AllowedNose = new List<string>();
    public List<string> AllowedMouth = new List<string>();
    int hairIndex;
    int eyesIndex;
    int noseIndex;
    int mouthIndex;
    int skinIndex;

    public Coroutine RandomizeStatsInstance { get; private set; }

    void Start()
    {
        m_createCharacter = new CreateCharacter(CreateCharacterResponse);
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
            m_ActorInfo.Equipment.SetItem(item.Type, new ItemInfo(item, 1));
        }

        ToggleGender();
        RandomizeStats();
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

        if (skinIndex >= 3)
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
            skinIndex = 2;
        }

        m_ActorInfo.SkinColor = skinIndex;
        m_ActorInstance.UpdateVisual();
    }

    public void RandomizeStats()
    {
        if(RandomizeStatsInstance != null)
        {
            StopCoroutine(RandomizeStatsInstance);
        }

        RandomizeStatsInstance = StartCoroutine(RandomizeStatsRoutine());
        AudioControl.Instance.Play("sound_equip");
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
            AudioControl.Instance.Play("sound_positiveprogress");
        }
    }




    protected IEnumerator RandomizeStatsRoutine()
    {
        int rndRoll = Random.Range(3, 6);
        while(rndRoll > 0)
        {
            RandomizeStatsSingle();
            rndRoll--;

            yield return new WaitForSeconds(0.025f);
        }

        RandomizeStatsInstance = null;
    }

    protected void RandomizeStatsSingle()
    {
        int spendablePoints = 4;
        m_ActorInfo.STR = 1;
        m_ActorInfo.MAG = 1;
        m_ActorInfo.DEX = 1;

        int rnd;
        while (spendablePoints > 0)
        {
            rnd = Random.Range(0, 3);
            if (rnd == 0)
            {
                m_ActorInfo.STR++;
            }
            else if (rnd == 1)
            {
                m_ActorInfo.MAG++;
            }
            else if (rnd == 2)
            {
                m_ActorInfo.DEX++;
            }

            spendablePoints--;
        }

        m_txtSTR.text = m_ActorInfo.STR.ToString();
        m_txtMAG.text = m_ActorInfo.MAG.ToString();
        m_txtDEX.text = m_ActorInfo.DEX.ToString();
    }

}
