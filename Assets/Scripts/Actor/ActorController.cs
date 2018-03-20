using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
    
    public GatePortal CurrentPortal;
    public BoxCollider2D CurrentRope;


    #endregion

    #region Parameters

    public bool Grounded = false;
    public bool OnRope = false;
    public bool LeftSlope = false;
    public bool RightSlope = false;
    public bool isAiming;

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



    public Coroutine JumpRoutineInstance;

    LayerMask GroundLayerMask = 0 << 0 | 1 | 16;

    Vector3 initScale;

    Vector3 lastSentPosition;
    float lastSentAngle;

    float MovementDirection;

    //Rotation Parameters;
    Vector3 tempRot;
    float rotDegrees;
    bool aimRight;

    bool Invincible;
    bool TakingDamageInAir;
    bool Stunned;
    bool Slowed;

    public Enemy CollidingEnemy;

    float AimTimeout;

    float LoadAttackValue = 0f;
    Coroutine LoadAttackValueInstance;

    float SpellCooldown;

    DevSpell CurrentSpellInCast = null;

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
        if (Instance.Info.AttackSpeed != null)
        {
            SetAttackSpeed((float)Instance.Info.AttackSpeed);
        }
        initScale = Anim.transform.localScale;
        EndAttack();

    }

    void Update()
    {
        if (CanDoAction())
        {
            if ((Input.GetMouseButton(0) || Input.GetMouseButton(1)) && !Game.Instance.isInteractingWithUI && !OnRope)
            {
                Aim();
            }
            else if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
            {
                StopAim();
            }


            if (Input.GetKeyDown(InputMap.Map["Pick Up"]))
            {
                Instance.AttemptPickUp();
            }

            //for (int i = 0; i < Instance.Info.PrimaryAbilities.Count; i++)
            //{
            //    if (Input.GetKeyDown(InputMap.Map["PrimaryAbility" + (i + 1)]))
            //    {
            //        Instance.Info.SwitchPrimaryAbility(Instance.Info.PrimaryAbilities[i].Key);
                    
                    
            //    }
            //}


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
            if (Instance.EyesEmoteInstance == null)
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

        if(!isAiming && AimTimeout > 0)
        {
            AimTimeout -= 1f * Time.deltaTime;
        }
    }

    private bool CanDoAction()
    {
        return Game.Instance.CanUseUI && !Game.Instance.InChat && !Game.Instance.MovingTroughPortal && CurrentSpellInCast == null && !Stunned;
    }

    void LateUpdate()
    {
        CollidingEnemy = null;

        if (CanDoAction() && !OnRope)
        {
            AttackCharge();

            if (InGameMainMenuUI.Instance.PrimaryAbilitiesGrid.isOpen)
            {
                for (int i = 0; i < LocalUserInfo.Me.ClientCharacter.PrimaryAbilities.Count; i++)
                {
                    if (Input.GetKeyDown(InputMap.Map["PrimaryAbility" + (i + 1)]))
                    {
                        InGameMainMenuUI.Instance.PrimaryAbilitiesGrid.SetPrimaryAbility(LocalUserInfo.Me.ClientCharacter.PrimaryAbilities[i].Key);
                        InturruptAttack();
                        EndAttack();
                    }
                }
            }
            else
            {
                if (SpellCooldown <= 0)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        if (Input.GetKeyDown(InputMap.Map["Spell" + (i + 1)]))
                        {
                            CastSpell(i);
                            SpellCooldown = 0.5f;
                        }
                    }
                }
            }
        }

        if(SpellCooldown > 0)
        {
            SpellCooldown -= 1f * Time.deltaTime;
        }
    }

    private void AttackCharge()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Instance.SetAttackAnimation();
        }

        Anim.SetBool("Charging", Input.GetMouseButton(0) && !Game.Instance.isInteractingWithUI && CanUsePA());
    }

    void FixedUpdate()
    {

        Anim.SetBool("InAir", false);
        Anim.SetBool("Walking", false);

        if (CanDoAction())
        {
            if (!OnRope)
            {

                SideRayRight = Physics2D.Raycast(transform.position + transform.transform.TransformDirection(Collider.size.x / 16f, -Collider.size.y / 16f, 0), transform.right, GroundedThreshold, GroundLayerMask);
                SideRayLeft = Physics2D.Raycast(transform.position + transform.transform.TransformDirection(-Collider.size.x / 16f, -Collider.size.y / 16f, 0), -transform.right, GroundedThreshold, GroundLayerMask);

                LeftSlope = (SideRayLeft.normal.x < -0.7 || SideRayLeft.normal.x > 0.7);
                RightSlope = (SideRayRight.normal.x < -0.7 || SideRayRight.normal.x > 0.7);

                if (Input.GetKey(InputMap.Map["Move Left"]) && !LeftSlope)
                {
                    MoveLeft();
                    Anim.SetBool("Walking", true);
                }
                else if (Input.GetKey(InputMap.Map["Move Right"]) && !RightSlope)
                {
                    MoveRight();
                    Anim.SetBool("Walking", true);
                }
                else
                {
                    StandStill();
                }

                if (Input.GetKey(InputMap.Map["Jump"]))
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

                if (Input.GetKey(InputMap.Map["Enter Portal"]))
                {
                    if (CurrentRope != null && IsCharBelowRope())
                    {
                        ClimbRope();
                    }
                    else
                    {
                        EnterPortal();
                    }
                }
                else if (Input.GetKey(InputMap.Map["Climb Down"]) && CurrentRope != null && IsCharAboveRope())
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

                    if (Input.GetKey(InputMap.Map["Enter Portal"]))
                    {
                        if (Input.GetKey(InputMap.Map["Jump"]))
                        {
                            Anim.SetBool("ClimbingUp", false);
                            Anim.SetBool("ClimbingDown", false);
                        }
                        else
                        {
                            if (IsCharBelowRope())
                            {
                                Anim.SetBool("ClimbingUp", true);
                                Anim.SetBool("ClimbingDown", false);

                                transform.position += Vector3.up * InternalClimbSpeed * Time.deltaTime;
                            }
                            else
                            {
                                UnclimbRope();
                            }
                        }
                    }
                    else if (Input.GetKey(InputMap.Map["Climb Down"]))
                    {
                        if (Input.GetKey(InputMap.Map["Jump"]))
                        {
                            Anim.SetBool("ClimbingUp", false);
                            Anim.SetBool("ClimbingDown", false);

                        }
                        else
                        {
                            if (IsCharAboveRope())
                            {
                                Anim.SetBool("ClimbingDown", true);
                                Anim.SetBool("ClimbingUp", false);

                                transform.position += -Vector3.up * InternalClimbSpeed * Time.deltaTime;
                            }
                            else
                            {
                                UnclimbRope();
                            }
                        }
                    }
                    else
                    {
                        Anim.SetBool("ClimbingUp", false);
                        Anim.SetBool("ClimbingDown", false);

                        if ((Input.GetKey(InputMap.Map["Jump"])))
                        {
                            UnclimbRope();
                        }
                    }
                }
            }

        }
        else
        {
            StandStill();
        }

        if (!Game.Instance.MovingTroughPortal)
        {
            if (lastSentPosition != transform.position || lastSentAngle != rotDegrees)
            {
                SocketClient.Instance.EmitMovement(transform.position, rotDegrees);
                lastSentPosition = transform.position;
                lastSentAngle = rotDegrees;
            }
        }
    }

    private bool IsCharAboveRope()
    {
        return transform.position.y > CurrentRope.bounds.min.y;
    }

    private bool IsCharBelowRope()
    {
        return transform.position.y < CurrentRope.bounds.max.y;
    }

    private void EnterPortal()
    {
        if (Game.Instance.ClientCharacter.GetComponent<ActorController>().CurrentPortal == null)
        {
            return;
        }

        for(int i=0;i<CurrentPortal.RequiresItems.Count;i++)
        {
            if(LocalUserInfo.Me.ClientCharacter.Inventory.GetItem(CurrentPortal.RequiresItems[i]) == null)
            {
                InGameMainMenuUI.Instance.ShockMessageCenter.CallMessage("Need the item: \"" + Content.Instance.GetItem(CurrentPortal.RequiresItems[i]).Name + "\" to enter.");
                return;
            }
        }

        Game.Instance.MovingTroughPortal = true;
        SocketClient.Instance.EmitEnteredPortal(CurrentPortal.Key);
        
    }

    private void ClimbRope()
    {
        EndAttack();
        InturruptAttack();
        OnRope = true;
        Anim.SetBool("OnRope", true);
        Rigid.velocity = Vector2.zero;
        Rigid.bodyType = RigidbodyType2D.Kinematic;
        Instance.SortingGroup.enabled = false;
        Anim.transform.localScale = new Vector3(1 * initScale.x, initScale.y, initScale.z);
        StopAim();
        EndAttack();

        Instance.TorsoBone.transform.localScale = Vector3.one;
        Instance.TorsoBone.transform.rotation = Quaternion.Euler(Vector3.zero);

        SocketClient.Instance.SendStartedClimbing();
    }

    private void UnclimbRope()
    {
        OnRope = false;
        Anim.SetBool("OnRope", false);
        Anim.SetBool("ClimbingUp", false);
        Anim.SetBool("ClimbingDown", false);
        Rigid.bodyType = RigidbodyType2D.Dynamic;
        Instance.SortingGroup.enabled = true;

        SocketClient.Instance.SendStoppedClimbing();
    }

    public void AttackMelee()
    {
        GameObject damageZone = ResourcesLoader.Instance.GetRecycledObject("DI_OneHand");

        damageZone.transform.position = Instance.transform.position;
        damageZone.transform.rotation = Instance.LastFireRot;

        damageZone.GetComponent<ActorDamageInstance>().Open(Instance, "melee");
    }

    public void FireProjectile()
    {
        Instance.FireProjectile(true, LoadAttackValue);
    }

    private void CastSpell(int spellIndex)
    {
        Aim();
        StopAim();
        DevSpell spell = Content.Instance.GetSpellAtIndex(spellIndex);
        
        if(spell == null)
        {
            return;
        }

        if(LocalUserInfo.Me.ClientCharacter.CurrentPrimaryAbility.LVL < spell.Level)
        {
            return;
        }

        InGameMainMenuUI.Instance.ActivatedSpell(spell.Key);

        bool usedSpell = ManaUsage.Instance.UseMana(spell.Mana);
        if (usedSpell)
        {
            CurrentSpellInCast = spell;

            SocketClient.Instance.SendUsedSpell(spell.Key);

            Instance.CastSpell(spell);
        }
    }


    #endregion

    #region Public Methods

    public void MoveLeft()
    {
        Rigid.position += GetNextMovementPosition(Vector2.left);

        Anim.transform.localScale = new Vector3(-1 * initScale.x, initScale.y, initScale.z);

        Anim.SetBool("ReverseWalk", aimRight);

        if (!isAiming && AimTimeout <= 0)
        {
            Instance.TorsoBone.transform.localScale = new Vector3(-1f, -1f, 1f);
            Instance.TorsoBone.transform.rotation = Quaternion.Euler(0f, 0f, 180f);
        }
    }

    public void MoveRight()
    {
        Rigid.position += GetNextMovementPosition(Vector2.right);

        Anim.transform.localScale = new Vector3(1 * initScale.x, initScale.y, initScale.z);

        Anim.SetBool("ReverseWalk", !aimRight);

        if (!isAiming && AimTimeout <= 0)
        {
            Instance.TorsoBone.transform.localScale = Vector3.one;
            Instance.TorsoBone.transform.rotation = Quaternion.Euler(Vector3.zero);
        }
    }

    private Vector2 GetNextMovementPosition(Vector2 side)
    {
        return (side + Vector2.up * 0.1f) * GetMovementSpeed() * Time.deltaTime;
    }

    private float GetMovementSpeed()
    {
        float speed;
        if (TakingDamageInAir) 
        {
            speed = 1f;
        }
        else 
        {
            speed = InternalSpeed + Instance.Info.SpeedBonus;
            if (Slowed) {
                speed /= 2f;
            }
        }
        return speed;
    }

    public void StandStill()
    {
    }

    public void Jump()
    {
        if(JumpRoutineInstance==null && Grounded)
        {
            JumpRoutineInstance = StartCoroutine(JumpRoutine());
        }
    }

    internal void ColliderHitMobs(List<Enemy> sentTargets, string actionKey, string actionValue = "")
    {
        List<string> targetIDs = new List<string>();

        for(int i=0;i<sentTargets.Count;i++)
        {
            targetIDs.Add(sentTargets[i].Info.ID);
        }

        switch(actionKey)
        {
            case "melee":
                {
                    SocketClient.Instance.SendUsedPrimaryAbility(targetIDs);

                    int rnd = Random.Range(0, 3);
                    AudioControl.Instance.PlayInPosition("sound_hit_" + (rnd + 1), transform.position);

                    GameObject tempHit;
                    tempHit = ResourcesLoader.Instance.GetRecycledObject("HitEffect");
                    tempHit.transform.position = Instance.Weapon.transform.position;
                    tempHit.GetComponent<HitEffect>().Play();

                    break;
                }
            case "spell":
                {
                    SocketClient.Instance.SendHitSpell(actionValue, targetIDs);

                    int rnd = Random.Range(0, 3);

                    GameObject tempHit;
                    tempHit = ResourcesLoader.Instance.GetRecycledObject("HitEffect");
                    tempHit.transform.position = Instance.Weapon.transform.position;
                    tempHit.GetComponent<HitEffect>().Play();

                    DevSpell tempSpell = Content.Instance.GetPlayerSpell(actionValue);
                    if (!string.IsNullOrEmpty(tempSpell.HitSound))
                    {
                        AudioControl.Instance.PlayInPosition(tempSpell.HitSound, tempHit.transform.position);
                    }

                    break;
                }
        }
    }

    protected IEnumerator JumpRoutine()
    {

        if (Rigid.velocity.y <= 1.5f)
        {
            Rigid.AddForce((InternalJumpForce + Instance.Info.JumpBonus) * transform.up, ForceMode2D.Impulse);
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

        isAiming = true;
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
                Instance.TorsoBone.transform.rotation = Quaternion.Euler(0, 0, rotDegrees - 4f);
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
                Instance.TorsoBone.transform.rotation = Quaternion.Euler(0, 0, rotDegrees + 4f);
            }
        }

    }

    private void StopAim()
    {
        if (aimRight)
        {
            Instance.LastFireRot = Quaternion.Euler(0, 0, rotDegrees - 4f);
        }
        else
        {
            Instance.LastFireRot = Quaternion.Euler(0, 0, rotDegrees + 4f);
        }

        Anim.SetBool("Aim", false);

        isAiming = false;

        AimTimeout = 1f;
        //if (aimRight)
        //{
        //    Instance.TorsoBone.transform.localScale = Vector3.one;
        //    Instance.TorsoBone.transform.rotation = Quaternion.Euler(Vector3.zero);
        //}
        //else
        //{
        //    Instance.TorsoBone.transform.localScale = new Vector3(-1f, -1f, 1f);
        //    Instance.TorsoBone.transform.rotation = Quaternion.Euler(0f,0f,180f);
        //}

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

        ActivatePrimaryAbility();

        LoadAttackValue = 0f;
    }

    public void ActivatePrimaryAbility()
    {
        DevAbility devAbility = Content.Instance.GetAbility(Instance.Info.CurrentPrimaryAbility.Key);
        if (devAbility.ManaCost > 0) 
        {
            ManaUsage.Instance.UseMana(devAbility.ManaCost);
        }

        if (string.IsNullOrEmpty(devAbility.ProjectilePrefab))
        {
            AttackMelee();
        }
        else
        {
            FireProjectile();
        }
    }

    public bool CanUsePA()
    {
        DevAbility devAbility = Content.Instance.GetAbility(Instance.Info.CurrentPrimaryAbility.Key);
        bool canUse = true;
        if (devAbility.ManaCost > 0) 
        {
            if (!ManaUsage.Instance.CanUseMana(devAbility.ManaCost))
            {
                ManaUsage.Instance.WarnAboutMana();
                canUse = false;
            }
        }
        return canUse;
    }

    public void InturruptAttack()
    {
        EndAttack();
        Instance.SetAttackAnimation();
        Anim.SetTrigger("InturruptAttack");

        Instance.InturruptAttack();

        CurrentSpellInCast = null;
    }

    public void EndAttack()
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

        Game.Instance.IsAlive = false;
    }

    public void InteractWithNpc()
    {
        Game.Instance.IsChattingWithNpc = true;
        InturruptAttack();
        Anim.SetBool("ClimbingUp", false);
        Anim.SetBool("ClimbingDown", false);
    }

    public void CastSpellComplete()
    {
        if (CurrentSpellInCast != null)
        {
            GameObject damageZone = ResourcesLoader.Instance.GetRecycledObject(CurrentSpellInCast.ColliderPrefab);

            damageZone.transform.position = Instance.transform.position;
            damageZone.transform.rotation = Instance.LastFireRot;

            damageZone.GetComponent<ActorDamageInstance>().Open(Instance, "spell", CurrentSpellInCast.Key);

            CurrentSpellInCast = null;
        }
    }


    public void StartBuffEffect(string buffKey)
    {
        switch (buffKey)
        {
            case "stunChance":
                {
                    this.Stunned = true;
                    break;
                }
            case "crippleChance":
                {
                    this.Slowed = true;
                    break;
                }
        }
    }

    public void StopBuffEffect(string buffKey)
    {
        if (Instance.GetBuff(buffKey) == null)
        {
            switch (buffKey)
            {
                case "stunChance":
                    {
                        this.Stunned = false;
                        break;
                    }
                case "crippleChance":
                    {
                        this.Slowed = false;
                        break;
                    }
            }
        }
    }

    
    public void TookSpellDamage(EnemyDamageInstance instance)
    {
        SocketClient.Instance.SendTookSpellDamage(instance.ActionKey, instance.ParentEnemy.Info.ID);
    }

    #endregion

    void OnTriggerEnter2D(Collider2D obj)
    {
        //Debug.Log("IN -" + obj.gameObject.name);

        if (obj.tag == "GatePortal")
        {
            CurrentPortal = obj.GetComponent<GatePortal>();
        }
        else if(obj.tag == "StrobeObject")
        {
            obj.GetComponent<PlayerStayStrobe>().Activate();
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
        else if (obj.tag == "StrobeObject")
        {
            obj.GetComponent<PlayerStayStrobe>().Deactivate();
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
        if (!Invincible && Content.Instance.GetMonsterByName(enemy.Info.Name).DMG != 0)
        {
            EndAttack();

            SocketClient.Instance.SendTookDMG(enemy.Info);
            Instance.Hurt();

            Instance.PlayEyesEmote("angry");
            Instance.PlayMouthEmote("sad");

            StartCoroutine(InvincibilityRoutine());

            StartCoroutine(DisableSpeedUntilGrounded());

            if (enemy.transform.position.x < transform.position.x)
            {
                Rigid.AddForce(2.5f * transform.right, ForceMode2D.Impulse);
            }
            else
            {
                Rigid.AddForce(2.5f * -transform.right, ForceMode2D.Impulse);
            }
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

    private IEnumerator DisableSpeedUntilGrounded()
    {
        TakingDamageInAir = true;
        // first take extra time for the addforce to kick in
        yield return new WaitForSeconds(0.2f);
        while (!Grounded)
        {
            yield return null;
        }
        TakingDamageInAir = false;
    }

    public void SetAttackSpeed(float attackSpeed)
    {
        Anim.SetFloat(AnimationAssist.PAREMETER_ATTACK_SPEED_MULTIPLIER, attackSpeed);        
    }
}
