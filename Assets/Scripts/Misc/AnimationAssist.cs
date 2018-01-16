using UnityEngine;
using System.Collections;

public class AnimationAssist : MonoBehaviour {

    [SerializeField]
    ActorController Controller;

    [SerializeField]
    ParticleSystem m_Particles;

    [SerializeField]
    GameObject ArrowObject;

    [SerializeField]
    Enemy Enemy;

    [SerializeField]
    Transform WeaponBone;

    public void PlaySound(string soundKey)
    {
        AudioControl.Instance.PlayInPosition(soundKey, transform.position);
    }

    public void PlayRandomMissSound()
    {
        PlaySound("sword_miss_" + Random.Range(0, 3));
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

    public void SpawnEffect(string EffectKey)
    {
        GameObject tempObj = ResourcesLoader.Instance.GetRecycledObject(EffectKey);

        tempObj.transform.position = transform.position;

        if(tempObj.GetComponent<PosFollower>() != null)
        {
            tempObj.GetComponent<PosFollower>().SetTransform(transform);
        }

        tempObj.GetComponent<ParticleSystem>().Play();
    }

    public void UnbendBow()
    {
        Controller.GetComponent<ActorInstance>().UnbendBow();
    }

    public void BendBow()
    {
        Controller.GetComponent<ActorInstance>().BendBow();
    }

    public void TryVarriation()
    {
        Enemy.TryIdleVarriation();
    }

    public void CastSpellComplete()
    {
        if (Controller != null)
        {
            Controller.GetComponent<ActorInstance>().CastSpellComplete();
        }
        else if (Enemy != null)
        {
            Enemy.CastSpellComplete();
        }
    }
}
