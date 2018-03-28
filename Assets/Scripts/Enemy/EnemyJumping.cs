using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyJumping : EnemyMoving
{
    [SerializeField]
    float JumpForce;

    [SerializeField]
    float GroundedThreshold = 0.1f;

    protected RaycastHit2D GroundRayRight;
    protected RaycastHit2D GroundRayLeft;

    public bool isGrounded
    {
        get
        {
            GroundRayRight = Physics2D.Raycast(transform.position + transform.transform.TransformDirection(Collider.size.x / 16f, -Collider.size.y / 13f, 0), -transform.up, GroundedThreshold, GroundLayerMask);
            GroundRayLeft = Physics2D.Raycast(transform.position + transform.transform.TransformDirection(-Collider.size.x / 16f, -Collider.size.y / 13f, 0), -transform.up, GroundedThreshold, GroundLayerMask);

            return (GroundRayRight || GroundRayLeft);
        }
    }

    protected void Awake()
    {
        OriginalMovementSpeed = MovementSpeed;
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

            Anim.SetBool("inAir", !isGrounded);
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

    #region AI

    protected override IEnumerator WanderLeftRotuine()
    {
        float t = Random.Range(0.5f, 5f);

        if (Random.Range(0, 3) == 0)
        {
            Jump();
        }

        while (t > 0f)
        {
            t -= 1f * Time.deltaTime;

            WalkLeft();

            if(Stunned)
            {
                break;
            }

            if (isLeftBlocked())
            {
                yield return StartCoroutine(TryJumpOverLeft());

                if (isLeftBlocked())
                {
                    break;
                }
            }

            yield return 0;
        }

        CurrentAction = AIAction.Thinking;
    }

    protected override IEnumerator WanderRightRotuine()
    {
        float t = Random.Range(0.5f, 5f);

        if (Random.Range(0, 3) == 0)
        {
            Jump();
        }

        while (t > 0f)
        {
            t -= 1f * Time.deltaTime;

            WalkRight();

            if(Stunned)
            {
                break;
            }

            if (isRightBlocked())
            {
                yield return StartCoroutine(TryJumpOverRight());

                if (isRightBlocked())
                {
                    break;
                }
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
                    if(Stunned)
                    {
                        break;
                    }

                    if (isRightBlocked())
                    {
                        yield return StartCoroutine(TryJumpOverRight());
                        if(isRightBlocked())
                        {
                            break;
                        }
                    }
                    else
                    {
                        yield return StartCoroutine(WanderRightRotuine());
                        CurrentAction = AIAction.Chasing;
                    }
                }
                else if (transform.position.x > CurrentTarget.transform.position.x) // Chase Left
                {
                    if(Stunned)
                    {
                        break;
                    }

                    if (isLeftBlocked())
                    {
                        yield return StartCoroutine(TryJumpOverLeft());
                        if (isLeftBlocked())
                        {
                            break;
                        }
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

    protected IEnumerator TryJumpOverRight()
    {
        Jump();

        yield return 0;

        float lastY = Mathf.NegativeInfinity; //Reached top of jump check
        while(lastY < transform.position.y)
        {
            lastY = transform.position.y;
            WalkRight();
            yield return 0;
        }

        if(isRightBlocked())
        {
            StandStill();
        }
    }

    protected IEnumerator TryJumpOverLeft()
    {
        Jump();

        yield return 0;

        float lastY = Mathf.NegativeInfinity; //Reached top of jump check
        while (lastY < transform.position.y)
        {
            lastY = transform.position.y;
            WalkLeft();
            yield return 0;
        }

        if (isLeftBlocked())
        {
            StandStill();
        }
    }

    #endregion

    #region Basic Actions

    public void Jump()
    {
        if (isGrounded)
        {
            Rigid.AddForce(Vector2.up * JumpForce);
        }
    }

    #endregion

    #region UnderControl

    public override void UpdateMovement(float x, float y ,float velocity)
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

        Anim.SetBool("inAir", !isGrounded);

        Rigid.velocity = new Vector2(0f, velocity);
    }

    #endregion
}

