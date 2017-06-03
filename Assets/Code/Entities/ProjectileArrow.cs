using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileArrow : MonoBehaviour {

    [SerializeField]
    float MovementSpeed = 15f;

    [SerializeField]
    float DecayTime;

    [SerializeField]
    Rigidbody2D m_Rigid;

    [SerializeField]
    Animator m_Anim;

    [SerializeField]
    ParticleSystem m_Particles;

    public bool InFlight = false;

    public bool TriggerHit;

    public ActorInstance ParentActor;

    private HitBox CurrentHitbox;
    private float CurrentDecay;

    public void Launch(ActorInstance parent ,bool triggerHit = false ,float speed = 15f)
    {
        this.ParentActor = parent;

        CurrentDecay = DecayTime;
        MovementSpeed = speed;
        TriggerHit = triggerHit;

        InFlight = true;
        m_Particles.Play();
    }

    void FixedUpdate()
    {
        if (InFlight)
        {
            if(CurrentDecay > 0)
            {
                CurrentDecay -= 1f * Time.deltaTime;
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
            m_Particles.Stop();
            InFlight = false;

            if (TargetCollider.tag == "Enemy")
            {
                CurrentHitbox = TargetCollider.GetComponent<HitBox>();

                AudioControl.Instance.Play("sound_arrow_hit");

                if (TriggerHit)
                {
                    SocketClient.Instance.SendMobTookDamage(ParentActor, TargetCollider.GetComponent<HitBox>().EnemyReference);
                }
            }
            else
            {
                AudioControl.Instance.Play("sound_arrow_blunt");
                m_Anim.SetTrigger("Bounce");
            }

            transform.SetParent(TargetCollider.transform, true);

            StartCoroutine(DeathRoutine());
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
