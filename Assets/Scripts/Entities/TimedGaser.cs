using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedGaser : MonoBehaviour {

    [SerializeField]
    float Delay;

    [SerializeField]
    public float Duration;

    [SerializeField]
    public float Interval;

    float CurrentTime;

    bool isOn;

    [SerializeField]
    ParticleSystem m_Particles;

    [SerializeField]
    BoxCollider2D m_AreaEffect;

    private void Start()
    {
        CurrentTime = Delay;
        Deactivate();
    }

    private void Update()
    {
        if(CurrentTime > 0)
        {
            CurrentTime -= 1 * Time.deltaTime;
        }
        else
        {
            if(isOn)
            {
                CurrentTime = Interval;
                Deactivate();
            }
            else
            {
                CurrentTime = Duration;
                Activate();
            }
        }
    }

    void Activate()
    {
        m_Particles.Play();
        m_AreaEffect.enabled = true;
        isOn = true;
    }

    void Deactivate()
    {
        m_Particles.Stop();
        m_AreaEffect.enabled = false;
        isOn = false;
    }
}
