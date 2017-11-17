﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActorDamageInstance : MonoBehaviour {

    public ActorInstance ParentActor;
    float TimeAlive = 0.1f;
    bool Hit = false;

    [SerializeField]
    BoxCollider2D m_Collider;

    public void Open(ActorInstance instance)
    {
        this.ParentActor = instance;
        this.gameObject.SetActive(true);
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
    List<Enemy> sentTargets;

    void OnTriggerStay2D(Collider2D TargetCollider)
    {
        if (!Hit)
        {

            if (TargetCollider.tag == "Enemy")
            {
                sentTargets = new List<Enemy>();

                collectedColliders = Physics2D.OverlapBoxAll(m_Collider.offset, m_Collider.size, m_Collider.transform.rotation.eulerAngles.z);

                for (int i = 0; i < collectedColliders.Length; i++)
                {
                    if(collectedColliders[i].tag == "Enemy")
                    {
                        sentTargets.Add(collectedColliders[i].GetComponent<HitBox>().EnemyReference);
                        Debug.Log("$$$$ "+collectedColliders[i].GetComponent<HitBox>().EnemyReference);
                    }
                }

                sentTargets.Remove(TargetCollider.GetComponent<HitBox>().EnemyReference);
                sentTargets.Insert(0 ,TargetCollider.GetComponent<HitBox>().EnemyReference);

                SocketClient.Instance.SendMobTookDamage(ParentActor, sentTargets);

                //TODO To be replaced with sound based on the actors weapon.
                int rnd = Random.Range(0, 3);
                AudioControl.Instance.PlayInPosition("sound_hit_" + (rnd + 1), transform.position);

                GameObject tempHit;
                tempHit = ResourcesLoader.Instance.GetRecycledObject("HitEffect");
                tempHit.transform.position = ParentActor.Weapon.transform.position;
                tempHit.GetComponent<HitEffect>().Play();

                Hit = true;
                this.gameObject.SetActive(false);
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
