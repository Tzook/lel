using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldProjectile : MonoBehaviour {

    [SerializeField]
    Rigidbody2D m_Rigid;

    [SerializeField]
    float Speed = 1f;

    [SerializeField]
    float DecayTime = 3f;

    float TimeAlive = 1f;

    private void OnEnable()
    {
        TimeAlive = DecayTime;
    }

    private void Update()
    {
        m_Rigid.position += (Vector2)transform.right * Speed * Time.deltaTime;

        if(TimeAlive > 0f)
        {
            TimeAlive -= 1f * Time.deltaTime;
        }
        else
        {
            this.gameObject.SetActive(false);
        }
    }
}
