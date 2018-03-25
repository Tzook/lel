using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour {

    public EnemyInfo Info;

    [SerializeField]
    protected Rigidbody2D Rigid;

    [SerializeField]
    protected Transform Body;

    [SerializeField]
    protected Animator Anim;

    [SerializeField]
    public BoxCollider2D m_HitBox;

    [SerializeField]
    SpriteAlphaGroup m_AlphaGroup;

    [SerializeField]
    protected int HurtTypes;

    [SerializeField]
    protected int DeathTypes;

    [SerializeField]
    protected List<SpriteRenderer> Sprites = new List<SpriteRenderer>();

    [SerializeField]
    protected List<string> WoundSounds = new List<string>();

    [SerializeField]
    protected List<string> DeathSounds = new List<string>();

    [SerializeField]
    protected int IdleVarriations = 0;

    [SerializeField]
    protected int VarriationChance = 3;

    [SerializeField]
    public HealthBar m_HealthBar;

    [SerializeField]
    protected Transform SpellSource;

    List<Buff> CurrentBuffs = new List<Buff>();

    //[SerializeField]
    protected Vector3 initScale;

    public DevSpell SpellInCast;

    public ActorInstance CurrentTarget;

    public bool Dead = false;

    void LateUpdate()
    {
        if (m_HealthBar != null)
        {
            m_HealthBar.transform.position = Vector2.Lerp(m_HealthBar.transform.position, new Vector2(transform.position.x, m_HitBox.bounds.max.y), Time.deltaTime * 3f);
        }
    }

    public virtual void Initialize(string instanceID ,DevMonsterInfo givenInfo ,int currentHP = 0)
    {
        Info = new EnemyInfo(givenInfo, instanceID);
        Info.CurrentHealth = currentHP;

        if (Body != null && initScale == Vector3.zero)
        {
            initScale = Body.localScale;
        }

        RegisterEnemy();

        if(m_AlphaGroup!=null)
        {
            m_AlphaGroup.SetAlpha(1f);
        }
    }

    private void RegisterEnemy()
    {
        StartCoroutine(RegisterRoutine());
    }

    private void UnregisterEnemy()
    {
        Game.Instance.CurrentScene.RemoveSceneEnemy(this);
    }

    private IEnumerator RegisterRoutine()
    {
        while(Game.Instance.CurrentScene == null)
        {
            yield return 0;
        }

        Game.Instance.CurrentScene.AddSceneEnemy(this);
    }

    public virtual void SetAION()
    {

    }

    public virtual void SetAIOFF()
    {

    }

    public virtual void UpdateMovement(float x, float y, float velocity)
    {
        transform.position = new Vector3(x, y, transform.position.z);
    }

    public void PopHint(string text, Color clr, Color outlineClr = new Color())
    {
        GameObject pop = ResourcesLoader.Instance.GetRecycledObject("PopHint");
        pop.transform.position = transform.position + new Vector3(0f, 1f, 0f);
        pop.GetComponent<PopText>().Pop(text, clr, outlineClr);
    }

    public virtual void SetTarget(ActorInstance target)
    {
        CurrentTarget = target;
    }

    public virtual void Hurt(ActorInstance attackSource, int damage = 0, int currentHP = 0, string cause = "attack", bool crit = false)
    {
        Anim.SetInteger("HurtType", Random.Range(0, HurtTypes));
        Anim.SetTrigger("Hurt");

        string dmgMessage = damage > 0 ? damage.ToString("N0") : "BLOCKED";

        int TextSize = 40; 

        if (crit)
        {
            TextSize = 60;
        }

        switch (cause)
        {
            case "attack":
                {
                    if (crit)
                    {
                        AudioControl.Instance.PlayInPosition("sound_crit", attackSource.transform.position);
                    }
                    else
                    {
                        AudioControl.Instance.PlayInPosition(WoundSounds[Random.Range(0, WoundSounds.Count)], attackSource.transform.position);
                    }

                    if (attackSource.Info.ID == LocalUserInfo.Me.ClientCharacter.ID)
                    {
                        if (crit)
                        {
                            PopHint("<size=" + TextSize + "> " + dmgMessage + "</size>", new Color(1f, 0.619f, 0.325f, 1f), Color.black);
                        }
                        else
                        {
                            PopHint("<size=" + TextSize + "> " + dmgMessage + "</size>", Color.green);
                        }
                    }
                    else
                    {
                        PopHint("<size=" + TextSize + "> " + dmgMessage + "</size>", Color.blue);
                    }



                    m_AlphaGroup.BlinkDamage();
                    break;
                }
            case "aoe":
                {
                    TextSize -= 15;

                    AudioControl.Instance.PlayInPosition(WoundSounds[Random.Range(0, WoundSounds.Count)], attackSource.transform.position);

                    if (attackSource.Info.ID == LocalUserInfo.Me.ClientCharacter.ID)
                    {
                        PopHint("<color=#ffff00ff><size=" + TextSize +"> " + dmgMessage + "</size></color>", Color.green);
                    }
                    else
                    {
                        PopHint("<size=25>" + dmgMessage + "</size>", Color.blue);
                    }

                    break;
                }
            case "burn":
            case "bleed":
            case "spikes":
                {
                    TextSize -= 15;

                    AudioControl.Instance.PlayInPosition("sound_smallHit", attackSource.transform.position);

                    if (attackSource.Info.ID == LocalUserInfo.Me.ClientCharacter.ID)
                    {
                        PopHint("<color=#ffff00ff><size="+TextSize+">" + dmgMessage + "</size></color>", Color.green);
                    }

                    break;
                }
        }




        if(m_HealthBar == null)
        {
            m_HealthBar = ResourcesLoader.Instance.GetRecycledObject("HealthBar").GetComponent<HealthBar>();
            m_HealthBar.transform.position = transform.position;
        }
			
        m_HealthBar.SetHealthbar(Info.CurrentHealth, currentHP, Info.MaxHealth, 2f);
        Info.CurrentHealth = currentHP;
    }

    public virtual void Miss(ActorInstance attackSource, string cause = "attack")
    {
        AudioControl.Instance.PlayInPosition(WoundSounds[Random.Range(0, WoundSounds.Count)], attackSource.transform.position);

        if (attackSource.Info.ID == LocalUserInfo.Me.ClientCharacter.ID)
        {
            PopHint("MISS", Color.green);
        }
        else
        {
            PopHint("MISS", Color.blue);
        }
    }

    public virtual void Death()
    {
        if (!Dead)
        {   
            Dead = true;

            m_HitBox.enabled = false;

            m_HealthBar.gameObject.SetActive(false);
            m_HealthBar = null;

            CurrentTarget = null;

            AudioControl.Instance.PlayInPosition(DeathSounds[Random.Range(0, DeathSounds.Count)], transform.position);

            ClearBuffs();

            UnregisterEnemy();

            StartCoroutine(DeathRoutine());
        }
    }

    private IEnumerator DeathRoutine()
    {
        Anim.SetInteger("DeathType", Random.Range(0, DeathTypes));
        Anim.SetBool("Dead", true);

        yield return new WaitForSeconds(3f);
        
        m_HitBox.enabled = true;
        Anim.SetBool("Dead", false);
        Dead = false;

        this.gameObject.SetActive(false);
    }

    public MonsterSpawner GetClosestSpawner()
    {
        float ClosestDistance = Mathf.Infinity;
        float tempDistance;
        MonsterSpawner tempSpawner = null;
        MonsterSpawner ClosestSpawner = null;

        for (int i = 0; i < SceneInfo.Instance.Spawners.Count; i++)
        {
            tempSpawner = SceneInfo.Instance.Spawners[i];
             
            if (tempSpawner.MonsterKey == Info.Name)
            {
                tempDistance = Vector2.Distance(transform.position, tempSpawner.transform.position);
                if (tempDistance < ClosestDistance)
                {
                    ClosestDistance = tempDistance;
                    ClosestSpawner = tempSpawner;
                }
            }
        }

        return ClosestSpawner;
    }

    public void TryIdleVarriation()
    {
        if(IdleVarriations <= 0)
        {
            return;
        }

        if(Random.Range(0,VarriationChance) == 0)
        {
            Anim.SetInteger("IdleVarriationIndex", Random.Range(0, IdleVarriations));
            Anim.SetTrigger("IdleVarriation");
        }
    }

    public void ClearBuffs()
    {
        for(int i=0;i<CurrentBuffs.Count;i++)
        {
            StopCoroutine(CurrentBuffs[i].RunningRoutine);

            CurrentBuffs[i].EffectPrefab.transform.SetParent(null);
            CurrentBuffs[i].EffectPrefab.gameObject.SetActive(false);
        }

        CurrentBuffs.Clear();
    }

    public void AddBuff(string buffKey, float buffDuration)
    {
        Buff tempBuff = new Buff(buffKey, Info.ID, buffDuration);

        CurrentBuffs.Add(tempBuff);

        tempBuff.RunningRoutine = StartCoroutine(HandleBuff(tempBuff));

        if(Game.Instance.isBitch)
        {
            StartBuffEffect(buffKey);
        }
    }

    public void RemoveBuff(Buff buff)
    {
        if(buff.RunningRoutine != null)
        {
            StopCoroutine(buff.RunningRoutine);
        }

        buff.EffectPrefab.transform.SetParent(null);
        buff.EffectPrefab.transform.gameObject.SetActive(false);

        CurrentBuffs.Remove(buff);

        if (Game.Instance.isBitch)
        {
            StopBuffEffect(buff.Key);
        }
    }

    public Buff GetBuff(string buffKey)
    {
        for(int i=0;i<CurrentBuffs.Count;i++)
        {
            if(CurrentBuffs[i].Key == buffKey)
            {
                return CurrentBuffs[i];
            }
        }

        return null;
    }

    protected virtual void StartBuffEffect(string buffKey)
    {
    }

    protected virtual void StopBuffEffect(string buffKey)
    {
    }

    protected IEnumerator HandleBuff(Buff buff)
    {
        DevBuff buffRef = Content.Instance.GetBuff(buff.Key);

        buff.EffectPrefab = ResourcesLoader.Instance.GetRecycledObject(buffRef.EffectPrefab);

        buff.EffectPrefab.transform.SetParent(transform);
        buff.EffectPrefab.transform.position = transform.position;

        AudioControl.Instance.Play(buffRef.AudioKey);

        while(buff.Duration > 0)
        {
            yield return new WaitForSeconds(1f);
            buff.Duration--;
        }

        buff.RunningRoutine = null;
        RemoveBuff(buff);
    }

    public void ActivateSpell(DevSpell spellRef)
    {
        Anim.SetTrigger(spellRef.Key);
        SpellInCast = spellRef;
    }

    public void CastSpellComplete()
    {
        GameObject tempObj = ResourcesLoader.Instance.GetRecycledObject(SpellInCast.ColliderPrefab);

        if(SpellSource != null)
        {
            tempObj.transform.position = SpellSource.position;
        }
        else
        {
            tempObj.transform.position = transform.position;
        }

        tempObj.GetComponent<EnemyDamageInstance>().SetInfo(this, SpellInCast.Key);
    }

}
