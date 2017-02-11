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
        AIRoutineInstance = StartCoroutine(AIRoutine());
    }

    public override void SetAIOFF()
    {
        base.SetAIOFF();

        if (AIRoutineInstance != null)
        {
            StopCoroutine(AIRoutineInstance);
            AIRoutineInstance = null;
        }

        StopAllCoroutines();
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
        Rigid.velocity = new Vector2(0f, Rigid.velocity.y);
        Anim.SetBool("Walk", false);    
    }

    public void WalkLeft()
    {
        Rigid.velocity  = new Vector2( -MovementSpeed * Time.deltaTime, Rigid.velocity.y);
        Body.localScale = new Vector3( -initScale.x, initScale.y, initScale.z);

        Anim.SetBool("Walk", true);
    }

    public void WalkRight()
    {
        Rigid.velocity  = new Vector2( MovementSpeed * Time.deltaTime, Rigid.velocity.y);
        Body.localScale = new Vector3( initScale.x, initScale.y, initScale.z);

        Anim.SetBool("Walk", true);
    }

    #endregion

    public enum AIAction
    {
        Thinking, Idle, WanderingLeft, WanderingRight, Chasing, Flee
    }
}
