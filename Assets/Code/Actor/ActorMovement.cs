using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(ActorInstance))]
public class ActorMovement : MonoBehaviour, IUpdatePositionListener
{

    public ActorInstance Instance;


    [SerializeField]
    protected float relocateSpeed = 15f;

    protected Vector3 lastPosition;
    protected Vector3 initScale;
    protected Animator Anim;

    protected bool MovingHorizontal;
    protected bool MovingVertical;


    void Start()
    {
        Instance = GetComponent<ActorInstance>();
        Instance.RegisterMovementController(this);
        lastPosition = transform.position;
        initScale = transform.localScale;

        Anim = transform.GetChild(0).GetComponent<Animator>();
    }

    public void UpdatePosition(Vector3 TargetPos)
    {
        if (TargetPos.x > lastPosition.x)
        {
            transform.localScale = new Vector3(1 * initScale.x, initScale.y, initScale.z);
        }
        else if (TargetPos.x < lastPosition.x)
        {
            transform.localScale = new Vector3(-1 * initScale.x, initScale.y, initScale.z);
        }

        lastPosition = TargetPos;
    }

    void Update()
    {
        LerpToPosition();
    }

    protected void LerpToPosition()
    {
        Anim.SetBool("InAir", false);
        Anim.SetBool("Walking", false);
        MovingHorizontal = false;
        MovingVertical = false;

        if (Mathf.Abs(transform.position.y - lastPosition.y) > 0.1f)
        {
            Anim.SetBool("InAir", true);
            MovingVertical = true;
        }

        if (Mathf.Abs(transform.position.x - lastPosition.x) > 0.1f)
        {
            Anim.SetBool("Walking", true);
            MovingHorizontal = true;
        }

        transform.position = Vector3.Lerp(transform.position, lastPosition, Time.deltaTime * relocateSpeed);


        
    }

}