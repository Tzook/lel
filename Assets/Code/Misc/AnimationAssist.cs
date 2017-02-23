using UnityEngine;
using System.Collections;

public class AnimationAssist : MonoBehaviour {

    [SerializeField]
    ActorController Controller;

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
}
