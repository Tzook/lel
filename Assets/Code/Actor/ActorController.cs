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

    public bool Grounded = false;

    [SerializeField]
    protected float GroundedThreshold;

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

    protected float MovementDirection;

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

        Debug.DrawRay(transform.position + transform.transform.TransformDirection(Collider.size.x / 16f, -Collider.size.y / 13f, 0), -transform.up * GroundedThreshold, Color.red);

        Grounded = (GroundRayRight || GroundRayLeft);

        if (!Grounded)
        {
            Anim.SetBool("InAir", true);
        }

        if (!ClientOnly)
        {
            if (lastSentPosition != transform.position)
            {
                SocketClient.Instance.EmitMovement(transform.position);
                lastSentPosition = transform.position;
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
        transform.localScale = new Vector3(-1 * initScale.x, initScale.y,initScale.z);
    }

    public void MoveRight()
    {
        //TODO Remove if no problems occur
        //Rigidbody.position += (Vector2.right * InternalSpeed * Time.deltaTime);

        Rigidbody.velocity = new Vector2(InternalSpeed * Time.deltaTime, Rigidbody.velocity.y);
        transform.localScale = new Vector3(1 * initScale.x, initScale.y, initScale.z);
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
        Debug.Log(Rigidbody.velocity.y);

        if (Rigidbody.velocity.y <= 1.5f)
        {
            Rigidbody.AddForce(InternalJumpForce * transform.up, ForceMode2D.Impulse);
        }

        yield return new WaitForSeconds(JumpDelay);

        JumpRoutineInstance = null;
    }

    #endregion


}
