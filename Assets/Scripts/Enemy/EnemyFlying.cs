using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFlying : Enemy {

    public AIAction CurrentAction;

    protected Coroutine AIRoutineInstance;

    [SerializeField]
    protected float MovementSpeed = 1f;
    protected float OriginalMovementSpeed;

    [SerializeField]
    protected float MinChaseDistance = 0.4f;

    [SerializeField]
    protected float MaxChaseDistance = 20f;

    [SerializeField]
    protected BoxCollider2D Collider;

    protected RaycastHit2D TargetDirRay;

    protected LayerMask GroundLayerMask = 0 << 0 | 1;

    protected Coroutine CurrentActionRoutine;

    [SerializeField]
    float SpawnerPatrolRadius = 5f;

    protected bool Stunned;
    protected bool Slowed
    {
        set
        {
            slowed = value;
            if (value)
            {
                MovementSpeed = OriginalMovementSpeed * 0.5f;
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

        AIRoutineInstance = StartCoroutine(AIRoutine());
    }

    public override void SetAIOFF()
    {
        base.SetAIOFF();

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

        if (target == null)
        {
            StopCurrentActionRoutine();
        }
    }

    protected Vector3 LastSentPosition;
    protected Vector3 LastGivenPosition;
    protected void FixedUpdate()
    {
        if (Game.Instance.isBitch && !Dead && LastSentPosition != transform.position)
        {
            LastSentPosition = transform.position;
            LastGivenPosition = LastSentPosition;
            EnemyUpdater.Instance.UpdateMob(Info.ID, transform.position, Rigid.velocity.y);
        }
    }

    protected void Update()
    {
        if (!Game.Instance.isBitch && LastGivenPosition != Vector3.zero)
        {
            Rigid.position = Vector2.Lerp(Rigid.position, LastGivenPosition, Time.deltaTime * 5f);
            LastSentPosition = LastGivenPosition;
        }
    }

    protected void LateUpdate()
    {
        if (m_HealthBar != null)
        {
            m_HealthBar.transform.position = Vector2.Lerp(m_HealthBar.transform.position, new Vector2(transform.position.x, m_HitBox.bounds.max.y), Time.deltaTime * 3f);
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
                    rndDecision = Random.Range(0, 2);

                    if (rndDecision == 0)
                    {
                        CurrentAction = AIAction.Patrolling;
                    }
                    else
                    {
                        CurrentAction = AIAction.Idle;
                    }
                }
            }

            //ACT
            switch (CurrentAction)
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
                case AIAction.Patrolling:
                    {
                        CurrentActionRoutine = StartCoroutine(FlyAroundRoutine());
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
        float t = Random.Range(0.5f, 1f);

        StandStill();

        while (t > 0f)
        {
            t -= 1f * Time.deltaTime;

            yield return 0;
        }

        CurrentAction = AIAction.Thinking;
    }

    protected virtual IEnumerator FlyAroundRoutine()
    {
        Vector2 PatrolPoint = GetClosestSpawner().transform.position;
        Vector2 targetPos = new Vector2(PatrolPoint.x + Random.Range(-SpawnerPatrolRadius, SpawnerPatrolRadius), PatrolPoint.y + Random.Range(-SpawnerPatrolRadius, SpawnerPatrolRadius));

        while (Vector2.Distance(transform.position, targetPos) > 0.2f)
        {

            Rigid.position += Vector2.ClampMagnitude((targetPos - ((Vector2)transform.position)),1) * MovementSpeed * Time.deltaTime;

            
            if(targetPos.y < transform.position.y)
            {
                if (Mathf.Abs(transform.position.y - targetPos.y) > 1f)
                {
                    Anim.SetInteger("FlyDir", 2);
                }
            }
            else
            {
                Anim.SetInteger("FlyDir", 1);
            }

            if (transform.position.x < targetPos.x)
            {
                Body.localScale = new Vector3(initScale.x, initScale.y, initScale.z);
            }
            else
            {
                Body.localScale = new Vector3(-initScale.x, initScale.y, initScale.z);
            }


            if (isDirectionBlocked(targetPos) || Stunned)
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

        while (currentDistance < MaxChaseDistance)
        {
            currentDistance = Vector3.Distance(transform.position, CurrentTarget.transform.position);
            
            if (Mathf.Abs(transform.position.x - CurrentTarget.transform.position.x) < MinChaseDistance || Stunned)
            {
                StandStill();
            }
            else
            {
                Rigid.position += (Vector2) (CurrentTarget.transform.position - transform.position) * MovementSpeed * Time.deltaTime;

                if (CurrentTarget.transform.position.y < transform.position.y)
                {
                    if (Mathf.Abs(transform.position.y - CurrentTarget.transform.position.y) > 1f)
                    {
                        Anim.SetInteger("FlyDir", 2);
                    }
                }
                else
                {
                    Anim.SetInteger("FlyDir", 1);
                }

                if (transform.position.x < CurrentTarget.transform.position.x)
                {
                    Body.localScale = new Vector3(initScale.x, initScale.y, initScale.z);
                }
                else
                {
                    Body.localScale = new Vector3(-initScale.x, initScale.y, initScale.z);
                }


                if (isDirectionBlocked(CurrentTarget.transform.position - transform.position))
                {
                    break;
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
        Anim.SetInteger("FlyDir", 0);
    }

    public override void Hurt(ActorInstance actor, int damage = 0, int currentHP = 0, string cause = "attack", bool crit = false)
    {
        base.Hurt(actor, damage, currentHP, cause, crit);

        if (Game.Instance.isBitch)
        {
            Rigid.AddForce(2f * actor.Info.ClientPerks.KnockbackModifier * transform.position-actor.transform.position, ForceMode2D.Impulse);
        }
    }

    public override void Death()
    {
        SetAIOFF();

        base.Death();
    }

    public virtual bool isDirectionBlocked(Vector2 dir)
    {
        return (Physics2D.Raycast(transform.position, dir, 1f, GroundLayerMask));
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


        if (transform.position.x != x)
        {
            if (transform.position.x < x)
            {
                Body.localScale = new Vector3(initScale.x, initScale.y, initScale.z);
            }
            else
            {
                Body.localScale = new Vector3(-initScale.x, initScale.y, initScale.z);
            }

            if(transform.position.y < y)
            {
                Anim.SetInteger("FlyDir", 1);
            }
            else
            {
                Anim.SetInteger("FlyDir", 2);
            }
        }
        else
        {
            Anim.SetInteger("FlyDir", 0);
        }
    }

    #endregion

    public enum AIAction
    {
        Thinking, Idle, Patrolling , Chasing, Flee
    }
}
