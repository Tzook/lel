using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class ActorController : MonoBehaviour
{
    #region References

    protected Rigidbody2D Rigidbody;
    protected BoxCollider2D Collider;
    protected RaycastHit2D GroundRay;
    protected Animator Anim;

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
        var moved = false;
        if(Input.GetKey(KeyCode.A))
        {
            MoveLeft();
            moved = true;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            MoveRight();
            moved = true;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            Jump();
            moved = true;
        }

        GroundRay = Physics2D.Raycast(transform.position, -transform.up, Collider.size.y/12f , GroundLayerMask);
        Grounded = GroundRay;

        if (moved)
        {
            Anim.SetBool("Walking", true);
            SM.SocketClient.EmitMovement(transform.position);
        }

        if(!Grounded)
        {
            Anim.SetBool("InAir", true);
            SM.SocketClient.EmitMovement(transform.position);
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
        Rigidbody.AddForce(InternalJumpForce * transform.up, ForceMode2D.Impulse);

        yield return new WaitForSeconds(JumpDelay);

        JumpRoutineInstance = null;
    }

    #endregion


}
