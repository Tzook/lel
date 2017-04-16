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
    BoxCollider2D m_HitBox;

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

    protected Vector3 initScale;

    public bool Dead = false;
    
    public virtual void Initialize(string instanceID, int currentHP = 0)
    {
        Info.ID = instanceID;

        if (Body != null && initScale == Vector3.zero)
        {
            initScale = Body.localScale;
        }

        RegisterEnemy();
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

    public virtual void UpdateMovement(float x, float y)
    {
        transform.position = new Vector3(x, y, transform.position.z);
    }

    public void PopHint(string text, Color clr)
    {
        GameObject pop = ResourcesLoader.Instance.GetRecycledObject("PopHint");
        pop.transform.position = transform.position + new Vector3(0f, 1f, 0f);
        pop.GetComponent<PopText>().Pop(text, clr);
    }

    public virtual void Hurt(ActorInstance attackSource, int damage = 0)
    {
        Anim.SetInteger("HurtType", Random.Range(0, HurtTypes));
        Anim.SetTrigger("Hurt");

        AudioControl.Instance.Play(WoundSounds[Random.Range(0, WoundSounds.Count)]);

        if (attackSource.Info.ID == Game.Instance.CurrentScene.ClientCharacter.ID)
        {
            PopHint(damage.ToString(), Color.green);
        }
        else
        {
            PopHint(damage.ToString(), Color.blue);
        }

    }

    public virtual void Death()
    {
        if (!Dead)
        {   
            Dead = true;

            m_HitBox.enabled = false;

            AudioControl.Instance.Play(DeathSounds[Random.Range(0, WoundSounds.Count)]);

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
}
