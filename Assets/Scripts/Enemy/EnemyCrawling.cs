using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCrawling : EnemyMoving {

    [SerializeField]
    float GroundedThreshold = 0.1f;

    [SerializeField]
    Crawler CrawlerEntity;

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
                EnemyUpdater.Instance.UpdateMob(Info.ID, transform.position);
            }

            Anim.SetBool("inAir", !isGrounded);
        }

        if (CrawlerEntity.CurrentRotation == Quaternion.identity)
        {
            Rigid.gravityScale = 1f;

            Debug.Log(Rigid.velocity.y);
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

                    if (isRightBlocked())
                    {
                        if (isRightBlocked())
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
                    if (Stunned)
                    {
                        break;
                    }

                    if (isLeftBlocked())
                    {
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

    #endregion

    #region UnderControl

    public override void UpdateMovement(float x, float y)
    {
        transform.position = Vector3.Lerp(transform.position, new Vector3(x, y, transform.position.z), Time.deltaTime * 6f);

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

    }

    #endregion
}
