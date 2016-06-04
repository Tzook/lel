using UnityEngine;
using System.Collections;

public class ActorInstance : MonoBehaviour
{

    #region References

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

    #endregion

    #region Public Parameters

    public ActorInfo Info;
    public ActorMovement MovementController { protected set; get; }

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
        if(Info.Gender == Gender.Male)
        {
            m_Head.sprite  = SM.Resources.GetSprite("MaleHead");
            m_Chest.sprite = SM.Resources.GetSprite("MaleChest");

            m_RightArm.sprite = SM.Resources.GetSprite("MaleArm");
            m_LeftArm.sprite  = SM.Resources.GetSprite("MaleArm");

            m_RightElbow.sprite = SM.Resources.GetSprite("MaleElbow");
            m_LeftElbow.sprite  = SM.Resources.GetSprite("MaleElbow");

            m_RightFist.sprite = SM.Resources.GetSprite("Fist");
            m_LeftFist.sprite  = SM.Resources.GetSprite("Fist");

            m_RightLeg.sprite = SM.Resources.GetSprite("Leg");
            m_LeftLeg.sprite  = SM.Resources.GetSprite("Leg");

            m_RightKnee.sprite = SM.Resources.GetSprite("Knee");
            m_LeftKnee.sprite  = SM.Resources.GetSprite("Knee");

            m_RightFoot.sprite = SM.Resources.GetSprite("Foot");
            m_LeftFoot.sprite  = SM.Resources.GetSprite("Foot");
        }   
        else
        {
            m_Head.sprite = SM.Resources.GetSprite("FemaleHead");
            m_Chest.sprite = SM.Resources.GetSprite("FemaleChest");

            m_RightArm.sprite = SM.Resources.GetSprite("FemaleArm");
            m_LeftArm.sprite = SM.Resources.GetSprite("FemaleArm");

            m_RightElbow.sprite = SM.Resources.GetSprite("FemaleElbow");
            m_LeftElbow.sprite = SM.Resources.GetSprite("FemaleElbow");

            m_RightFist.sprite = SM.Resources.GetSprite("Fist");
            m_LeftFist.sprite = SM.Resources.GetSprite("Fist");

            m_RightLeg.sprite = SM.Resources.GetSprite("Leg");
            m_LeftLeg.sprite = SM.Resources.GetSprite("Leg");

            m_RightKnee.sprite = SM.Resources.GetSprite("Knee");
            m_LeftKnee.sprite = SM.Resources.GetSprite("Knee");

            m_RightFoot.sprite = SM.Resources.GetSprite("Foot");
            m_LeftFoot.sprite = SM.Resources.GetSprite("Foot");
        }
    }

    public void RegisterMovementController(ActorMovement controller)
    {
        MovementController = controller;
        SM.SocketClient.Subscribe(Info.ID ,controller);
    }

    #endregion



}
