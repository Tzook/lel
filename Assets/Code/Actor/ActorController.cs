using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class ActorController : MonoBehaviour
{
    #region References

    public ActorInstance Instance;

    Rigidbody2D Rigidbody;
    BoxCollider2D Collider;

    RaycastHit2D GroundRayRight;
    RaycastHit2D GroundRayLeft;

    Animator Anim;

    [SerializeField]
    bool ClientOnly = false;

    #endregion

    #region Parameters

    public bool Grounded = false;

    [SerializeField]
    float GroundedThreshold;

    [SerializeField]
    float InternalSpeed = 1f;

    [SerializeField]
    float InternalJumpForce = 1f;

    [SerializeField]
    float JumpDelay = 0.1f;

    Coroutine JumpRoutineInstance;

    LayerMask GroundLayerMask = 0 << 0 | 1;

    Vector3 initScale;

    Vector3 lastSentPosition;
    float lastSentAngle;

    float MovementDirection;

    //Rotation Parameters;
    Vector3 tempRot;
    float rotDegrees;
    bool aimRight;

    #endregion

    #region Mono

    void Awake()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        Instance  = GetComponent<ActorInstance>();

        if(Rigidbody == null)
        {
            Rigidbody = this.gameObject.AddComponent<Rigidbody2D>();
            Rigidbody.freezeRotation = true;
        }

        Collider = GetComponent<BoxCollider2D>();
        Collider.enabled = true;

        Anim = transform.FindChild("Body").GetComponent<Animator>();
    }

    void Start()
    {
        initScale = Anim.transform.localScale;
    }

    void Update()
    {
        if(Input.GetMouseButton(1))
        {
            Aim();
        }
        else if (Input.GetMouseButtonUp(1))
        {
            StopAim();
        }

    }

    void FixedUpdate()
    {

        Anim.SetBool("InAir", false);
        Anim.SetBool("Walking", false);

        if (Input.GetKey(KeyCode.A))
        {
            MoveLeft();
            Anim.SetBool("Walking", true);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            MoveRight();
            Anim.SetBool("Walking", true);
        }
        else
        {
            StandStill();
        }

        if (Input.GetKey(KeyCode.Space))
        {
            Jump();
            //Anim.SetBool("Walking", true);
        }

        GroundRayRight = Physics2D.Raycast(transform.position + transform.transform.TransformDirection(Collider.size.x / 16f, -Collider.size.y / 13f, 0), -transform.up, GroundedThreshold, GroundLayerMask);
        GroundRayLeft = Physics2D.Raycast(transform.position + transform.transform.TransformDirection(-Collider.size.x / 16f, -Collider.size.y / 13f, 0), -transform.up, GroundedThreshold, GroundLayerMask);

        //Debug.DrawRay(transform.position + transform.transform.TransformDirection(Collider.size.x / 16f, -Collider.size.y / 13f, 0), -transform.up * GroundedThreshold, Color.red);

        Grounded = (GroundRayRight || GroundRayLeft);

        if (!Grounded)
        {
            Anim.SetBool("InAir", true);
        }

        if (!ClientOnly)
        {
            if (lastSentPosition != transform.position || lastSentAngle != rotDegrees)
            {
                SocketClient.Instance.EmitMovement(transform.position, rotDegrees);
                lastSentPosition = transform.position;
                lastSentAngle = rotDegrees;
            }
        }

       
    }

    #endregion

    #region Public Methods

    public void MoveLeft()
    {
        //TODO Remove if no problems occur
        //Rigidbody.position += (Vector2.left * InternalSpeed * Time.deltaTime);


        Rigidbody.velocity = new Vector2(-InternalSpeed * Time.deltaTime , Rigidbody.velocity.y);
        Anim.transform.localScale = new Vector3(-1 * initScale.x, initScale.y,initScale.z);

        Anim.SetBool("ReverseWalk", aimRight);
    }

    public void MoveRight()
    {
        //TODO Remove if no problems occur
        //Rigidbody.position += (Vector2.right * InternalSpeed * Time.deltaTime);

        Rigidbody.velocity = new Vector2(InternalSpeed * Time.deltaTime, Rigidbody.velocity.y);
        Anim.transform.localScale = new Vector3(1 * initScale.x, initScale.y, initScale.z);

        Anim.SetBool("ReverseWalk", !aimRight);
    }

    public void StandStill()
    {
        Rigidbody.velocity = new Vector2(0, Rigidbody.velocity.y);
    }

    public void Jump()
    {
        if(JumpRoutineInstance==null && Grounded)
        {
            JumpRoutineInstance = StartCoroutine(JumpRoutine());
        }
    }

    protected IEnumerator JumpRoutine()
    {

        if (Rigidbody.velocity.y <= 1.5f)
        {
            Rigidbody.AddForce(InternalJumpForce * transform.up, ForceMode2D.Impulse);
        }

        yield return new WaitForSeconds(JumpDelay);

        JumpRoutineInstance = null;
    }

    private void Aim()
    {
        tempRot = (GameCamera.MousePosition - Instance.TorsoBone.transform.position);
        tempRot.Normalize();
        rotDegrees = Mathf.Atan2(tempRot.y, tempRot.x) * Mathf.Rad2Deg;

        Anim.SetBool("Aim", true);

        if (rotDegrees < 0 && rotDegrees > -90f || rotDegrees > 0 && rotDegrees < 90f)
        {
            Anim.transform.localScale = new Vector3(1 * initScale.x, initScale.y, initScale.z);
            Instance.TorsoBone.transform.localScale = Vector3.one;

            aimRight = true;

            if (rotDegrees < 0f && rotDegrees < -40f)
            {
                Instance.TorsoBone.transform.rotation = Quaternion.Euler(0, 0, -40f);
            }
            else if (rotDegrees > 0 && rotDegrees > 40f)
            {
                Instance.TorsoBone.transform.rotation = Quaternion.Euler(0, 0, 40f);
            }
            else
            {
                Instance.TorsoBone.transform.rotation = Quaternion.Euler(0, 0, rotDegrees);
            }
        }
        else
        {
            Anim.transform.localScale = new Vector3(-1 * initScale.x, initScale.y, initScale.z);
            Instance.TorsoBone.transform.localScale = new Vector3(-1f, -1f, 1f);

            aimRight = false;
            if (rotDegrees < 0f && rotDegrees > -130f)
            {
                Instance.TorsoBone.transform.rotation = Quaternion.Euler(0, 0, -130f);
            }
            else if (rotDegrees > 0f && rotDegrees < 140f)
            {
                Instance.TorsoBone.transform.rotation = Quaternion.Euler(0, 0, 140f);
            }
            else
            {
                Instance.TorsoBone.transform.rotation = Quaternion.Euler(0, 0, rotDegrees);
            }
        }
    }

    private void StopAim()
    {
        Anim.SetBool("Aim", false);
        Instance.TorsoBone.transform.rotation = Quaternion.Euler(Vector3.zero);
        Instance.TorsoBone.transform.localScale = Vector3.one;
        rotDegrees = 0f;
    }

    #endregion


}
