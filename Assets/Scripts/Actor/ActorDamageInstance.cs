using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActorDamageInstance : MonoBehaviour {

    public ActorInstance ParentActor;

    [SerializeField]
    protected string ActionKey;

    [SerializeField]
    protected string ActionValue;

    [SerializeField]
    protected float DecayTime = 0.1f;

    protected bool Hit = false;

    [SerializeField]
    protected BoxCollider2D m_Collider;

    protected DevAbility CurrentAbility;

    protected uint AttackIdCounter;

    protected Collider2D[] collectedColliders;


    public virtual void SetInfo(ActorInstance instance, string actionKey, string actionValue)
    {
        this.ParentActor = instance;
        this.ActionKey = actionKey;
        this.ActionValue = actionValue;

        this.gameObject.SetActive(true);

        CurrentAbility = Content.Instance.GetAbility(actionKey);

        Hit = false;
    }

    public virtual void SetInfo(ActorInstance instance, string actionKey, string actionValue, uint attackIdCounter)
    {
        SetInfo(instance, actionKey, actionValue);
        this.AttackIdCounter = attackIdCounter;
    }

    protected virtual void OnEnable()
    {
        DecayTime = 0.1f;
    }

    protected virtual void Update()
    {
        if(DecayTime > 0f)
        {
            DecayTime -= 1f * Time.deltaTime;
        }
        else
        {
            this.gameObject.SetActive(false);
        }
    }

    protected virtual void OnTriggerStay2D(Collider2D TargetCollider)
    {
        HandleCollision(TargetCollider);
    }

    public virtual void HandleCollision(Collider2D TargetCollider)
    {
        
        if (!Hit)
        {
            string TargetTag = "Enemy";

            if (ActionKey == "spell")
            {
                TargetTag = Content.Instance.GetPlayerSpell(ActionValue).hitTargetEnumState.ToString();
            }
            else
            {
                TargetTag = CurrentAbility.hitTargetEnumState.ToString();
            }


            if (TargetTag == TargetCollider.tag.ToString())
            {
                if (TargetCollider.tag == "Enemy")
                {
                    List<Enemy> sentTargets = new List<Enemy>();

                    collectedColliders = Physics2D.OverlapBoxAll(m_Collider.transform.position, m_Collider.size * 2f, 0f);

                    //float minDistance = Mathf.Infinity;
                    for (int i = 0; i < collectedColliders.Length; i++)
                    {
                        if (collectedColliders[i].tag == "Enemy")
                        {
                            sentTargets.Add(collectedColliders[i].GetComponent<HitBox>().EnemyReference);

                            //if(Mathf.Abs(sentTargets[sentTargets.Count - 1].transform.position.x - transform.position.x) < minDistance)
                            //{
                            //    minDistance = Mathf.Abs(sentTargets[sentTargets.Count - 1].transform.position.x - transform.position.x);
                            //    sentTargets.Remove(TargetCollider.GetComponent<HitBox>().EnemyReference);
                            //    sentTargets.Insert(0, TargetCollider.GetComponent<HitBox>().EnemyReference);
                            //}
                        }
                    }

                    sentTargets.Remove(TargetCollider.GetComponent<HitBox>().EnemyReference);
                    sentTargets.Insert(0, TargetCollider.GetComponent<HitBox>().EnemyReference);



                    LocalUserInfo.Me.ClientCharacter.Instance.InputController.ColliderHitMobs(sentTargets, ActionKey, ActionValue, AttackIdCounter);

                    Hit = true;
                    this.gameObject.SetActive(false);
                }
                else if (TargetCollider.tag == "Actor"  && TargetCollider.GetComponent<ActorInstance>().Info.ID != LocalUserInfo.Me.ClientCharacter.ID)
                {
                    
                    List<ActorInstance> sentTargets = new List<ActorInstance>();

                    collectedColliders = Physics2D.OverlapBoxAll(m_Collider.transform.position, m_Collider.size * 2f, 0f);

                    for (int i = 0; i < collectedColliders.Length; i++)
                    {
                        if (collectedColliders[i].tag == "Actor")
                        {
                            sentTargets.Add(collectedColliders[i].GetComponent<ActorInstance>());
                        }
                    }

                    sentTargets.Remove(TargetCollider.GetComponent<ActorInstance>());
                    sentTargets.Insert(0, TargetCollider.GetComponent<ActorInstance>());

                    LocalUserInfo.Me.ClientCharacter.Instance.InputController.ColliderHitPlayers(sentTargets, ActionKey, ActionValue, AttackIdCounter);

                    Hit = true;
                    this.gameObject.SetActive(false);
                }
                else if (TargetCollider.tag == "HitEntity")
                {
                    TargetCollider.GetComponent<HittableEntity>().Hurt(ActionKey);

                    Hit = true;
                    this.gameObject.SetActive(false);
                }
            }
            else
            {
                if (TargetCollider.tag == "Enemy")
                {
                    List<Enemy> sentTargets = new List<Enemy>();

                    collectedColliders = Physics2D.OverlapBoxAll(m_Collider.transform.position, m_Collider.size * 2f, 0f);

                    for (int i = 0; i < collectedColliders.Length; i++)
                    {
                        if (collectedColliders[i].tag == "Enemy")
                        {
                            sentTargets.Add(collectedColliders[i].GetComponent<HitBox>().EnemyReference);
                        }
                    }

                    sentTargets.Remove(TargetCollider.GetComponent<HitBox>().EnemyReference);
                    sentTargets.Insert(0, TargetCollider.GetComponent<HitBox>().EnemyReference);

                    LocalUserInfo.Me.ClientCharacter.Instance.InputController.ColliderHitMobs(sentTargets, ActionKey, ActionValue, AttackIdCounter);

                    Hit = true;
                    this.gameObject.SetActive(false);
                }
                else if (TargetCollider.tag == "HitEntity")
                {
                    TargetCollider.GetComponent<HittableEntity>().Hurt(ActionKey);

                    Hit = true;
                    this.gameObject.SetActive(false);
                }
            }

        }
    }

    //void OnTriggerStay2D(Collider2D TargetCollider)
    //{
    //    if (!Hit)
    //    {
    //        if (TargetCollider.tag == "Enemy")
    //        {
    //            SocketClient.Instance.SendMobTookDamage(ParentActor, TargetCollider.GetComponent<HitBox>().EnemyReference);

    //            //TODO To be replaced with sound based on the actors weapon.
    //            int rnd = Random.Range(0, 3);
    //            AudioControl.Instance.PlayInPosition("sound_hit_" + (rnd + 1), transform.position);

    //            GameObject tempHit;
    //            tempHit = ResourcesLoader.Instance.GetRecycledObject("HitEffect");
    //            tempHit.transform.position = ParentActor.Weapon.transform.position;
    //            tempHit.GetComponent<HitEffect>().Play();

    //            Hit = true;
    //            this.gameObject.SetActive(false);
    //        }
    //    }
    //}
}
