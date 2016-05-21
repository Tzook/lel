using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(ActorInstance))]
public class ActorMovement : MonoBehaviour, IUpdatePositionListener
{

    public ActorInstance Instance;

    void Awake()
    {
        Instance = GetComponent<ActorInstance>();
        Instance.RegisterMovementController(this);
    }

    public void UpdatePosition(Vector3 TargetPos)
    {
        transform.position = Vector3.Lerp(transform.position, TargetPos, Time.deltaTime);
    }
}
