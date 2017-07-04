using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class ActorController : MonoBehaviour
{
    #region References

    public ActorInstance Instance;

    Rigidbody2D Rigid;
    BoxCollider2D Collider;

    RaycastHit2D GroundRayRight;
    RaycastHit2D GroundRayLeft;
    RaycastHit2D SideRayRight;
    RaycastHit2D SideRayLeft;

    Animator Anim;

    [SerializeField]
    bool ClientOnly = false;

    public GatePortal CurrentPortal;
    public BoxCollider2D CurrentRope;


    #endregion

    #region Parameters

    public bool Grounded = false;
    public bool OnRope = false;
    public bool LeftSlope = false;
    public bool RightSlope = false;
    public bool CanInput = true;

    [SerializeField]
    float GroundedThreshold;

    [SerializeField]
    float InternalSpeed = 1f;

    [SerializeField]
    float InternalClimbSpeed = 1f;

    [SerializeField]
    float InternalJumpForce = 1f;

    [SerializeField]
    float JumpDelay = 0.1f;

    [SerializeField]
    public ActorDamageInstance OneHandDamageInstance;



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

    bool Invincible;

    public Enemy CollidingEnemy;

    float LoadAttackValue = 0f;
    Coroutine LoadAttackValueInstance;

    #endregion

    #region Mono

    void Awake()
    {
        Rigid = GetComponent<Rigidbody2D>();
        Instance  = GetComponent<ActorInstance>();

        if(Rigid == null)
        {
            Rigid = this.gameObject.AddComponent<Rigidbody2D>();
            Rigid.freezeRotation = true;
        }

        Collider = GetComponent<BoxCollider2D>();
        Collider.enabled = true;

        Anim = transform.Find("Body").GetComponent<Animator>();
    }

    void Start()
    {
        initScale = Anim.transform.localScale;
    }

    void Update()
    {
        if (CanInput)
        {
            if (Input.GetMouseButton(1) && !OnRope)
            {
                Aim();
            }
            else if (Input.GetMouseButtonUp(1))
            {
                StopAim();
            }

            if (Input.GetKeyDown(InputMap.Map["Pick Up"]))
            {
                Instance.AttemptPickUp();
            }

            for (int i = 0; i < Instance.Info.PrimaryAbilities.Count; i++)
            {
                if (Input.GetKeyDown(InputMap.Map["PrimaryAbility"+(i+1)]))
                {
                    Instance.Info.SwitchPrimaryAbility(Instance.Info.PrimaryAbilities[i]);
                    InturruptAttack();
                }
            }


            if (!Invincible)
            {
                if (CollidingEnemy != null)
                {
                    Hurt(CollidingEnemy);
                }
            }
        }

        if (Input.GetKeyDown(InputMap.Map["Wink Emote"]))
        {
            if(Instance.EyesEmoteInstance == null)
            {
                SocketClient.Instance.SendEmote("eyes", "wink");
                Instance.PlayEyesEmote("wink");
            }
        }
        else if (Input.GetKeyDown(InputMap.Map["Sad Eyes Emote"]))
        {
            if (Instance.EyesEmoteInstance == null)
            {
                SocketClient.Instance.SendEmote("eyes", "cry");
                Instance.PlayEyesEmote("cry");
            }
        }
        else if (Input.GetKeyDown(InputMap.Map["Mad Eyes Emote"]))
        {
            if (Instance.EyesEmoteInstance == null)
            {
                SocketClient.Instance.SendEmote("eyes", "angry");
                Instance.PlayEyesEmote("angry");
            }
        }
        else if (Input.GetKeyDown(InputMap.Map["Happy Mouth Emote"]))
        {
            if (Instance.MouthEmoteInstance == null)
            {
                SocketClient.Instance.SendEmote("mouth", "happy");
                Instance.PlayMouthEmote("happy");
            }
        }
        else if (Input.GetKeyDown(InputMap.Map["Sad Mouth Emote"]))
        {
            if (Instance.MouthEmoteInstance == null)
            {
                SocketClient.Instance.SendEmote("mouth", "sad");
                Instance.PlayMouthEmote("sad");
            }
        }
        else if (Input.GetKeyDown(InputMap.Map["Angry Mouth Emote"]))
        {
            if (Instance.MouthEmoteInstance == null)
            {
                SocketClient.Instance.SendEmote("mouth", "angry");
                Instance.PlayMouthEmote("angry");
            }
        }
        else if (Input.GetKeyDown(InputMap.Map["Surprised Mouth Emote"]))
        {
            if (Instance.MouthEmoteInstance == null)
            {
                SocketClient.Instance.SendEmote("mouth", "surprised");
                Instance.PlayMouthEmote("surprised");
            }
        }

    }

    void LateUpdate()
    {
        CollidingEnemy = null;

        if (CanInput && !Game.Instance.isInteractingWithUI)
        {
            AttackCharge();
        }

    }

    private void AttackCharge()
    {
        if (Input.GetMouseButtonDown(0))
        {
            switch (Instance.Info.CurrentPrimaryAbility)
            {
                case "melee":
                    {
                        Anim.SetInteger("AttackType", Random.Range(0, 3));
                        break;
                    }
                case "range":
                    {
                        Anim.SetInteger("AttackType", 3);
                        break;
                    }
            }
        }

        Anim.SetBool("Charging", Input.GetMouseButton(0));
    }

    void FixedUpdate()
    {

        Anim.SetBool("InAir", false);
        Anim.SetBool("Walking", false);

        if (CanInput)
        {
            if (!OnRope)
            {

                SideRayRight = Physics2D.Raycast(transform.position + transform.transform.TransformDirection(Collider.size.x / 16f, -Collider.size.y / 13f, 0), transform.right, GroundedThreshold, GroundLayerMask);
                SideRayLeft = Physics2D.Raycast(transform.position + transform.transform.TransformDirection(-Collider.size.x / 16f, -Collider.size.y / 13f, 0), -transform.right, GroundedThreshold, GroundLayerMask);

                LeftSlope = (SideRayLeft.normal.x < -0.7 || SideRayLeft.normal.x > 0.7);
                RightSlope = (SideRayRight.normal.x < -0.7 || SideRayRight.normal.x > 0.7);


                if (Input.GetKey(InputMap.Map["Move Left"]) && !LeftSlope && !Game.Instance.InChat)
                {
                    MoveLeft();
                    Anim.SetBool("Walking", true);
                }
                else if (Input.GetKey(InputMap.Map["Move Right"]) && !RightSlope && !Game.Instance.InChat)
                {
                    MoveRight();
                    Anim.SetBool("Walking", true);
                }
                else
                {
                    StandStill();
                }

                if (Input.GetKey(InputMap.Map["Jump"]) && !Game.Instance.InChat)
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

                if (Input.GetKey(InputMap.Map["Enter Portal"]) && !Game.Instance.InChat && transform.position.y < CurrentRope.bounds.max.y)
                {
                    if (CurrentRope != null)
                    {
                        ClimbRope();
                    }
                    else
                    {
                        EnterPortal();
                    }
                } 
                else if (Input.GetKey(InputMap.Map["Climb Down"]) && !Game.Instance.InChat && CurrentRope != null && transform.position.y > CurrentRope.bounds.min.y) 
                {
                    ClimbRope();
                }
            }
            else
            {
                if (CurrentRope == null)
                {
                    UnclimbRope();
                }
                else
                {
                    transform.position = Vector2.Lerp(transform.position, new Vector2(CurrentRope.transform.position.x, transform.position.y), Time.deltaTime * 5f);

                    if (Input.GetKey(InputMap.Map["Enter Portal"]) && !Game.Instance.InChat)
                    {
                        // if rope is above our upper body
                        if (transform.position.y < CurrentRope.bounds.max.y) {
                            Anim.SetBool("ClimbingUp", true);
                            Anim.SetBool("ClimbingDown", false);

                            transform.position += Vector3.up * InternalClimbSpeed * Time.deltaTime;
                        } else {
                            UnclimbRope();
                        }
                    }
                    else if (Input.GetKey(InputMap.Map["Climb Down"]) && !Game.Instance.InChat)
                    {
                        if (transform.position.y > CurrentRope.bounds.min.y) {
                            Anim.SetBool("ClimbingDown", true);
                            Anim.SetBool("ClimbingUp", false);

                            transform.position += -Vector3.up * InternalClimbSpeed * Time.deltaTime;
                        } else {
                            UnclimbRope();
                        }
                    }
                    else
                    {
                        Anim.SetBool("ClimbingUp", false);
                        Anim.SetBool("ClimbingDown", false);
                    }

                    if ((Input.GetKey(InputMap.Map["Jump"]) && !Game.Instance.InChat))
                    {
                        UnclimbRope();
                    }
                }
            }

        }
        else
        {
            StandStill();
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

    private void EnterPortal()
    {
        if (Game.Instance.ClientCharacter.GetComponent<ActorController>().CurrentPortal != null && !Game.Instance.MovingTroughPortal)
        {
            Game.Instance.MovingTroughPortal = true;
            SocketClient.Instance.EmitEnteredPortal(CurrentPortal.Key);
        }
    }

    private void ClimbRope()
    {
        OnRope = true;
        Anim.SetBool("OnRope", true);
        Rigid.velocity = Vector2.zero;
        Rigid.bodyType = RigidbodyType2D.Kinematic;
        Instance.SortingGroup.enabled = false;
        Anim.transform.localScale = new Vector3(1 * initScale.x, initScale.y, initScale.z);
        StopAim();
        InturruptAttack();

        SocketClient.Instance.SendStartedClimbing();
    }

    private void UnclimbRope()
    {
        CurrentRope = null;
        OnRope = false;
        Anim.SetBool("OnRope", false);
        Anim.SetBool("ClimbingUp", false);
        Anim.SetBool("ClimbingDown", false);
        Rigid.bodyType = RigidbodyType2D.Dynamic;
        Instance.SortingGroup.enabled = true;

        SocketClient.Instance.SendStoppedClimbing();
    }

    #endregion

    #region Public Methods

    public void MoveLeft()
    {
        //TODO Remove if no problems occur
        //Rigidbody.position += (Vector2.left * InternalSpeed * Time.deltaTime);


        Rigid.velocity = new Vector2(-InternalSpeed * Time.deltaTime , Rigid.velocity.y);
        Anim.transform.localScale = new Vector3(-1 * initScale.x, initScale.y,initScale.z);

        Anim.SetBool("ReverseWalk", aimRight);
    }

    public void MoveRight()
    {
        //TODO Remove if no problems occur
        //Rigidbody.position += (Vector2.right * InternalSpeed * Time.deltaTime);

        Rigid.velocity = new Vector2(InternalSpeed * Time.deltaTime, Rigid.velocity.y);
        Anim.transform.localScale = new Vector3(1 * initScale.x, initScale.y, initScale.z);

        Anim.SetBool("ReverseWalk", !aimRight);
    }

    public void StandStill()
    {
        Rigid.velocity = new Vector2(0f, Rigid.velocity.y);
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

        if (Rigid.velocity.y <= 1.5f)
        {
            Rigid.AddForce(InternalJumpForce * transform.up, ForceMode2D.Impulse);
            AudioControl.Instance.Play("sound_bloop");
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

    public void BeginLoadAttack()
    {
        LoadAttackValueInstance = StartCoroutine(LoadAttackValueRoutine());
        SocketClient.Instance.SendLoadedAttack();
        InGameMainMenuUI.Instance.StartChargingAttack();
    }

    private IEnumerator LoadAttackValueRoutine()
    {
        while(LoadAttackValue < 1f)
        {
            LoadAttackValue += 1f * Time.deltaTime;
            InGameMainMenuUI.Instance.SetChargeAttackValue(LoadAttackValue);
            yield return 0;
        }

        InGameMainMenuUI.Instance.SetChargeAttackValue(LoadAttackValue);
        LoadAttackValueInstance = null;
    }

    public void ReleaseAttack()
    {
        Instance.StartCombatMode();

        if(LoadAttackValueInstance!=null)
        {
             StopCoroutine(LoadAttackValueInstance);
             LoadAttackValueInstance = null;
        }

        InGameMainMenuUI.Instance.StopChargingAttack();
        SocketClient.Instance.SendPreformedAttack(LoadAttackValue);
        LoadAttackValue = 0f;

        ActivatePrimaryAbility();
    }

    public void ActivatePrimaryAbility()
    {
        InturruptAttack();

        switch (Instance.Info.CurrentPrimaryAbility)
        {
            case "melee":
                {
                    OneHandDamageInstance.gameObject.SetActive(true);
                    break;
                }
            case "range":
                {
                    Instance.ShootArrow(true);
                    break;
                }
        }
    }

    public void InturruptAttack()
    {
        Instance.StartCombatMode();

        if (LoadAttackValueInstance != null)
        {
            StopCoroutine(LoadAttackValueInstance);
            LoadAttackValueInstance = null;
        }
        LoadAttackValue = 0f;

        Anim.SetBool("Charging", false);
        InGameMainMenuUI.Instance.StopChargingAttack();
    }

    public void Death()
    {
        Anim.SetTrigger("Kill");
        Anim.SetBool("isDead", true);

        Instance.PlayEyesEmote("cry");
        Instance.PlayMouthEmote("angry");

        CanInput = false;
        Game.Instance.CanUseUI = false;
    }

    #endregion

    void OnTriggerEnter2D(Collider2D obj)
    {
        //Debug.Log("IN -" + obj.gameObject.name);

        if (obj.tag == "GatePortal")
        {
            CurrentPortal = obj.GetComponent<GatePortal>();
        }
        else if (obj.tag == "Rope")
        {
            CurrentRope = obj as BoxCollider2D;
        }
    }

    void OnTriggerStay2D(Collider2D obj)
    {
        if (obj.tag == "Enemy")
        {
            CollidingEnemy = obj.GetComponent<HitBox>().EnemyReference;
        }
    }

    void OnTriggerExit2D(Collider2D obj)
    {
        //Debug.Log("OUT -"+obj.gameObject.name);

        if (obj.tag == "GatePortal" && CurrentPortal == obj.GetComponent<GatePortal>())
        {
            CurrentPortal = null;
        }
        else if (obj.tag == "Rope" && CurrentRope == obj.GetComponent<BoxCollider2D>())
        {
            CurrentRope = null;
        }
        else if (obj.tag == "Enemy")
        {
            if(obj.GetComponent<HitBox>().EnemyReference == CollidingEnemy)
            {
                CollidingEnemy = null;
            }
        }
    }

    private void Hurt(Enemy enemy)
    {
        if (!Invincible)
        {
            SocketClient.Instance.SendTookDMG(enemy.Info);
            Instance.Hurt();

            Instance.PlayEyesEmote("angry");
            Instance.PlayMouthEmote("sad");

            StartCoroutine(InvincibilityRoutine());

            if (enemy.transform.position.x < transform.position.x)
            {
                Rigid.AddForce((InternalJumpForce * 2f * transform.right), ForceMode2D.Impulse);
            }
            else
            {
                Rigid.AddForce((InternalJumpForce * 2f * -transform.right), ForceMode2D.Impulse);
            }

            InturruptAttack();
        }
    }

    private IEnumerator InvincibilityRoutine()
    {
        Invincible = true;

        Coroutine StrobeInstance = StartCoroutine(StrobeRoutine());

        yield return new WaitForSeconds(2);

        StopCoroutine(StrobeInstance);

        Instance.SetOpacity(1f);

        Invincible = false;
    }

    private IEnumerator StrobeRoutine()
    {
        while(true)
        {
            Instance.SetOpacity(0.5f);
            yield return new WaitForSeconds(0.1f);
            Instance.SetOpacity(1f);
            yield return new WaitForSeconds(0.1f);
        }
    }


}
