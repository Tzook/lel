using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(ActorInstance))]
public class ActorMovement : MonoBehaviour, IUpdatePositionListener
{

    public ActorInstance Instance;

    protected Vector3 lastPosition;

    void Awake()
    {
        Instance = GetComponent<ActorInstance>();
        Instance.RegisterMovementController(this);
        lastPosition = transform.position;
    }

    public void UpdatePosition(Vector3 TargetPos)
    {
        lastPosition = TargetPos;
    }

    void Update()
    {
        LerpToPosition();
    }

    protected void LerpToPosition()
    {
        transform.position = Vector3.Lerp(transform.position, lastPosition, Time.deltaTime * 4);
    }
}
