using UnityEngine;
using System.Collections;

public class AnimationAssist : MonoBehaviour {

    [SerializeField]
    ActorController Controller;

    Animator m_Anim;

    [SerializeField]
    ParticleSystem m_Particles;

    [SerializeField]
    GameObject ArrowObject;

    [SerializeField]
    Enemy Enemy;

    [SerializeField]
    Transform WeaponBone;

    public const string PAREMETER_ATTACK_SPEED_MULTIPLIER = "AttackSpeedMultiplier";

    private void Awake()
    {
        m_Anim = GetComponent<Animator>();
    }

    public void PlaySound(string soundKey)
    {
        AudioControl.Instance.PlayInPosition(soundKey, transform.position, 30f);
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
        if (Enemy != null)
        {
            Enemy.TryIdleVarriation();
        }
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

    public void CameraShake(float Power)
    {
        GameCamera.Instance.Shake(10f, Power);
    }

    public void SetAnimatorBoolTrue(string boolKey)
    {
        m_Anim.SetBool(boolKey, true);
    }

    public void SetAnimatorBoolFalse(string boolKey)
    {
        m_Anim.SetBool(boolKey, false);
    }

    public void ForwardLeftHand()
    {
        Controller.Instance.ForwardLeftHand();
    }

    public void BackwardLeftHand()
    {
        Controller.Instance.BackwardLeftHand();
    }

    public void ResetTriggers()
    {
        for (int i = 0; i < m_Anim.parameterCount; i++)
        {
            if(m_Anim.parameters[i].type == AnimatorControllerParameterType.Trigger)
            {
                m_Anim.ResetTrigger(m_Anim.parameters[i].name);
            }
        }
    }

    public void ResetBools()
    {
        for (int i = 0; i < m_Anim.parameterCount; i++)
        {
            if (m_Anim.parameters[i].type == AnimatorControllerParameterType.Bool)
            {
                m_Anim.SetBool(m_Anim.parameters[i].name, false);
            }
        }
    }

    public void ResetParameters()
    {
        ResetTriggers();
        ResetBools();
    }

    public void ResetSpellParameters()
    {
        ResetTriggers();

        m_Anim.SetBool("CastingSpell", false);
    }
}
