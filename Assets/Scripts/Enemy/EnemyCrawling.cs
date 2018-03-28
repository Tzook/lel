using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCrawling : EnemyMoving {

    [SerializeField]
    float GroundedThreshold = 0.1f;

    [SerializeField]
    Crawler CrawlerEntity;

    protected void Awake()
    {
        OriginalMovementSpeed = MovementSpeed;
    }

    private void Start()
    {
        SetAION();
    }

    void FixedUpdate()
    {
        if (Game.Instance.isBitch)
        {
            if (!Dead && LastSentPosition != transform.position)
            {
                LastSentPosition = transform.position;
                EnemyUpdater.Instance.UpdateMob(Info.ID, transform.position, Rigid.velocity.y);
            }

            //Anim.SetBool("inAir", !isGrounded);
        }

        if (CrawlerEntity.CurrentRotation == Quaternion.identity)
        {
            Rigid.gravityScale = 1f;

            if (Rigid.velocity.y < -8f)
            {
                Anim.SetBool("inAir", true);
            }
        }
        else
        {
            Body.transform.rotation = CrawlerEntity.CurrentRotation;
            Rigid.gravityScale = 0f;
            Anim.SetBool("inAir", false);
        }
    }

    private void Update()
    {
        if (!Game.Instance.isBitch)
        {
            Rigid.position = new Vector2(Vector2.Lerp(Rigid.position, LastGivenPosition, Time.deltaTime * 5f).x, Vector2.Lerp(Rigid.position, LastGivenPosition, Time.deltaTime * 10f).y);
        }
    }

    void LateUpdate()
    {
        if (m_HealthBar != null)
        {
            m_HealthBar.transform.position = Vector2.Lerp(m_HealthBar.transform.position, new Vector2(transform.position.x, m_HitBox.bounds.max.y), Time.deltaTime * 3f);
        }
    }

    public override void Hurt(ActorInstance actor, int damage = 0, int currentHP = 0, string cause = "attack", bool crit = false)
    {
        base.Hurt(actor, damage, currentHP, cause, crit);

        if (Game.Instance.isBitch)
        {

            if (actor.transform.position.x < transform.position.x)
            {
                Rigid.gravityScale = 1f;

                Rigid.AddForce((damage / Info.MaxHealth) * 3f * transform.right, ForceMode2D.Impulse);
            }
            else
            {
                Rigid.gravityScale = 1f;

                Rigid.AddForce((damage / Info.MaxHealth) * 3f * -transform.right, ForceMode2D.Impulse);
            }
        }
    }

    #region AI


    public override IEnumerator AIRoutine()
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

            yield return 0;

            if (CurrentActionRoutine != null)
            {
                yield return CurrentActionRoutine;
            }

            yield return 0;
        }
    }

    protected override IEnumerator IdleRoutine()
    {
        float t = Random.Range(0.5f, 5f);

        StandStill();

        while (t > 0f)
        {
            t -= 1f * Time.deltaTime;

            yield return 0;
        }

        CurrentAction = AIAction.Thinking;
    }

    protected override IEnumerator WanderLeftRotuine()
    {
        float t = Random.Range(0.5f, 5f);

        while (t > 0f)
        {
            t -= 1f * Time.deltaTime;

            WalkLeft();

            if (Stunned)
            {
                break;
            }

            yield return 0;
        }

        CurrentAction = AIAction.Thinking;
    }

    protected override IEnumerator WanderRightRotuine()
    {
        float t = Random.Range(0.5f, 5f);

        while (t > 0f)
        {
            t -= 1f * Time.deltaTime;

            WalkRight();

            if (Stunned)
            {
                break;
            }

            yield return 0;
        }

        CurrentAction = AIAction.Thinking;
    }

    protected override IEnumerator ChaseTargetRoutine()
    {

        float currentDistance = Mathf.NegativeInfinity;

        while (currentDistance < MaxChaseDistance)
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
                    if (Stunned)
                    {
                        break;
                    }

                    yield return StartCoroutine(WanderRightRotuine());
                }
                else if (transform.position.x > CurrentTarget.transform.position.x) // Chase Left
                {
                    if (Stunned)
                    {
                        break;
                    }

                    yield return StartCoroutine(WanderLeftRotuine());
                }
            }

            yield return 0;
        }

        CurrentAction = AIAction.Thinking;
    }

    public override void StandStill()
    {
        Anim.SetBool("Walk", false);
    }

    public override void WalkLeft()
    {
        Rigid.position += -(Vector2)Body.transform.right * MovementSpeed * Time.deltaTime;
        Body.localScale = new Vector3(-initScale.x, initScale.y, initScale.z);

        Debug.DrawRay(transform.position, Body.transform.right, Color.blue);

        Anim.SetBool("Walk", true);
    }

    public override void WalkRight()
    {
        Rigid.position += (Vector2)Body.transform.right * MovementSpeed * Time.deltaTime;
        Body.localScale = new Vector3(initScale.x, initScale.y, initScale.z);

        Debug.DrawRay(transform.position, -Body.transform.right, Color.blue);

        Anim.SetBool("Walk", true);
    }

    #endregion

    #region UnderControl

    public override void UpdateMovement(float x, float y, float velocity)
    {
        LastGivenPosition = new Vector2(x, y);

        if (transform.position.x != x)
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


        Rigid.velocity = new Vector2(0f, velocity);

    }

    #endregion
}
