using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyProjectile : EnemyDamageInstance
{
    [SerializeField]
    protected float Speed;

    [SerializeField]
    protected float InitTimeAlive;

    [SerializeField]
    protected string DeathEffect;

    [SerializeField]
    protected string HitSound;

    protected Rigidbody2D m_Rigid;

    protected virtual void Awake()
    {
        m_Rigid = GetComponent<Rigidbody2D>();
    }

    protected override void OnEnable()
    {
        TimeAlive = InitTimeAlive;
        Hit = false;
    }

    protected void FixedUpdate()
    {
        m_Rigid.position += ((Vector2)transform.right) * Speed * Time.deltaTime;
    }

    protected override void OnTriggerStay2D(Collider2D TargetCollider)
    {
        if (!Hit && TargetCollider.tag != "Enemy" && TargetCollider.GetComponent<Enemy>() == null)
        {
            ResourcesLoader.Instance.GetRecycledObject(DeathEffect).transform.position = transform.position;
            AudioControl.Instance.PlayInPosition(HitSound,transform.position);

            if (TargetCollider.tag == "Actor")
            {
                ActorInstance actorInstance = TargetCollider.GetComponent<ActorInstance>();

                if (actorInstance.Info.ID == LocalUserInfo.Me.ClientCharacter.ID)
                {
                    actorInstance.InputController.TookSpellDamage(this);
                }
            }

            Hit = true;
            this.gameObject.SetActive(false);
        }
    }
}
