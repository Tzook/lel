using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamageInstance  : MonoBehaviour {

    public string PopSound;
    public Enemy ParentEnemy;
    public string ActionKey;
    public string ActionValue;

    protected float TimeAlive = 0.1f;
    protected bool Hit = false;

    [SerializeField]
    protected BoxCollider2D m_Collider;


    public virtual void SetInfo(Enemy instance, string actionKey, string actionValue = "")
    {
        this.ParentEnemy = instance;
        this.ActionKey = actionKey;
        this.ActionValue = actionValue;
        this.gameObject.SetActive(true);

        if (!string.IsNullOrEmpty(PopSound))
        {
            AudioControl.Instance.PlayInPosition(PopSound, transform.position);
        }
    }

    protected virtual void OnEnable()
    {
        TimeAlive = 0.1f;
        Hit = false;
    }

    protected virtual void Update()
    {
        if (TimeAlive > 0f)
        {
            TimeAlive -= 1f * Time.deltaTime;
        }
        else
        {
            this.gameObject.SetActive(false);
        }
    }

    protected virtual void OnTriggerStay2D(Collider2D TargetCollider)
    {
        if (!Hit)
        {

            if (TargetCollider.tag == "Actor")
            {
                ActorInstance actorInstance = TargetCollider.GetComponent<ActorInstance>();
                
                if(actorInstance.Info.ID == LocalUserInfo.Me.ClientCharacter.ID)
                {
                    actorInstance.InputController.TookSpellDamage(this);
                }

                Hit = true;
                this.gameObject.SetActive(false);
            }

        }
    }

}
