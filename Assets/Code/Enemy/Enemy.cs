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

    protected virtual void Start()
    {
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
}
