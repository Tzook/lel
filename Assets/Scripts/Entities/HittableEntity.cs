using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D))]
public class HittableEntity : MonoBehaviour {

    [SerializeField]
    UnityEvent HitCallbacks;

    BoxCollider2D m_Collider;

    private void Awake()
    {
        this.gameObject.tag = "HitEntity";

        m_Collider = GetComponent<BoxCollider2D>();
    }

    public void Hurt()
    {
        HitCallbacks.Invoke();
    }

    public void Enable()
    {
        m_Collider.enabled = true;
    }

    public void Disable()
    {
        m_Collider.enabled = false;
    }
}
