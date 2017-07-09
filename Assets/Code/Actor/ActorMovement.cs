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

    [SerializeField]
    GameObject m_HealthBar;

    BoxCollider2D HitBox;

    void Start()
    {
        Instance = GetComponent<ActorInstance>();
        lastPosition = transform.position;

        Anim = transform.Find("Body").GetComponent<Animator>();

        initScale = Anim.transform.localScale;

        if (Instance.Info.Climbing) {
            StartClimbing();
        }

        HitBox = GetComponent<BoxCollider2D>();

        if (LocalUserInfo.Me.CurrentParty != null)
        {
            if (LocalUserInfo.Me.CurrentParty.Members.Contains(Instance.Info.Name))
            {
                ShowHealth();
            }
        }
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

    void LateUpdate()
    {
        if(m_HealthBar != null)
        {
            m_HealthBar.transform.position = Vector2.Lerp(m_HealthBar.transform.position, new Vector2(transform.position.x, -HitBox.bounds.max.y), Time.deltaTime * 3f);
        }
    }

    protected void LerpToPosition()
    {
        Anim.SetBool("InAir", false);
        Anim.SetBool("Walking", false);
        MovingHorizontal = false;
        MovingVertical = false;

        if (Instance.Info.Climbing)
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
        Instance.Info.Climbing = true;
    }

    public void StopClimbing()
    {
        Anim.SetBool("OnRope", false);
        Anim.SetBool("ClimbingUp", false);
        Anim.SetBool("ClimbingDown", false);

        Instance.Info.Climbing = false;
    }

    public void ActivatePrimaryAbility()
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

    public void RefreshHealth()
    {
        if (m_HealthBar != null)
        {
            m_HealthBar.GetComponent<HealthBar>().SetHealthbar(Instance.Info.CurrentHealth, Instance.Info.MaxHealth);
        }
    }

    public void ShowHealth()
    {
        if(m_HealthBar == null)
        {
            m_HealthBar = ResourcesLoader.Instance.GetRecycledObject("HealthBarActor");
            m_HealthBar.transform.position = transform.position;
            RefreshHealth();
        }
    }

    public void HideHealth()
    {
        if(m_HealthBar != null)
        {
            m_HealthBar.gameObject.SetActive(false);
            m_HealthBar = null;
        }
    }

}