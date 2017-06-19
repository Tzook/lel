using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyMoving : Enemy
{
    public AIAction CurrentAction;

    protected Coroutine AIRoutineInstance;

    [SerializeField]
    protected float MovementSpeed = 1f;

    [SerializeField]
    protected float MaxChaseDistance = 20f;

    [SerializeField]
    protected BoxCollider2D Collider;

    protected RaycastHit2D SideRayRight;
    protected RaycastHit2D SideRayLeft;

    protected LayerMask GroundLayerMask = 0 << 0 | 1;

    protected Coroutine CurrentActionRoutine;

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
        Rigid.velocity = Vector2.zero;

        StopCurrentActionRoutine();

        if (AIRoutineInstance != null)
        {
            StopCoroutine(AIRoutineInstance);
            AIRoutineInstance = null;
        }

        StopAllCoroutines();
    }

    public override void SetTarget(ActorInstance target)
    {
        base.SetTarget(target);

        if(target == null)
        {
            StopCurrentActionRoutine();
        }
    }

    public bool isRightBlocked()
    {
        SideRayRight = Physics2D.Raycast(transform.position, transform.right, 1f, GroundLayerMask);

        return (SideRayRight.normal.x < -0.3f || SideRayRight.normal.x > 0.3f);
    }

    public bool isLeftBlocked()
    {
        SideRayLeft = Physics2D.Raycast(transform.position, -transform.right, 1f, GroundLayerMask);

        return (SideRayLeft.normal.x < -0.3f || SideRayLeft.normal.x > 0.3f);
    }

    protected Vector3 LastSentPosition;
    void FixedUpdate()
    {
        if(Game.Instance.isBitch && !Dead && LastSentPosition != transform.position)
        {
            LastSentPosition = transform.position;
            SocketClient.Instance.SendMobMove(Info.ID, transform.position.x, transform.position.y);
        }
    }

    void LateUpdate()
    {
        if(m_HealthBar != null)
        {
            m_HealthBar.transform.position = Vector2.Lerp(m_HealthBar.transform.position, new Vector2(transform.position.x,m_HitBox.bounds.max.y) , Time.deltaTime * 3f);
        }
    }

    #region AI

    public void StopCurrentActionRoutine()
    {
        if (CurrentActionRoutine != null)
        {
            StopCoroutine(CurrentActionRoutine);
            CurrentActionRoutine = null;
            CurrentAction = AIAction.Thinking;
        }
    }

    public virtual IEnumerator AIRoutine()
    {
        int rndDecision;
        while (true)
        {
            //MAKE DECISION
            if (CurrentAction == AIAction.Thinking)
            {
                if (CurrentTarget != null)
                {
                    CurrentAction = AIAction.Chasing;
                }
                else
                {
                    rndDecision = Random.Range(0, 5);

                    if (rndDecision == 0 || rndDecision == 1)
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
            }

            //ACT
            switch(CurrentAction)
            {
                case AIAction.Thinking:
                    {
                        CurrentActionRoutine = null;

                        yield return 0;
                        break;
                    }
                case AIAction.Idle:
                    {
                        CurrentActionRoutine = StartCoroutine(IdleRoutine());
                        break;
                    }
                case AIAction.WanderingLeft:
                    {
                        CurrentActionRoutine = StartCoroutine(WanderLeftRotuine());
                        break;
                    }
                case AIAction.WanderingRight:
                    {
                        CurrentActionRoutine = StartCoroutine(WanderRightRotuine());
                        break;
                    }
                case AIAction.Chasing:
                    {
                        CurrentActionRoutine = StartCoroutine(ChaseTargetRoutine());

                        break;
                    }
            }

            if (CurrentActionRoutine != null)
            {
                yield return CurrentActionRoutine;
            }
        }
    }

    protected virtual IEnumerator IdleRoutine()
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

    protected virtual IEnumerator WanderLeftRotuine()
    {
        float t = Random.Range(0.5f, 5f);

        while (t > 0f)
        {
            t -= 1f * Time.deltaTime;

            WalkLeft();

            if (isLeftBlocked())
            {
                break;
            }

            yield return 0;
        }

        CurrentAction = AIAction.Thinking;
    }

    protected virtual IEnumerator WanderRightRotuine()
    {
        float t = Random.Range(0.5f, 5f);

        while (t > 0f)
        {
            t -= 1f * Time.deltaTime;

            WalkRight();

            if (isRightBlocked())
            {
                break;
            }

            yield return 0;
        }

        CurrentAction = AIAction.Thinking;
    }

    protected virtual IEnumerator ChaseTargetRoutine()
    {
        float currentDistance = Mathf.NegativeInfinity;

        while(currentDistance < MaxChaseDistance)
        {
            currentDistance = Vector3.Distance(transform.position, CurrentTarget.transform.position);

            if (currentDistance < 0.4f)
            {
                StandStill();
            }
            else
            {
                if (transform.position.x < CurrentTarget.transform.position.x) // Chase Right
                {
                    if (isRightBlocked())
                    {
                        StandStill();
                    }
                    else
                    {
                        WalkRight();
                    }
                }
                else if (transform.position.x > CurrentTarget.transform.position.x) // Chase Left
                {
                    if (isLeftBlocked())
                    {
                        StandStill();
                    }
                    else
                    {
                        WalkLeft();
                    }
                }
            }

            yield return 0;
        }

        SetTarget(null);
        CurrentAction = AIAction.Thinking;
    }

    #endregion

    #region Basic Actions

    public virtual void StandStill()
    {
        Anim.SetBool("Walk", false);    
    }

    public virtual void WalkLeft()
    {
        Rigid.position += -Vector2.right * MovementSpeed * Time.deltaTime;
        Body.localScale = new Vector3( -initScale.x, initScale.y, initScale.z);

        Anim.SetBool("Walk", true);
    }

    public virtual void WalkRight()
    {
        Rigid.position += Vector2.right *  MovementSpeed * Time.deltaTime;
        Body.localScale = new Vector3( initScale.x, initScale.y, initScale.z);

        Anim.SetBool("Walk", true);
    }

    public override void Hurt(ActorInstance actor, int damage = 0, int currentHP = 0)
    {
        base.Hurt(actor,damage, currentHP);

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
        transform.position = Vector3.Lerp(transform.position, new Vector3(x, y, transform.position.z), Time.deltaTime * 6f);

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
