using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorDamageInstance : MonoBehaviour {

    public ActorInstance ParentActor;
    float TimeAlive = 1f;
    bool Hit = false;


    public void Open(ActorInstance instance)
    {
        this.ParentActor = instance;
        this.gameObject.SetActive(true);
    }

    void OnEnable()
    {
        TimeAlive = 1f;
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
   

    void OnTriggerStay2D(Collider2D TargetCollider)
    {
        if (!Hit)
        {
            if (TargetCollider.tag == "Enemy")
            {
                SocketClient.Instance.SendMobTookDamage(ParentActor, TargetCollider.GetComponent<HitBox>().EnemyReference);
                Debug.Log("SUP");
                Hit = true;
                this.gameObject.SetActive(false);
            }
        }
    }
}
