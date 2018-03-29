﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamageInstanceContinous : EnemyDamageInstance
{

    public float InitTimeAlive = 1f;
    public float HitInterval = 1f;
    public bool Shake = false;

    float HitTime;

    protected override void OnEnable()
    {
        TimeAlive = InitTimeAlive;
        Hit = false;

        if (Shake)
        {
            GameCamera.Instance.Shake(10f, 10f);
        }
    }

    protected override void Update()
    {
        if (TimeAlive > 0f)
        {
            TimeAlive -= 1f * Time.deltaTime;
        }
        else
        {
            this.gameObject.SetActive(false);
        }

        if (HitTime > 0f)
        {
            HitTime -= 1f * Time.deltaTime;
        }
        else
        {
            Hit = false;
        }
    }

    protected override void OnTriggerStay2D(Collider2D TargetCollider)
    {
        if (!Hit)
        {

            if (TargetCollider.tag == "Actor")
            {
                ActorInstance actorInstance = TargetCollider.GetComponent<ActorInstance>();

                if (actorInstance.Info.ID == LocalUserInfo.Me.ClientCharacter.ID)
                {
                    actorInstance.InputController.TookSpellDamage(this);
                }

                Hit = true;
                HitTime = HitInterval;
            }

        }
    }
}
