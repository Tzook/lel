using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesExecuter : MonoBehaviour {

    [SerializeField]
    List<ParticleSystem> ParticlesList = new List<ParticleSystem>();

    [SerializeField]
    public string ExecuteSoundKey;

    public void ExecuteParticles()
    {
        if(!string.IsNullOrEmpty(ExecuteSoundKey))
        {
            AudioControl.Instance.PlayInPosition(ExecuteSoundKey, transform.position);
        }

        foreach(ParticleSystem particles in ParticlesList)
        {
            particles.Play();
        }
    }

    public void StopParticles()
    {
        foreach (ParticleSystem particles in ParticlesList)
        {
            particles.Stop();
        }
    }
}
