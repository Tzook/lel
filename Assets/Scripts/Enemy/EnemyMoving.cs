using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyMoving : Enemy
{
    public AIAction CurrentAction;

    protected Coroutine AIRoutineInstance;

    [SerializeField]
    protected float MovementSpeed = 1f;
    protected float OriginalMovementSpeed;

    [SerializeField]
    protected float MaxChaseDistance = 20f;

    [SerializeField]
    protected BoxCollider2D Collider;

    [SerializeField]
    protected bool Stunnable = true;

    protected RaycastHit2D SideRayRight;
    protected RaycastHit2D SideRayLeft;

    protected LayerMask GroundLayerMask = 0 << 0 | 1;

    protected Coroutine CurrentActionRoutine;

    protected bool Stunned;
    protected bool Slowed
    {
        set
        {
            slowed = value;
            if(value)
            {
                MovementSpeed = OriginalMovementSpeed * 0.5f; ;
            }
            else
            {
                MovementSpeed = OriginalMovementSpeed;
            }
        }
        get
        {
            return slowed;
        }
    }
    protected bool slowed;

    protected void Awake()
    {
        OriginalMovementSpeed = MovementSpeed;
    }

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

    public virtual bool isRightBlocked()
    {
        SideRayRight = Physics2D.Raycast(transform.position, transform.right, 1f, GroundLayerMask);

        Debug.DrawRay(transform.position, transform.right * 1f, Color.red);

        return (SideRayRight.normal.x < -0.3f || SideRayRight.normal.x > 0.3f);
    }

    public virtual bool isLeftBlocked()
    {
        SideRayLeft = Physics2D.Raycast(transform.position, -transform.right, 1f, GroundLayerMask);

        Debug.DrawRay(transform.position, -transform.right * 1f, Color.red);

        return (SideRayLeft.normal.x < -0.3f || SideRayLeft.normal.x > 0.3f);
    }

    protected Vector3 LastSentPosition;
    protected Vector3 LastGivenPosition;

    void FixedUpdate()
    {
        if(Game.Instance.isBitch && !Dead && LastSentPosition != transform.position)
        {
            LastSentPosition = transform.position;
            EnemyUpdater.Instance.UpdateMob(Info.ID, transform.position, Rigid.velocity.y);
        }
    }

    private void Update()
    {
        if(!Game.Instance.isBitch && LastGivenPosition != Vector3.zero)
        {
            Rigid.position = new Vector2(Vector2.Lerp(Rigid.position, LastGivenPosition, Time.deltaTime * 5f).x, Vector2.Lerp(Rigid.position, LastGivenPosition, Time.deltaTime * 10f).y);
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

            yield return 0;
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

            if (Stunned || isLeftBlocked())
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

            if (Stunned || isRightBlocked())
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

            if (Mathf.Abs(transform.position.x - CurrentTarget.transform.position.x) < 0.4f)
            {
                StandStill();
            }
            else
            {
                if (transform.position.x < CurrentTarget.transform.position.x) // Chase Right
                {
                    if (Stunned || isRightBlocked())
                    {
                        StandStill();
                    }
                    else
                    {
                        yield return StartCoroutine(WanderRightRotuine());
                        CurrentAction = AIAction.Chasing;
                    }
                }
                else if (transform.position.x > CurrentTarget.transform.position.x) // Chase Left
                {
                    if (Stunned || isLeftBlocked())
                    {
                        StandStill();
                    }
                    else
                    {
                        yield return StartCoroutine(WanderLeftRotuine());
                        CurrentAction = AIAction.Chasing;
                    }
                }
            }

            yield return 0;
        }

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
        Rigid.position += -(Vector2)Body.transform.right * MovementSpeed * Time.deltaTime;
        Body.localScale = new Vector3( -initScale.x, initScale.y, initScale.z);

        Anim.SetBool("Walk", true);
    }

    public virtual void WalkRight()
    {
        Rigid.position += (Vector2)Body.transform.right *  MovementSpeed * Time.deltaTime;
        Body.localScale = new Vector3( initScale.x, initScale.y, initScale.z);

        Anim.SetBool("Walk", true);
    }

    public override void Hurt(ActorInstance actor, int damage = 0, int currentHP = 0, string cause = "attack", bool crit = false)
    {
        base.Hurt(actor, damage, currentHP, cause, crit);

        if (Game.Instance.isBitch)
        {

            if (actor.transform.position.x < transform.position.x)
            {
                Rigid.AddForce((damage/Info.MaxHealth) * 3f * transform.right, ForceMode2D.Impulse);
            }
            else
            {
                Rigid.AddForce((damage / Info.MaxHealth) * 3f * -transform.right, ForceMode2D.Impulse);
            }
        }
    }

    public override void Death()
    {
        SetAIOFF();

        Rigid.velocity = Vector2.zero;

        base.Death();
    }

    protected override void StartBuffEffect(string buffKey)
    {
        switch (buffKey)
        {
            case "stunChance":
                {
                    this.Stunned = true;
                    break;
                }
            case "crippleChance":
                {
                    this.Slowed = true;
                    break;
                }
        }
    }

    protected override void StopBuffEffect(string buffKey)
    {
        if (GetBuff(buffKey) == null)
        {
            switch (buffKey)
            {
                case "stunChance":
                    {
                        this.Stunned = false;
                        break;
                    }
                case "crippleChance":
                    {
                        this.Slowed = false;
                        break;
                    }
            }
        }
    }

    #endregion

    #region UnderControl

    public override void UpdateMovement(float x, float y, float velocity)
    {
        LastGivenPosition = new Vector2(x, y);

        if(Mathf.Abs(Rigid.position.x - x) > 0.05f)
        {
            Anim.SetBool("Walk", true);

            if (Rigid.position.x < x)
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

        Rigid.velocity = new Vector2(0f, velocity);
    }

    #endregion

    public enum AIAction
    {
        Thinking, Idle, WanderingLeft, WanderingRight, Chasing, Flee
    }
}
