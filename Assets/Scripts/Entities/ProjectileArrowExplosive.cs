using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileArrowExplosive : MonoBehaviour {


    [SerializeField]
    float MovementSpeed = 15f;

    [SerializeField]
    float MaxFlightTime;

    [SerializeField]
    Rigidbody2D m_Rigid;

    [SerializeField]
    Animator m_Anim;

    [SerializeField]
    ParticleSystem m_Particles;

    [SerializeField]
    string ImpactEffect;

    [SerializeField]
    TrailRenderer Trail;

    [SerializeField]
    bool ProjectileStayAfterHit;

    [SerializeField]
    bool Physics = false;

    [SerializeField]
    string ColliderPrefab;

    [SerializeField]
    protected float DecayTime = 0.1f;

    public string ActionKey;
    public string ActionValue;
    public uint AttackIDCounter;

    ActorInstance ParentActor;

    public bool InFlight = false;

    public bool TriggerHit;

    private GameObject CurrentObject;
    private float CurrentMaxFlightTime;

    float ChargeValue = 1f;

    DevAbility CurrentAbility;

    public void SetInfo(ActorInstance parent, string actionKey, string actionValue, bool triggerHit, uint attackIdCounter, float chargeValue = 1f, float speed = 15f)
    {
        ActionKey = actionKey;
        ActionValue = actionValue;
        AttackIDCounter = attackIdCounter;
        CurrentAbility = Content.Instance.GetAbility(actionKey);
        ParentActor = parent;

        transform.parent = null;

        CurrentMaxFlightTime = MaxFlightTime;
        TriggerHit = triggerHit;

        InFlight = true;

        if (m_Particles != null) m_Particles.Play();

        ChargeValue = chargeValue;

        if (Trail != null)
        {
            Trail.Clear();
        }
    }

    protected virtual void FixedUpdate()
    {
        if (InFlight)
        {
            if (CurrentMaxFlightTime > 0)
            {
                CurrentMaxFlightTime -= 1f * Time.deltaTime;
            }
            else
            {
                Shut();
            }

            m_Rigid.position += ((Vector2)transform.right) * MovementSpeed * Time.deltaTime;
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D TargetCollider)
    {
        if (InFlight)
        {
            bool TargetHit = false;
            string TargetTag = "Enemy";

            if (ActionKey == "spell")
            {
                TargetTag = Content.Instance.GetPlayerSpell(ActionValue).hitTargetEnumState.ToString();
            }
            else
            {
                TargetTag = CurrentAbility.hitTargetEnumState.ToString();
            }





            if (TargetCollider.tag == "Enemy")
            {
                TargetHit = true;

                if (TriggerHit)
                {

                    Explode();
                }
            }
            else if (TargetCollider.tag == "Actor")
            {
                if (TargetCollider.GetComponent<ActorInstance>() != ParentActor)
                {
                    TargetHit = true;

                    if (TriggerHit)
                    {

                        Explode();
                    }
                }

            }
            else if (TargetCollider.tag == "HitEntity")
            {
                Explode();
                TargetCollider.GetComponent<HittableEntity>().Hurt(ActionKey);
                TargetHit = true;
            }
            else
            {
                Explode();
                TargetHit = true;
                if (m_Anim != null)
                {
                    m_Anim.SetTrigger("Bounce");
                }
            }

            if (TargetHit)
            {
                HitObject(TargetCollider);
            }
        }
    }

    void Explode()
    {
        GameObject tempObj = ResourcesLoader.Instance.GetRecycledObject(ColliderPrefab);
        tempObj.transform.position = transform.position;
        tempObj.GetComponent<ActorDamageInstance>().SetInfo(ParentActor, ActionKey, ActionValue, AttackIDCounter);
    }

    void HitObject(Collider2D TargetCollider)
    {

        m_Particles.Stop();
        InFlight = false;
        CurrentObject = TargetCollider.gameObject;

        if (!string.IsNullOrEmpty(ImpactEffect))
        {
            GameObject impactEffect = ResourcesLoader.Instance.GetRecycledObject(ImpactEffect);
            impactEffect.transform.position = transform.position;
            impactEffect.transform.right = ParentActor.transform.position - transform.position;
            impactEffect.transform.SetParent(TargetCollider.transform, true);
            impactEffect.transform.localScale = Vector3.one;
        }

        if (ProjectileStayAfterHit)
        {
            if (TargetCollider.gameObject.activeInHierarchy)
            {
                transform.SetParent(TargetCollider.transform, true);
            }

            if (this.gameObject.activeInHierarchy)
            {
                StartCoroutine(DeathRoutine());
            }
            else
            {
                Shut();
            }
        }
        else
        {
            Shut();
        }
    }

    protected void OnEnable()
    {
    }

    protected void Update()
    {
        if (Physics && InFlight)
        {
            //if (ChargeValue < 1f)
            //{
            if (ChargeValue > 0f)
            {
                ChargeValue -= 1f * Time.deltaTime;


            }
            else
            {
                transform.Rotate(ParentActor.TorsoBone.localScale.x * transform.forward * -50f * Time.deltaTime);
            }
            //}
        }
    }

    protected void OnTriggerStay2D(Collider2D TargetCollider)
    {
    }

    private IEnumerator DeathRoutine()
    {
        float tempTime = DecayTime;

        while (tempTime > 0)
        {
            tempTime -= 1f * Time.deltaTime;

            if (transform.parent == null || !transform.parent.gameObject.activeInHierarchy || !CurrentObject.activeInHierarchy || (CurrentObject.GetComponent<Enemy>() != null && CurrentObject.GetComponent<Enemy>().Dead))
            {

                break;
            }

            yield return 0;
        }

        Shut();
    }

    public void Shut()
    {
        transform.SetParent(null);
        this.gameObject.SetActive(false);
    }
}
