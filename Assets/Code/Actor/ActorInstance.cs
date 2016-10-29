using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ActorInstance : MonoBehaviour
{

    #region References

    public Transform TorsoBone;

    [SerializeField]
    protected SpriteRenderer m_Head;

    [SerializeField]
    protected SpriteRenderer m_Chest;

    [SerializeField]
    protected SpriteRenderer m_RightArm;
    [SerializeField]
    protected SpriteRenderer m_RightElbow;
    [SerializeField]
    protected SpriteRenderer m_RightFist;

    [SerializeField]
    protected SpriteRenderer m_LeftArm;
    [SerializeField]
    protected SpriteRenderer m_LeftElbow;
    [SerializeField]
    protected SpriteRenderer m_LeftFist;

    [SerializeField]
    protected SpriteRenderer m_RightLeg;
    [SerializeField]
    protected SpriteRenderer m_RightKnee;
    [SerializeField]
    protected SpriteRenderer m_RightFoot;

    [SerializeField]
    protected SpriteRenderer m_LeftLeg;
    [SerializeField]
    protected SpriteRenderer m_LeftKnee;
    [SerializeField]
    protected SpriteRenderer m_LeftFoot;


    [SerializeField]
    protected GameObject NameLabel;

    [SerializeField]
    protected MessageBubbleUI MessageBubble;

    #endregion

    #region Public Parameters

    public ActorInfo Info;
    public ActorMovement MovementController { protected set; get; }
    public bool HideName = false;

    #endregion

    #region Public Methods

    public void RegisterInfo(ActorInfo info)
    {
        this.Info = info;
        this.Info.Instance = this;
    }

    public void UpdateVisual(ActorInfo info)
    {
        RegisterInfo(info);

        UpdateVisual();
    }
    
    public void Reset()
    {
        UpdateVisual(new ActorInfo());
    }

    public void UpdateVisual()
    {
        if (!HideName)
        {
            NameLabel.SetActive(true);
            NameLabel.transform.GetChild(0).GetComponent<Text>().text = Info.Name;
        }
        else
        {
            NameLabel.SetActive(false);
        }

        if (Info.Gender == Gender.Male)
        {
            m_Head.sprite  = ResourcesLoader.Instance.GetSprite("char_base_male_head");
            m_Chest.sprite = ResourcesLoader.Instance.GetSprite("char_base_male_torso");

            m_RightArm.sprite = ResourcesLoader.Instance.GetSprite("char_base_male_arm");
            m_LeftArm.sprite  = ResourcesLoader.Instance.GetSprite("char_base_male_arm");

            m_RightElbow.sprite = ResourcesLoader.Instance.GetSprite("char_base_male_elbow");
            m_LeftElbow.sprite  = ResourcesLoader.Instance.GetSprite("char_base_male_elbow");

            m_RightFist.sprite = ResourcesLoader.Instance.GetSprite("char_base_male_hand");
            m_LeftFist.sprite  = ResourcesLoader.Instance.GetSprite("char_base_male_hand");

            m_RightLeg.sprite = ResourcesLoader.Instance.GetSprite("char_base_male_hip");
            m_LeftLeg.sprite  = ResourcesLoader.Instance.GetSprite("char_base_male_hip");

            m_RightKnee.sprite = ResourcesLoader.Instance.GetSprite("char_base_male_knee");
            m_LeftKnee.sprite  = ResourcesLoader.Instance.GetSprite("char_base_male_knee");

            m_RightFoot.sprite = ResourcesLoader.Instance.GetSprite("char_base_male_foot");
            m_LeftFoot.sprite  = ResourcesLoader.Instance.GetSprite("char_base_male_foot");
        }   
        else
        {
            m_Head.sprite = ResourcesLoader.Instance.GetSprite("char_base_female_head");
            m_Chest.sprite = ResourcesLoader.Instance.GetSprite("char_base_female_torso");

            m_RightArm.sprite = ResourcesLoader.Instance.GetSprite("char_base_female_arm");
            m_LeftArm.sprite = ResourcesLoader.Instance.GetSprite("char_base_female_arm");

            m_RightElbow.sprite = ResourcesLoader.Instance.GetSprite("char_base_female_elbow");
            m_LeftElbow.sprite = ResourcesLoader.Instance.GetSprite("char_base_female_elbow");

            m_RightFist.sprite = ResourcesLoader.Instance.GetSprite("char_base_female_hand");
            m_LeftFist.sprite = ResourcesLoader.Instance.GetSprite("char_base_female_hand");

            m_RightLeg.sprite = ResourcesLoader.Instance.GetSprite("char_base_female_hip");
            m_LeftLeg.sprite = ResourcesLoader.Instance.GetSprite("char_base_female_hip");

            m_RightKnee.sprite = ResourcesLoader.Instance.GetSprite("char_base_female_knee");
            m_LeftKnee.sprite = ResourcesLoader.Instance.GetSprite("char_base_female_knee");

            m_RightFoot.sprite = ResourcesLoader.Instance.GetSprite("char_base_female_foot");
            m_LeftFoot.sprite = ResourcesLoader.Instance.GetSprite("char_base_female_foot");
        }
    }

    public void RegisterMovementController(ActorMovement controller)
    {
        MovementController = controller;
        SocketClient.Instance.Subscribe(Info.ID ,controller);
    }

    public void ChatBubble(string Message)
    {
        if (Message.Length < 65)
        {
            MessageBubble.GetComponent<MessageBubbleUI>().PopMessage(Message);
        }
    }
    #endregion



}
