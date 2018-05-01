using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileThrowersEntity : MonoBehaviour {

    [SerializeField]
    PlayerAreaEntity Area;

    [SerializeField]
    Animator m_Animator;

    [SerializeField]
    ActorInstance CurrentTarget;

    [SerializeField]
    Transform ThrowPoint;

    [SerializeField]
    string ProjectilePrefab;

    public void StartThrowing()
    {
        if(m_Animator != null)
        {
            m_Animator.SetTrigger("Start");
            CurrentTarget = Area.CurrentPlayer;
        }
    }

    public void StopThrowing()
    {
        if (m_Animator != null)
        {
            m_Animator.SetTrigger("Stop");
            CurrentTarget = null;
        }
    }

    public void Throw()
    {
        WorldProjectile tempProj = ResourcesLoader.Instance.GetRecycledObject(ProjectilePrefab).GetComponent<WorldProjectile>();
        tempProj.transform.position = transform.position;
        tempProj.transform.right = CurrentTarget.transform.position - transform.position;
    }
}
