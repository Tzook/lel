using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ActorInstance))]
public class ActorMovement : MonoBehaviour
{

    public ActorInstance Instance;

    [SerializeField]
    protected float relocateSpeed = 15f;

    protected Vector3 lastPosition;
    protected Vector3 initScale;
    protected Animator Anim;

    protected bool MovingHorizontal;
    protected bool MovingVertical;
    protected bool aimRight;

    protected bool OnRope;

    void Start()
    {
        Instance = GetComponent<ActorInstance>();
        lastPosition = transform.position;

        Anim = transform.Find("Body").GetComponent<Animator>();

        initScale = Anim.transform.localScale;
    }

    public void UpdateMovement(Vector3 TargetPos, float angle)
    {
        if (TargetPos.x > lastPosition.x)
        {
            Anim.transform.localScale = new Vector3(1 * initScale.x, initScale.y, initScale.z);
        }
        else if (TargetPos.x < lastPosition.x)
        {
            Anim.transform.localScale = new Vector3(-1 * initScale.x, initScale.y, initScale.z);
        }

        if (angle == 0f)
        {
            StopAim();
        }
        else
        {
            Aim(angle);
        }

        lastPosition = TargetPos;
    }

    private void Aim(float angle)
    {

        Anim.SetBool("Aim", true);

        if (angle < 0 && angle > -90f || angle > 0 && angle < 90f)
        {
            Anim.transform.localScale = new Vector3(1 * initScale.x, initScale.y, initScale.z);
            Instance.TorsoBone.transform.localScale = Vector3.one;

            aimRight = true;

            if (angle < 0f && angle < -40f)
            {
                Instance.TorsoBone.transform.rotation = Quaternion.Lerp(Instance.TorsoBone.transform.rotation, Quaternion.Euler(0, 0, -40f), Time.deltaTime * 20f);
            }
            else if (angle > 0 && angle > 40f)
            {
                Instance.TorsoBone.transform.rotation = Quaternion.Lerp(Instance.TorsoBone.transform.rotation, Quaternion.Euler(0, 0, 40f), Time.deltaTime * 20f);
            }
            else
            {
                Instance.TorsoBone.transform.rotation = Quaternion.Lerp(Instance.TorsoBone.transform.rotation, Quaternion.Euler(0, 0, angle), Time.deltaTime * 20f);
            }
        }
        else
        {
            Anim.transform.localScale = new Vector3(-1 * initScale.x, initScale.y, initScale.z);
            Instance.TorsoBone.transform.localScale = new Vector3(-1f, -1f, 1f);

            aimRight = false;
            if (angle < 0f && angle > -130f)
            {
                Instance.TorsoBone.transform.rotation = Quaternion.Lerp(Instance.TorsoBone.transform.rotation, Quaternion.Euler(0, 0, -130f), Time.deltaTime * 20f);
            }
            else if (angle > 0f && angle < 140f)
            {
                Instance.TorsoBone.transform.rotation = Quaternion.Lerp(Instance.TorsoBone.transform.rotation, Quaternion.Euler(0, 0, 140f), Time.deltaTime * 20f);
            }
            else
            {
                Instance.TorsoBone.transform.rotation = Quaternion.Lerp(Instance.TorsoBone.transform.rotation, Quaternion.Euler(0, 0, angle), Time.deltaTime *20f);
            }
        }
    }

    private void StopAim()
    {
        Anim.SetBool("Aim", false);
        Instance.TorsoBone.transform.rotation = Quaternion.Euler(Vector3.zero);
        Instance.TorsoBone.transform.localScale = Vector3.one;
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

        if (OnRope)
        {
            if (Mathf.Abs(transform.position.y - lastPosition.y) > 0.05f)
            {
                Anim.SetBool("ClimbingUp", true);
                Anim.SetBool("ClimbingDown", false);
            }
            else if (Mathf.Abs(transform.position.y - lastPosition.y) < -0.05f)
            {
                Anim.SetBool("ClimbingUp", false);
                Anim.SetBool("ClimbingDown", true);
            }
            else
            {
                Anim.SetBool("ClimbingUp", false);
                Anim.SetBool("ClimbingDown", false);
            }
        }
        else
        {
            if (Mathf.Abs(transform.position.y - lastPosition.y) > 0.05f)
            {
                Anim.SetBool("InAir", true);
                MovingVertical = true;
            }

            if (Mathf.Abs(transform.position.x - lastPosition.x) > 0.05f)
            {
                Anim.SetBool("Walking", true);
                MovingHorizontal = true;
            }
        }


        transform.position = Vector3.Lerp(transform.position, lastPosition, Time.deltaTime * relocateSpeed);
    }

    public void StartClimbing()
    {
        Anim.SetBool("OnRope", true);
        Anim.transform.localScale = new Vector3(1 * initScale.x, initScale.y, initScale.z);
        OnRope = true;
    }

    public void StopClimbing()
    {
        Anim.SetBool("OnRope", false);
        Anim.SetBool("ClimbingUp", false);
        Anim.SetBool("ClimbingDown", false);

        OnRope = false;
    }

    public void ActivatePrimaryAbility(string ability)
    {
        switch (Instance.Info.CurrentPrimaryAbility)
        {
            case "range":
                {
                    Instance.ShootArrow(false);
                    break;
                }
        }
    }

}