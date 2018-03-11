using UnityEngine;
using System.Collections;
using System;

public class LevelUpEffect : MonoBehaviour {

    [SerializeField]
    Animator m_Anim;

    [SerializeField]
    ParticleSystem Sparkles;

    [SerializeField]
    ParticleSystem Smoke;

    public Coroutine PlayRoutineInstance { get; private set; }

    public void Play()
    {

        if (PlayRoutineInstance != null)
        {
            StopCoroutine(PlayRoutineInstance);
        }

        PlayRoutineInstance = StartCoroutine(PlayRoutine());
    }

    private IEnumerator PlayRoutine()
    {
        m_Anim.SetTrigger("Play");
        Sparkles.Play();
        Smoke.Play();

        yield return new WaitForSeconds(2f);

        Smoke.Stop();
        Sparkles.Stop();
        
        while(Sparkles.isPlaying)
        {
            yield return 0;
        }

        PlayRoutineInstance = null;
        this.gameObject.SetActive(false);
    }
}
