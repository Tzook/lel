using UnityEngine;
using System.Collections;
using System;
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
    protected int HurtTypes;

    [SerializeField]
    protected int DeathTypes;

    [SerializeField]
    protected List<SpriteRenderer> Sprites = new List<SpriteRenderer>();

    protected Vector3 initScale;

    public virtual void Initialize(string instanceID, int currentHP = 0)
    {
        Info.ID = instanceID;


        if (Body != null)
        {
            initScale = Body.localScale;
        }

        RegisterEnemy();
    }

    private void RegisterEnemy()
    {
        StartCoroutine(RegisterRoutine());
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
}
