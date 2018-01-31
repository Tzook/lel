using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectileExplosion : EnemyDamageInstance
{
    [SerializeField]
    protected string ProjectilePrefab;

    [SerializeField]
    protected int Amount;

    [SerializeField]
    protected float Delay = 0f;

    [SerializeField]
    protected float InitTimeAlive = 1f;


    protected override void OnEnable()
    {
        TimeAlive = InitTimeAlive;
        Hit = false;
    }

    public override void SetInfo(Enemy instance, string actionKey, string actionValue = "")
    {
        base.SetInfo(instance, actionKey, actionValue);

        StartCoroutine(TimerRoutine());
    }

    IEnumerator TimerRoutine()
    {
        yield return new WaitForSeconds(Delay);

        GameObject tempObj;
        for (int i = 0; i < Amount; i++)
        {
            tempObj = ResourcesLoader.Instance.GetRecycledObject(ProjectilePrefab);

            if (tempObj.GetComponent<EnemyProjectile>() != null)
            {
                tempObj.GetComponent<EnemyProjectile>().SetInfo(this.ParentEnemy, this.ActionKey, this.ActionValue);
            }
            else if(tempObj.GetComponent<EnemyDamageInstance>() != null)
            {
                tempObj.GetComponent<EnemyDamageInstance>().SetInfo(this.ParentEnemy, this.ActionKey, this.ActionValue);
            }

            tempObj.transform.position = transform.position;
            tempObj.transform.rotation = Quaternion.Euler(0f, 0f, i * (360f / Amount));
        }
    }

    protected override void OnTriggerStay2D(Collider2D TargetCollider)
    {
    }
}
