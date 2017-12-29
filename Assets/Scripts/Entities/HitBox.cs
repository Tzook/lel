using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class HitBox : MonoBehaviour
{
    [SerializeField]
    public Enemy EnemyReference;

    public List<Collider2D> Collisions = new List<Collider2D>();

    public bool hasCollision
    {
        get
        {
            return (Collisions.Count > 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!Collisions.Contains(collision))
        {
            Collisions.Add(collision);
        }   
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (Collisions.Contains(collision))
        {
            Collisions.Remove(collision);
        }
    }
}
