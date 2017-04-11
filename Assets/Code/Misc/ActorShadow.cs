using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorShadow : MonoBehaviour {

    LayerMask GroundLayerMask = 0 << 0 | 1;

    RaycastHit2D GroundRay;
    
    [SerializeField]
    BoxCollider2D Collider;

    SpriteRenderer m_SpriteRenderer;

    [SerializeField]
    float MaxDistance = 5;

    void Awake()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        GroundRay = Physics2D.Raycast(Collider.transform.position + Collider.transform.TransformDirection(0,-Collider.size.y / 13f, 0), -Collider.transform.up, MaxDistance, GroundLayerMask);

        m_SpriteRenderer.enabled = GroundRay;

        if (GroundRay)
        {
            transform.position = GroundRay.point;
        }
    }


}
