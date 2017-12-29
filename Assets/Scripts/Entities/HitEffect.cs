using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEffect : MonoBehaviour {

    [SerializeField]
    Animator m_Animator;

    [SerializeField]
    ParticleSystem m_Particles;

    [SerializeField]
    SpriteRenderer m_SpriteRenderer;

    [SerializeField]
    List<string> SpriteVarriation = new List<string>();

	public void Play()
    {
        m_SpriteRenderer.sprite = ResourcesLoader.Instance.GetSprite(SpriteVarriation[Random.Range(0, SpriteVarriation.Count)]);

        m_Animator.SetTrigger("Play");
    }

    public void PlayParticles()
    {
        m_Particles.Play();
    }

    public void Shut()
    {
        m_Particles.Stop();
        this.gameObject.SetActive(false);
    }


}
