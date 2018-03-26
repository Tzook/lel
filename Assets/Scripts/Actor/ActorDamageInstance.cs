using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActorDamageInstance : MonoBehaviour {

    public ActorInstance ParentActor;

    [SerializeField]
    string ActionKey;

    [SerializeField]
    string ActionValue;

    float TimeAlive = 0.1f;
    bool Hit = false;

    [SerializeField]
    BoxCollider2D m_Collider;

    DevAbility CurrentAbility;

    public void Open(ActorInstance instance, string actionKey, string actionValue = "")
    {
        this.ParentActor = instance;
        this.ActionKey = actionKey;
        this.ActionValue = actionValue;
        this.gameObject.SetActive(true);

        CurrentAbility = Content.Instance.GetAbility(actionKey);
    }

    void OnEnable()
    {
        TimeAlive = 0.1f;
        Hit = false;
    }

    void Update()
    {
        if(TimeAlive > 0f)
        {
            TimeAlive -= 1f * Time.deltaTime;
        }
        else
        {
            this.gameObject.SetActive(false);
        }
    }

    Collider2D[] collectedColliders;

    void OnTriggerStay2D(Collider2D TargetCollider)
    {
        if (!Hit)
        {
            if (CurrentAbility != null && TargetCollider.tag == CurrentAbility.hitTargetEnumState.ToString())
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

                    LocalUserInfo.Me.ClientCharacter.Instance.InputController.ColliderHitMobs(sentTargets, ActionKey, ActionValue);

                    Hit = true;
                    this.gameObject.SetActive(false);
                }
                else if(TargetCollider.tag == "Actor" && TargetCollider.GetComponent<ActorInstance>().Info.ID != LocalUserInfo.Me.ClientCharacter.ID)
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

                    LocalUserInfo.Me.ClientCharacter.Instance.InputController.ColliderHitPlayers(sentTargets, ActionKey, ActionValue);

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

                    LocalUserInfo.Me.ClientCharacter.Instance.InputController.ColliderHitMobs(sentTargets, ActionKey, ActionValue);

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
