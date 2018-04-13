using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileArrow : ActorDamageInstance {

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

    public bool InFlight = false;

    public bool TriggerHit;

    private HitBox CurrentHitbox;
    private float CurrentMaxFlightTime;

    public void SetInfo(ActorInstance parent, string actionKey, string actionValue, bool triggerHit, uint attackIdCounter, float speed = 15f)
    {
        base.SetInfo(parent ,actionKey ,actionValue , attackIdCounter);

        transform.parent = null;

        CurrentMaxFlightTime = MaxFlightTime;
        TriggerHit = triggerHit;

        InFlight = true;
        m_Particles.Play();

        if (Trail != null)
        {
            Trail.Clear();
        }
    }

    protected virtual void FixedUpdate()
    {
        if (InFlight)
        {
            if(CurrentMaxFlightTime > 0)
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
            m_Particles.Stop();
            InFlight = false;

            if (TargetCollider.tag == "Enemy")
            {
                CurrentHitbox = TargetCollider.GetComponent<HitBox>();

                if (TriggerHit)
                {
                    HandleCollision(TargetCollider);
                    //List<string> tempList = new List<string>();
                    //tempList.Add(TargetCollider.GetComponent<HitBox>().EnemyReference.Info.ID);
                    //SocketClient.Instance.SendUsedPrimaryAbility(tempList, (uint)AttackIdCounter);
                }
            }
            else if (TargetCollider.tag == "HitEntity")
            {
                TargetCollider.GetComponent<HittableEntity>().Hurt(ActionKey);
            }
            else
            {
                if (m_Anim != null)
                {
                    m_Anim.SetTrigger("Bounce");
                }
            }

            if(!string.IsNullOrEmpty(ImpactEffect))
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
            }
            else
            {
                Shut();
            }
        }
    }

    protected override void OnEnable()
    {
    }

    protected override void Update()
    {
    }

    protected override void OnTriggerStay2D(Collider2D TargetCollider)
    {
    }

    private IEnumerator DeathRoutine()
    {
        float tempTime = DecayTime;

        while (tempTime > 0)
        {
            tempTime -= 1f * Time.deltaTime;

            if (transform.parent == null || !transform.parent.gameObject.activeInHierarchy || (CurrentHitbox != null && CurrentHitbox.EnemyReference.Dead))
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
