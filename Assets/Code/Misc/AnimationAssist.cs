using UnityEngine;
using System.Collections;

public class AnimationAssist : MonoBehaviour {

    [SerializeField]
    ActorController Controller;

    [SerializeField]
    ParticleSystem m_Particles;

    [SerializeField]
    GameObject ArrowObject;

    public void PlaySound(string soundKey)
    {
        AudioControl.Instance.Play(soundKey);
    }

    public void BeginChargeAttack()
    {
        if(Controller!=null && Controller.enabled)
        {
            Controller.BeginLoadAttack();
        }
    }

    public void ReleaseAttack()
    {
        if (Controller != null && Controller.enabled)
        {
            Controller.ReleaseAttack();
        }
    }

    public void BeginChargeParticles()
    {
        m_Particles.Play();
    }

    public void EndChargeParticles()
    {
        m_Particles.Stop();
    }

    public void UnbendBow()
    {
        Controller.GetComponent<ActorInstance>().UnbendBow();
    }

    public void BendBow()
    {
        Controller.GetComponent<ActorInstance>().BendBow();
    }

}
