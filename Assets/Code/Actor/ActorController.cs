using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class ActorController : MonoBehaviour
{
    #region References

    protected Rigidbody2D Rigidbody;
    protected BoxCollider2D Collider;
    protected RaycastHit2D GroundRayRight;
    protected RaycastHit2D GroundRayLeft;
    protected Animator Anim;

    [SerializeField]
    protected bool ClientOnly = false;

    #endregion

    #region Parameters

    public bool InAir = true;
    public bool Grounded = false;

    [SerializeField]
    protected float InternalSpeed = 1f;

    [SerializeField]
    protected float InternalJumpForce = 1f;

    [SerializeField]
    protected float JumpDelay = 0.1f;

    protected Coroutine JumpRoutineInstance;

    protected LayerMask GroundLayerMask = 0 << 0 | 1;

    protected Vector3 initScale;

    protected Vector3 lastSentPosition;

    #endregion

    #region Mono

    void Awake()
    {
        Rigidbody = GetComponent<Rigidbody2D>();

        if(Rigidbody == null)
        {
            Rigidbody = this.gameObject.AddComponent<Rigidbody2D>();
            Rigidbody.freezeRotation = true;
        }

        Collider = GetComponent<BoxCollider2D>();
        Collider.enabled = true;

        Anim = transform.GetChild(0).GetComponent<Animator>();
    }

    void Start()
    {
        initScale = transform.localScale;
    }

    void FixedUpdate()
    {
        Anim.SetBool("InAir", false);
        Anim.SetBool("Walking", false);
        if(Input.GetKey(KeyCode.A))
        {
            MoveLeft();
            Anim.SetBool("Walking", true);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            MoveRight();
            Anim.SetBool("Walking", true);
        }

        if (Input.GetKey(KeyCode.Space))
        {
            Jump();
            Anim.SetBool("Walking", true);
        }

        GroundRayRight = Physics2D.Raycast(transform.position + transform.transform.TransformDirection(Collider.size.x / 16f, -Collider.size.y / 13f, 0), -transform.up, 0.15f , GroundLayerMask);
        GroundRayLeft = Physics2D.Raycast(transform.position - transform.transform.TransformDirection(Collider.size.x / 16f, -Collider.size.y / 13f, 0), -transform.up, 0.15f, GroundLayerMask);

        Debug.DrawRay(transform.position + transform.transform.TransformDirection(Collider.size.x / 16f,  -Collider.size.y / 13f, 0), -transform.up * 0.15f, Color.red);

        Grounded = (GroundRayRight || GroundRayLeft);

        if (!Grounded)
        {
            Anim.SetBool("InAir", true);
        }

        if (!ClientOnly)
        {
            if (lastSentPosition != transform.position)
            {
                SM.SocketClient.EmitMovement(transform.position);
                lastSentPosition = transform.position;
            }
        }

       
    }

    #endregion

    #region Public Methods

    public void MoveLeft()
    {
        Rigidbody.position += (Vector2) (transform.TransformDirection(-1f,0f,0f) * InternalSpeed * Time.deltaTime);
        transform.localScale = new Vector3(1*initScale.x, initScale.y,initScale.z);
    }

    public void MoveRight()
    {
        Rigidbody.position += (Vector2)(transform.TransformDirection(1f, 0f, 0f) * InternalSpeed * Time.deltaTime);
        transform.localScale = new Vector3(-1 * initScale.x, initScale.y, initScale.z);
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
        if (Rigidbody.velocity.y <= 0f)
        {
            Rigidbody.AddForce(InternalJumpForce * transform.up, ForceMode2D.Impulse);
        }

        yield return new WaitForSeconds(JumpDelay);

        JumpRoutineInstance = null;
    }

    #endregion


}
