using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileArrow : MonoBehaviour {

    [SerializeField]
    float MovementSpeed = 15f;

    [SerializeField]
    float DecayTime;

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

    public bool InFlight = false;

    public bool TriggerHit;

    public ActorInstance ParentActor;

    public string PrimaryAbilitySourceKey;

    private HitBox CurrentHitbox;
    private float CurrentMaxFlightTime;

    private uint? AttackIdCounter;

    public void Launch(ActorInstance parent, string primaryAbilitySourceKey, bool triggerHit, uint? attackIdCounter, float speed = 15f)
    {
        transform.parent = null;

        this.ParentActor = parent;

        CurrentMaxFlightTime = MaxFlightTime;
        TriggerHit = triggerHit;
        AttackIdCounter = attackIdCounter;

        InFlight = true;
        m_Particles.Play();

        PrimaryAbilitySourceKey = primaryAbilitySourceKey;

        if(Trail != null)
        {
            Trail.Clear();
        }
    }

    void FixedUpdate()
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

    void OnTriggerEnter2D(Collider2D TargetCollider)
    {
        if (InFlight)
        {
            DevAbility dPA = Content.Instance.GetAbility(PrimaryAbilitySourceKey);

            m_Particles.Stop();
            InFlight = false;

            if (TargetCollider.tag == "Enemy")
            {
                CurrentHitbox = TargetCollider.GetComponent<HitBox>();

                AudioControl.Instance.PlayInPosition(dPA.ProjectileHitSound, transform.position);

                if (TriggerHit)
                {
                    //TODO Fix me
                    List<string> tempList = new List<string>();
                    tempList.Add(TargetCollider.GetComponent<HitBox>().EnemyReference.Info.ID);
                    SocketClient.Instance.SendUsedPrimaryAbility(tempList, (uint)AttackIdCounter);
                }
            }
            else if (TargetCollider.tag == "HitEntity")
            {
                TargetCollider.GetComponent<HittableEntity>().Hurt(PrimaryAbilitySourceKey);
            }
            else
            {
                AudioControl.Instance.PlayInPosition(dPA.ProjectileWallSound, transform.position);

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

            if (dPA.ProjectileStayAfterHit)
            {
                if (TargetCollider.gameObject.activeInHierarchy)
                {
                    transform.SetParent(TargetCollider.transform, true);
                }

                StartCoroutine(DeathRoutine());
            }
            else
            {
                Shut();
            }
        }
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
