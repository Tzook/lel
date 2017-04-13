using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyMoving : Enemy
{
    public AIAction CurrentAction;

    protected Coroutine AIRoutineInstance;

    [SerializeField]
    protected float MovementSpeed = 1f;

    public override void SetAION()
    {
        base.SetAION();
        Rigid.bodyType = RigidbodyType2D.Dynamic;

        AIRoutineInstance = StartCoroutine(AIRoutine());
    }

    public override void SetAIOFF()
    {
        base.SetAIOFF();
        Rigid.bodyType = RigidbodyType2D.Kinematic;

        if (AIRoutineInstance != null)
        {
            StopCoroutine(AIRoutineInstance);
            AIRoutineInstance = null;
        }

        StopAllCoroutines();
    }

    Vector3 LastSentPosition;
    void FixedUpdate()
    {
        if(Game.Instance.isBitch && LastSentPosition != transform.position)
        {
            LastSentPosition = transform.position;
            SocketClient.Instance.SendMobMove(Info.ID, transform.position.x, transform.position.y);
        }
    }

    #region AI

    public virtual IEnumerator AIRoutine()
    {
        int rndDecision;
        while (true)
        {
            //MAKE DECISION
            if (CurrentAction == AIAction.Thinking)
            {
                rndDecision = Random.Range(0, 5);

                if(rndDecision == 0 || rndDecision == 1)
                {
                    CurrentAction = AIAction.WanderingLeft;
                }
                else if (rndDecision == 2 || rndDecision == 3)
                {
                    CurrentAction = AIAction.WanderingRight;
                }
                else
                {
                    CurrentAction = AIAction.Idle;
                }
            }

            //ACT
            switch(CurrentAction)
            {
                case AIAction.Thinking:
                    {
                        yield return 0;
                        break;
                    }
                case AIAction.Idle:
                    {
                        yield return StartCoroutine(IdleRoutine());
                        break;
                    }
                case AIAction.WanderingLeft:
                    {
                        yield return StartCoroutine(WanderLeftRotuine());
                        break;
                    }
                case AIAction.WanderingRight:
                    {
                        yield return StartCoroutine(WanderRightRotuine());
                        break;
                    }
            }
        }
    }

    protected IEnumerator IdleRoutine()
    {
        float t = Random.Range(0.5f, 5f);

        StandStill();
        
        while (t>0f)
        {
            t -= 1f * Time.deltaTime;
            
            yield return 0;
        }

        CurrentAction = AIAction.Thinking;
    }

    protected IEnumerator WanderLeftRotuine()
    {
        float t = Random.Range(0.5f, 5f);

        while (t > 0f)
        {
            t -= 1f * Time.deltaTime;

            WalkLeft();

            yield return 0;
        }

        CurrentAction = AIAction.Thinking;
    }

    protected IEnumerator WanderRightRotuine()
    {
        float t = Random.Range(0.5f, 5f);

        while (t > 0f)
        {
            t -= 1f * Time.deltaTime;

            WalkRight();

            yield return 0;
        }

        CurrentAction = AIAction.Thinking;
    }

    #endregion

    #region Basic Actions

    public void StandStill()
    {
        Anim.SetBool("Walk", false);    
    }

    public void WalkLeft()
    {
        Rigid.position += -Vector2.right * MovementSpeed * Time.deltaTime;
        Body.localScale = new Vector3( -initScale.x, initScale.y, initScale.z);

        Anim.SetBool("Walk", true);
    }

    public void WalkRight()
    {
        Rigid.position += Vector2.right *  MovementSpeed * Time.deltaTime;
        Body.localScale = new Vector3( initScale.x, initScale.y, initScale.z);

        Anim.SetBool("Walk", true);
    }

    public override void Hurt(ActorInstance actor, int damage = 0 )
    {
        base.Hurt(actor,damage);

        if (Game.Instance.isBitch)
        {

            if (actor.transform.position.x < transform.position.x)
            {
                Rigid.AddForce(2f * transform.right, ForceMode2D.Impulse);
            }
            else
            {
                Rigid.AddForce(2f * -transform.right, ForceMode2D.Impulse);
            }
        }
    }

    public override void Death()
    {
        SetAIOFF();

        Rigid.velocity = Vector2.zero;

        base.Death();
    }

    #endregion

    #region UnderControl

    public override void UpdateMovement(float x, float y)
    {
        transform.position = Vector3.Lerp(transform.position, new Vector3(x, y, transform.position.z), Time.deltaTime * 2f);

        if(transform.position.x != x)
        {
            Anim.SetBool("Walk", true);

            if (transform.position.x < x)
            {
                Body.localScale = new Vector3(initScale.x, initScale.y, initScale.z);
            }
            else
            {
                Body.localScale = new Vector3(-initScale.x, initScale.y, initScale.z);
            }
        }
        else
        {
            Anim.SetBool("Walk", false);
        }
    }

    #endregion

    public enum AIAction
    {
        Thinking, Idle, WanderingLeft, WanderingRight, Chasing, Flee
    }
}
