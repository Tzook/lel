using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectileExplosion : EnemyDamageInstance
{
    [SerializeField]
    protected string ProjectilePrefab;

    [SerializeField]
    protected int Amount;

    protected override void OnEnable()
    {
        Hit = false;
    }

    public override void SetInfo(Enemy instance, string actionKey, string actionValue = "")
    {
        base.SetInfo(instance, actionKey, actionValue);

        GameObject tempObj;
        for (int i = 0; i < Amount; i++)
        {
            tempObj = ResourcesLoader.Instance.GetRecycledObject(ProjectilePrefab);
            tempObj.GetComponent<EnemyProjectile>().SetInfo(this.ParentEnemy, this.ActionKey, this.ActionValue);
            tempObj.transform.position = transform.position;
            tempObj.transform.rotation = Quaternion.Euler(0f, 0f, i * (360f / Amount));
        }
    }

    protected override void Update()
    {
    }

    protected override void OnTriggerStay2D(Collider2D TargetCollider)
    {
    }
}
