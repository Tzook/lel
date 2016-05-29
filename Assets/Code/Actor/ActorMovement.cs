using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(ActorInstance))]
public class ActorMovement : MonoBehaviour, IUpdatePositionListener
{

    public ActorInstance Instance;

    [SerializeField]
    protected float relocateSpeed = 3f;

    protected Vector3 lastPosition;
    protected Vector3 initScale;

    void Start()
    {
        Instance = GetComponent<ActorInstance>();
        Instance.RegisterMovementController(this);
        lastPosition = transform.position;
        initScale = transform.localScale;
    }

    public void UpdatePosition(Vector3 TargetPos)
    {
        if(TargetPos.x > lastPosition.x)
        {
            transform.localScale = new Vector3(-1 * initScale.x, initScale.y, initScale.z);
        }
        else if (TargetPos.x < lastPosition.x)
        {
            transform.localScale = new Vector3(1 * initScale.x, initScale.y, initScale.z);
        }

        lastPosition = TargetPos;
    }

    void Update()
    {
        LerpToPosition();
    }

    protected void LerpToPosition()
    {
        transform.position = Vector3.MoveTowards(transform.position, lastPosition, Time.deltaTime * relocateSpeed);
    }

}